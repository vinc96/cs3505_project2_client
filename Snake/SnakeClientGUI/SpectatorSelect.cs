using SnakeClient;
using SnakeModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SnakeClientGUI
{
   
    public partial class SpectatorSelect : Form
    {
        /// <summary>
        /// The snakes that we're choosing to spectate from.
        /// </summary>
        IEnumerable<Snake> snakesToChooseFrom;
        /// <summary>
        /// The window that created this spectator select box. Used to call the SetSpectatorID method.
        /// </summary>
        MainWindow parent;
        public SpectatorSelect(IEnumerable<Snake> snakesToChooseFrom, MainWindow parent)
        {
            this.snakesToChooseFrom = snakesToChooseFrom;
            this.parent = parent;
            InitializeComponent();
        }

        /// <summary>
        /// Called when the form loads. Populates the spectateSelectBox with the options we constructed with.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpectatorSelect_Load(object sender, EventArgs e)
        {
            foreach (Snake s in snakesToChooseFrom)
            {
                spectateSelectBox.Items.Add(s);
            }
        }
        /// <summary>
        /// Called when the "Spectate" button on the form is clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            //If we haven't selected something, we've "canceled"
            if (ReferenceEquals(spectateSelectBox.SelectedItem, null))
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                //If we have, set the spectate ID to be this ID.
                DialogResult = DialogResult.OK;
                parent.setSpectateID(((Snake)(spectateSelectBox.SelectedItem)).ID);
            }
            //Close the window.
            this.Close();
        }
        /// <summary>
        /// Called when we hit the "Cancel" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
