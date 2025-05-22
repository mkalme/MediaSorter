using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaSorter {
    public class CustomCommonDuplicateProvider : IAssetProvider<IEnumerable<FileDate>> {
        public IAssetProvider<IEnumerable<IEnumerable<string>>> DuplicateProvider { get; set; }
        public IAssetProvider<IEnumerable<FileDate>> CommonFileProvider { get; set; }
        public IAssetProvider<IEnumerable<FileDate>> FileDateProvider { get; set; }

        public IEnumerable<FileDate> ProvideAsset() {
            IDictionary<string, FileDate> fileDates = FileDateProvider.ProvideAsset().ToDictionary(x => x.FilePath);
            IDictionary<string, FileDate> commonFiles = CommonFileProvider.ProvideAsset().ToDictionary(x => x.FilePath);

            List<FileDate> output = new List<FileDate>();
            foreach (var duplicate in DuplicateProvider.ProvideAsset()) {
                FileDate date = null;
                foreach (var file in duplicate) {
                    if (!fileDates.ContainsKey(file)) continue;

                    date = fileDates[file];
                    break;
                }

                foreach (var file in duplicate) {
                    if (!commonFiles.ContainsKey(file)) continue;

                    output.Add(date);
                    break;
                }
            }

            return output;
        }
    }
}
