using System;

namespace MediaSorter {
    public interface IAssetProvider<IAsset> {
        IAsset ProvideAsset();
    }
}
