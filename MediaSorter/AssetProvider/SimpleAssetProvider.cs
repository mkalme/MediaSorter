using System;

namespace MediaSorter {
    public class SimpleAssetProvider<TAsset> : IAssetProvider<TAsset> {
        public TAsset Asset { get; set; }

        public SimpleAssetProvider(TAsset asset) {
            Asset = asset;
        }

        public TAsset ProvideAsset() {
            return Asset;
        }
    }
}
