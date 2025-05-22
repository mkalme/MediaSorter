using System;
using System.Collections.Generic;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class CustomMediaDateFinder : IMediaDateFinder {
        public Format[] Formats { get; set; }
        public ConcurrentBag<FileCreationDate> Files { get; set; } = new ConcurrentBag<FileCreationDate>();

        public INodeProvider<IDictionary<string, string>> ExifDataProvider { get; set; }

        public CustomMediaDateFinder() {
            Formats = new Format[] {
                new Format("yyyy-MM-dd HH.mm.ss"),
                new Format("yyyy-MM-dd-HH-mm-ss"),
                new Format("yyyy-MM-dd-HH-mm-ss", 6),
                new Format("yyyyMMdd_HHmmss"),
                new Format("yyyyMMdd", 4)
            };
        }

        public bool TryFindDate(string path, out DateTime dateTime) {
            FileCreationDate date = new FileCreationDate() {
                FilePath = path
            };
            Files.Add(date);

            date.CreationDates.Add(new FileInfo(path).LastWriteTime);

            try {
                dateTime = DateTime.MinValue;
                foreach (Format format in Formats) {
                    if (format.TryParse(Path.GetFileNameWithoutExtension(path), out dateTime)) {
                        date.CreationDates.Add(dateTime);
                        date.FileNameDated = true;

                        break;
                    }
                }

                dateTime = DateTime.MaxValue;
                IDictionary<string, string> tags = ExifDataProvider.ProvideNode(path);

                Format parseFormat = new Format("yyyy:MM:dd HH:mm:ss");
                foreach (var pair in tags) {
                    if (parseFormat.TryParse(pair.Value, out DateTime time) && time < dateTime) {
                        dateTime = time;
                    }

                    if (parseFormat.TryParse(pair.Value, out DateTime fileTime)) {
                        date.CreationDates.Add(fileTime);
                    }
                }

                if (dateTime < DateTime.MaxValue) {
                    return true;
                }
            } catch {}

            dateTime = DateTime.MinValue;
            return false;
        }
    }
}
