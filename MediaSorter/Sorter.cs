using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Concurrent;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class Sorter {
        public IEnumerable<string> InputFolders { get; set; }
        public string BaseFolder { get; set; }
        public string OutputFolder { get; set; }

        private CacheBag _cache;
        private IMediaComparer _mediaComparer;
        private IAllMediaDuplicateSeperator _allMediaDuplicateSeperator;
        private IMediaDateFinder _mediaDateFinder;
        private ICommonMediaDuplicateSeperator _commonMediaDuplicateSeperator;

        private ConcurrentBag<FileCreationDate> _fileDates = new ConcurrentBag<FileCreationDate>();

        public Sorter(IEnumerable<string> inputFolders, string baseFolder, string outputFolder) {
            InputFolders = inputFolders;
            BaseFolder = baseFolder;
            OutputFolder = outputFolder;

            Directory.CreateDirectory("Storage");
        }

        public void Sort() {
            CreateMediaComparer();
            CreateAllMediaDuplicateSeperator();
            CreateDateFinder();
            CreateCommonMediaDuplicateSeperator();

            IEnumerable<string> allFiles = new LoggableAssetProvider<IEnumerable<string>>("Storage\\all_files.json") {
                AssetProvider = new FolderAssetProvider(InputFolders)
            }.ProvideAsset();

            IEnumerable<IEnumerable<string>> duplicates = new LoggableAssetProvider<IEnumerable<IEnumerable<string>>>("Storage\\duplicates.json") {
                AssetProvider = new CachedDuplicateLoader(_cache) {
                    DuplicateLoader = new SimpleDuplicateLoader() {
                        Seperator = _allMediaDuplicateSeperator,
                        FileProvider = new SimpleAssetProvider<IEnumerable<string>>(allFiles)
                    }
                }
            }.ProvideAsset();

            new FileDateLoader() {
                MediaDateFinder = _mediaDateFinder,
                DuplicateProvider = new SimpleAssetProvider<IEnumerable<IEnumerable<string>>>(duplicates)
            }.ProvideAsset();

            IEnumerable<FileCreationDate> allDates = new LoggableAssetProvider<IEnumerable<FileCreationDate>>("Storage\\all_dates.json") {
                AssetProvider = new SimpleAssetProvider<IEnumerable<FileCreationDate>>(_fileDates)
            }.ProvideAsset();

            IEnumerable<FileDate> fileDates = new LoggableAssetProvider<IEnumerable<FileDate>>("Storage\\file_dates.json") {
                AssetProvider = new DuplicateFileDateProvider() {
                    DuplicateProvider = new SimpleAssetProvider<IEnumerable<IEnumerable<string>>>(duplicates),
                    DateProvider = new ValidFileDateProvider() {
                        DateProvider = new SimpleAssetProvider<IEnumerable<FileCreationDate>>(allDates)
                    }
                }
            }.ProvideAsset();

            IEnumerable<FileDate> commonDuplicates = new LoggableAssetProvider<IEnumerable<FileDate>>("Storage\\common_duplicates.json") {
                AssetProvider = new CachedCommonDuplicateLoader(_cache) {
                    CommonDuplicateLoader = new SimpleCommonDuplicateLoader() {
                        CommonMediaDuplicateSeperator = _commonMediaDuplicateSeperator,
                        UnsortedFileDateProvider = new SimpleAssetProvider<IEnumerable<FileDate>>(fileDates),
                        MainFileProvider = new FolderAssetProvider(new string[] { BaseFolder })
                    }
                }
            }.ProvideAsset();

            IEnumerable<FileDate> filtered = new LoggableAssetProvider<IEnumerable<FileDate>>("Storage\\filtered.json") {
                AssetProvider = new FileDateFilterProvider() {
                    AssetProvider = new SimpleAssetProvider<IEnumerable<FileDate>>(commonDuplicates),
                    FilterProvider = new SimpleAssetProvider<IEnumerable<string>>(File.ReadLines("Storage\\filter.txt"))
                }
            }.ProvideAsset();

            IEnumerable<FileDate> switchedDates = new LoggableAssetProvider<IEnumerable<FileDate>>("Storage\\switched_dates.json") {
                AssetProvider = new FileDateSwitcher() {
                    AssetProvider = new SimpleAssetProvider<IEnumerable<FileDate>>(filtered),
                    DatesToSwitchProvider = new SimpleAssetProvider<IEnumerable<string>>(File.ReadLines("Storage\\switched_dates.txt"))
                }
            }.ProvideAsset();

            new LoggableAssetProvider<IEnumerable<FileDateOutput>>("Storage\\organized_files.json") {
                AssetProvider = new FolderOrganizer(OutputFolder, "yyyyMMdd_HHmmss") {
                    FileDateProvider = new SimpleAssetProvider<IEnumerable<FileDate>>(switchedDates)
                }
            }.ProvideAsset();
        }

        private void CreateMediaComparer() {
            ShallowMediaComparerFactory shallowFactory = new ShallowMediaComparerFactory();
            CustomMediaComparerFactory deepFactory = new CustomMediaComparerFactory();

            shallowFactory.CachedBitmapProvider.NodeProvider = new ResizedBitmapProvider(new PixelCorrectionBitmapProvider(new BitmapProvider())) {
                Size = new Size(64, 64)
            };
            shallowFactory.CachedBitmapProvider.NodeProvider = new DisposedBitmapProvider() {
                BitmapProvider = shallowFactory.CachedBitmapProvider.NodeProvider
            };

            _cache = new CacheBag(shallowFactory.CachedBitmapProvider, shallowFactory.CachedVideoProvider);

            shallowFactory.BitmapCache = _cache.BitmapCache;
            shallowFactory.VideoCache = _cache.VideoCache;

            deepFactory.BitmapCache = _cache.BitmapCache;
            deepFactory.VideoCache = _cache.VideoCache;

            _mediaComparer = new LayeredMediaComparer() {
                MediaComparers = new List<IMediaComparer>() {
                    new MediaComparer() { MediaComparerFactory = shallowFactory },
                    new MediaComparer() { MediaComparerFactory = deepFactory }
                }
            };
        }
        private void CreateAllMediaDuplicateSeperator() {
            _allMediaDuplicateSeperator = new AllMediaDuplicateSeperator() {
                DuplicateSeperator = new SingleDuplicateSeperator() {
                    MediaComparer = _mediaComparer
                }
            };
        }
        private void CreateDateFinder() {
            _mediaDateFinder = new CustomMediaDateFinder() {
                Files = _fileDates,
                ExifDataProvider = new ExifDataProvider()
            };
        }
        private void CreateCommonMediaDuplicateSeperator() {
            _commonMediaDuplicateSeperator = new CommonMediaDuplicateSeperator() {
                DuplicateSeperator = new SingleDuplicateSeperator() {
                    MediaComparer = _mediaComparer
                },
                StopAtFirstEncounter = false
            };
        }

        public static IEnumerable<string> GetInputFiles(IEnumerable<string> outputFiles) {
            IEnumerable<FileDateOutput> output = new SavedAssetProvider<IEnumerable<FileDateOutput>>("Storage\\organized_files.json").ProvideAsset();
            IDictionary<string, FileDateOutput> names = output.ToDictionary(x => Path.GetFileName(x.OutputFile));

            List<string> files = new List<string>();
            foreach (var file in outputFiles) {
                if (names.TryGetValue(Path.GetFileName(file), out FileDateOutput originalFile)) {
                    files.Add(originalFile.OriginalFile.FilePath);
                } else {
                    Console.WriteLine(file);
                }
            }

            return files;
        }

        public static void GetMissingFiles(string output, string oldOutput) {
            HashSet<string> old = new HashSet<string>(Directory.GetFiles(output, "*", SearchOption.AllDirectories).Select(x => Path.GetFileName(x)));

            foreach (var file in Directory.GetFiles(oldOutput, "*", SearchOption.AllDirectories)) {
                if (!old.Contains(Path.GetFileName(file))) {
                    Console.WriteLine(file);
                }
            }
        }
    }
}
