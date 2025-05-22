using System;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class DisposedBitmapProvider : INodeProvider<LockedBitmap> {
        public INodeProvider<LockedBitmap> BitmapProvider { get; set; }

        public LockedBitmap ProvideNode(string nodePath) {
            LockedBitmap bmp = BitmapProvider.ProvideNode(nodePath);
            bmp.Source.Dispose();

            return bmp;
        }
    }
}
