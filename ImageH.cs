using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

public static class ImageH {

    public static BitmapImage ToBitmapImage (Bitmap bmp) {
        using (var memory = new MemoryStream()) {
            bmp.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
    }

    public static BitmapImage ToBitmapImage (string file) {
        var bmp = new Bitmap(file);
        return ToBitmapImage(bmp);
    }

}