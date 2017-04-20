﻿/// Citation: 
/// Based on Snake.SnakeClient.ClientSnakeNetworkController.cs written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
/// Authors:
/// Vincent Cheng (u0887427)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkController;

namespace SpreadsheetClient
{
    public class ClientController
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
        public struct StartupData
        {
            public bool ErrorOccured { get; private set; }
            public string ErrorMessage { get; private set; }

            public Dictionary<String, String> Cells { get; private set; }

            public StartupData(Dictionary<String, String> startupData)
            {
                // vinc: store the whole startup data.
                Cells = new Dictionary<string, string>();

                ErrorOccured = false;
                ErrorMessage = null;
            }

            public StartupData(string errorMessage)
            {
                // vinc: set cells to null when there are errors
                Cells = null;

                ErrorOccured = true;
                ErrorMessage = errorMessage;
            }
        }
        /// <summary>
        /// A delegate used to handle the startup data.
        /// </summary>
        /// <param name="startupData"></param>
        public delegate void handleStartupData(StartupData startupData);
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
        /// Connect this controller to the specified server, using the passed hostname, spreadsheet name, and handler to call when the handshake is complete.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="spreadsheetName"></param>
        /// <param name="handshakeCompletedHandler"></param>
        public void connectToServer(string hostname, string spreadsheetName, handleStartupData handshakeCompletedHandler)
        {
            if (isTheConnectionAlive()) { return; }

            int connectedTimeout = 2500;
            Socket s = Networking.ConnectToNetworkNode(
                hostname,
                (ss) => { handleConnectedToServer(ss, spreadsheetName, handshakeCompletedHandler); },
                Networking.DEFAULT_PORT,
                connectedTimeout
                );
        }

        /// <summary>
        /// The method to use when we've connected to the Server.
        /// </summary>
        /// <param name="aSocketState"></param>
        /// <param name="spreadsheetName"></param>
        /// <param name="handshakeCompletedHandler"></param>
        private void handleConnectedToServer(SocketState aSocketState, string spreadsheetName, handleStartupData handshakeCompletedHandler)
        {
            clientSocketState = aSocketState;

            if (aSocketState.ErrorOccured)
            {
                handshakeCompletedHandler(new StartupData(aSocketState.ErrorMesssage));
                closeConnection();
                return;
            }

            //Networking.Send(aSocketState.TheSocket, spreadsheetName + '\n');
            Networking.Send(aSocketState.TheSocket, "Connect\t" + spreadsheetName + "\t\n");
            Networking.listenForData(aSocketState, (ss) => { startupDataRecieved(ss, handshakeCompletedHandler); });
        }
        /// <summary>
        /// Takes the data from the very first server transmission (spreadsheet data), and sets up the spreadsheet using it.
        /// </summary>
        /// <param name="aSocketState"></param>
        /// <param name="handshakeCompletedHandler"></param>
        private void startupDataRecieved(SocketState aSocketState, handleStartupData handshakeCompletedHandler)
        {
            if (!isTheConnectionAlive())
            {
                return;
            }

            if (aSocketState.ErrorOccured)
            {
                handshakeCompletedHandler(new StartupData(aSocketState.ErrorMesssage));
                closeConnection();
                return;
            }

            IList<String> setupData = Networking.getMessageStringsFromBufferSeperatedByCharacter(aSocketState, '\n');

            //Expects 1 Lines Of Startup Data, If It Isn't Recieved Continue Listening And Resets Buffer
            if (setupData.Count() < 1)
            {
                Networking.resetGrowableBufferWithMessagesSeperatedByCharacter(aSocketState, setupData, '\n');
                Networking.listenForData(aSocketState, (ss) => { startupDataRecieved(ss, handshakeCompletedHandler); });
                return;
            }

            //int playerId;
            //int worldWidth;
            //int worldHeight;
            //Int32.TryParse(setupData[0], out playerId);
            //Int32.TryParse(setupData[1], out worldWidth);
            //Int32.TryParse(setupData[2], out worldHeight);
            // vinc: parse startup message to dictionary
            string[] setupData_array = setupData[0].Trim().Split('\t');
            Dictionary<string, string> cells = new Dictionary<string, string>();
            if (!setupData_array[0].Equals("Startup") || setupData_array.Length%2!=1) // vinc: ensure there are odd number of string in setupData
            {
                Console.WriteLine("invalid message: " + setupData[0]);
                throw new ArgumentException();
            }
            for(int i=1; i<setupData_array.Length; i+=2)
            {
                cells[setupData_array[i]] = setupData_array[i + 1];
            }

            handshakeCompletedHandler(new StartupData(cells));
            initialized = true;
        }
        /// <summary>
        /// Start the loop to constantly listen for command transmissions from the server.
        /// </summary>
        /// <param name="dataReceivedHandler"></param>
        public void startDataListenerLoop(handleDataReceived dataReceivedHandler)
        {
            Networking.listenForData(clientSocketState, (ss) => { receiveDataAndStartListeningForMoreData(ss, dataReceivedHandler); });
        }
        /// <summary>
        /// A callback that takes whatever data the socket has spit out, parse it out into a list of objects, 
        /// and then passes it out to the specified handler.
        /// </summary>
        /// <param name="aSocketState"></param>
        /// <param name="dataReceivedHandler"></param>
        public void receiveDataAndStartListeningForMoreData(SocketState aSocketState, handleDataReceived dataReceivedHandler)
        {
            if (aSocketState.ErrorOccured)
            {
                string[] errorStringArray = { "ERROR", aSocketState.ErrorMesssage };
                dataReceivedHandler(errorStringArray);
                closeConnection();
                return;
            }

            if (!isTheConnectionAlive())
            {
                return;
            }

            IList<string> data = Networking.getMessageStringsFromBufferSeperatedByCharacter(aSocketState, '\n');

            dataReceivedHandler(data);

            startDataListenerLoop(dataReceivedHandler);
        }

        /// <summary>
        ///// Sends the specified direction input to the server.
        /// vinc: Sends the specified spreadsheet message to the server
        /// vinc: despite the type of message, each message content should be splited by '\t'
        /// </summary>
        //public void sendMessage(int direction)
        public void sendMessage(string messageType, string messageContent)
        {
            if (!initialized)
            {
                return;
            }

            //Networking.Send(clientSocketState.TheSocket, "(" + direction + ")\n");
            // vinc: send the message to server (notice that we don't have code check the validity of the message)
            Networking.Send(clientSocketState.TheSocket, messageType + "\t" + messageContent + ")\n");
        }
        /// <summary>
        /// Check to see if the connection is currently alive (e.g. connected to a server).
        /// </summary>
        /// <returns></returns>
        public bool isTheConnectionAlive()
        {
            return clientSocketState != null && clientSocketState.TheSocket != null && clientSocketState.SafeToSendRequest;
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
            if (ss.TheSocket != null)
            {
                ss.TheSocket.Close();
            }
            handleDisconnect();
        }
    }
}