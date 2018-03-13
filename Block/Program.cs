using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using GDIDrawer;
using System.Threading;

namespace Block
{
    class Program
    {
        static void Main(string[] args)
        {
            //program information
            Console.WriteLine("Instruction:");
            Console.WriteLine("Left click to create a RED BLOCK");
            Console.WriteLine("Right click to create a YELLOW BLOCK");
            Console.WriteLine("Right click on exist BLOCK to DELETE it\n");
            Console.WriteLine("RED BLOCK will fall to the bottom");
            Console.WriteLine("YELLOW BLOCK will stay solid in the click location");

            //initial constant variables
            const int blockSize = 50;

            const int windowX = 1000;
            const int windowY = 1000;

            const int blockX = windowX / blockSize;
            const int blockY = windowY / blockSize;

            //initial point for clicking left or right
            Point currentPos = new Point();

            CDrawer canvas = new CDrawer(windowX, windowY, false);
         
            if (blockSize >= 10)
            {
                GridManager gm = new GridManager(blockX, blockY, blockSize, canvas);

                do
                {
                    //add grid if left click or right click
                    if (canvas.GetLastMouseLeftClick(out currentPos))
                    {
                        gm.AddLeft(currentPos);
                    }
                    if (canvas.GetLastMouseRightClick(out currentPos))
                    {
                        gm.AddRight(currentPos);
                    }

                    gm.Render(canvas);
                    gm.Falling();

                    //for TESTING ONLY
                    //gm.ShowAll(canvas);
                    Thread.Sleep(10);

                } while (true);
            }
            else
            {
                Console.WriteLine("Block size must be greater than or equal 10");
            }
            Console.ReadKey();
        }
        


    }
}
