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

            Thread.Sleep(1000);
            //Game.GetScreenshot("pib").Save("b:\\current.png", System.Drawing.Imaging.ImageFormat.Png);
            //Board.setImage(Game.GetScreenshot("pib"));
            Board.setImage(tester);
            Console.ReadKey();
        }
    }
}
