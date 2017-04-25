/// Citation: 
/// Based on Snake.SnakeClient.ClientSnakeNetworkController.cs written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
/// Authors:
/// Vincent Cheng (u0887427)
/// Atul Sharma (u1001513)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

            //public Dictionary<String, String> Cells { get; private set; }
            public string[] Cells { get; private set; }
            public string StartupString { get; private set; }


            public StartupData(string[] Cells, string StartupString)
            {
                // vinc: store the whole startup data.
                this.Cells = Cells;
                this.StartupString = StartupString;

                ErrorOccured = false;
                ErrorMessage = null;
            }

            public StartupData(string errorMessage)
            {
                // vinc: set cells to null when there are errors
                Cells = null;
                this.StartupString = null;

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

            IList<String> startupData = Networking.getMessageStringsFromBufferSeperatedByCharacter(aSocketState, '\n');

            //Expects 1 Lines Of Startup Data, If It Isn't Recieved Continue Listening And Resets Buffer
            if (startupData.Count() < 1)
            {
                Networking.resetGrowableBufferWithMessagesSeperatedByCharacter(aSocketState, startupData, '\n');
                Networking.listenForData(aSocketState, (ss) => { startupDataRecieved(ss, handshakeCompletedHandler); });
                return;
            }
            
            // vinc: parse startup message to dictionary
            //string[] startupData_array = startupData[0].Trim().Split('\t');
            string[] startupData_array = startupData[0].Split('\t');

            handshakeCompletedHandler(new StartupData(startupData_array, startupData[0]));
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
        public bool sendMessage(string messageType, string messageContent)
        {
            if (!initialized || !isTheConnectionAlive())
            {
                return false;
            }

            // vinc: send the message to server (notice that we don't have code check the validity of the message)
            string message;
            if (messageContent == null)
            {
                message = messageType + "\t\n";
            }else
            {
                message = messageType + "\t" + messageContent + "\t\n";
            }
            Networking.Send(clientSocketState.TheSocket, message);
            return true;
        }

        /// <summary>
        /// Check to see if the connection is currently alive (e.g. connected to a server).
        /// </summary>
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
