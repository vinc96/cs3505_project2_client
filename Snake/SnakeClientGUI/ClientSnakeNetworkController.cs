///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using NetworkController;
using SnakeModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SnakeClient
{
    /// <summary>
    /// The controler for the Snake class. Uses the Networking static class to send and recieve snake commands.
    /// </summary>
    public class ClientSnakeNetworkController
    {
       
        /// <summary>
        /// The socket state that we use to communicate with the remote server.
        /// </summary>
        SocketState clientSocketState;

        /// <summary>
        /// Indicates whether or not we have completed all the neccisary steps required to send movement connections.
        /// </summary>
        bool initialized = false;

        /// <summary>
        /// The data we recieved from the server on the handshake.
        /// </summary>
        public struct InitData
        {
            public InitData(int playerId, int WorldWidth, int WorldHeight)
            {
                PlayerId = playerId;
                WorldSize = new World.Dimensions(WorldWidth, WorldHeight);

                ErrorOccured = false;
                ErrorMessage = null;
            }

            public InitData(string errorMessage)
            {
                ErrorOccured = true;
                ErrorMessage = errorMessage;
                PlayerId = -1;
                WorldSize = new World.Dimensions(-1, -1);
            }


            public bool ErrorOccured { get; private set; }
            public string ErrorMessage { get; private set; }

            public int PlayerId {get; private set; }
            public World.Dimensions WorldSize {get; private set; }
        }
        /// <summary>
        /// A delegate used to handle the initial setup data.
        /// </summary>
        /// <param name="initData"></param>
        public delegate void handleInitData(InitData initData);
        /// <summary>
        /// A delegate used to handle the data we recieve on the socket (in list form)
        /// </summary>
        /// <param name="data"></param>
        public delegate void handleDataReceived(IList<string> data);
        /// <summary>
        /// A delegate that we use when the socket is closed.
        /// </summary>
        public delegate void handleSocketClosed();

        /// <summary>
        /// Connect this controler to the specified server, using the passed hostname, playername, and handler to call when the handshake is complete.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="playerName"></param>
        /// <param name="handshakeCompletedHandler"></param>
        public void connectToServer(string hostname, string playerName, handleInitData handshakeCompletedHandler)
        {
            if (clientSocketState != null) { return; }

            int connectedTimeout = 2500;
            Socket s = Networking.ConnectToNetworkNode(
                hostname,
                Networking.DEFAULT_PORT,
                (ss) => { handleConnectedToServer(ss, playerName, handshakeCompletedHandler); },
                connectedTimeout
                );
        }

        /// <summary>
        /// The method to use when we've connected to the Server.
        /// </summary>
        /// <param name="aSocketState"></param>
        /// <param name="playerName"></param>
        /// <param name="handshakeCompletedHandler"></param>
        private void handleConnectedToServer(SocketState aSocketState, string playerName, handleInitData handshakeCompletedHandler)
        {
            clientSocketState = aSocketState;

            if (aSocketState.errorOccured)
            {
                handshakeCompletedHandler(new InitData(aSocketState.errorMesssage));
                closeConnection();
                return;
            }

            Networking.Send(aSocketState.theSocket, playerName + '\n');
            Networking.listenForData(aSocketState, (ss) => { worldSetupDataRecieved(ss, handshakeCompletedHandler); });
        }
        /// <summary>
        /// Takes the data from the very first server transmission (world size and player ID), and sets up the world using it.
        /// </summary>
        /// <param name="aSocketState"></param>
        /// <param name="handshakeCompletedHandler"></param>
        private void worldSetupDataRecieved(SocketState aSocketState, handleInitData handshakeCompletedHandler)
        {
            if (!isTheConnectionAlive())
            {
                return;
            }

            if (aSocketState.errorOccured)
            {
                handshakeCompletedHandler(new InitData(aSocketState.errorMesssage));
                closeConnection();
                return;
            }

            IList<String> setupData = Networking.getMessageStringsFromBufferSeperatedByCharacter(aSocketState, '\n');

            //Expects 3 Lines Of Startup Data, If It Isn't Recieved Continue Listening And Resets Buffer
            if(setupData.Count() < 3)
            {
                Networking.resetGrowableBufferWithMessagesSeperatedByCharacter(aSocketState, setupData, '\n');
                Networking.listenForData(aSocketState, (ss) => { worldSetupDataRecieved(ss, handshakeCompletedHandler); });
                return;
            }

            int playerId;
            int worldWidth;
            int worldHeight;

            Int32.TryParse(setupData[0], out playerId);
            Int32.TryParse(setupData[1], out worldWidth);
            Int32.TryParse(setupData[2], out worldHeight);
            
            handshakeCompletedHandler(new InitData(playerId, worldWidth, worldHeight));
            initialized = true;
        }
        /// <summary>
        /// Start the loop to constantly listen for snake and food transmissions from the server.
        /// </summary>
        /// <param name="dataReceivedHandler"></param>
        public void startDataListenerLoop(handleDataReceived dataReceivedHandler)
        {
            Networking.listenForData(clientSocketState, (ss) => { receiveDataAndStartListeningForMoreData(ss, dataReceivedHandler); });
        }
        /// <summary>
        /// A callback that takes whatever data the socket has spit out, parse it out into a list of objects, 
        /// and then passes it out to the specifed handler.
        /// </summary>
        /// <param name="aSocketState"></param>
        /// <param name="dataReceivedHandler"></param>
        public void receiveDataAndStartListeningForMoreData(SocketState aSocketState, handleDataReceived dataReceivedHandler)
        {
            if (!isTheConnectionAlive())
            {
                return;
            }

            if (aSocketState.errorOccured)
            {
                string[] errorStringArray = {"ERROR", aSocketState.errorMesssage}; 
                dataReceivedHandler(errorStringArray);
                closeConnection();
                return;
            }

            IList<string> data = Networking.getMessageStringsFromBufferSeperatedByCharacter(aSocketState, '\n');

            dataReceivedHandler(data);

            startDataListenerLoop(dataReceivedHandler);
        }

        /// <summary>
        /// Sends the specified direction input to the server.
        /// </summary>
        public void sendDirection(int direction)
        {
            if (!initialized)
            {
                return;
            }

            Networking.Send(clientSocketState.theSocket, "("+direction+")\n");
        }
        /// <summary>
        /// Check to see if the connection is currently alive (e.g. connected to a server).
        /// </summary>
        /// <returns></returns>
        public bool isTheConnectionAlive()
        {
            return clientSocketState != null && clientSocketState.safeToSendRequest;
        }       
        /// <summary>
        /// Close the connection for this server, with no callback.
        /// </summary>
        internal void closeConnection()
        {
            closeConnection(() => { });
        }
        /// <summary>
        /// Close the connection for this server, using the specified handler passed.
        /// </summary>
        /// <param name="handleDisconnect"></param>
        internal void closeConnection(handleSocketClosed handleDisconnect)
        {
            Networking.Disconnect(clientSocketState, false, (ss) => { socketDisconected(ss, handleDisconnect); });
        }
        /// <summary>
        /// A callback that's used when we disconnect from the socket using the Networking class.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="handleDisconnect"></param>
        private void socketDisconected(SocketState ss, handleSocketClosed handleDisconnect)
        {
            ss.theSocket.Close();
            handleDisconnect();
        }
    }

}
