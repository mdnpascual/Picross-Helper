using Tesseract;
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
        Bitmap greyscaled;
        Tuple<Double, Double> startPixel_Column;
        Tuple<Double, Double> startPixel_Row;
        int widthColumn = 0;
        int widthRow = 0;
        int heightColumn = 0;
        int heightRow = 0;
        TesseractEngine engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

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
            heightColumn = (int)(search3.Item2 - search1.Item2);
            //Get values for left hints
            search2 = DetectColorWithUnsafe(screenshot, (byte)82, (byte)146, (byte)198, 0, 0, (int)search3.Item2 - 1, (int)search1.Item1, screenshot.Size.Height, 1);
            startPixel_Row = search2;
            search1 = DetectColorWithUnsafe(screenshot, (byte)66, (byte)117, (byte)173, 0, (int)search2.Item1, (int)search2.Item2, screenshot.Size.Width, (int)search2.Item2 + 1, 1);
            widthRow = (int)(search1.Item1 - search2.Item1);
            search3 = DetectColorWithUnsafeDown(screenshot, (byte)66, (byte)117, (byte)173, 0, (int)search2.Item1, (int)search2.Item2, screenshot.Size.Width, screenshot.Size.Height, 1);
            heightRow = (int)(search3.Item2 - search2.Item2);

            //Grayscaling image and Playing with OCR
            Bitmap Section = MakeGrayscale(screenshot.Clone(new System.Drawing.Rectangle((int)startPixel_Column.Item1, (int)startPixel_Column.Item2, widthColumn, heightColumn), screenshot.PixelFormat));
            MakeGrayscale(Section).Save("B:\\wat2.png");
            Section.Save("B:\\wat.png");
            Pix ocrImage = PixConverter.ToPix(MakeGrayscale(Section));
            var page = engine.Process(ocrImage);
            Console.WriteLine(page.GetText());

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

        //Source: https://web.archive.org/web/20130111215043/http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        public Bitmap MakeGrayscale(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            Graphics g = Graphics.FromImage(newBitmap);
            ColorMatrix colorMatrix;

            //create the grayscale then invert ColorMatrix
            colorMatrix = new ColorMatrix(
                new float[][]{
                        new float[] {-.3f, -.3f, -.3f, 0, 0},
                        new float[] {-.59f, -.59f, -.59f, 0, 0},
                        new float[] {-.11f, -.11f, -.11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {1, 1, 1, 0, 1}
                }
            );

            //create some image attributes
            ImageAttributes attributes = new ImageAttributes();

            //set the color matrix attribute
            attributes.SetColorMatrix(colorMatrix);

            //draw the original image on the new image
            //using the grayscale color matrix
            g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
               0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);

            //dispose the Graphics object
            g.Dispose();
            return newBitmap;
        }
    }
}
