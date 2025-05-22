using System;

namespace MediaSorter {
    public class LoggableAssetProvider<TAsset> : IAssetProvider<TAsset> {
        public string FilePath { get; set; }
        public IAssetProvider<TAsset> AssetProvider { get; set; }

        public LoggableAssetProvider(string filePath) {
            FilePath = filePath;
        }

        public TAsset ProvideAsset() {
            TAsset asset = AssetProvider.ProvideAsset();
            JsonUtilities.SaveAsset(asset, FilePath);

            return asset;
        }
    }
}
