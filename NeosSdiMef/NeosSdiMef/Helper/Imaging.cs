using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Helper
{
    internal class Imaging
    {

        public static void GetImage(object obj, Image _image, string imageName)
        {

            System.IO.Stream fileStream = obj.GetType().Assembly.GetManifestResourceStream(imageName);
            if (fileStream != null)
            {
                PngBitmapDecoder bitmapDecoder = new PngBitmapDecoder(fileStream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                ImageSource imageSource = bitmapDecoder.Frames[0];
                _image.Source = imageSource;
                _image.Width = 16;
                _image.Height = 16;
            }
            else
            {
                throw new Exception("The file was not found!");
            }
        }
    }
}
