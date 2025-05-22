using System;

namespace MediaSorter {
    public class SavedAssetProvider<TAsset> : IAssetProvider<TAsset> {
        public string File { get; set; }

        public SavedAssetProvider(string file) {
            File = file;
        }

        public TAsset ProvideAsset() {
            return JsonUtilities.ReadAsset<TAsset>(File);
        }
    }
}
