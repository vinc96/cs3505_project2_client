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
            if (!(Disposing || IsDisposed))
            {
                Invoke(new MethodInvoker(() =>
                {

                    //Update game display
                    snakeDisplayPanel1.updatePanel(GameWorld, PlayerId);
                    //Update score panel
                    snakePlayerPanel1.UpdatePlayerNames(GameWorld, PlayerId);
                }));
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

        private void snakeDisplayPanel1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
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
            clientNetworkController.closeConnection();
        }
    }
}
