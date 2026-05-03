using System;
using System.Collections.Generic;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Input;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeFramework.Signal;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Simulators
{
    /// <summary>
    ///     Gameplay控制动作
    /// </summary>
    public interface IGameplayAction
    {
        /// <summary>
        ///     执行动作
        /// </summary>
        /// <param name="runtime"></param>
        /// <param name="edgeQueue">复合步待决边沿队列</param>
        public void DoAction(GorgeSimulationRuntime runtime, MultichannelEdgeQueue edgeQueue);

        /// <summary>
        /// 该动作是否会触发自动机状态变换
        /// </summary>
        public bool ChangeAutomaton { get; }

        /// <summary>
        /// 该动作是否触发信号变换
        /// </summary>
        public bool ChangeSignal { get; }
    }

    public class GenerateElementAutowireValues
    {
        public bool IsAutoPlay { get; }
        public bool IsReverse { get; }
        public SimulateDirection SimulateDirection { get; }

        public GenerateElementAutowireValues(bool isAutoPlay, bool isReverse, SimulateDirection simulateDirection)
        {
            IsAutoPlay = isAutoPlay;
            IsReverse = isReverse;
            SimulateDirection = simulateDirection;
        }

        /// <summary>
        /// 根据构造方法的形参表生成自动注入的实参表
        /// </summary>
        /// <param name="constructor"></param>
        /// <returns></returns>
        public object[] AutowiredArguments(ConstructorInformation constructor)
        {
            var arguments = new List<object>();
            foreach (var parameter in constructor.Parameters)
            {
                switch (parameter.Name)
                {
                    case "isAutoPlay":
                        arguments.Add(IsAutoPlay);
                        break;
                    case "isReverse":
                        arguments.Add(IsReverse);
                        break;
                    case "simulateSpeed":
                        // TODO
                        throw new Exception();
                    default:
                        throw new Exception($"无法自动注入名为{parameter.Name}的参数");
                }
            }

            return arguments.ToArray();
        }
    }

    public class GenerateElement : IGameplayAction
    {
        private readonly Injector _elementInjector;
        private readonly ConstructorInformation _constructorInformation;
        private readonly GenerateElementAutowireValues _autowireValues;

        public GenerateElement(Injector elementInjector, ConstructorInformation constructorInformation,
            GenerateElementAutowireValues autowireValues)
        {
            _elementInjector = elementInjector;
            _constructorInformation = constructorInformation;
            _autowireValues = autowireValues;

            ChangeAutomaton = _elementInjector.InjectedClassDeclaration.Is("GorgeFramework.Note");
        }

        public void DoAction(GorgeSimulationRuntime runtime, MultichannelEdgeQueue edgeQueue)
        {
            var arguments = _autowireValues.AutowiredArguments(_constructorInformation);
            var element = _elementInjector.Instantiate(_constructorInformation.Id, arguments);
            var elementNative = Element.FromGorgeObject(element);

            // 注册到总表中
            runtime.Chart.AliveElements.Add(elementNative);
            runtime.Chart.AliveNonDerivativeElementInjectorDictionary.Add(elementNative, _elementInjector);

            // 注册模拟器
            var simulator = elementNative.simulator;
            if (simulator != null)
            {
                runtime.Simulation.Simulators.Register(10, simulator);
            }

            var lateIndependentSimulator = elementNative.lateIndependentSimulator;
            if (lateIndependentSimulator != null)
            {
                runtime.Simulation.LateIndependentSimulators.Register(10, lateIndependentSimulator);
            }

            // 判断是否为note，如果是则注册自动机
            if (element.GorgeClass.Declaration.Is("GorgeFramework.Note"))
            {
                // 目前自动机注册是合并的，TODO 考虑改成动作派生模式？
                var note = Note.FromGorgeObject(element);
                var automaton = note.automaton;
                if (automaton != null)
                {
                    runtime.Automaton.Automatons.Add(automaton);
                    runtime.Automaton.PendingDetectionConditions[automaton] =
                        automaton.GetDetectionConditions(_autowireValues.SimulateDirection);
                }

                runtime.Chart.AliveNotes.Add(note);
            }

            #region 注册定时销毁

            // TODO 合并检索
            // TODO 这些信息可能可以在编译时提取，例如提供注解的反向索引，避免即时搜索

            MethodInformation forwardDestroyTime = null;
            foreach (var method in element.RealObject.GorgeClass.Declaration.Methods)
            {
                foreach (var annotation in method.Annotations)
                {
                    if (annotation.Name is "ForwardTimedDestroy")
                    {
                        forwardDestroyTime = method;
                        break;
                    }
                }

                if (forwardDestroyTime != null)
                {
                    break;
                }
            }

            if (forwardDestroyTime != null)
            {
                // TODO 目前不检查参数，默认无参，返回float
                // TODO 如果不考虑自动注入，那么可能可以在编译阶段检查
                var time = (float) element.RealObject.InvokeMethod(forwardDestroyTime);
                runtime.Chart.ForwardTimedDestroyElementObjects.Add(new Tuple<float, Element>(time,
                    elementNative));
            }

            MethodInformation backwardDestroyTime = null;
            foreach (var method in element.RealObject.GorgeClass.Declaration.Methods)
            {
                foreach (var annotation in method.Annotations)
                {
                    if (annotation.Name is "BackwardTimedDestroy")
                    {
                        backwardDestroyTime = method;
                        break;
                    }
                }

                if (backwardDestroyTime != null)
                {
                    break;
                }
            }

            if (backwardDestroyTime != null)
            {
                // TODO 目前不检查参数，默认无参，返回float
                // TODO 如果不考虑自动注入，那么可能可以在编译阶段检查
                var time = (float) element.RealObject.InvokeMethod(backwardDestroyTime);
                runtime.Chart.BackwardTimedDestroyElementObjects.Add(new Tuple<float, Element>(time,
                    elementNative));
            }

            #endregion

            #region 注册图形节点

            var nodes = elementNative.nodes;
            if (nodes != null)
            {
                for (var i = 0; i < nodes.length; i++)
                {
                    var node = Node.FromGorgeObject(nodes.Get(i));
                    RuntimeStatic.Runtime.SimulationRuntime.Graphics.Nodes.Add(node);
                }
            }

            #endregion
        }

        public bool ChangeAutomaton { get; }

        public bool ChangeSignal => false;
    }

    public class DestroyElement : IGameplayAction
    {
        private readonly Element _element;

        public DestroyElement(Element element)
        {
            _element = element;
            ChangeAutomaton = _element is Note;
        }

        public void DoAction(GorgeSimulationRuntime runtime, MultichannelEdgeQueue edgeQueue)
        {
            // 判断是否为note，如果是则注销自动机
            if (_element is Note note)
            {
                // 目前自动机销毁是合并的
                var automaton = note.automaton;
                if (automaton != null)
                {
                    runtime.Automaton.PendingDetectionConditions.Remove(automaton);
                    runtime.Automaton.Automatons.Remove(automaton);
                }

                runtime.Chart.AliveNotes.Remove(note);
            }

            // 注销模拟器
            var simulator = _element.simulator;
            if (simulator != null)
            {
                runtime.Simulation.Simulators.Remove(simulator);
            }

            var lateIndependentSimulator = _element.lateIndependentSimulator;
            if (lateIndependentSimulator != null)
            {
                runtime.Simulation.LateIndependentSimulators.Remove(lateIndependentSimulator);
            }

            // 从统一表中注销
            runtime.Chart.AliveElements.Remove(_element);
            runtime.Chart.AliveNonDerivativeElementInjectorDictionary.Remove(_element); // 由于不记录派生，所以key不一定存在

            // TODO 可能考虑把注销定时销毁合并到此处

            #region 注销图形节点

            var nodes = _element.nodes;
            if (nodes != null)
            {
                for (var i = 0; i < nodes.length; i++)
                {
                    var node = Node.FromGorgeObject(nodes.Get(i));
                    node.Destroy();
                    RuntimeStatic.Runtime.SimulationRuntime.Graphics.Nodes.Remove(node);
                }
            }

            #endregion
        }

        public bool ChangeAutomaton { get; }
        public bool ChangeSignal => false;
    }

    /// <summary>
    /// 派生element
    /// 由派生者及时构造，并在动作执行时调用DeriveGenerate方法(如有)
    /// </summary>
    public class DeriveElement : IGameplayAction
    {
        private readonly Element _elementObject;
        private readonly SimulateDirection _simulateDirection;

        public DeriveElement(Element elementObject, bool changeAutomaton, SimulateDirection simulateDirection)
        {
            _elementObject = elementObject;
            _simulateDirection = simulateDirection;
            ChangeAutomaton = changeAutomaton;
        }

        public void DoAction(GorgeSimulationRuntime runtime, MultichannelEdgeQueue edgeQueue)
        {
            #region 如果有DeriveGenerate方法，则执行

            // TODO 合并检索
            // TODO 这些信息可能可以在编译时提取，例如提供注解的反向索引，避免即时搜索

            MethodInformation generateMethod = null;
            foreach (var method in _elementObject.RealObject.GorgeClass.Declaration.Methods)
            {
                foreach (var annotation in method.Annotations)
                {
                    if (annotation.Name is "DeriveGenerate")
                    {
                        generateMethod = method;
                        break;
                    }
                }

                if (generateMethod != null)
                {
                    break;
                }
            }

            if (generateMethod != null)
            {
                // TODO 目前不检查参数，默认无参，无返回
                // TODO 如果不考虑自动注入，那么可能可以在编译阶段检查
                _elementObject.RealObject.InvokeMethod(generateMethod);
            }

            #endregion

            // 注册到总表中
            runtime.Chart.AliveElements.Add(_elementObject);

            // 注册模拟器
            var simulator = (ISimulator) _elementObject.RealObject.GetObjectField("simulator");
            runtime.Simulation.Simulators.Register(10, simulator);

            // 判断是否为note，如果是则注册自动机
            if (_elementObject.GorgeClass.Declaration.Is("GorgeFramework.Note"))
            {
                // 目前自动机注册是合并的，TODO 考虑改成动作派生模式？
                var note = Note.FromGorgeObject(_elementObject);
                var automaton = note.automaton;
                runtime.Automaton.Automatons.Add(automaton);
                runtime.Automaton.PendingDetectionConditions[automaton] =
                    automaton.GetDetectionConditions(_simulateDirection);
                runtime.Chart.AliveNotes.Add(note);
            }

            #region 注册定时销毁

            // TODO 合并检索
            // TODO 这些信息可能可以在编译时提取，例如提供注解的反向索引，避免即时搜索

            MethodInformation forwardDestroyTime = null;
            foreach (var method in _elementObject.RealObject.GorgeClass.Declaration.Methods)
            {
                foreach (var annotation in method.Annotations)
                {
                    if (annotation.Name is "ForwardTimedDestroy")
                    {
                        forwardDestroyTime = method;
                        break;
                    }
                }

                if (forwardDestroyTime != null)
                {
                    break;
                }
            }

            if (forwardDestroyTime != null)
            {
                // TODO 目前不检查参数，默认无参，返回float
                // TODO 如果不考虑自动注入，那么可能可以在编译阶段检查
                var time = (float) _elementObject.RealObject.InvokeMethod(forwardDestroyTime);
                runtime.Chart.ForwardTimedDestroyElementObjects.Add(new Tuple<float, Element>(time,
                    _elementObject));
            }

            MethodInformation backwardDestroyTime = null;
            foreach (var method in _elementObject.RealObject.GorgeClass.Declaration.Methods)
            {
                foreach (var annotation in method.Annotations)
                {
                    if (annotation.Name is "BackwardTimedDestroy")
                    {
                        backwardDestroyTime = method;
                        break;
                    }
                }

                if (backwardDestroyTime != null)
                {
                    break;
                }
            }

            if (backwardDestroyTime != null)
            {
                // TODO 目前不检查参数，默认无参，返回float
                // TODO 如果不考虑自动注入，那么可能可以在编译阶段检查
                var time = (float) _elementObject.RealObject.InvokeMethod(backwardDestroyTime);
                runtime.Chart.BackwardTimedDestroyElementObjects.Add(new Tuple<float, Element>(time,
                    _elementObject));
            }

            #endregion

            #region 注册图形节点

            var nodes = _elementObject.nodes;
            if (nodes != null)
            {
                for (var i = 0; i < nodes.length; i++)
                {
                    var node = Node.FromGorgeObject(nodes.Get(i));
                    RuntimeStatic.Runtime.SimulationRuntime.Graphics.Nodes.Add(node);
                }
            }

            #endregion
        }

        public bool ChangeAutomaton { get; }
        public bool ChangeSignal => false;
    }

    /// <summary>
    /// 在当前时间点追加信号边沿
    /// </summary>
    public class AppendSignal : IGameplayAction
    {
        public string ChannelName { get; }
        public int SignalId { get; }
        public float DelaySimulateTime { get; }
        public GorgeObject Value { get; }

        public AppendSignal(string channelName, int signalId, float delaySimulateTime, GorgeObject value)
        {
            ChannelName = channelName;
            SignalId = signalId;
            DelaySimulateTime = delaySimulateTime;
            Value = value;
        }

        public void DoAction(GorgeSimulationRuntime runtime, MultichannelEdgeQueue edgeQueue)
        {
            var addSuccess = runtime.Automaton.AddSignalEdge(ChannelName, SignalId,
                runtime.Simulation.SimulationMachine.SimulateTime + DelaySimulateTime, Value);

            if (edgeQueue != null && addSuccess && DelaySimulateTime == 0)
            {
                edgeQueue.Enqueue(ChannelName, SignalId, new Edge<GorgeObject>()
                {
                    Time = runtime.Simulation.SimulationMachine.SimulateTime,
                    Value = Value
                });
            }
        }

        public bool ChangeAutomaton => false;
        public bool ChangeSignal => true;
    }

    /// <summary>
    ///     更新某自动机的待决检测项
    /// </summary>
    public class UpdatePendingDetectionCondition : IGameplayAction
    {
        private readonly SignalTsiga _automaton;
        private readonly SimulateDirection _simulateDirection;

        public UpdatePendingDetectionCondition(SignalTsiga automaton, SimulateDirection simulateDirection)
        {
            _automaton = automaton;
            _simulateDirection = simulateDirection;
        }

        public void DoAction(GorgeSimulationRuntime runtime, MultichannelEdgeQueue edgeQueue)
        {
            runtime.Automaton.PendingDetectionConditions[_automaton] =
                _automaton.GetDetectionConditions(_simulateDirection);
        }

        public bool ChangeAutomaton => true;
        public bool ChangeSignal => false;
    }

    /// <summary>
    ///     谱面终结动作
    /// </summary>
    public class Terminate : IGameplayAction
    {
        public void DoAction(GorgeSimulationRuntime runtime, MultichannelEdgeQueue edgeQueue)
        {
            Base.Instance.Log("Terminated");
            runtime.OnTerminate?.Invoke();
        }

        public bool ChangeAutomaton => false;
        public bool ChangeSignal => false;
    }
}