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

            // Call The Callback To Signal The Connection Is Complete
            ss.processorCallback(ss);
        }

        public static void listenForData(SocketState ss)
        {
            // Start listening for a message
            // When a message arrives, handle it on a new thread with ReceiveCallback
            ss.theSocket.BeginReceive(ss.messageBuffer, 0, ss.messageBuffer.Length, SocketFlags.None, Networking.ReceiveCallback, ss);
        }

        public static void listenForData(SocketState ss, SocketState.EventProccessor dataRecievedCallback)
        {
            ss.processorCallback = dataRecievedCallback;
            Networking.listenForData(ss);
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;

            int bytesRead = ss.theSocket.EndReceive(ar);

            // If the socket is still open
            if (bytesRead > 0)
            {
                string theMessage = Encoding.UTF8.GetString(ss.messageBuffer, 0, bytesRead);
                // Append the received data to the growable buffer.
                // It may be an incomplete message, so we need to start building it up piece by piece
                ss.stringGrowableBuffer.Append(theMessage);

                ss.processorCallback(ss);
            }
        }

        /// <summary>
        ///  Helper Function To Quickly Get String Data From SocketState
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="seperator"></param>
        /// <returns> Returns The Full Messages that </returns>
        public static IList<string> getMessageStringsFromBufferSeperatedByCharacter(SocketState ss, Char seperator)
        {
            List<string> messages = new List<string>();

            // Loop until we have processed all messages.
            // We may have received more than one.
            string totalData = ss.stringGrowableBuffer.ToString();
            foreach (string p in Regex.Split(totalData, @"(?<=[\n])"))
            {
                // Ignore empty strings added by the regex splitter
                if (p.Length == 0)
                    continue;
                // The regex splitter will include the last string even if it doesn't end with a '\n',
                // So we need to ignore it if this happens. 
                if (p[p.Length - 1] != seperator)
                    break;

                // add the string to the message list 
                messages.Add(p.TrimEnd(seperator));

                // Then remove it from the SocketState's growable buffer
                ss.stringGrowableBuffer.Remove(0, p.Length);
            }

            return messages;
        }

        /// <summary>
        ///  Sends The String Of The Data To Network Node Associated With The Socket
        /// </summary>
        /// <param name="s">The Socket Used For Communicating With The Other Network Node</param>
        /// <param name="data"> The Data To Send</param>
        public static void Send(Socket s, string data)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(data);
            SocketState socketWrapper = new SocketState(s, (e) => {});
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
    }
}
