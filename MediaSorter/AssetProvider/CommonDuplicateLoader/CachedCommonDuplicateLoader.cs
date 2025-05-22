using System;
using System.Collections.Generic;
using System.Linq;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class CachedCommonDuplicateLoader : CommonDuplicateLoader {
        public CommonDuplicateLoader CommonDuplicateLoader { get; set; }
        public CacheBag Cache { get; set; }

        public override ICommonMediaDuplicateSeperator CommonMediaDuplicateSeperator {
            get => CommonDuplicateLoader.CommonMediaDuplicateSeperator;
            set => CommonDuplicateLoader.CommonMediaDuplicateSeperator = value;
        }
        public override IAssetProvider<IEnumerable<FileDate>> UnsortedFileDateProvider {
            get => CommonDuplicateLoader.UnsortedFileDateProvider;
            set => CommonDuplicateLoader.UnsortedFileDateProvider = value;
        }
        public override IAssetProvider<IEnumerable<string>> MainFileProvider {
            get => CommonDuplicateLoader.MainFileProvider;
            set => CommonDuplicateLoader.MainFileProvider = value;
        }

        public CachedCommonDuplicateLoader(CacheBag cache) {
            Cache = cache;
        }

        public override IEnumerable<FileDate> ProvideAsset() {
            IEnumerable<FileDate> unsortedFiles = UnsortedFileDateProvider.ProvideAsset();
            IEnumerable<string> files = unsortedFiles.Select(x => x.FilePath);

            IEnumerable<string> mainFiles = MainFileProvider.ProvideAsset();
            Cache.LoadCache(mainFiles);

            IEnumerable<CommonMediaDuplicate> duplicates = CommonMediaDuplicateSeperator.SeperateDuplicates(mainFiles, new List<IEnumerable<string>>() { files });
            return FilterDuplicates(duplicates, unsortedFiles);
        }
    }
}
