using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;

namespace Picross_Helper
{
    class Program
    {
        static void Main(string[] args)
        {
            CaptureApplication Game = new CaptureApplication();
            BoardAnalyze Board = new BoardAnalyze();
            Bitmap tester = new Bitmap("B:\\current.png");
            //Bitmap tester = new Bitmap("B:\\dobang2.jpg");

            Console.ReadKey();
            //Thread.Sleep(300);
            Board.MakeGrayscale(Game.GetScreenshot("pib")).Save("C:\\Users\\MDuh\\Desktop\\ocr ttrain\\captured2\\pib.custom1.exp10.tiff", System.Drawing.Imaging.ImageFormat.Png);
            //Board.MakeGrayscale(Game.GetScreenshot("pib")).Save("C:\\Users\\MDuh\\Desktop\\ocr ttrain\\captured\\10.tiff", System.Drawing.Imaging.ImageFormat.Tiff);
            //Board.setImage(Game.GetScreenshot("pib"));

            Board.setImage(tester);
            Console.ReadKey();
        }
    }
}
