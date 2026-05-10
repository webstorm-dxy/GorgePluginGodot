using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Gorge.GorgeCompiler;
using Gorge.GorgeCompiler.CompileContext;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Chart;
using Gorge.GorgeFramework.Runtime.Environment;
using Gorge.GorgeLanguage.Objective;
using Gorge.GorgeLanguage.Serialization;

namespace Gorge.GorgeFramework.Runtime
{
    /// <summary>
    /// 运行时管理器
    /// </summary>
    public class RuntimeManager
    {
        public RuntimeState State { get; private set; }
        public GorgeLanguageRuntime LanguageRuntime { get; private set; }
        public RuntimeFormContainer FormContainer { get; private set; }
        public List<Package> PackageList { get; private set; }
        public SimulationScore Score { get; private set; }
        public GorgeSimulationRuntime SimulationRuntime { get; private set; }
        
        public RuntimeManager()
        {
            State = RuntimeState.Uninitialized;
        }

        /// <summary>
        /// 编译源码包
        /// </summary>
        /// <param name="sourcePackages"></param>
        /// <returns></returns>
        public static ClassImplementationContext Compile(List<Package> sourcePackages)
        {
            return Compiler.Compile(sourcePackages.SelectMany(sourcePackage => sourcePackage.SourceCodeFiles));
        }

        /// <summary>
        /// 准备运行时环境
        /// </summary>
        /// <param name="packages"></param>
        public ClassImplementationContext CreateLanguageRuntime(List<Package> packages)
        {
            PackageList = new List<Package>();
            PackageList.AddRange(packages);

            // Try loading compiled bytecode from cache first
            var compiledContext = LoadFromCache(PackageList);
            if (compiledContext != null)
            {
                LanguageRuntime = new GorgeLanguageRuntime(GorgeNative.GorgeNativeImplementationBase(), compiledContext);
                GorgeLanguageRuntime.Instance = LanguageRuntime;
                FormContainer = new RuntimeFormContainer(LanguageRuntime);
                State = RuntimeState.Compiled;
                return null; // No ClassImplementationContext returned when loading from cache
            }

            // Cache miss — compile from source
            var freshContext = Compile(PackageList);
            compiledContext = freshContext;

            // Save compiled bytecode to cache for next run
            SaveToCache(PackageList, compiledContext);

            LanguageRuntime = new GorgeLanguageRuntime(GorgeNative.GorgeNativeImplementationBase(), compiledContext);

            GorgeLanguageRuntime.Instance = LanguageRuntime;

            FormContainer = new RuntimeFormContainer(LanguageRuntime);

            State = RuntimeState.Compiled;

            return freshContext;
        }

        /// <summary>
        /// 提取运行时资源
        /// </summary>
        public void ExtractSimulationResources()
        {
            if (State is RuntimeState.Uninitialized)
            {
                throw new Exception("尝试在Gorge语言运行时准备完成前提取仿真资源");
            }

            Score = new SimulationScore(-1, StaticConfig.TerminateTime, 1);

            // 提取资产文件
            foreach (var package in PackageList)
            {
                Score.ExtractAssetsFromPackage(package);
            }

            PrepareScore();

            State = RuntimeState.SimulationResourceLoaded;
        }

        /// <summary>
        /// 重新加载资源，但不从LanguageRuntime提取谱表
        /// </summary>
        public void ReloadAssets()
        {
            if (Score == null)
            {
                return;
            }

            // 自动将资产文件处理为资产注入器
            Score.AddFileAsset();

            // 加载所有资产
            Score.LoadAssets();

            // 加载即时音频
            Score.LoadInstantAudio();
        }

        public void PrepareScore()
        {
            if (Score == null)
            {
                return;
            }

            // 自动将资产文件处理为资产注入器
            Score.AddFileAsset();

            // 加载所有资产
            Score.LoadAssets();

            // 提取谱表
            Score.ExtractStaveFromRuntime(LanguageRuntime);

            // 加载即时音频
            Score.LoadInstantAudio();
        }

        /// <summary>
        /// 准备仿真环境
        /// </summary>
        public void CreateSimulationRuntime(Action onTerminate = null)
        {
            if (State is RuntimeState.Uninitialized or RuntimeState.Compiled)
            {
                throw new Exception("尝试在运行时资源提取完成前准备仿真环境");
            }

            SimulationRuntime = Base.Instance.CreateSimulationRuntime(onTerminate);

            // SimulationRuntime = new GorgeSimulationRuntime(gameplayPanel, renderBase, onTerminate);

            State = RuntimeState.SimulationInitialized;
        }

        public void DestructSimulationRuntime()
        {
            if (State is RuntimeState.Simulating)
            {
                StopSimulation();
            }

            if (State is RuntimeState.ScoreLoaded)
            {
                UnloadScore();
            }

            if (State is not RuntimeState.SimulationInitialized)
            {
                return;
            }

            SimulationRuntime = null;

            State = RuntimeState.SimulationResourceLoaded;
        }

        public void LoadScore()
        {
            if (State is RuntimeState.Uninitialized or RuntimeState.Compiled or RuntimeState.SimulationResourceLoaded)
            {
                throw new Exception("尝试在仿真环境准备完成前开始仿真");
            }

            SimulationRuntime.LoadScore();
            State = RuntimeState.ScoreLoaded;
        }

        public void UnloadScore()
        {
            if (State is RuntimeState.Simulating)
            {
                StopSimulation();
            }

            if (State is not RuntimeState.ScoreLoaded)
            {
                return;
            }

            SimulationRuntime.UnloadScore();
            State = RuntimeState.SimulationInitialized;
        }

        /// <summary>
        /// 启动仿真
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void StartSimulation()
        {
            if (State is RuntimeState.Uninitialized or RuntimeState.Compiled or RuntimeState.SimulationResourceLoaded
                or RuntimeState.SimulationInitialized)
            {
                throw new Exception("尝试在谱面加载完成前开始仿真");
            }

            SimulationRuntime.StartSimulation();
            State = RuntimeState.Simulating;
        }

        public void StopSimulation()
        {
            if (State is RuntimeState.Uninitialized or RuntimeState.Compiled or RuntimeState.SimulationResourceLoaded
                or RuntimeState.SimulationInitialized)
            {
                return;
            }

            SimulationRuntime.StopSimulation();
            State = RuntimeState.SimulationInitialized;
        }

        #region Bytecode Cache

        /// <summary>
        /// Compute SHA256 hash of all source file paths + contents across packages.
        /// Returns a hex string used as the cache key.
        /// </summary>
        private static string ComputeSourceHash(List<Package> packages)
        {
            using var sha = SHA256.Create();
            // Hash each source file in a deterministic order
            foreach (var pkg in packages)
            {
                foreach (var src in pkg.SourceCodeFiles.OrderBy(s => s.Path))
                {
                    var pathBytes = Encoding.UTF8.GetBytes(src.Path);
                    sha.TransformBlock(pathBytes, 0, pathBytes.Length, null, 0);
                    var codeBytes = Encoding.UTF8.GetBytes(src.Code);
                    sha.TransformBlock(codeBytes, 0, codeBytes.Length, null, 0);
                }
            }
            sha.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
            return Convert.ToHexString(sha.Hash).ToLowerInvariant();
        }

        /// <summary>
        /// Try to load a previously cached .gorge bytecode file.
        /// Returns null if cache is not configured, doesn't exist, or is stale.
        /// </summary>
        private static IImplementationBase LoadFromCache(List<Package> packages)
        {
            if (string.IsNullOrEmpty(StaticConfig.CacheDirectory))
                return null;

            var hash = ComputeSourceHash(packages);
            var cachePath = GetCacheFilePath(hash);

            if (!File.Exists(cachePath))
                return null;

            try
            {
                return GorgeBinaryReader.ReadFromFile(cachePath, GorgeNative.GorgeNativeImplementationBase());
            }
            catch (Exception)
            {
                // Corrupt cache — delete and fall through to recompilation
                try { File.Delete(cachePath); } catch { /* best-effort */ }
                return null;
            }
        }

        /// <summary>
        /// Save compiled context as .gorge bytecode for future runs.
        /// Only saves if CacheDirectory is configured.
        /// </summary>
        private static void SaveToCache(List<Package> packages, IImplementationBase context)
        {
            if (string.IsNullOrEmpty(StaticConfig.CacheDirectory))
                return;

            var hash = ComputeSourceHash(packages);
            var cachePath = GetCacheFilePath(hash);

            try
            {
                var dir = Path.GetDirectoryName(cachePath);
                if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                GorgeBinaryWriter.WriteToFile(context, cachePath);
            }
            catch (Exception)
            {
                // Cache write failure is non-fatal — compilation succeeded, just can't persist
            }
        }

        /// <summary>
        /// Get the filesystem path to the .gorge cache file for the given source hash.
        /// </summary>
        private static string GetCacheFilePath(string hash)
        {
            return Path.Combine(StaticConfig.CacheDirectory, $"{hash}.gorge");
        }

        /// <summary>
        /// Compile the given packages to .gorge bytecode and save to cache.
        /// Returns the cache file path on success, or null on failure.
        /// This bypasses the cache — always recompiles from source.
        /// </summary>
        public static string CompilePackagesToCache(List<Package> packages)
        {
            if (string.IsNullOrEmpty(StaticConfig.CacheDirectory))
                throw new InvalidOperationException("CacheDirectory is not configured. Set StaticConfig.CacheDirectory first.");

            var context = Compile(packages);
            SaveToCache(packages, context);

            var hash = ComputeSourceHash(packages);
            var cachePath = GetCacheFilePath(hash);
            return File.Exists(cachePath) ? cachePath : null;
        }

        #endregion
    }

    /// <summary>
    /// 运行时状态
    /// </summary>
    public enum RuntimeState
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        Uninitialized,

        /// <summary>
        /// 编译完成
        /// </summary>
        Compiled,

        /// <summary>
        /// 仿真资源加载完成
        /// </summary>
        SimulationResourceLoaded,

        /// <summary>
        /// 仿真初始化完成
        /// </summary>
        SimulationInitialized,

        /// <summary>
        /// 谱面加载完成
        /// </summary>
        ScoreLoaded,

        /// <summary>
        /// 仿真中
        /// </summary>
        Simulating
    }
}