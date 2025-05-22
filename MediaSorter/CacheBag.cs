using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class CacheBag {
        public CachedNodeProvider<LockedBitmap> BitmapCache { get; set; }
        public CachedNodeProvider<Video> VideoCache { get; set; }

        public CacheBag(CachedNodeProvider<LockedBitmap> bitmapCache, CachedNodeProvider<Video> videoCache) {
            BitmapCache = bitmapCache;
            VideoCache = videoCache;
        }

        public void LoadCache(IEnumerable<string> files) {
            BitmapCache.PathValidator = path => {
                string extension = Path.GetExtension(path).ToLower();
                return MediaUtilities.ImageFormats.Contains(extension) || string.IsNullOrEmpty(extension);
            };

            //BitmapCache.Preload(files);
            Preload(BitmapCache, files);

            HashSet<string> videoFiles = new HashSet<string>(files);

            videoFiles.AddRange(BitmapCache.UnloadeableFiles.Where(x => string.IsNullOrEmpty(Path.GetExtension(x))));
            videoFiles.RemoveWhere(x => BitmapCache.Cache.ContainsKey(x));

            VideoCache.PathValidator = path => {
                string extension = Path.GetExtension(path).ToLower();
                return MediaUtilities.VideoFormats.Contains(extension) || string.IsNullOrEmpty(extension);
            };

            //VideoCache.Preload(videoFiles);
            Preload(VideoCache, videoFiles);
        }
        private static void Preload<TAsset>(CachedNodeProvider<TAsset> cache, IEnumerable<string> files) {
            int count = 0, fileCount = files.Count();

            ThreadUtilities.ForEach(files, 2, file => {
                Interlocked.Increment(ref count);

                if (!cache.PathValidator(file)) return;
                if (!cache.Cache.ContainsKey(file)) {

                    try {
                        cache.Cache.TryAdd(file, cache.NodeProvider.ProvideNode(file));
                    } catch {
                        cache.UnloadeableFiles.Add(file);
                    }
                }

                Console.WriteLine($"{count}/{fileCount} | {file}");
            });
        }

        public void RemoveCache(IEnumerable<string> files) {
            foreach (var file in files) {
                RemoveCache(file);
            }
        }
        public void RemoveCache(string file) {
            if (BitmapCache.Cache.TryRemove(file, out LockedBitmap bitmap)) {
                bitmap.Dispose();
            } else if (VideoCache.Cache.TryRemove(file, out Video video)) {
                video.Dispose();
            }
        }

        public IEnumerable<string> GetAllCachedFiles() {
            List<string> files = new List<string>(BitmapCache.Cache.Keys);
            files.AddRange(VideoCache.Cache.Keys);

            return files;
        }
    }
}
