using System;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeLanguage.Objective;

namespace Gorge.Native.GorgeFramework
{
    public partial class Logger
    {
        private static partial Annotation[] ClassAnnotations() => Array.Empty<Annotation>();

        public static partial void Log(string info)
        {
            Base.Instance.Log(info);
        }
    }
}