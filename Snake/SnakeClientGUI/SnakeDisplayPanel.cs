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
        /// Draws on this panel, the specified world, centering it around the specified player ID. 
        /// If the player ID does not correspond to a live player, draws the entire world.
        /// </summary>
        /// <param name="world"></param>
        public void updatePanel(World world, int PlayerID)
        {
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
            Rectangle background = new Rectangle(0, 0, Size.Width, Size.Height);
            e.Graphics.FillRectangle(Brushes.White, background);
            //Insert code here to render the panel.
        }

        /// <summary>
        /// Draws the snakes for the world, centered on the snake designated as PlayerID 
        /// </summary>
        private void drawSnakes(PaintEventArgs e, World world, int PlayerID)
        {
            //Look at each snake, to see if its worth drawing
            foreach (Snake s in world.getLiveSnakes())
            {
                foreach (SnakeModel.Point p in s.getVerticies())
                {
                    //If any of the snakes verticies are in our view, draw said snake.
                    if (viewContains(e, world, PlayerID, p))
                    {
                        drawSnake(e, world, PlayerID, s.getID());
                    }
                    
                }
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
        private void drawSnake(PaintEventArgs e, World world, int playerID, int snakeID)
        {
            throw new NotImplementedException();
        }

        private bool viewContains(PaintEventArgs e, World world, int playerID, SnakeModel.Point p)
        {
            throw new NotImplementedException();
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
