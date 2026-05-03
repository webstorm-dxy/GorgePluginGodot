using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gorge.GorgeCompiler;
using ICSharpCode.SharpZipLib.Zip;

namespace Gorge.GorgeFramework.Chart
{
    /// <summary>
    /// 谱面文件的内存结构
    /// </summary>
    public class Package
    {
        public readonly List<AssetFile> AssetFiles;

        public readonly List<SourceCodeFile> SourceCodeFiles;

        public Package()
        {
            AssetFiles = new List<AssetFile>();
            SourceCodeFiles = new List<SourceCodeFile>();
        }

        /// <summary>
        /// 从文件夹中加载包
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="isChart"></param>
        /// <returns></returns>
        public static Package LoadFolderPackage(string folderPath, bool isChart)
        {
            var di = new DirectoryInfo(folderPath);

            if (!di.Exists)
            {
                throw new Exception("目标文件夹不存在");
            }

            var package = new Package();

            foreach (var file in di.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                var fullPath = file.FullName;
                var relativePath = Path.GetRelativePath(folderPath, file.FullName).Replace('\\', '/');
                if (file.Name.EndsWith(".g"))
                {
                    var codeString = File.ReadAllText(fullPath);
                    var sourceCodeFile = new SourceCodeFile(relativePath, codeString, isChart);
                    package.SourceCodeFiles.Add(sourceCodeFile);
                }
                else
                {
                    var assetData = File.ReadAllBytes(fullPath);
                    var assetFile = new AssetFile(relativePath, assetData, isChart);
                    package.AssetFiles.Add(assetFile);
                }
            }

            return package;
        }

        // 暂不提供从Unity资源系统加载Package
        // public static Package LoadAddressablePackage(string addressableName, bool isChart)
        // {
        //     Addressables.InitializeAsync().WaitForCompletion();
        //
        //     #region 手动过滤文件夹名
        //
        //     var targetKeys = new List<string>();
        //
        //     foreach (var locator in Addressables.ResourceLocators)
        //     {
        //         foreach (var key in locator.Keys)
        //         {
        //             if (key is string stringKey)
        //             {
        //                 if (stringKey.StartsWith(addressableName))
        //                 {
        //                     targetKeys.Add(stringKey);
        //                 }
        //             }
        //         }
        //     }
        //
        //     if (targetKeys.Count == 0)
        //     {
        //         throw new Exception($"{addressableName} Addressables文件夹不存在");
        //     }
        //
        //     #endregion
        //
        //     var package = new Package();
        //
        //     foreach (var location in targetKeys)
        //     {
        //         var loadHandle = Addressables.LoadAssetAsync<TextAsset>(location);
        //         var asset = loadHandle.WaitForCompletion();
        //
        //         if (asset == null)
        //         {
        //             throw new Exception($"{location}加载失败");
        //         }
        //         
        //         if (location.EndsWith(".g"))
        //         {
        //             if (asset is TextAsset textAsset)
        //             {
        //                 var codeString = textAsset.text;
        //                 var sourceCodeFile = new SourceCodeFile(location, codeString, isChart);
        //                 package.SourceCodeFiles.Add(sourceCodeFile);
        //             }
        //             else
        //             {
        //                 throw new Exception($"{location}未接加载为TextAsset");
        //             }
        //         }
        //         else
        //         {
        //             Debug.Log(asset);
        //
        //             
        //             // if (asset is byte[] assetData)
        //             // {
        //             //     var assetFile = new AssetFile(location.PrimaryKey, assetData, isChart);
        //             //     package.AssetFiles.Add(assetFile);
        //             // }
        //             // else
        //             // {
        //             //     Debug.LogWarning($"文件 [{location.PrimaryKey}] 类型错误，期待 byte[]！");
        //             // }
        //         }
        //
        //         Addressables.Release(loadHandle);
        //     }
        //
        //     return package;
        // }

        /// <summary>
        /// 从zip压缩文件中加载代码包
        /// </summary>
        /// <param name="zipFilePath">zip压缩文件地址</param>
        /// <param name="isChart">指示是否为谱面文件</param>
        /// <returns>代码包</returns>
        public static Package LoadZipPackage(string zipFilePath, bool isChart)
        {
            if (!File.Exists(zipFilePath))
            {
                throw new Exception("文件不存在");
            }

            return LoadZipPackage(File.OpenRead(zipFilePath), isChart);
        }
        
        /// <summary>
        /// 从zip压缩文件中加载代码包
        /// </summary>
        /// <param name="zipFileBinary">zip压缩文件字节</param>
        /// <param name="isChart">指示是否为谱面文件</param>
        /// <returns>代码包</returns>
        public static Package LoadZipPackage(byte[] zipFileBinary, bool isChart)
        {
            return LoadZipPackage(new MemoryStream(zipFileBinary), isChart);
        }

        /// <summary>
        /// 从zip压缩文件中加载代码包
        /// </summary>
        /// <param name="zipFileBinary">zip压缩文件字节流</param>
        /// <param name="isChart">指示是否为谱面文件</param>
        /// <returns>代码包</returns>
        public static Package LoadZipPackage(Stream zipFileBinary, bool isChart)
        {
            var package = new Package();

            using var zipStream = new ZipInputStream(zipFileBinary);

            while (zipStream.GetNextEntry() is { } entry)
            {
                if (entry.Name.EndsWith(".g"))
                {
                    using var memoryStream = new MemoryStream();
                    zipStream.CopyTo(memoryStream);
                    var data = memoryStream.ToArray();
                    string codeString;
                    if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
                    {
                        // 跳过 UTF-8 BOM
                        codeString = Encoding.UTF8.GetString(data, 3, data.Length - 3);
                    }
                    else
                    {
                        // 无 BOM，直接转换
                        codeString = Encoding.UTF8.GetString(data);
                    }

                    var sourceCodeFile = new SourceCodeFile(entry.Name, codeString, isChart);
                    package.SourceCodeFiles.Add(sourceCodeFile);
                }
                else
                {
                    using var memoryStream = new MemoryStream();
                    zipStream.CopyTo(memoryStream);
                    var assetFile = new AssetFile(entry.Name, memoryStream.ToArray(), isChart);
                    package.AssetFiles.Add(assetFile);
                }
            }

            return package;
        }

        public void SaveZipPackage(string zipFilePath)
        {
            var outputFile = File.Create(zipFilePath);
            using var s = new ZipOutputStream(outputFile);
            s.SetLevel(6);

            foreach (var sourceCodeFile in SourceCodeFiles)
            {
                if (!sourceCodeFile.IsChartSourceCode)
                {
                    continue;
                }

                var entry = new ZipEntry(sourceCodeFile.Path);
                entry.DateTime = DateTime.Now;

                s.PutNextEntry(entry);

                var textBytes = Encoding.UTF8.GetBytes(sourceCodeFile.Code);
                s.Write(textBytes, 0, textBytes.Length);
            }

            foreach (var assetFile in AssetFiles)
            {
                if (!assetFile.IsChartAsset)
                {
                    continue;
                }

                var entry = new ZipEntry(assetFile.Path);
                entry.DateTime = DateTime.Now;
                s.PutNextEntry(entry);

                s.Write(assetFile.Data, 0, assetFile.Data.Length);
            }
        }
    }
}