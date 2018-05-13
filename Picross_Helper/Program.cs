using System;
using System.Collections.Generic;
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

            Thread.Sleep(1000);
            Game.GetScreenshot("pib").Save("b:\\current.png", System.Drawing.Imaging.ImageFormat.Png);
            Board.setImage(Game.GetScreenshot("pib"));
            Console.ReadKey();
        }
    }
}
