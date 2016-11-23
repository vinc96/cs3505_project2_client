///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SnakeClientGUI;

namespace SnakeClient
{
    /// <summary>
    /// The main window of the snake client. 
    /// </summary>
    public partial class MainWindow : Form
    {
        /// <summary>
        /// The network controller to recieve and send commands.
        /// </summary>
        public ClientSnakeNetworkController clientNetworkController;
        /// <summary>
        /// The PlayerID that we're focusing on.
        /// </summary>
        private int PlayerId;
        /// <summary>
        /// The game world to draw (and pull names from).
        /// </summary>
        private World GameWorld;
        /// <summary>
        /// Creates a new MainWindow.
        /// </summary>
        public MainWindow()
        {
            clientNetworkController = new ClientSnakeNetworkController();
            InitializeComponent();
        }
        /// <summary>
        /// Sets the spectate ID to the specified ID. If that ID corresponds to a live snake in the current world, we now view the world from their perspective.
        /// </summary>
        /// <param name="id"></param>
        public void setSpectateID(int id)
        {
            PlayerId = id;
            spectateButton.Text = "Spectating";
        }

        /// <summary>
        /// On load, register our own PreviewKeyDown listener with every control on the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Load(object sender, EventArgs e)
        {

            foreach (Control control in this.Controls)
            {
                control.PreviewKeyDown += new PreviewKeyDownEventHandler(MainWindow_PreviewKeyDown);
            }

        }
        /// <summary>
        /// Executed when we hit the Connect button. Connects to a server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnectToServer_Click(object sender, EventArgs e)
        {
            string hostname = inpHostname.Text;
            string playerName = inpPlayerName.Text;

            if (hostname == "")
            {
                MessageBox.Show("The Server Hostname Cannot Be Blank");
                return;
            }

            if (playerName == "")
            {
                MessageBox.Show("The Player Name Cannot Be Blank");
                return;
            }


            this.btnConnectToServer.Enabled = false;
            bool result = clientNetworkController.connectToServer(hostname, playerName, handleHandshakeSuccess);

            if (!result)
            {
                MessageBox.Show("Unable To Connect To The Server");
                this.btnConnectToServer.Enabled = true;
                return;
            }
        }
        /// <summary>
        /// A callback to handle a successful server connection.
        /// </summary>
        /// <param name="initData"></param>
        private void handleHandshakeSuccess(ClientSnakeNetworkController.InitData initData)
        {
            Invoke(new MethodInvoker(() => {
                this.inpHostname.Enabled = false;
                this.inpPlayerName.Enabled = false;
                this.snakeDisplayPanel1.Focus();
            }));

            this.PlayerId = initData.PlayerId;
            this.GameWorld = new World(initData.WorldSize.X, initData.WorldSize.Y);

            clientNetworkController.startDataListenerLoop(recievedData);
        }
        /// <summary>
        /// Handle recieving data from the socket. Update the apropriate parts of the model.
        /// </summary>
        /// <param name="data"></param>
        private void recievedData(IList<string> data)
        {
            foreach(string json in data)
            {
                object gameObj = parseJsonIntoGameModel(json);
                lock (GameWorld)
                {
                    
                    if (gameObj is Snake)
                    {
                        GameWorld.updateWorldSnakes((Snake)gameObj);
                        continue;
                    }

                    if (gameObj is Food)
                    {
                        GameWorld.updateWorldFood((Food)gameObj);
                        continue;
                    }
                }
            }
            Invoke(new MethodInvoker(updateView));
        }
        /// <summary>
        /// Updates the view. Called on a per-tick basis, whenever there's new data.
        /// </summary>
        private void updateView()
        {
            //Update game display
            snakeDisplayPanel1.updatePanel(GameWorld, PlayerId);
            //Update score panel
            snakePlayerPanel1.UpdatePlayerNames(GameWorld, PlayerId);
            //If we're dead, enable the spectate button.
            if (!GameWorld.IsPlayerAlive(PlayerId))
            {
                if (!spectateButton.Enabled)
                {
                    spectateButton.Enabled = true;
                }
            }
        }

        private object parseJsonIntoGameModel(string json)
        {
            JObject obj = JObject.Parse(json);

            bool isSnake = obj.Property("vertices") != null;
            if (isSnake)
            {
                return obj.ToObject<Snake>();
            }

            bool isFood = obj.Property("loc") != null;
            if (isFood)
            {
                return obj.ToObject<Food>();
            }

            return null;
        }
        /// <summary>
        /// Called when our spectate button is clicked. Brings up a selection menu, so you can select which player you want to spectate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void spectateButton_Click(object sender, EventArgs e)
        {
            SpectatorSelect s = new SpectatorSelect(GameWorld.getLiveSnakesOrdered().Values, this);
            s.ShowDialog();

        }

        /// <summary>
        /// The PreviewKeyDown listener registered with every control on the form. 
        /// Ensures that all input will always be forwarded to the snake, as opposed to it being stolen by a dropdown/menu/textbox/etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            int direction = e.KeyValue - 37;
            if (direction >= 0 && direction <= 3)
            {
                direction = (direction == 0) ? 4 : direction;
                clientNetworkController.sendDirection(direction);
            }
        }
        /// <summary>
        /// When we start to close the form, start closing the connection to the server, with the settings to close the form when the socket is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!clientNetworkController.isTheConnectionAlive())
            {
                return;
            }

            clientNetworkController.closeConnection(handleSocketClosed);
            e.Cancel = true;
        }
        /// <summary>
        /// A simple callback to close the window. Used as a callback when we're closing the socket.
        /// </summary>
        private void handleSocketClosed()
        {
            Invoke(new MethodInvoker(this.Close));
        }

        
    }
}
