using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Picross_Helper
{
    class BoardAnalyze
    {
        Bitmap screenshot;
        Graphics yikes;
        int startPixel_Column = 0;
        int startPixel_Row = 0;
        int widthColumn = 0;
        int widthRow = 0;
        int HeightColumn = 0;
        int HeightRow = 0;

        public void setImage(Bitmap screenshot)
        {
            this.screenshot = screenshot;

            //image.Bitmap = screenshot;
        }
        public Boolean isSolving()
        {
            //Look for pause picture
            if (DetectColorWithUnsafe(screenshot, (byte)255, (byte)255, (byte)255, 0, 0, 990, 80, 1080) > 0.16)
                return true;
            return false;
        }

        //Source: https://www.codeproject.com/Articles/617613/Fast-pixel-operations-in-NET-with-and-without-unsa
        static unsafe double DetectColorWithUnsafe(Bitmap image,
        byte searchedR, byte searchedG, int searchedB, int tolerance, int startX, int startY, int endX, int endY)
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width,
              image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();
            int stride = imageData.Stride;
            int totalPixels = (endX - startX) * (endY - startY);
            double foundPixels = 0;

            for (int y = startY; y < endY; y++)
            {
                byte* row = scan0 + (y * stride);

                for (int x = startX; x < endX; x++)
                {
                    // Watch out for actual order (BGR)!
                    int bIndex = x * bytesPerPixel;
                    int gIndex = bIndex + 1;
                    int rIndex = bIndex + 2;

                    byte pixelR = row[rIndex];
                    byte pixelG = row[gIndex];
                    byte pixelB = row[bIndex];

                    if ((pixelR == searchedR) && (pixelG == searchedG) && (pixelB == searchedB))
                        foundPixels++;
                }
            }

            image.UnlockBits(imageData);
            return foundPixels / totalPixels;
        }
    }
}
