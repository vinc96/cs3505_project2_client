using NetworkController;
using SnakeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SnakeClient
{
    class SnakeController
    {
        /// <summary>
        /// The world that this SnakeController controls.
        /// </summary>
        World world;

        /// <summary>
        /// The socket state that we use to communicate with the remote server.
        /// </summary>
        SocketState clientSocketState;

        /// <summary>
        /// Indicates whether or not we have completed all the neccisary steps required to send movement connections.
        /// </summary>
        bool initialized;

        /// <summary>
        /// The name of the player, passed when we try to connect to a server
        /// </summary>
        string playerName;

        /// <summary>
        /// 
        /// </summary>
        int playerID;

        /// <summary>
        /// Creates a new controler to recieve user input, and update the local world when the server sends updates
        /// </summary>
        /// <param name="world"></param>
        SnakeController(World world, Func<int> assignedID)
        {
            this.world = world;
        }
        
        public bool connectToServer(string hostname, string playerName)
        {
            this.playerName = playerName;
            Socket s = Networking.ConnectToNetworkNode(hostname, Networking.DEFAULT_PORT, handleNetworkNodeConnected);
            return !ReferenceEquals(s, null);
        }

        private void handleNetworkNodeConnected(SocketState aSocketState)
        {
            clientSocketState = aSocketState;
            Networking.Send(clientSocketState.theSocket, playerName + '\n');
            Networking.listenForData(clientSocketState, worldSetupDataRecieved);
        }

        private void worldSetupDataRecieved(SocketState aSocketState)
        {
            IList<String> setupData = Networking.getMessageStringsFromBufferSeperatedByCharacter(clientSocketState, '\n');
            //Parse out our client ID

        }

        /// <summary>
        /// Sends the specified direction input to the server.
        /// If the controller has not successfully connected to a server yet, returns false.
        /// </summary>
        public bool sendDirection(int direction)
        {
            if (!initialized)
            {
                return false;
            }

            //Send Command

            return true;
        }

    }

}
