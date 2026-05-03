#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Gorge.GorgeFramework.Chart;
using Gorge.GorgeFramework.Simulators;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    /// <summary>
    /// 管理运行时谱面
    /// </summary>
    public class ChartManager
    {
        private readonly GorgeSimulationRuntime _runtime;

        public ChartManager(GorgeSimulationRuntime runtime)
        {
            _runtime = runtime;
        }

        #region 加载阶段

        // TODO 寻找更好的数据结构
        // TODO 目前认识到需求1，按时间顺序创建，可以排序
        // TODO 需求2，Editor中按Injector对象删除，可以做索引
        public List<Tuple<Injector, ConstructorInformation>> InitializeGenerate { get; } = new();
        public List<TimedGenerateElement> ForwardTimedGenerateList { get; } = new();
        public List<TimedGenerateElement> BackwardTimedGenerateList { get; } = new();
        public float BeginChartTime { get; private set; }
        public float TerminateChartTime { get; private set; }
        public float BeginSimulateSpeed { get; private set; }

        /// <summary>
        /// 总谱使用的记载版Injector和Gameplay使用的修改版Injector的对应关系
        /// </summary>
        public Dictionary<Injector, Injector> InjectorModifications { get; } = new();

        public void LoadScore()
        {
            var score = RuntimeStatic.Runtime.Score;
            BeginChartTime = score.StartTime;
            TerminateChartTime = score.TerminateTime;
            BeginSimulateSpeed = score.SimulationSpeed;

            foreach (var staff in score.Stave)
            {
                if (staff is ElementStaff elementStaff)
                {
                    foreach (var period in elementStaff.Periods)
                    {
                        foreach (var element in period.Elements)
                        {
                            if (element.InjectedClassDeclaration.Is("GorgeFramework.Element"))
                            {
                                AddScoreElement(element, period.Config);
                            }
                        }
                    }
                }
            }
        }

        public void UnloadScore()
        {
            foreach (var injector in InjectorModifications.Keys.ToList())
            {
                RemoveScoreElement(injector);
            }

            BeginChartTime = 0;
            TerminateChartTime = 0;
            BeginSimulateSpeed = 0;

            ForwardTimedGenerateList.Clear();
            BackwardTimedGenerateList.Clear();
            InitializeGenerate.Clear();
            InjectorModifications.Clear();
        }


        /// <summary>
        /// 向生成表添加元素
        /// </summary>
        /// <param name="scoreElement">谱面记载Injector</param>
        /// <param name="periodConfig">Element所属乐段</param>
        /// <returns>新添加的GameplayInjector</returns>
        public Injector AddScoreElement(Injector scoreElement, PeriodConfig periodConfig)
        {
            var elementContainer =
                RuntimeStatic.Runtime.FormContainer.ElementModifiers[scoreElement.InjectedClassDeclaration.Name];

            var gameplayInjector = Modify(scoreElement, periodConfig);
            InjectorModifications.Add(scoreElement, gameplayInjector);

            if (elementContainer.InitializeGenerateConstructor != null)
            {
                InitializeGenerate.Add(
                    new Tuple<Injector, ConstructorInformation>(gameplayInjector,
                        elementContainer.InitializeGenerateConstructor));
            }

            if (elementContainer.ForwardTimedGenerateConstructor != null)
            {
                ForwardTimedGenerateList.Add(new TimedGenerateElement(gameplayInjector,
                    elementContainer.ForwardTimedGenerateConstructor,
                    elementContainer.ForwardGenerateTime));
            }

            if (elementContainer.BackwardTimedGenerateConstructor != null)
            {
                BackwardTimedGenerateList.Add(new TimedGenerateElement(gameplayInjector,
                    elementContainer.BackwardTimedGenerateConstructor, elementContainer.BackwardGenerateTime));
            }

            return gameplayInjector;
        }

        /// <summary>
        /// 删除生成表中的元素
        /// </summary>
        /// <param name="scoreElement">谱面记载Injector</param>
        /// <returns>已删除的GameplayInjector</returns>
        public Injector RemoveScoreElement(Injector scoreElement)
        {
            var gameplayInjector = InjectorModifications[scoreElement];
            InjectorModifications.Remove(scoreElement);

            // TODO 可以计算然后筛选，不需要全部遍历
            // TODO 并且可能可以存一些索引
            InitializeGenerate.RemoveAll(i => i.Item1.Equals(gameplayInjector));
            ForwardTimedGenerateList.RemoveAll(e => e.Injector.Equals(gameplayInjector));
            BackwardTimedGenerateList.RemoveAll(e => e.Injector.Equals(gameplayInjector));
            return gameplayInjector;
        }

        /// <summary>
        /// 将总谱记载Injector修改为Gameplay的Injector
        /// </summary>
        /// <param name="scoreElementInjector"></param>
        /// <param name="periodConfig"></param>
        /// <returns></returns>
        private Injector Modify(Injector scoreElementInjector, PeriodConfig periodConfig)
        {
            var gameplayInjector = (Injector) scoreElementInjector.Clone();
            var elementClassName = gameplayInjector.InjectedClassDeclaration.Name;
            var elementClass = RuntimeStatic.Runtime.LanguageRuntime.GetClass(elementClassName);
            // TODO 暂时不限定顺序
            // TODO 暂时只执行乐段修改器
            // TODO 这里可以通过遍历继承树来避免遍历大表
            foreach (var (className, runtimeElementContainer) in RuntimeStatic.Runtime.FormContainer.ElementModifiers)
            {
                if (elementClass.Declaration.Is(className))
                {
                    foreach (var modifierMethod in runtimeElementContainer.Modifiers)
                    {
                        elementClass.InvokeStaticMethod(modifierMethod, new object[] {gameplayInjector, periodConfig});
                    }
                }
            }

            return gameplayInjector;
        }

        #endregion

        #region 运行阶段

        /// <summary>
        /// 存活元素总表
        /// </summary>
        public List<Element> AliveElements;

        /// <summary>
        /// 存活Note表，单独记录以供模拟输入
        /// </summary>
        public List<Note> AliveNotes;

        /// <summary>
        /// 存储存活中的非派生元素和创生使用的Injector的关系表
        /// </summary>
        public Dictionary<Element, Injector> AliveNonDerivativeElementInjectorDictionary;

        /// <summary>
        /// 正转定时销毁元素表
        /// </summary>
        public List<Tuple<float, Element>> ForwardTimedDestroyElementObjects;

        /// <summary>
        /// 反转定时销毁元素表
        /// </summary>
        public List<Tuple<float, Element>> BackwardTimedDestroyElementObjects;

        public void StartSimulation()
        {
            AliveElements = new List<Element>();
            AliveNotes = new List<Note>();
            AliveNonDerivativeElementInjectorDictionary = new Dictionary<Element, Injector>();
            ForwardTimedDestroyElementObjects = new List<Tuple<float, Element>>();
            BackwardTimedDestroyElementObjects = new List<Tuple<float, Element>>();

            #region 删除已计算的定时时间

            foreach (var element in ForwardTimedGenerateList)
            {
                element.RemoveCalculatedTime();
            }

            foreach (var element in BackwardTimedGenerateList)
            {
                element.RemoveCalculatedTime();
            }

            #endregion

            #region element初始化创生

            // 执行初始化创生
            foreach (var (injector, constructor) in InitializeGenerate)
            {
                // TODO 这里应该读取正确的初始Autowire参数
                new GenerateElement(injector, constructor,
                        new GenerateElementAutowireValues(false, false, SimulateDirection.Infinitesimal))
                    .DoAction(_runtime, null);
            }
            // TODO 暂时考虑不销毁lane
            // TODO 如果要销毁，注意销毁附着的note

            #endregion
        }

        public void StopSimulation()
        {
            // 注销所有现存Element
            var destroyActions = AliveElements.Select(element => new DestroyElement(element)).Cast<IGameplayAction>()
                .ToList();

            foreach (var action in destroyActions)
            {
                action.DoAction(_runtime, null);
            }

            AliveElements = null;
            AliveNotes = null;
            AliveNonDerivativeElementInjectorDictionary = null;
            ForwardTimedDestroyElementObjects = null;
            BackwardTimedDestroyElementObjects = null;
        }

        #region 暂存

        /// <summary>
        ///     获取Note所在轨道，实例取实例
        /// </summary>
        /// <param name="noteClassName"></param>
        /// <param name="laneName"></param>
        /// <returns>轨道实例，不存在则返回null</returns>
        /// <exception cref="Exception"></exception>
        public GorgeObject? FindLaneSimulatorByNote(string noteClassName, string laneName)
        {
            var requiredLaneType = noteClassName switch
            {
                "Tap" or "Catch" or "Hold" => "Line",
                "SkyTap" or "Slider" => "SkyArea",
                _ => throw new Exception("Note类型未定义")
            };

            return AliveElements.Find(lane =>
                lane.RealObject.GorgeClass.Declaration.Name == requiredLaneType &&
                lane.RealObject.GetStringField("name") == laneName);
        }

        /// <summary>
        ///     获取Note所在轨道
        /// TODO 这应该是Note的一个方法，暂时放在这里
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">如果获取失败，则抛出Linq相关异常</exception>
        public static GorgeObject Lane(GorgeObject note)
        {
            return RuntimeStatic.Runtime.SimulationRuntime.Chart.FindLaneSimulatorByNote(
                note.RealObject.GorgeClass.Declaration.Name,
                note.GetStringField("laneName"));
        }

        #endregion

        #endregion
    }
}