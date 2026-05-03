using System.Linq;
using Gorge.Native.Gorge;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Chart
{
    public interface IPeriod
    {
        public string MethodName { get; set; }

        public PeriodConfig Config { get; }

        public Injector ConfigInjector { get; }

        /// <summary>
        /// 更新乐段设置
        /// </summary>
        /// <param name="injector"></param>
        public void UpdateConfig(Injector injector);

        public IPeriod Clone();
    }

    public abstract class Period : IPeriod
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public string MethodName { get; set; }

        public PeriodConfig Config { get; private set; }

        public Injector ConfigInjector { get; private set; }

        protected Period(string methodName, Injector configInjector)
        {
            MethodName = methodName;
            UpdateConfig(configInjector);
        }

        public void UpdateConfig(Injector injector)
        {
            ConfigInjector = injector;
            var constructorId = injector.InjectedClassDeclaration.Constructors.First(c => c.Parameters.Length == 0).Id;
            Config = PeriodConfig.FromGorgeObject(ConfigInjector.Instantiate(constructorId));
        }

        public abstract string ToGorgeCode(int indentation);

        IPeriod IPeriod.Clone() => Clone();

        public abstract Period Clone();
    }
}