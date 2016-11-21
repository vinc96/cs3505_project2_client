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
    class SnakePlayerPanel : Panel
    {
        /// <summary>
        /// The world that the DrawPlayerNames method was called with most recently.
        /// </summary>
        private World world;

        /// <summary>
        /// The playerID that the DrawPlayerNames method was called with most recently.
        /// </summary>
        private int playerID;

        private const int PlayerNameTextHeight = 16;

        private const int PlayerBuffer = 4;

        public SnakePlayerPanel() : base()
        { 
            AutoScroll = true;
            DoubleBuffered = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Color.DarkGray); //Draw the BG
            int textY = 0;
            e.Graphics.DrawString("Players: ", new Font("Times New Roman", PlayerNameTextHeight, FontStyle.Bold, new GraphicsUnit()), Brushes.White, 0, textY);
            if (!ReferenceEquals(world, null))
            {
                lock (world)
                {
                    foreach (Snake s in world.getLiveSnakes())
                    {
                        textY += PlayerNameTextHeight + PlayerBuffer;
                        e.Graphics.DrawString(s.name + ": " + s.length, new Font("Times New Roman", PlayerNameTextHeight,
                            FontStyle.Bold, new GraphicsUnit()), new SolidBrush(s.Color), 0, textY);
                    }
                }
            }                      
        }
        /// <summary>
        /// Draws the names of the players on this panel, with the specified SnakeID at the top of the panel in bold.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="world"></param>
        /// <param name="SnakeID"></param>
        public void UpdatePlayerNames(World world, int SnakeID)
        {
            this.world = world;
            this.playerID = SnakeID;
            Invalidate();
        }
    }
}
