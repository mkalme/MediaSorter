using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaSorter {
    public class FileFinder : IAssetProvider<IEnumerable<string>> {
        public IAssetProvider<IEnumerable<string>> OriginalFileProvider { get; set; }
        public IAssetProvider<IEnumerable<IEnumerable<string>>> DuplicateProvider { get; set; }
        public IAssetProvider<IEnumerable<FileDate>> FileDateProvider { get; set; }

        public IEnumerable<string> ProvideAsset() {
            IDictionary<string, FileDate> fileDates = FileDateProvider.ProvideAsset().ToDictionary(x => x.FilePath);
            
            Dictionary<string, FileDate> duplicates = new Dictionary<string, FileDate>();
            foreach (var duplicate in DuplicateProvider.ProvideAsset()) {
                FileDate date = null;
                foreach (var file in duplicate) {
                    if (!fileDates.ContainsKey(file)) continue;

                    date = fileDates[file];
                    break;
                }

                foreach (var file in duplicate) {
                    duplicates.Add(file, date);
                }
            }

            List<string> output = new List<string>();
            foreach (string file in OriginalFileProvider.ProvideAsset()) {
                if (fileDates.ContainsKey(file)) {
                    output.Add(file);
                } else {
                    output.Add(duplicates[file].FilePath);
                }
            }

            return output;
        }
    }
}
