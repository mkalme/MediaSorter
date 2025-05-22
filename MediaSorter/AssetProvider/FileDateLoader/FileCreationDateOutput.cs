using System;

namespace MediaSorter {
    public class FileCreationDateOutput {
        public string FilePath { get; set; }
        public DateTime CreationDate { get; set; }
        public bool FileNameDated { get; set; } = false;
    }
}
