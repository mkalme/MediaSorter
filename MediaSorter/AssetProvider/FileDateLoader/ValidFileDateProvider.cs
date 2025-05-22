using System;
using System.Collections.Generic;
using System.Linq;

namespace MediaSorter {
    public class ValidFileDateProvider : IAssetProvider<IEnumerable<FileCreationDateOutput>> {
        public IAssetProvider<IEnumerable<FileCreationDate>> DateProvider { get; set; }

        public IEnumerable<FileCreationDateOutput> ProvideAsset() {
            IDictionary<string, FileCreationDate> dates = DateProvider.ProvideAsset().ToDictionary(x => x.FilePath);

            List<FileCreationDateOutput> output = new List<FileCreationDateOutput>();
            foreach (var pair in dates) {
                FileCreationDateOutput date = new FileCreationDateOutput() {
                    FilePath = pair.Key,
                    FileNameDated = pair.Value.FileNameDated
                };
                output.Add(date);

                if (pair.Value.FileNameDated) {
                    date.CreationDate = pair.Value.CreationDates.ElementAt(1);
                } else {
                    date.CreationDate = GetValidDate(pair.Value.CreationDates);
                }
            }

            return output;
        }

        private static DateTime GetValidDate(IList<DateTime> dates) {
            IEnumerable<DateTime> sorted = dates.OrderByDescending(x => x);

            DateTime oldest = DateTime.MaxValue;
            foreach (var date in sorted) {
                if (IsDateValid(date)) oldest = date;
            }

            return oldest == DateTime.MaxValue ? sorted.Last() : oldest;
        }
        private static bool IsDateValid(DateTime date) {
            return date >= new DateTime(2005, 1, 1) && date != new DateTime(2008, 1, 1, 0, 0, 0) && date < new DateTime(2021, 1, 1);
        }
    }
}
