using System;
using System.Collections.Generic;

namespace MediaSorter {
    public class FileCreationDate {
        public string FilePath { get; set; }
        public bool FileNameDated { get; set; } = false;

        public IList<DateTime> CreationDates { get; set; } = new List<DateTime>();
    }
}
