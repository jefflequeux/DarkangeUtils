using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AForge.Imaging.Filters;
using Microsoft.Win32;

namespace _3DDesktop
{
    public class Imaging
    {

        /// <summary>
        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        /// <param name="source">The source Image.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(System.Drawing.Image source)
        {
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(source);

            var bitSrc = ToBitmapSource(bitmap);

            bitmap.Dispose();
            bitmap = null;

            return bitSrc;
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Bitmap"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <remarks>Uses GDI to do the conversion. Hence the call to the marshalled DeleteObject.
        /// </remarks>
        /// <param name="source">The source bitmap.</param>
        /// <returns>A BitmapSource</returns>
        public static BitmapSource ToBitmapSource(System.Drawing.Bitmap source)
        {
            BitmapSource bitSrc = null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }
            finally
            {
                IconManagement.NativeMethods.DeleteObject(hBitmap);
            }

            return bitSrc;
        }

        /// <summary>
        /// Get current wallpaper
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentWallpaper()
        {
            // The current wallpaper path is stored in the registry at HKEY_CURRENT_USER\\Control Panel\\Desktop\\WallPaper
            RegistryKey rkWallPaper = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", false);
            string WallpaperPath = rkWallPaper.GetValue("WallPaper").ToString();
            rkWallPaper.Close();
            // Return the current wallpaper path
            return WallpaperPath;
        }


        #region SelectImage



        public static ImageSource SelectImage(Image img)
        {
            Bitmap bmp = img as Bitmap;
            AForge.Imaging.Image.FormatImage(ref bmp);
            Sepia filter = new Sepia();
            filter.ApplyInPlace(bmp);
            return Imaging.ToBitmapSource(bmp);
        }

        public static ImageSource SelectImage1(Image img)
        {
            Bitmap bmpPicture;
            System.Drawing.Imaging.ImageAttributes iaPicture;
            System.Drawing.Imaging.ColorMatrix cmPicture;
            Graphics gfxPicture;
            System.Drawing.Rectangle rctPicture;

            // Create new Bitmap object with the size of the picture
            bmpPicture = new Bitmap(img.Width, img.Height);

            // Image attributes for setting the attributes of the picture
            iaPicture = new System.Drawing.Imaging.ImageAttributes();

            cmPicture = new System.Drawing.Imaging.ColorMatrix(new float[][]
            {
                new float[] {1, 0, 0, 0, 0},
                new float[] {0, 1, 0, 0, 0},
                new float[] {0, 0, 1, 0, 0},
                new float[] {0, 0, 0, 1, 0},
                new float[] {.0f, .0f, .50f, .0f, 1}
            });

            // Set the new color matrix
            iaPicture.SetColorMatrix(cmPicture);
            // Set the Graphics object from the bitmap
            gfxPicture = Graphics.FromImage(bmpPicture);
            // New rectangle for the picture, same size as the original picture
            rctPicture = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
            // Draw the new image
            gfxPicture.DrawImage(img, rctPicture, 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, iaPicture);
            // Set the PictureBox to the new inverted colors bitmap
            return Imaging.ToBitmapSource(bmpPicture);
        }

        #endregion
    }
}
