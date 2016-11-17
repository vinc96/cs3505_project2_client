using SnakeModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeClientGUI
{
    class SnakeDisplayPanel : Panel
    {
        /// <summary>
        /// The world that the updatePanel method was called with most recently.
        /// </summary>
        World world;

        /// <summary>
        /// The playerID that the updatePanel method was called with most recently.
        /// </summary>
        int playerID;

        int cellSize;
        int PanelSize;



        /// <summary>
        /// Draws on this panel, the specified world, centering it around the specified player ID. 
        /// If the player ID does not correspond to a live player, draws the entire world.
        /// </summary>
        /// <param name="world"></param>
        public void updatePanel(World world, int playerID)
        {
            this.world = world;
            this.playerID = playerID;

            //In here, we should do anything we need to do to prepare for painting a panel.
            this.Invalidate();//Tell the form to repaint the panel.
        }

        /// <summary>
        /// Override of the OnPaint method. Draws our perspective based upon the current zoom level and current snakes/food in the level.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //First, draw the white background
            e.Graphics.Clear(Color.White);
            if(!ReferenceEquals(world, null))
            {
                drawWorldBorders(e, world);
                drawSnakes(e, world, playerID);
            }



            //Insert code here to render the panel.
            //Draw all the snakes in the world
        }

        private void drawWorldBorders(PaintEventArgs e, World world)
        {
            int cellSizeX = Size.Width / world.Size.X;
            int cellSizeY = Size.Height / world.Size.Y;


            Rectangle leftWall = new Rectangle(0, 0, cellSizeX, Size.Height);
            Rectangle topWall = new Rectangle(0, 0, Size.Width, cellSizeY);
            Rectangle rightWall = new Rectangle(Size.Width - cellSizeX, 0, cellSizeX, Size.Height);
            Rectangle bottomWall = new Rectangle(0, Size.Height - cellSizeY, Size.Width, cellSizeY);

            Rectangle[] walls = { leftWall, topWall, rightWall, bottomWall };

            e.Graphics.FillRectangles(Brushes.Black, walls);
        }

        /// <summary>
        /// Draws the snakes for the world, centered on the snake designated as PlayerID 
        /// </summary>
        private void drawSnakes(PaintEventArgs e, World world, int PlayerID)
        {
            //Look at each snake, to see if its worth drawing
            foreach (Snake s in world.getLiveSnakes())
            {
               drawSnake(e, s);
            }
        }
        /// <summary>
        /// Draws the snake specified by snakeID, based upon the view specified by playerID, 
        /// based upon data from world, in the graphics object specified by PaintEventArgs.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="world"></param>
        /// <param name="playerID"></param>
        /// <param name="snakeID"></param>
        private void drawSnake(PaintEventArgs e, Snake snake)
        {
            double cellSizeX = (double) Size.Width / world.Size.X;
            double cellSizeY = (double) Size.Height / world.Size.Y;

            IEnumerable<SnakeModel.Point> verts = snake.getVerticies();

            SnakeModel.Point previousVert = null;

            foreach(SnakeModel.Point vert in verts)
            {
                if(ReferenceEquals(previousVert, null))
                {
                    previousVert = vert;
                    continue;
                }

                int segmentCornerX = (int) (Math.Min(previousVert.PointX, vert.PointX) * cellSizeX);
                int segmentCornerY = (int)(Math.Min(previousVert.PointY, vert.PointY) * cellSizeY);

                int segmentWidth = (int)((Math.Abs(previousVert.PointX - vert.PointX) + 1) * cellSizeX);
                int segmentHeight = (int)((Math.Abs(previousVert.PointY - vert.PointY) + 1) * cellSizeY);

                Rectangle snakeSegment = new Rectangle(segmentCornerX, segmentCornerY, segmentWidth, segmentHeight);
                e.Graphics.FillRectangle(Brushes.Aqua, snakeSegment);
                e.Graphics.DrawRectangle(Pens.Black, snakeSegment);
                //our current vert is now the previous vert
                previousVert = vert;
            }
        }
        


        /// <summary>
        /// Takes a global point (referring to some entities absolute position on the map), and transforms it to a relative point, with coordinates 
        /// relative to the head specified by the passed PlayerID. If the pased PlayerID doesn't refer to a live snake in the passed world, returns the 
        /// same point, untransformed.
        /// </summary>
        /// <param name="globalPoint"></param>
        /// <returns></returns>
        private SnakeModel.Point transformGlobalToLocalPoint(World world, int PlayerID, SnakeModel.Point globalPoint)
        {
            if (!world.IsPlayerAlive(PlayerID))
            {
                return globalPoint;
            }
            //Grab the player head location
            SnakeModel.Point playerHead = world.getHead(PlayerID);

            //A relative point is just a global point, minus the head location of the specified snake. 
            return new SnakeModel.Point(globalPoint.PointX - playerHead.PointX, globalPoint.PointY - playerHead.PointY);
        }
    }
}
