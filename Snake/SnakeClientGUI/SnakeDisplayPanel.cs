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

        /// <summary>
        /// A struct containing paramaters for drawing a frame. Contains cell size in double form for maximum accuracy.
        /// </summary>
        struct ViewParams
        {
            public ViewParams(double CellSizeX, double CellSizeY)
            {
                this.CellSizeX = CellSizeX;
                this.CellSizeY = CellSizeY;
                this.CellSizeXRounded = (int)Math.Round(CellSizeX);
                this.CellSizeYRounded = (int)Math.Round(CellSizeY);
            }
            public double CellSizeY
            {
                get; private set;
            }

            public double CellSizeX
            {
                get; private set;
            }

            public int CellSizeYRounded
            {
                get; private set;
            }

            public int CellSizeXRounded
            {
                get; private set;
            }
        }

        override protected void OnResize(EventArgs e)
        {
            base.OnResize(e); //Call the parent method.
            Invalidate();
        }

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
            

            if (!ReferenceEquals(world, null))
            {
                //Turn on antialiasing.
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                //Now, calculate cell size and pass it to a new ViewParams object.
                ViewParams view = new ViewParams(((double)Size.Width / world.Size.X), ((double)Size.Height / world.Size.Y));
                drawWorldBorders(e, world, view);
                drawSnakes(e, world, view, playerID);
            }
        }

        private void drawWorldBorders(PaintEventArgs e, World world, ViewParams view)
        {
            Rectangle leftWall = new Rectangle(0, 0, view.CellSizeXRounded, Size.Height);
            Rectangle topWall = new Rectangle(0, 0, Size.Width, view.CellSizeYRounded);
            Rectangle rightWall = new Rectangle(Size.Width - view.CellSizeXRounded, 0,
                view.CellSizeXRounded, Size.Height);
            Rectangle bottomWall = new Rectangle(0, Size.Height - view.CellSizeYRounded, 
                Size.Width, view.CellSizeYRounded);

            Rectangle[] walls = { leftWall, topWall, rightWall, bottomWall };

            e.Graphics.FillRectangles(Brushes.Black, walls);
        }

        /// <summary>
        /// Draws the snakes for the world, centered on the snake designated as PlayerID 
        /// </summary>
        private void drawSnakes(PaintEventArgs e, World world, ViewParams view, int PlayerID)
        {
            //Look at each snake, to see if its worth drawing
            foreach (Snake s in world.getLiveSnakes())
            {
               drawSnake(e, view, s);
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
        private void drawSnake(PaintEventArgs e, ViewParams view, Snake snake)
        {
            IEnumerable<SnakeModel.Point> verts = snake.getVerticies();
            
            SnakeModel.Point previousVert = null;

            //Derive our brush from our playerID:
            Brush snakeBrush = new SolidBrush(Color.Red);
            foreach(SnakeModel.Point vert in verts)
            {
                if(ReferenceEquals(previousVert, null))
                {
                    previousVert = vert;
                    continue;
                }
                
                //We round all our values in order to attempt to eliminate being off by one pixel.
                int topCornerX = (int)Math.Round(Math.Min(previousVert.PointX, vert.PointX) * view.CellSizeX);
                int topCornerY = (int)Math.Round(Math.Min(previousVert.PointY, vert.PointY) * view.CellSizeY);
                int rectangleCornerX, rectangleCornerY;
                int bottomCornerX, bottomCornerY;

                int segmentWidth = (int)Math.Round((Math.Abs(previousVert.PointX - vert.PointX) + 1) * view.CellSizeX);
                int segmentHeight = (int)Math.Round((Math.Abs(previousVert.PointY - vert.PointY) + 1) * view.CellSizeY);
                //Case where the segment is horizontal
                if (segmentWidth > segmentHeight)
                {
                    //Shorten the rectangle part of the segment, and offset it.
                    segmentWidth -= view.CellSizeXRounded;
                    rectangleCornerX = topCornerX + view.CellSizeXRounded/2;
                    rectangleCornerY = topCornerY;
                    //Set the parameters for the bottom corner circle.
                    bottomCornerX = topCornerX + segmentWidth;
                    bottomCornerY = topCornerY;

                } else //Case where segment is vertical
                {
                    segmentHeight -= view.CellSizeYRounded;
                    rectangleCornerX = topCornerX;
                    rectangleCornerY = topCornerY + view.CellSizeYRounded/2;
                    //Set the parameters for the bottom corner circle.
                    bottomCornerX = topCornerX;
                    bottomCornerY = topCornerY + segmentHeight;
                }
                //Draw the circular endpoints for the snake.
                Rectangle topEndpoint = new Rectangle(topCornerX, topCornerY, view.CellSizeXRounded, view.CellSizeYRounded);
                Rectangle snakeSegment = new Rectangle(rectangleCornerX, rectangleCornerY, segmentWidth, segmentHeight);
                Rectangle bottomEndpoint = new Rectangle(bottomCornerX, bottomCornerY, view.CellSizeXRounded, view.CellSizeYRounded);
                e.Graphics.FillRectangle(snakeBrush, snakeSegment);
                e.Graphics.FillEllipse(snakeBrush, topEndpoint);
                e.Graphics.FillEllipse(snakeBrush, bottomEndpoint);
                //e.Graphics.DrawRectangle(Pens.Black, snakeSegment); //Useful for debugging
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
