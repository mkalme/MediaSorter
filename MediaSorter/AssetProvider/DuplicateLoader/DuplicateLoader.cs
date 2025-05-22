using System;
using System.Collections.Generic;
using MediaDuplicateSeperator;

namespace MediaSorter{
    public abstract class DuplicateLoader : IAssetProvider<IEnumerable<IEnumerable<string>>> {
        public abstract IAllMediaDuplicateSeperator Seperator { get; set; }
        public abstract IAssetProvider<IEnumerable<string>> FileProvider { get; set; }

        public virtual IEnumerable<IEnumerable<string>> ProvideAsset() {
            return Seperator.SeperateDuplicates(FileProvider.ProvideAsset());
        }
    }
}
