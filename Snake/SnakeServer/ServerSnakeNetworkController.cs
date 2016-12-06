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

        public ServerSnakeNetworkController()
        {
            clients = new List<SocketState>();
        }

        public delegate string getInitializtionDataForNewClient(string playerName);
        public delegate void handleDataReceived(IList<string> data);
        public delegate void handleSocketClosed();
        
        public void startListeningForClients(getInitializtionDataForNewClient clientInitDataFetcher, handleDataReceived dataRecievedHandler)
        {
            if(theTcpListener != null)
            {
                return;
            }

            //Starts An Event Loop;
            theTcpListener = Networking.startListeningForTcpConnections((ss) => { connectedToAClient(ss, clientInitDataFetcher, dataRecievedHandler); });
        }

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

            Networking.Send(client.TheSocket, clientInitDataFetcher(playerName));
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

            dataReceivedHandler(data);

            startDataListenerLoop(client, dataReceivedHandler);
        }

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
