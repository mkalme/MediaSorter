using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaSorter {
    public class FileDateFilterProvider : IAssetProvider<IEnumerable<FileDate>> {
        public IAssetProvider<IEnumerable<FileDate>> AssetProvider { get; set; }
        public IAssetProvider<IEnumerable<string>> FilterProvider { get; set; }

        public IEnumerable<FileDate> ProvideAsset() {
            HashSet<string> filter = FilterProvider.ProvideAsset().ToHashSet();

            List<FileDate> dates = new List<FileDate>();
            foreach (var date in AssetProvider.ProvideAsset()) {
                if (filter.Contains(date.FilePath)) continue;
                
                dates.Add(date);
            }

            return dates;
        }
    }
}
