using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediaSorter {
    public class FolderOrganizer : IAssetProvider<IEnumerable<FileDateOutput>> {
        public IAssetProvider<IEnumerable<FileDate>> FileDateProvider { get; set; }
        
        public string OutputFolder { get; set; }
        public string Format { get; set; }

        public FolderOrganizer(string outputFolder, string format) {
            OutputFolder = outputFolder;
            Format = format;
        }

        public IEnumerable<FileDateOutput> ProvideAsset() {
            IEnumerable<FileDate> files = FileDateProvider.ProvideAsset();
            files = files.OrderBy(x => x.FilePath);

            ConcurrentBag<FileDateOutput> output = new ConcurrentBag<FileDateOutput>();

            int count = 0;
            foreach (var file in files) {
                string outputFile = GetOutputFile(file);

                output.Add(new FileDateOutput() {
                    OriginalFile = file,
                    OutputFile = outputFile
                });

                Directory.CreateDirectory(Path.GetDirectoryName(outputFile));
                File.Copy(file.FilePath, outputFile);

                Console.WriteLine($"{++count}/{files.Count()}");
            }

            return output;
        }

        private string GetOutputFile(FileDate fileDate) {
            string outputFile = OutputFolder;

            if (fileDate.CreationDate > DateTime.MinValue) {
                string fileName = $"{fileDate.CreationDate.ToString(Format)}{Path.GetExtension(fileDate.FilePath)}";
                outputFile = $"{outputFile}\\{fileDate.CreationDate.ToString("yyyy-MM")}\\{fileName}";
            } else {
                outputFile = $"{outputFile}\\unsorted\\{Path.GetFileName(fileDate.FilePath)}";
            }

            return GetFileName(outputFile);
        }
        private static string GetFileName(string file) {
            string output = file;

            int count = 2;
            while (File.Exists(output)) {
                output = $"{Path.GetDirectoryName(file)}\\{Path.GetFileNameWithoutExtension(file)} ({count++}){Path.GetExtension(file)}";
            }

            return output;
        }
    }
}
