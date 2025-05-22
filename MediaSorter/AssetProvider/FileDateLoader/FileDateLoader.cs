using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class FileDateLoader : IAssetProvider<IEnumerable<FileDate>> {
        public IMediaDateFinder MediaDateFinder { get; set; }
        public IAssetProvider<IEnumerable<IEnumerable<string>>> DuplicateProvider { get; set; }

        public IEnumerable<FileDate> ProvideAsset() {
            ConcurrentBag<FileDate> dates = new ConcurrentBag<FileDate>();

            IEnumerable<IEnumerable<string>> duplicates = DuplicateProvider.ProvideAsset();

            int count = 0;
            Parallel.ForEach(duplicates, duplicate => {
                Interlocked.Increment(ref count);
                
                DateTime oldestDate = DateTime.MaxValue;
                string oldestPath = string.Empty;

                foreach (var file in duplicate) {
                    if (MediaDateFinder.TryFindDate(file, out DateTime dateTime) && dateTime < oldestDate) {
                        oldestDate = dateTime;
                        oldestPath = file;
                    }
                }

                if (string.IsNullOrEmpty(oldestPath)) {
                    oldestDate = DateTime.MinValue;
                    oldestPath = duplicate.First();
                }

                dates.Add(new FileDate() {
                    CreationDate = oldestDate,
                    FilePath = oldestPath
                });

                Console.WriteLine($"{count}/{duplicates.Count()}");
            });

            return dates;
        }
    }
}
