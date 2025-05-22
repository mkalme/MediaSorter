using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaSorter {
    public class FileDateSwitcher : IAssetProvider<IEnumerable<FileDate>> {
        public IAssetProvider<IEnumerable<FileDate>> AssetProvider { get; set; }
        public IAssetProvider<IEnumerable<string>> DatesToSwitchProvider { get; set; }

        public IEnumerable<FileDate> ProvideAsset() {
            HashSet<string> datesToSwitch = DatesToSwitchProvider.ProvideAsset().ToHashSet();

            List<FileDate> dates = new List<FileDate>();
            foreach (var date in AssetProvider.ProvideAsset()) {
                if (datesToSwitch.Contains(date.FilePath)) {
                    date.CreationDate = new DateTime(date.CreationDate.Year, date.CreationDate.Day, date.CreationDate.Month, date.CreationDate.Hour, date.CreationDate.Minute, date.CreationDate.Second);
                }

                dates.Add(date);
            }

            return dates;
        }
    }
}
