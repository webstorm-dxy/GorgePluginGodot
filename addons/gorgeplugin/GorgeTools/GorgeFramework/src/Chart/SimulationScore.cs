using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Gorge.GorgeCompiler;
using Gorge.GorgeFramework.Adaptor;
using Gorge.GorgeFramework.Runtime;
using Gorge.GorgeLanguage.Objective;
using Gorge.Native.Gorge;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Chart
{
    /// <summary>
    /// Gorge仿真总谱
    /// 包含来自所有模态和谱面的元素和资源信息
    /// 向上与运行时环境对接，提供初始化依据
    /// 向下与谱面文件互相转换
    /// </summary>
    public class SimulationScore
    {
        #region 谱面协调参数（跨模态唯一）

        #region 仿真初始化参数

        /// <summary>
        /// 仿真起点
        /// </summary>
        public readonly float StartTime;

        /// <summary>
        /// 仿真终点
        /// </summary>
        public readonly float TerminateTime;

        /// <summary>
        /// 仿真倍速
        /// </summary>
        public readonly float SimulationSpeed;

        #endregion

        #endregion

        #region 资源

        /*
         * 考虑两种方式，资源加载和资源生成，资源生成没有原文件
         * 资源覆盖顺序：谱面 > 模态 > Native
         * 按资源名覆盖
         * 加载器/生成器和文件是独立覆盖的
         * 资源所在覆盖层次由加载器/生成器决定
         *   谱面加载器重新加载模态资源认为是谱面级
         *   模态加载谱面提供的新文件认为是模态级
         * 也就是加载器/生成器看到的是覆盖后的文件视图
         */

        /// <summary>
        /// 谱面资源文件表
        /// </summary>
        public readonly List<AssetFile> ChartAssetFiles;

        /// <summary>
        /// 资源加载器表
        /// </summary>
        public readonly List<AssetLoader> AssetLoaders;

        /// <summary>
        /// 即时播放音效
        /// </summary>
        public readonly Dictionary<string, AudioAsset> InstantAudio;

        /// <summary>
        /// 已加载的资源
        /// </summary>
        public readonly Dictionary<string, Asset> LoadedAssets;

        #endregion

        /// <summary>
        /// 谱表列表
        /// </summary>
        public readonly List<IStaff> Stave;

        public SimulationScore(float startTime, float terminateTime, float simulationSpeed)
        {
            StartTime = startTime;
            TerminateTime = terminateTime;
            SimulationSpeed = simulationSpeed;
            Stave = new List<IStaff>();
            ChartAssetFiles = new List<AssetFile>();
            AssetLoaders = new List<AssetLoader>();
            InstantAudio = new Dictionary<string, AudioAsset>();
            LoadedAssets = new Dictionary<string, Asset>();
        }

        public static SimulationScore LoadScoreFromElementList(string formName, List<Injector> elementInjectors,
            List<Injector> assetInjectors, float startTime, float terminateTime, float simulationSpeed)
        {
            var score = new SimulationScore(startTime, terminateTime, simulationSpeed);

            var staff = new ElementStaff("Chart", true, "Chart", formName);
            score.Stave.Add(staff);

            var config = new PeriodConfig.SpecificInjector();
            config.timeOffset = 0;
            var period = new ElementPeriod("Period", config);
            foreach (var element in elementInjectors)
            {
                period.Elements.Add(element);
            }

            staff.AddPeriod(period);

            if (assetInjectors.Count > 0)
            {
                var assetLoader = new AssetLoader("Asset", true);
                score.AssetLoaders.Add(assetLoader);

                var assetSet = new AssetSet("AssetSet");
                assetLoader.AssetSets.Add(assetSet);

                foreach (var asset in assetInjectors)
                {
                    assetSet.Assets.Add(asset);
                }
            }

            return score;
        }

        /// <summary>
        /// 从Gorge语言运行时中提取谱表
        /// </summary>
        /// <returns></returns>
        public void ExtractStaveFromRuntime(GorgeLanguageRuntime runtime)
        {
            Stave.Clear();

            foreach (var gorgeClass in runtime.Classes)
            {
                if (gorgeClass.Declaration.TryGetAnnotationByName("AudioStaff", out var audioStaffAnnotation))
                {
                    var className = gorgeClass.Declaration.Name;
                    var displayName = className;
                    if (audioStaffAnnotation.TryGetMetadata("displayName", out var displayNameObject))
                    {
                        displayName = (string) displayNameObject.Value;
                    }

                    var staff = new AudioStaff(className, true, displayName);
                    Stave.Add(staff);

                    foreach (var method in gorgeClass.Declaration.StaticMethods)
                    {
                        if (method.TryGetAnnotationByName("Song", out var songAnnotation))
                        {
                            if (!songAnnotation.TryGetMetadata("config", out var configMetadata))
                            {
                                throw new Exception("Song注解没有名为config的元数据");
                            }

                            var periodConfig = (Injector) configMetadata.Value;
                            var audio = (Injector) gorgeClass.InvokeStaticMethod(method, Array.Empty<object>());
                            var period = new AudioPeriod(method.Name, periodConfig, audio);
                            staff.Periods.Add(period);
                        }
                    }
                }
                else if (gorgeClass.Declaration.TryGetAnnotationByName("ElementStaff", out var elementStaffAnnotation))
                {
                    var className = gorgeClass.Declaration.Name;
                    var displayName = className;
                    if (elementStaffAnnotation.TryGetMetadata("displayName", out var displayNameObject))
                    {
                        displayName = (string) displayNameObject.Value;
                    }

                    if (!elementStaffAnnotation.TryGetMetadata("form", out var formNameObject))
                    {
                        throw new Exception("ElementStaff注解没有名为form的元数据");
                    }

                    var formName = (string) formNameObject.Value;

                    var staff = new ElementStaff(className, true, displayName, formName);
                    Stave.Add(staff);

                    foreach (var method in gorgeClass.Declaration.StaticMethods)
                    {
                        if (method.TryGetAnnotationByName("Chart", out var chartAnnotation))
                        {
                            if (!chartAnnotation.TryGetMetadata("config", out var configMetadata))
                            {
                                throw new Exception("Chart注解没有名为config的元数据");
                            }

                            var periodConfig = (Injector) configMetadata.Value;
                            var block = new ElementPeriod(method.Name, periodConfig);
                            staff.Periods.Add(block);
                            var elementInjectorArray =
                                (ObjectArray) gorgeClass.InvokeStaticMethod(method, Array.Empty<object>());
                            for (var i = 0; i < elementInjectorArray.length; i++)
                            {
                                block.Elements.Add((Injector) elementInjectorArray.Get(i));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 从资源包中提取资产
        /// </summary>
        /// <param name="package"></param>
        public void ExtractAssetsFromPackage(Package package)
        {
            ChartAssetFiles.AddRange(package.AssetFiles);
        }

        /// <summary>
        /// 从资源文件中自动加载资源到命名资源表中
        /// </summary>
        public void AddFileAsset()
        {
            AssetLoaders.Clear();

            var assetLoader = new AssetLoader("AutoLoaded", false);
            AssetLoaders.Add(assetLoader);
            var assetSet = new AssetSet("AutoLoaded");
            assetLoader.AssetSets.Add(assetSet);

            foreach (var assetFile in ChartAssetFiles)
            {
                var path = assetFile.Path;
                var extension = Path.GetExtension(path);
                if (extension is ".png" or ".jpg")
                {
                    var imageInjector = ImageAsset.EmptyInjector();
                    imageInjector.name = "image:" + path[..^extension.Length];
                    imageInjector.texture = Base.Instance.CreateGraph(path, assetFile.Data);
                    assetSet.Assets.Add(imageInjector);
                }
                else if (extension is ".wav" or ".mp3" or ".ogg")
                {
                    var audioInjector = NativeAudioAsset.EmptyInjector();
                    audioInjector.name = "audio:" + path[..^extension.Length];
                    audioInjector.audio = Base.Instance.CreateAudio(path, assetFile.Data);
                    assetSet.Assets.Add(audioInjector);
                }
                else if (extension is ".mp4")
                {
                    Base.Instance.Warning("MP4资源功能暂时被关闭");
                    // TODO 为了保持最小移植环境，暂时注释实际功能
                    // var videoInjector = NativeVideoAsset.EmptyInjector();
                    // videoInjector.name = "video:" + path[..^extension.Length];
                    // videoInjector.video = new Video(assetFile.Data, extension);
                    // assetSet.Assets.Add(videoInjector);
                }
            }
        }

        /// <summary>
        /// 加载资源加载器中的全部资源
        /// </summary>
        public void LoadAssets()
        {
            LoadedAssets.Clear();

            foreach (var assetLoader in AssetLoaders)
            {
                foreach (var assetSet in assetLoader.AssetSets)
                {
                    foreach (var assetInjector in assetSet.Assets)
                    {
                        var constructorIndex =
                            assetInjector.InjectedClassDeclaration.Constructors.First(c => c.Parameters.Length == 0).Id;

                        var asset = Asset.FromGorgeObject(assetInjector.Instantiate(constructorIndex));

                        asset.LoadAsset();
                        LoadedAssets.Add(asset.name, asset);
                    }
                }
            }
        }

        public Asset GetAssetByName(string assetName)
        {
            if (LoadedAssets.TryGetValue(assetName, out var asset))
            {
                return asset;
            }

            throw new Exception($"试图获取名为{assetName}的资源，但未加载");
        }

        /// <summary>
        /// 加载响应音效
        /// </summary>
        public void LoadInstantAudio()
        {
            InstantAudio.Clear();

            foreach (var (name, (gorgeClass, method)) in RuntimeStatic.Runtime.FormContainer.InstantAudioMethods)
            {
                var audioAsset =
                    (AudioAsset) gorgeClass.InvokeStaticMethod(method, Array.Empty<object>());
                InstantAudio.Add(name, audioAsset);
            }
        }

        public Package ExportChartPackage()
        {
            var package = new Package();

            foreach (var staff in Stave)
            {
                if (!staff.IsChartClass)
                {
                    continue;
                }

                package.SourceCodeFiles.Add(new SourceCodeFile(staff.ClassName + ".g", staff.ToGorgeCode(), true));
            }

            foreach (var assetLoader in AssetLoaders)
            {
                if (!assetLoader.IsChartClass)
                {
                    continue;
                }

                package.SourceCodeFiles.Add(new SourceCodeFile(assetLoader.ClassName + ".g", assetLoader.ToGorgeCode(),
                    true));
            }

            foreach (var chartAssetFile in ChartAssetFiles)
            {
                if (!chartAssetFile.IsChartAsset)
                {
                    continue;
                }

                package.AssetFiles.Add(chartAssetFile);
            }

            return package;
        }

        public bool TryGetStaff(string staffName, out IStaff staff)
        {
            staff = Stave.FirstOrDefault(s => s.ClassName == staffName);
            return staff != null;
        }

        public bool TryGetPeriod(string staffName, string periodName, out IPeriod period)
        {
            if (TryGetStaff(staffName, out var staff) && staff.TryGetPeriod(periodName, out period))
            {
                return true;
            }

            period = null;
            return false;
        }

        /// <summary>
        /// 检查目标谱表名是否和已有谱表名冲突
        /// </summary>
        /// <returns>true表示冲突，不可添加该名字的乐段</returns>
        public bool CheckStaffNameConflict(string staffNameToInsert)
        {
            // TODO 目前没有检查非谱面的类名
            return Stave.Any(s => s.ClassName == staffNameToInsert);
        }
    }


    /// <summary>
    /// 资源加载器，对应一个Gorge类
    /// </summary>
    public class AssetLoader
    {
        /// <summary>
        /// 资源加载器类名
        /// </summary>
        public readonly string ClassName;

        /// <summary>
        /// 本加载器属于谱面还是模态
        /// 为true则认为是谱面的资源加载器
        /// </summary>
        public readonly bool IsChartClass;

        /// <summary>
        /// 资源组列表
        /// </summary>
        public readonly List<AssetSet> AssetSets;

        public AssetLoader(string className, bool isChartClass)
        {
            ClassName = className;
            IsChartClass = isChartClass;
            AssetSets = new List<AssetSet>();
        }

        /// <summary>
        /// 转换为代码
        /// </summary>
        /// <returns></returns>
        public string ToGorgeCode()
        {
            if (!IsChartClass)
            {
                throw new Exception("尝试将非谱面谱表转化为谱面代码");
            }

            var sb = new StringBuilder();

            sb.AppendLine("@AudioStaff");
            sb.AppendLine($"class {ClassName}");
            sb.AppendLine("{");
            foreach (var assetSet in AssetSets)
            {
                sb.AppendLine(assetSet.ToGorgeCode(1));
                sb.AppendLine();
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

    /// <summary>
    /// 资源组
    /// 对应一个Gorge方法
    /// </summary>
    public class AssetSet
    {
        /// <summary>
        /// 方法名
        /// </summary>
        public readonly string MethodName;

        /// <summary>
        /// 资源表
        /// </summary>
        public readonly List<Injector> Assets;

        public AssetSet(string methodName)
        {
            MethodName = methodName;
            Assets = new List<Injector>();
        }

        /// <summary>
        /// 转换为代码
        /// </summary>
        /// <returns></returns>
        public string ToGorgeCode(int indentation)
        {
            var sb = new StringBuilder();

            sb.AppendLine("@Asset", indentation);
            sb.AppendLine($"static Asset^[] {MethodName}()", indentation);
            sb.AppendLine("{", indentation);
            sb.AppendLine(
                $"return new Asset^[{Assets.Count}]{InjectorHardcodeGenerator.Generate(Element.Class.Declaration.Type, Assets, false, indentation + 1)};",
                indentation + 1);
            sb.AppendLine("}", indentation);

            return sb.ToString();
        }
    }

    /// <summary>
    /// 资源文件
    /// </summary>
    public class AssetFile
    {
        public readonly string Path;

        public readonly byte[] Data;

        public readonly bool IsChartAsset;

        public AssetFile(string path, byte[] data, bool isChartAsset)
        {
            Data = data;
            IsChartAsset = isChartAsset;
            Path = path;
        }
    }
}