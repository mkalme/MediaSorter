using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MediaDuplicateSeperator;
using Newtonsoft.Json;

namespace MediaSorter {
    public class SavedMediaDateFinder : IMediaDateFinder {
        public ConcurrentDictionary<string, DateCache> Cache { get; set; }
        
        public SavedMediaDateFinder(string path) {
            using (FileStream file = File.OpenRead(path))
            using (StreamReader reader = new StreamReader(file)) {
                IList<DateCache> dates = JsonConvert.DeserializeObject<IList<DateCache>>(reader.ReadToEnd());

                Cache = new ConcurrentDictionary<string, DateCache>();
                foreach (var date in dates) {
                    Cache.TryAdd(date.Path, date);
                }
            }
        }

        public bool TryFindDate(string path, out DateTime dateTime) {
            if (Cache.TryGetValue(path, out DateCache cache) && cache.Dates.Count > 0 && cache.Dates[0] > DateTime.MinValue) {
                dateTime = cache.Dates.Min();
                return true;
            }

            dateTime = DateTime.MinValue;
            return false;
        }
    }
}
