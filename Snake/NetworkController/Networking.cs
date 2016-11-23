using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkController;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace NetworkController
{
    static public class Networking
    {
        public static int DEFAULT_PORT = 11000;

        /// <summary>
        /// Start attempting to connect to the server
        /// </summary>
        /// <param name="host_name"> server to connect to </param>
        /// <returns></returns>
        public static Socket ConnectToNetworkNode(string hostName, int port, SocketState.EventProccessor nodeConnectedCallback)
        {
            System.Diagnostics.Debug.WriteLine("connecting  to " + hostName);

            // Connect to a remote device.
            try
            {

                // Establish the remote endpoint for the socket.
                IPHostEntry ipHostInfo;
                IPAddress ipAddress = IPAddress.None;

                // Determine if the server address is a URL or an IP
                try
                {
                    ipHostInfo = Dns.GetHostEntry(hostName);
                    bool foundIPV4 = false;
                    foreach (IPAddress addr in ipHostInfo.AddressList)
                        if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                        {
                            foundIPV4 = true;
                            ipAddress = addr;
                            break;
                        }
                    // Didn't find any IPV4 addresses
                    if (!foundIPV4)
                    {
                        System.Diagnostics.Debug.WriteLine("Invalid addres: " + hostName);
                        return null;
                    }
                }
                catch (Exception e1)
                {
                    // see if host name is actually an ipaddress, i.e., 155.99.123.456
                    System.Diagnostics.Debug.WriteLine("using IP");
                    ipAddress = IPAddress.Parse(hostName);
                }

                // Create a TCP/IP socket.
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                SocketState newSocketState = new SocketState(socket, nodeConnectedCallback);

                socket.BeginConnect(ipAddress, port, Networking.ConnectedToNetworkNode, newSocketState);

                return socket;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                return null;
            }
        }

        public static Socket ConnectToNetworkNode(string hostName, SocketState.EventProccessor nodeConnectedCallback)
        {
            return Networking.ConnectToNetworkNode(hostName, Networking.DEFAULT_PORT, nodeConnectedCallback);
        }

        private static void ConnectedToNetworkNode(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;

            try
            {
                // Complete the connection.
                ss.theSocket.EndConnect(ar);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to connect to server. Error occured: " + e);
                return;
            }

            ss.safeToSendRequest = true;

            // Call The Callback To Signal The Connection Is Complete
            ss.processorCallback(ss);
        }

        public static void listenForData(SocketState ss)
        {
            Networking.listenForData(ss, ss.processorCallback);
        }
        /// <summary>
        /// Listen for data, while explicitly defining what callback we should use when data is recieved.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="dataRecievedCallback"></param>
        public static void listenForData(SocketState ss, SocketState.EventProccessor dataRecievedCallback)
        {

            if (!ss.safeToSendRequest) { return; }

            ss.processorCallback = dataRecievedCallback;

            // Start listening for a message
            // When a message arrives, handle it on a new thread with ReceiveCallback
            ss.theSocket.BeginReceive(ss.messageBuffer, 0, ss.messageBuffer.Length, SocketFlags.None, Networking.ReceiveCallback, ss);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;

            int bytesRead = ss.theSocket.EndReceive(ar);

            // If the socket is still open
            if (bytesRead > 0)
            {
                lock (ss.stringGrowableBuffer)
                {
                    string theMessage = Encoding.UTF8.GetString(ss.messageBuffer, 0, bytesRead);
                    // Append the received data to the growable buffer.
                    // It may be an incomplete message, so we need to start building it up piece by piece
                    ss.stringGrowableBuffer.Append(theMessage);
                }

                ss.processorCallback(ss);
            }
        }

        /// <summary>
        ///  Helper Function To Quickly Get String Data From SocketState
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="terminator"></param>
        /// <returns> Returns The Full Messages that </returns>
        public static IList<string> getMessageStringsFromBufferSeperatedByCharacter(SocketState ss, Char terminator)
        {
            List<string> messages = new List<string>();

            // Loop until we have processed all messages.
            // We may have received more than one.
            string totalData = ss.stringGrowableBuffer.ToString();
            foreach (string p in Regex.Split(totalData, @"(?<=[" + terminator + "])"))
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != terminator)
                    break;

                // add the string to the message list 
                messages.Add(p.TrimEnd(terminator));

                // Then remove it from the SocketState's growable buffer
                lock (ss.stringGrowableBuffer)
                {
                    ss.stringGrowableBuffer.Remove(0, p.Length);
                }
            }

            return messages;
        }

        /// <summary>
        ///  Helper Function To Reset Buffer If Not Enough Data Was Recieved
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="messages"></param>
        /// <param name="terminator"></param>
        public static void resetGrowableBufferWithMessagesSeperatedByCharacter(SocketState ss, IList<string> messages,  Char terminator)
        {
            string currentStringsInBuffer = ss.stringGrowableBuffer.ToString();

            lock(ss.stringGrowableBuffer){
                ss.stringGrowableBuffer.Clear();

                foreach (string s in messages)
                    ss.stringGrowableBuffer.Append(s + terminator);

                ss.stringGrowableBuffer.Append(currentStringsInBuffer);
            }
        }

        /// <summary>
        ///  Sends The String Of The Data To Network Node Associated With The Socket
        /// </summary>
        /// <param name="s">The Socket Used For Communicating With The Other Network Node</param>
        /// <param name="data"> The Data To Send</param>
        public static void Send(Socket s, string data)
        {
            if (!s.Connected) { return; }

            byte[] messageBytes = Encoding.UTF8.GetBytes(data);
            SocketState socketWrapper = new SocketState(s, (ss) => {});
            s.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, Networking.SendCallback, socketWrapper);
        }

        /// <summary>
        /// A callback invoked when a send operation completes
        /// </summary>
        /// <param name="ar"></param>
        private static void SendCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;
            ss.theSocket.EndSend(ar);
        }

        public static void Disconnect(SocketState ss, bool reuse)
        {
            Networking.Disconnect(ss, reuse, ss.processorCallback);
        }

        public static void Disconnect(SocketState ss, bool reuse, SocketState.EventProccessor socketClosedHandler)
        {
            ss.processorCallback = socketClosedHandler;
            ss.safeToSendRequest = false;
            ss.theSocket.BeginDisconnect(reuse, DisconnectedCallback, ss);
        }

        private static void DisconnectedCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;
            ss.theSocket.EndDisconnect(ar);

            ss.processorCallback(ss);
        }
    }
}
