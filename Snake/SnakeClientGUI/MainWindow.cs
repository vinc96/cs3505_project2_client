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
    public partial class MainWindow : Form
    {
        public ClientSnakeNetworkController clientNetworkController;

        private int PlayerId;
        private World GameWorld;

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

        ///On load, register our own PreviewKeyDown listener with every control on the form.
        private void MainWindow_Load(object sender, EventArgs e)
        {

            foreach (Control control in this.Controls)
            {
                control.PreviewKeyDown += new PreviewKeyDownEventHandler(MainWindow_PreviewKeyDown);
            }

        }

        private void btnConnectToServer_Click(object sender, EventArgs e)
        {
            string hostname = inpHostname.Text;
            string playerName = inpPlayerName.Text;

            if (hostname == "")
            {
                return;
            }

            if (playerName == "")
            {
                return;
            }
            spectateButton.Enabled = false;
            clientNetworkController.connectToServer(hostname, playerName, handleHandshakeSuccess);
        }

        private void handleHandshakeSuccess(ClientSnakeNetworkController.InitData initData)
        {
            Invoke(new MethodInvoker(() => {
                this.inpHostname.Enabled = false;
                this.inpPlayerName.Enabled = false;
                this.btnConnectToServer.Enabled = false;
                this.snakeDisplayPanel1.Focus();
            }));

            this.PlayerId = initData.PlayerId;
            this.GameWorld = new World(initData.WorldSize.X, initData.WorldSize.Y);

            clientNetworkController.startDataListenerLoop(recievedData);
        }

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

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!clientNetworkController.isTheConnectionAlive())
            {
                return;
            }

            clientNetworkController.closeConnection(handleSocketClosed);
            e.Cancel = true;
        }

        private void handleSocketClosed()
        {
            Invoke(new MethodInvoker(this.Close));
        }

        
    }
}
