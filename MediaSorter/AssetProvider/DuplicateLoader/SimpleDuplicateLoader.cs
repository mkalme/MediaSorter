using System;
using System.Collections.Generic;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class SimpleDuplicateLoader : DuplicateLoader {
        public override IAllMediaDuplicateSeperator Seperator { get; set; }
        public override IAssetProvider<IEnumerable<string>> FileProvider { get; set; }
    }
}
