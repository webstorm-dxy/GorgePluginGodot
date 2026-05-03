using System.Collections.Generic;
using Gorge.GorgeFramework.Chart;
using Gorge.Native;
using Gorge.Native.GorgeFramework;

namespace Gorge.GorgeFramework.Runtime.Environment
{
    /// <summary>
    /// 管理运行时资源
    /// </summary>
    public class AssetManager
    {
        private Dictionary<string, Asset> _loadedAssets;

        public void LoadAssetsFromChart(SimulationScore score)
        {
            _loadedAssets = new Dictionary<string, Asset>();
            foreach (var assetLoader in score.AssetLoaders)
            {
                foreach (var assetSet in assetLoader.AssetSets)
                {
                    foreach (var asset in assetSet.Assets)
                    {
                        // TODO 这里的构造改成注解识别
                        LoadAsset(Asset.FromGorgeObject(asset.Instantiate(2)));
                    }
                }
            }
        }

        /// <summary>
        /// 向运行时环境加载资源
        /// </summary>
        /// <param name="asset"></param>
        public void LoadAsset(Asset asset)
        {
            asset.LoadAsset();
            _loadedAssets.Add(asset.name, asset);
        }
        
    }
}