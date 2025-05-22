using System;
using System.Collections.Generic;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class FileDateResult {
        public string OldestFile { get; set; }
        public DateTime OldestDate { get; set; }
        public IEnumerable<string> Files { get; set; }

        public FileDateResult(string oldestFile, DateTime oldestDate) {
            OldestFile = oldestFile;
            OldestDate = oldestDate;
        }

        public static IEnumerable<FileDateResult> CreateFileDates(IMediaDateFinder dateFinder, IEnumerable<IEnumerable<string>> duplicates) {
            List<FileDateResult> output = new List<FileDateResult>();

            foreach (var duplicate in duplicates) {
                string path = "";
                DateTime date = DateTime.MaxValue;

                foreach (var file in duplicate) {
                    if (dateFinder.TryFindDate(file, out DateTime time) && time < date) {
                        path = file;
                        date = time;
                    }
                }

                output.Add(new FileDateResult(path, date) {
                    Files = duplicate
                });
            }

            return output;
        }
    }
}
