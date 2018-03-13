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
    class GridManager
    {
        //initial instance of 2d array of Grid
        public Grid[,] arrayGrid = new Grid[0, 0];

        //instance for total X and Y blocks from MAIN program
        private int Xsize = 0;
        private int Ysize = 0;

        //sharing block size to all Grid
        static int Bsize = 0;

        CDrawer canvas;

        //setting variable for Grid
        public GridManager(int X, int Y, int size, CDrawer _canvas)
        {
            Bsize = size;
            Xsize = X;
            Ysize = Y;
            arrayGrid = initGrid(X, Y);
            canvas = _canvas;
        }

        //make an initial 2D array for Grid array
        public Grid[,] initGrid(int x, int y)
        {
            Grid[,] result = new Grid[x, y];

            for (int row = 0; row < result.GetLength(0); row++)
            {
                for (int col = 0; col < result.GetLength(1); col++)
                {
                    //variable for holding current row and col of current grid
                    Point tempGrid = new Point(row, col);
                    //variable for holding default point
                    Point tempPos = new Point(0, 0);
                    result[row, col] = new Grid(tempPos, Color.Black, Alive.dead, Type.empty, Action.empty, tempGrid, Bsize);
                }
            }

            return result;
        }

        //Show all Grid in white border
        //FOR TESTING ONLY
        public void ShowAll(CDrawer canvas)
        {
            foreach (Grid item in arrayGrid)
            {
                canvas.AddRectangle(item.currentGrid.X * Bsize, item.currentGrid.Y * Bsize, Bsize, Bsize, Color.Black, 2, Color.White);
                canvas.Render();
            }
        }


        //adding a red grid when click mouse left
        public void AddLeft(Point pos)
        {
            //get the location of X and Y base on 2D grid array
            int X = pos.X / Bsize;
            int Y = pos.Y / Bsize;

            //lock array for preventing extra data on current working data
            lock (arrayGrid)
            {
                //check if current grid is EMPTY or SOLID
                if (arrayGrid[X, Y].live == Alive.dead && arrayGrid[X, Y].type != Type.l0ck)
                {
                    arrayGrid[X, Y] = new Grid(new Point(X * Bsize, Y * Bsize), Color.Red, Alive.alive, Type.l0ck, Action.Falling, new Point(X, Y), Bsize);
                }
            }
        }

        //ADDING a yellow grid or REMOVE exist grid when click mouse right
        //
        //Because program working on console application, there is some troubles
        //for reading key from console and using canvas at the same time
        //
        //So my solution is just using only 1 right click,
        //and check if current grid is EMPTY, it will make a solid grid.
        //If it EXIST already, it will be removed
        //
        //When removing, program just work only with current removing grid
        //Another falling grids will be stopped until REMOVAL COMPLETE
        public void AddRight(Point pos)
        {
            //get the location of X and Y base on 2D grid array
            int X = pos.X / Bsize;
            int Y = pos.Y / Bsize;
            //lock array for preventing extra data on current working data
            lock (arrayGrid)
            {
                //if (arrayGrid[X, Y].live == Alive.dead && arrayGrid[X, Y].type != Type.l0ck)

                //check if current clicked location is DEAD and not LOCK by falling grid
                if (arrayGrid[X, Y].live == Alive.dead && arrayGrid[X, Y].type != Type.l0ck)
                {
                    arrayGrid[X, Y] = new Grid(new Point(X * Bsize, Y * Bsize), Color.Yellow, Alive.alive, Type.solid, Action.Stop, new Point(X, Y), Bsize);
                }
                //if current location is alive
                else if (arrayGrid[X, Y].type == Type.solid)
                {
                    //Remove(X, Y);
                    arrayGrid[X, Y].remove = Remove.removing;
                }
            }
        }

        //function for rendering all grid in array
        public void Render(CDrawer canvas)
        {
            canvas.Clear();
            //creating instance before loop for preventing creating new instance during rendering
            Point temp = new Point(0, 0);

            for (int row = 0; row < arrayGrid.GetLength(0); row++)
            {
                for (int col = 0; col < arrayGrid.GetLength(1); col++)
                {
                    if (arrayGrid[row, col].live == Alive.alive)
                    {
                        //get the location of grid for rendering
                        temp = arrayGrid[row, col].position;

                        canvas.AddRectangle(temp.X, temp.Y, arrayGrid[row, col].size, arrayGrid[row, col].size, arrayGrid[row, col].color, 3, Color.Black);

                        canvas.Render();
                    }
                }
            }
        }


        //function for falling grid
        public void Falling()
        {
            for (int row = 0; row <= Xsize - 1; row++)
            {
                for (int col = 0; col <= Ysize - 1; col++)
                {
                    //checking the grid below the SOLID falling grid. If it empty, set the SOLID into LOCK
                    //for making it falling again after stopped
                    if (col < Ysize - 1 )
                    {
                        //checking current grid is falling or not by using COLOR
                        if (arrayGrid[row, col].type == Type.solid && arrayGrid[row, col].color == Color.Red && arrayGrid[row, col + 1].live == Alive.dead)
                        {
                            arrayGrid[row, col].type = Type.l0ck;
                        }
                    }

                    //reduce the size of grid 1 by 1
                    //when Grid is on Removing State
                    if (arrayGrid[row, col].remove == Remove.removing)
                    {
                        
                        if (arrayGrid[row, col].size > 1)                     
                            arrayGrid[row, col].size--;
                        else
                            arrayGrid[row, col] = new Grid(new Point(0, 0), Color.Black, Alive.dead, Type.empty, Action.empty, new Point(0, 0), Bsize);

                    }

                    //if current grid is LOCK and FALLING
                    if (arrayGrid[row, col].type == Type.l0ck && arrayGrid[row, col].action == Action.Falling)
                    {
                        //set the limit for preventing out of border
                        if (col < Ysize)
                        {
                            //checking the next grid is solid and alive
                            //and making the current falling grid stop and solid
                            if (col < Ysize - 1 && arrayGrid[row, col + 1].type == Type.solid)
                            {
                                arrayGrid[row, col].type = Type.solid;
                                break;
                            }

                            //checking up to 1 row before the very bottom row 
                            //if the next row is EMPTY and LOCK already
                            //program will make the current grid fall down
                            if (col < Ysize - 1 && (arrayGrid[row, col + 1].type == Type.empty || arrayGrid[row, col + 1].type == Type.l0ck))
                            {
                                //instance for calculate what is the point for moving into next grid
                                int nextGrid;

                                //if the row is not the very top one
                                if (arrayGrid[row, col].currentGrid.Y != 0)
                                    nextGrid = (arrayGrid[row, col].currentGrid.Y * Bsize) + Bsize;
                                else
                                    nextGrid = Bsize;

                                //If the next grid is empty
                                //program will lock it for make grid fall into it
                                if (arrayGrid[row, col + 1].type == Type.empty)
                                    arrayGrid[row, col + 1].type = Type.l0ck;


                                //if the current grid position not reach the changing point into next grid
                                if (arrayGrid[row, col].position.Y <= nextGrid + 1)
                                {
                                    //Y position will increase by 1
                                    arrayGrid[row, col].position.Y += 1;
                                }
                                else
                                {
                                    //if the grid reach the point of next grid
                                    //transfer data from current grid to new grid, and make it alive
                                    arrayGrid[row, col + 1] = new Grid(arrayGrid[row, col].position, arrayGrid[row, col].color, Alive.alive, arrayGrid[row, col].type, arrayGrid[row, col].action, new Point(row, col + 1), Bsize);
                                    
                                    //set the current grid to default grid and DEAD
                                    arrayGrid[row, col] = new Grid(new Point(0, 0), Color.Black, Alive.dead, Type.empty, Action.empty, new Point(0, 0), Bsize);
                                }
                            }
                            //if checking row is the very bottom one
                            else if (col == Ysize - 1)
                            {
                                //make it SOLID and STOP
                                arrayGrid[row, col].type = Type.solid;
                            }                          
                        }
                      
                    }

                }
            }

        }

    }
}

