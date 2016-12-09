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

namespace SnakeServer
{
    /// <summary>
    /// The controler for the Snake class. Uses the Networking static class to send and recieve snake commands.
    /// </summary>
    public class ServerSnakeNetworkController
    {
        TcpListener theTcpListener;
        IList<SocketState> clients;
        /// <summary>
        /// Creates a new network controller for the server.
        /// </summary>
        public ServerSnakeNetworkController()
        {
            clients = new List<SocketState>();
        }

        public delegate string getInitializtionDataForNewClient(string playerName, SocketState client);
        public delegate void handleDataReceived(IList<string> data, SocketState client);
        public delegate void handleSocketClosed();
        /// <summary>
        /// Starts a loop to listen for new clients connecting with the specified delegate handlers.
        /// </summary>
        /// <param name="clientInitDataFetcher"></param>
        /// <param name="dataRecievedHandler"></param>
        public void startListeningForClients(getInitializtionDataForNewClient clientInitDataFetcher, handleDataReceived dataRecievedHandler)
        {
            if(theTcpListener != null)
            {
                return;
            }

            //Starts An Event Loop;
            theTcpListener = Networking.startListeningForTcpConnections((ss) => { connectedToAClient(ss, clientInitDataFetcher, dataRecievedHandler); });
        }
        /// <summary>
        /// The method that's called when we've connected to a client. 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="clientInitDataFetcher"></param>
        /// <param name="dataRecievedHandler"></param>
        private void connectedToAClient(SocketState client, getInitializtionDataForNewClient clientInitDataFetcher, handleDataReceived dataRecievedHandler)
        {
            Networking.listenForData(client, (ss) => { getInitialClientData(ss, clientInitDataFetcher, dataRecievedHandler); });
        }

        private void getInitialClientData(SocketState client, getInitializtionDataForNewClient clientInitDataFetcher, handleDataReceived dataRecievedHandler)
        {
            IList<string> initialMessages = Networking.getMessageStringsFromBufferSeperatedByCharacter(client, '\n');

            if (initialMessages.Count == 0)
            {
                throw new Exception("No Name Sent"); // Replace With Socket Closing Code
            }

            string playerName = initialMessages[0];

            if(initialMessages.Count > 0)
            {
                initialMessages.RemoveAt(0);
                Networking.resetGrowableBufferWithMessagesSeperatedByCharacter(client, initialMessages, '\n');
            }

            Networking.Send(client.TheSocket, clientInitDataFetcher(playerName, client));
            startDataListenerLoop(client, dataRecievedHandler);

            lock (clients)
            {
                clients.Add(client);
            }
        }

        private void startDataListenerLoop(SocketState client, handleDataReceived dataReceivedHandler)
        {
            Networking.listenForData(client, (ss) => { receiveDataAndStartListeningForMoreData(ss, dataReceivedHandler); });
        }

        public void receiveDataAndStartListeningForMoreData(SocketState client, handleDataReceived dataReceivedHandler)
        {
            if (client.ErrorOccured)
            {
                Console.WriteLine(client.ErrorMesssage);
            }

            if (!client.SafeToSendRequest)
            {
                lock (clients)
                {
                    clients.Remove(client);
                }
                return;
            }

            IList<string> data = Networking.getMessageStringsFromBufferSeperatedByCharacter(client, '\n');

            dataReceivedHandler(data, client);

            startDataListenerLoop(client, dataReceivedHandler);
        }
        /// <summary>
        /// Sends the passed Json to clients.
        /// </summary>
        /// <param name="worldJson"></param>
        public void sendWorldDataToClients(string worldJson)
        {
            lock (clients)
            {
                foreach (SocketState client in clients)
                {
                    Networking.Send(client.TheSocket, worldJson);
                }
            }
        }

    }

}
