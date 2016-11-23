///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
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
    /// <summary>
    /// A panel that displays playernames and scores.
    /// </summary>
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
        /// <summary>
        /// The height of the text of the displayed player names
        /// </summary>
        private const int PlayerNameTextHeight = 16;
        /// <summary>
        /// The number of pixels between each player name.
        /// </summary>
        private const int PlayerBuffer = 4;
        /// <summary>
        /// Creates a new SnakePlayerPanel.
        /// </summary>

        public SnakePlayerPanel() : base()
        { 
            AutoScroll = true;
            DoubleBuffered = true;
        }
        /// <summary>
        /// Paints the player names stored in this object (pulled from the most recent call of UpdatePlayerNames).
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Color.DarkGray); //Draw the BG
            //Draw the "Players: " Header
            int textY = 0;
            e.Graphics.DrawString("Players: ", new Font("Times New Roman", PlayerNameTextHeight, FontStyle.Bold, new GraphicsUnit()), Brushes.White, 0, textY);
            //Draw the playernames, as long as we have a world.
            if (!ReferenceEquals(world, null))
            {
                lock (world) //Make sure we don't draw players as we're editing the world.
                {
                    //Draw the player name at the top of the list, if it exists.
                    Snake playerSnake = world.getSnakeByID(playerID);
                    if (!ReferenceEquals(playerSnake, null))
                    {
                        textY += PlayerNameTextHeight + PlayerBuffer;
                        e.Graphics.DrawString(playerSnake.name + ": " + playerSnake.length, new Font("Times New Roman", PlayerNameTextHeight,
                            FontStyle.Underline | FontStyle.Bold, new GraphicsUnit()), new SolidBrush(playerSnake.Color), 0, textY);
                    }
                    //Draw all the rest of the player names, ordered by score.
                    foreach (Snake s in world.getLiveSnakesOrdered().Values)
                    {
                        //Make sure we don't re-draw the player's name.
                        if (ReferenceEquals(playerSnake, null) || s.ID != playerSnake.ID)
                        {
                            textY += PlayerNameTextHeight + PlayerBuffer;
                            e.Graphics.DrawString(s.name + ": " + s.length, new Font("Times New Roman", PlayerNameTextHeight,
                                FontStyle.Bold, new GraphicsUnit()), new SolidBrush(s.Color), 0, textY);
                        }
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
            Invalidate(); //This panel to be redrawn.
        }
    }
}
