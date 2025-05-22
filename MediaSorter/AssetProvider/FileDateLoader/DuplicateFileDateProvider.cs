using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaSorter {
    public class DuplicateFileDateProvider : IAssetProvider<IEnumerable<FileDate>> {
        public IAssetProvider<IEnumerable<IEnumerable<string>>> DuplicateProvider { get; set; }
        public IAssetProvider<IEnumerable<FileCreationDateOutput>> DateProvider { get; set; }

        public IEnumerable<FileDate> ProvideAsset() {
            IEnumerable<IEnumerable<string>> duplicates = DuplicateProvider.ProvideAsset();
            IDictionary<string, FileCreationDateOutput> dates = DateProvider.ProvideAsset().ToDictionary(x => x.FilePath);

            List<FileDate> output = new List<FileDate>();
            foreach (var duplicate in duplicates) {
                IList<FileCreationDateOutput> files = new List<FileCreationDateOutput>();
                foreach (var file in duplicate) {
                    files.Add(dates[file]);
                }
                files = files.OrderByDescending(x => x.CreationDate).ThenByDescending(x => x.FilePath).ToList();

                FileCreationDateOutput fileCreation = files.Last();
                for (int i = 1; i < files.Count; i++) {
                    if (files[i].FileNameDated || !fileCreation.FileNameDated) fileCreation = files[i];
                }

                output.Add(new FileDate() {
                    FilePath = fileCreation.FilePath,
                    CreationDate = fileCreation.CreationDate
                });
            }

            return output;
        }
    }
}
