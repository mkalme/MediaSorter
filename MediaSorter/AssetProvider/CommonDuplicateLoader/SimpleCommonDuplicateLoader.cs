using System;
using System.Collections.Generic;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class SimpleCommonDuplicateLoader : CommonDuplicateLoader {
        public override ICommonMediaDuplicateSeperator CommonMediaDuplicateSeperator { get; set; }
        public override IAssetProvider<IEnumerable<FileDate>> UnsortedFileDateProvider { get; set; }
        public override IAssetProvider<IEnumerable<string>> MainFileProvider { get; set; }
    }
}
