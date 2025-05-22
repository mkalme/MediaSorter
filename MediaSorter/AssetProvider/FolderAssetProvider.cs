using System;
using System.Collections.Generic;
using System.IO;

namespace MediaSorter {
    public class FolderAssetProvider : IAssetProvider<IEnumerable<string>> {
        public IEnumerable<string> Folders { get; set; }

        public FolderAssetProvider(IEnumerable<string> folders) {
            Folders = folders;
        }

        public IEnumerable<string> ProvideAsset() {
            List<string> files = new List<string>();

            foreach (var folder in Folders) {
                files.AddRange(Directory.GetFiles(folder, "*", SearchOption.AllDirectories));
            }

            return files;
        }
    }
}
