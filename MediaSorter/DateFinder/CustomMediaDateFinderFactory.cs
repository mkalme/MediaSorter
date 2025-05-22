using System;
using System.Collections.Generic;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class CustomMediaDateFinderFactory : IMediaDateFinderFactory {
        public IMediaDateFinder CreateMediaDateFinder(string path) {
            return new CustomMediaDateFinder();
        }
    }
}
