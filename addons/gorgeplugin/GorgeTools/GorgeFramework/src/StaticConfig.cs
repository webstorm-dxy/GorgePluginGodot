namespace Gorge.GorgeFramework
{
    public static class StaticConfig
    {
        public static float TerminateTime = 305;
        public static bool IsAutoPlay = true;
        /// <summary>Filesystem path to the .gorge bytecode cache directory. Set before calling RuntimeManager.CreateLanguageRuntime.</summary>
        public static string CacheDirectory;
    }
}