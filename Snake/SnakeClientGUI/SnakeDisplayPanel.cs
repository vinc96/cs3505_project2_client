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
    }
}
