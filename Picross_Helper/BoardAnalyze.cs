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
        Tuple<Double, Double> startPixel_Column;
        Tuple<Double, Double> startPixel_Row;
        int widthColumn = 0;
        int widthRow = 0;
        int HeightColumn = 0;
        int HeightRow = 0;

        public void setImage(Bitmap screenshot)
        {
            this.screenshot = screenshot;
            Tuple<Double, Double> search1, search2, search3;
            //Get required values
            //Get values for top hints
            search1 = DetectColorWithUnsafe(screenshot, (byte)66, (byte)117, (byte)173, 0, 0, 0, screenshot.Size.Width, screenshot.Size.Height, 1);
            startPixel_Column = search1;
            search2 = DetectColorWithUnsafe(screenshot, (byte)82, (byte)146, (byte)198, 0, (int)search1.Item1, (int)search1.Item2, screenshot.Size.Width, screenshot.Size.Height, 1);
            widthColumn = (int)(search2.Item1 - search1.Item1);
            search3 = DetectColorWithUnsafeDown(screenshot, (byte)255, (byte)255, (byte)255, 0, (int)search1.Item1, (int)search1.Item2, screenshot.Size.Width, screenshot.Size.Height, 1);
            HeightColumn = (int)(search3.Item2 - search1.Item2);
            //Get values for left hints
            search2 = DetectColorWithUnsafe(screenshot, (byte)82, (byte)146, (byte)198, 0, 0, (int)search3.Item2 - 1, (int)search1.Item1, screenshot.Size.Height, 1);
            startPixel_Row = search2;
            search1 = DetectColorWithUnsafe(screenshot, (byte)66, (byte)117, (byte)173, 0, (int)search2.Item1, (int)search2.Item2, screenshot.Size.Width, (int)search2.Item2 + 1, 1);
            widthRow = (int)(search1.Item1 - search2.Item1);
            search3 = DetectColorWithUnsafeDown(screenshot, (byte)66, (byte)117, (byte)173, 0, (int)search2.Item1, (int)search2.Item2, screenshot.Size.Width, screenshot.Size.Height, 1);
            HeightRow = (int)(search3.Item2 - search2.Item2);
            Console.WriteLine("debug breakpoint");
        }
        public Boolean isSolving()
        {
            //Look for pause picture if (DetectColorWithUnsafe(screenshot, (byte)255, (byte)255, (byte)255, 0, 0, 990, 80, 1080) > 0.16)
            if (DetectColorWithUnsafe(screenshot, (byte)255, (byte)255, (byte)255, 0, 0, (int)Math.Round((screenshot.Size.Height * (0.91666666666))), (int)Math.Round((screenshot.Size.Width * (0.041666666666))), screenshot.Size.Height, 0).Item1 > 0.18)
                return true;
            return false;
        }

        //Source: https://www.codeproject.com/Articles/617613/Fast-pixel-operations-in-NET-with-and-without-unsa
        static unsafe Tuple<double, double> DetectColorWithUnsafe(Bitmap image,
        byte searchedR, byte searchedG, int searchedB, int tolerance, int startX, int startY, int endX, int endY, int mode)
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width,
              image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;
            double posX = 0;
            double posY = 0;

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
                    {
                        foundPixels++;
                        if (mode == 1)
                        {
                            posY = y;
                            posX = x;
                            goto Mode;
                        }
                    }
                }
            }
            image.UnlockBits(imageData);
            return Tuple.Create((foundPixels / totalPixels), 0.0);
        Mode:
            image.UnlockBits(imageData);
            Console.WriteLine(posX + " and " + posY);
            return Tuple.Create(posX, posY);
        }

        static unsafe Tuple<double, double> DetectColorWithUnsafeDown(Bitmap image,
        byte searchedR, byte searchedG, int searchedB, int tolerance, int startX, int startY, int endX, int endY, int mode)
        {
            BitmapData imageData = image.LockBits(new Rectangle(0, 0, image.Width,
              image.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            int bytesPerPixel = 3;
            double posX = 0;
            double posY = 0;

            byte* scan0 = (byte*)imageData.Scan0.ToPointer();
            int stride = imageData.Stride;
            int totalPixels = (endX - startX) * (endY - startY);
            double foundPixels = 0;

            for (int y = startY; y < endY; y++)
            {
                byte* row = scan0 + (y * stride);

                // Watch out for actual order (BGR)!
                int bIndex = startX * bytesPerPixel;
                int gIndex = bIndex + 1;
                int rIndex = bIndex + 2;

                byte pixelR = row[rIndex];
                byte pixelG = row[gIndex];
                byte pixelB = row[bIndex];

                if ((pixelR == searchedR) && (pixelG == searchedG) && (pixelB == searchedB))
                {
                    foundPixels++;
                    if (mode == 1)
                    {
                        posY = y;
                        posX = startX;
                        goto Mode;
                    }
                }
            }
            image.UnlockBits(imageData);
            return Tuple.Create((foundPixels / totalPixels), 0.0);
        Mode:
            image.UnlockBits(imageData);
            Console.WriteLine(posX + " and " + posY);
            return Tuple.Create(posX, posY);
        }
    }
}
