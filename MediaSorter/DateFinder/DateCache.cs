using System;
using System.Collections.Generic;

namespace MediaSorter {
    public class DateCache {
        public string Path { get; set; }
        public IList<DateTime> Dates { get; set; }
    }
}
