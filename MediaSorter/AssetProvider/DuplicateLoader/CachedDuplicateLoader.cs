using System;
using System.Collections.Generic;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class CachedDuplicateLoader : DuplicateLoader {
        public DuplicateLoader DuplicateLoader { get; set; }
        public CacheBag Cache { get; set; }

        public override IAllMediaDuplicateSeperator Seperator {
            get => DuplicateLoader.Seperator;
            set => DuplicateLoader.Seperator = value;
        }
        public override IAssetProvider<IEnumerable<string>> FileProvider {
            get => DuplicateLoader.FileProvider;
            set => DuplicateLoader.FileProvider = value;
        }

        public CachedDuplicateLoader(CacheBag cache) {
            Cache = cache;
        }

        public override IEnumerable<IEnumerable<string>> ProvideAsset() {
            IEnumerable<string> files = DuplicateLoader.FileProvider.ProvideAsset();
            Cache.LoadCache(files);

            return DuplicateLoader.Seperator.SeperateDuplicates(Cache.GetAllCachedFiles());
        }
    }
}
