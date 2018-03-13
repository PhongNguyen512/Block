using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace Block
{
    //some Enum for control state and status of Grid
    public enum Alive { alive, dead }
    public enum Type { free, solid, empty, l0ck }
    public enum Action { Falling, Stop, empty }
    public enum Remove { removing, empty};
  
    class Grid
    {
        public Point position;
        public Color color;
        public Alive live;
        public Type type;
        public Action action;
        public Point currentGrid;
        public Remove remove;
        public int size;

        //set variable to each properties of Grid
        public Grid(Point p, Color c, Alive l, Type t, Action a, Point g, int s)
        {
            position = p;
            color = c;
            live = l;
            type = t;
            action = a;
            currentGrid = g;
            size = s;
            remove = Remove.empty;
        }

    }
}
