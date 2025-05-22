using System;
using System.Globalization;

namespace MediaSorter {
    public class Format {
        public string DateFormat { get; set; }
        public int SkipFirstCharacters { get; set; }

        public Format(string dateFormat, int skipFirstCharacters = 0) {
            DateFormat = dateFormat;
            SkipFirstCharacters = skipFirstCharacters;
        }

        public bool TryParse(string fileName, out DateTime date) {
            if ((fileName.Length - SkipFirstCharacters) < DateFormat.Length) {
                date = DateTime.MinValue;
                return false;
            }

            return DateTime.TryParseExact(fileName.Substring(SkipFirstCharacters, DateFormat.Length), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
