using System;
using System.Drawing;
using MediaDuplicateSeperator;

namespace MediaSorter {
    public class PixelCorrectionBitmapProvider : INodeProvider<LockedBitmap> {
        public INodeProvider<LockedBitmap> BitmapProvider { get; set; }

        public PixelCorrectionBitmapProvider(INodeProvider<LockedBitmap> bitmapProvider) {
            BitmapProvider = bitmapProvider;
        }

        public LockedBitmap ProvideNode(string nodePath) {
            LockedBitmap bitmap = BitmapProvider.ProvideNode(nodePath);

            if (bitmap.Width == 1 && bitmap.Height == 1) {
                bitmap.Unlock();
                Color color = bitmap.Source.GetPixel(0, 0);

                bitmap.Lock(bitmap.Source);
                bitmap.RGBValues = new byte[4] { color.B, color.G, color.R, color.A };
            }

            return bitmap;
        }
    }
}
