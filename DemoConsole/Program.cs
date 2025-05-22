using System;
using System.Diagnostics;
using System.IO;
using MediaSorter;

namespace DemoConsole {
    class Program {
        private static readonly string[] InputFolders = File.ReadAllLines("Storage\\folders.txt");
        private static readonly string BaseFolder = "D:\\Backup\\Pictures & Videos on Phone\\Samsung Galaxy Note 3";
        private static readonly string OutputFolder = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\Output";

        static void Main(string[] args) {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            //Sorter sorter = new Sorter(InputFolders, BaseFolder, OutputFolder);
            //sorter.Sort();

            //Console.WriteLine(string.Join("\n", Sorter.GetInputFiles(new FolderAssetProvider(args).ProvideAsset())));
            //Sorter.GetMissingFiles(OutputFolder, OutputFolder + "Old");

            watch.Stop();
            Console.WriteLine($"Elapsed milliseconds: {watch.ElapsedMilliseconds}");

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
