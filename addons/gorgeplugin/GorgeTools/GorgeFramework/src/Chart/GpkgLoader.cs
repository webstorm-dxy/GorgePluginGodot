using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Gorge.GorgeCompiler;
using ICSharpCode.SharpZipLib.Zip;
using Newtonsoft.Json.Linq;

namespace Gorge.GorgeFramework.Chart
{
    public class GpkgLoadResult
    {
        public readonly List<Package> RuntimePackages;
        public readonly Package ChartPackage;

        public GpkgLoadResult(List<Package> runtimePackages, Package chartPackage)
        {
            RuntimePackages = runtimePackages;
            ChartPackage = chartPackage;
        }
    }

    public static class GpkgLoader
    {
        public static bool IsGpkgPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            return path.EndsWith(".gpkg", StringComparison.OrdinalIgnoreCase);
        }

        public static GpkgLoadResult LoadGpkg(string path)
        {
            if (!File.Exists(path))
                throw new Exception("gpkg file not found: " + path);

            using var stream = File.OpenRead(path);
            return LoadGpkg(stream);
        }

        public static GpkgLoadResult LoadGpkg(byte[] data)
        {
            using var stream = new MemoryStream(data);
            return LoadGpkg(stream);
        }

        public static GpkgLoadResult LoadGpkg(Stream stream)
        {
            // Read all entries into memory in one pass
            var entryData = new Dictionary<string, byte[]>();
            var entryNames = new List<string>();

            using (var zis = new ZipInputStream(stream))
            {
                while (zis.GetNextEntry() is { } entry)
                {
                    if (entry.IsDirectory)
                        continue;

                    using (var ms = new MemoryStream())
                    {
                        zis.CopyTo(ms);
                        entryData[entry.Name] = ms.ToArray();
                        entryNames.Add(entry.Name);
                    }
                }
            }

            // Parse setting.json
            if (!entryData.TryGetValue("setting.json", out var settingBytes))
                throw new Exception("gpkg missing setting.json");

            var settingJson = Encoding.UTF8.GetString(settingBytes);
            JObject settings;
            try
            {
                settings = JObject.Parse(settingJson);
            }
            catch (Exception ex)
            {
                throw new Exception("gpkg setting.json is not valid JSON: " + ex.Message);
            }

            var formsToken = settings["Forms"];
            if (formsToken is not JArray forms || forms.Count == 0)
                throw new Exception("gpkg setting.json Forms is empty or not an array");

            var formNames = new List<string>();
            foreach (var form in forms)
            {
                var formName = form.ToString();
                if (string.IsNullOrWhiteSpace(formName))
                    throw new Exception("gpkg setting.json Forms contains an empty entry");
                formNames.Add(formName);
            }

            // Create runtime packages for each form (strip Forms/<formName>/ prefix)
            var runtimePackages = new List<Package>();
            foreach (var formName in formNames)
            {
                var formPrefix = "Forms/" + formName + "/";
                var runtimePackage = new Package();
                var formFound = false;

                foreach (var name in entryNames)
                {
                    if (!name.StartsWith(formPrefix, StringComparison.Ordinal))
                        continue;

                    formFound = true;
                    var relativePath = name.Substring(formPrefix.Length);
                    var data = entryData[name];

                    if (name.EndsWith(".g", StringComparison.OrdinalIgnoreCase))
                    {
                        runtimePackage.SourceCodeFiles.Add(
                            new SourceCodeFile(relativePath, DecodeGFile(data), false));
                    }
                    else
                    {
                        runtimePackage.AssetFiles.Add(
                            new AssetFile(relativePath, data, false));
                    }
                }

                if (!formFound)
                    throw new Exception($"gpkg form '{formName}' not found in archive (expected under Forms/{formName}/)");

                runtimePackages.Add(runtimePackage);
            }

            // Create chart package from non-Forms/, non-setting.json root files
            var chartPackage = new Package();
            foreach (var name in entryNames)
            {
                if (name == "setting.json")
                    continue;
                if (name.StartsWith("Forms/", StringComparison.Ordinal))
                    continue;

                var data = entryData[name];

                if (name.EndsWith(".g", StringComparison.OrdinalIgnoreCase))
                {
                    chartPackage.SourceCodeFiles.Add(
                        new SourceCodeFile(name, DecodeGFile(data), true));
                }
                else
                {
                    chartPackage.AssetFiles.Add(
                        new AssetFile(name, data, true));
                }
            }

            return new GpkgLoadResult(runtimePackages, chartPackage);
        }

        private static string DecodeGFile(byte[] data)
        {
            if (data.Length >= 3 && data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
                return Encoding.UTF8.GetString(data, 3, data.Length - 3);
            return Encoding.UTF8.GetString(data);
        }
    }
}
