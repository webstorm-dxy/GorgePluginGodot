using Gorge.Native;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;

namespace Gorge.GorgeLanguage.VirtualMachine
{
    public static class InvokeParameterPool
    {
        #region 调用参数池

        // TODO 这里的长度取值可以在编译时算出来

        public const int PoolSize = 256;

        public static int[] Int = new int[PoolSize];
        public static float[] Float = new float[PoolSize];
        public static bool[] Bool = new bool[PoolSize];
        public static string[] String = new string[PoolSize];
        public static GorgeObject[] Object = new GorgeObject[PoolSize];

        /// <summary>
        /// 构造方法传递Injector的专用位
        /// Injector作为参数传递使用Object，而不是用Injector位
        /// </summary>
        public static Injector Injector;

        #endregion

        #region 返回值池

        public static int IntReturn;
        public static float FloatReturn;
        public static bool BoolReturn;
        public static string StringReturn;
        public static GorgeObject ObjectReturn;

        #endregion
    }
}