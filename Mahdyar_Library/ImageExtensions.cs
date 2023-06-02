using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace Mahdyar_Library
{
   public static class ImageExtensions
    {
       public static Bitmap Ext_ScaleImage(this Image image, int maxWidth, int maxHeight,bool KeepRatio=false, bool DontGrowSize = false)
       {
           double ratioX = (double)maxWidth / image.Width;
           double ratioY = (double)maxHeight / image.Height;
           double ratio = Math.Min(ratioX, ratioY);
           int newWidth = (int)(image.Width * ratio);
           int newHeight = (int)(image.Height * ratio);
            if (DontGrowSize)
            {
                if (newWidth > image.Width || newHeight > image.Height)
                {
                    newHeight = image.Height;
                    newWidth = image.Width;
                }
            }

            var newImage = new Bitmap(newWidth, newHeight);

            if (KeepRatio)
           {
               using (var graphics = Graphics.FromImage(newImage))
                   graphics.DrawImage(image, 0, 0, newWidth, newHeight);
           }
           else
           {
               return new Bitmap(image, maxWidth, maxHeight);
           }
           return newImage;
       }
       public static Bitmap Ext_ToBitmap(this BitmapImage bitmapImage)
       {
           using (var outStream = new MemoryStream())
           {
               BitmapEncoder enc = new BmpBitmapEncoder();
               enc.Frames.Add(BitmapFrame.Create(bitmapImage));
               enc.Save(outStream);
               var bitmap = new Bitmap(outStream);

               return new Bitmap(bitmap);
           }
       }

       public static BitmapImage Ext_ToBitmapImage(this Image bitmap)
       {
           using (var memory = new MemoryStream())
           {
               bitmap.Save(memory, ImageFormat.Png);
               memory.Position = 0;
               var bitmapImage = new BitmapImage();
               bitmapImage.BeginInit();
               bitmapImage.StreamSource = memory;
               bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
               bitmapImage.EndInit();
           return bitmapImage;
           }
       }
       public static Bitmap Ext_ChangeQualityLevel(this Image bmp1, long QuilityLevel)
       {
           // Get a bitmap. The using statement ensures objects
           // are automatically disposed from memory after use.
          
               ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

               // Create an Encoder object based on the GUID
               // for the Quality parameter category.
               System.Drawing.Imaging.Encoder myEncoder =
                   System.Drawing.Imaging.Encoder.Quality;

               // Create an EncoderParameters object.
               // An EncoderParameters object has an array of EncoderParameter
               // objects. In this case, there is only one
               // EncoderParameter object in the array.
               EncoderParameters myEncoderParameters = new EncoderParameters(1);

               EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, QuilityLevel);
               myEncoderParameters.Param[0] = myEncoderParameter;
           MemoryStream ms=new MemoryStream();
           bmp1.Save(ms, jpgEncoder, myEncoderParameters);
           return new Bitmap(ms,true);
       }

       static ImageCodecInfo GetEncoder(ImageFormat format)
       {
           ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
           foreach (ImageCodecInfo codec in codecs)
           {
               if (codec.FormatID == format.Guid)
               {
                   return codec;
               }
           }
           return null;
       }

       public static string Ext_GetDimensions(this Image Bmp1)
       {
           return Bmp1.Width + " " +
                  SpecialCharacters.Cross + " " +
                  Bmp1.Height;
       }

       public static Bitmap Ext_Format(this Image img,ImageFormat img_format)
       {
           var ms = new MemoryStream();
           img.Save(ms,img_format);
           return new Bitmap(ms);
       }

       public static Bitmap Ext_GrayScale(this Bitmap Bmp)
       {
           int rgb;
           Color c;
           
           for (int y = 0; y < Bmp.Height; y++)
               for (int x = 0; x < Bmp.Width; x++)
               {
                   c = Bmp.GetPixel(x, y);
                   rgb = (int)((c.R + c.G + c.B) / 3);
                   Bmp.SetPixel(x, y, Color.FromArgb(rgb, rgb, rgb));
               }
           return Bmp;
       }
    }
}
