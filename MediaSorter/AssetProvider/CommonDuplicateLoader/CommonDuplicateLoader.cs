using System;
using System.Collections.Generic;
using System.Linq;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public abstract class CommonDuplicateLoader : IAssetProvider<IEnumerable<FileDate>> {
        public abstract ICommonMediaDuplicateSeperator CommonMediaDuplicateSeperator { get; set; }
        public abstract IAssetProvider<IEnumerable<FileDate>> UnsortedFileDateProvider { get; set; }
        public abstract IAssetProvider<IEnumerable<string>> MainFileProvider { get; set; }

        public virtual IEnumerable<FileDate> ProvideAsset() {
            IEnumerable<FileDate> unsortedFiles = UnsortedFileDateProvider.ProvideAsset();
            IEnumerable<string> files = unsortedFiles.Select(x => x.FilePath);
            IEnumerable<string> mainFiles = MainFileProvider.ProvideAsset();

            IEnumerable<CommonMediaDuplicate> duplicates = CommonMediaDuplicateSeperator.SeperateDuplicates(mainFiles, new List<IEnumerable<string>>() { files });

            return FilterDuplicates(duplicates, unsortedFiles);
        }
        protected IEnumerable<FileDate> FilterDuplicates(IEnumerable<CommonMediaDuplicate> duplicates, IEnumerable<FileDate> unsortedFiles) {
            IDictionary<string, FileDate> dictionary = unsortedFiles.ToDictionary(x => x.FilePath);

            foreach (var duplicate in duplicates) {
                foreach (var file in duplicate.Duplicates) {
                    dictionary.Remove(file);
                }
            }

            return dictionary.Values;
        }
    }
}