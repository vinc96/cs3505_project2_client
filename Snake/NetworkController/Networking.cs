///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
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
    /// <summary>
    /// A general class for networking using C# sockets.
    /// </summary>
    static public class Networking
    {
        public const int DEFAULT_PORT = 11000;

        public static TcpListener startListeningForTcpConnections(TcpListenerState.EventProcessor foundConnection, int port = DEFAULT_PORT)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            TcpListenerState newListenerState = new TcpListenerState(listener, foundConnection);

            listener.BeginAcceptSocket(foundTcpConnection, newListenerState);

            return listener;
        }
        
        private static void foundTcpConnection(IAsyncResult ar)
        {
            TcpListenerState ts = (TcpListenerState)ar.AsyncState;

            Socket newSocket = ts.TheTcpListener.EndAcceptSocket(ar);

            SocketState newSocketState = new SocketState(newSocket, (ss) => { });

            newSocketState.SafeToSendRequest = true;

            ts.TheCallback(newSocketState);
            
            // Starts Listening For Connections
            ts.TheTcpListener.BeginAcceptSocket(foundTcpConnection, ts);
        }

        /// <summary>
        /// Start attempting to connect to the server
        /// </summary>
        /// <param name="host_name"> server to connect to </param>
        /// <returns></returns>
        public static Socket ConnectToNetworkNode(string hostName, SocketState.EventProcessor nodeConnectedCallback, int port = DEFAULT_PORT, int timeoutMs = -1)
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
                catch (Exception)
                {
                    // see if host name is actually an ipaddress, i.e., 155.99.123.456
                    System.Diagnostics.Debug.WriteLine("using IP");
                    ipAddress = IPAddress.Parse(hostName);
                }

                // Create a TCP/IP socket.
                Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                socket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);

                SocketState newSocketState = new SocketState(socket, nodeConnectedCallback);

                IAsyncResult result = socket.BeginConnect(ipAddress, port, Networking.ConnectedToNetworkNode, newSocketState);

                bool timedOut = !result.AsyncWaitHandle.WaitOne(timeoutMs, true);
                if (timedOut)
                {
                    newSocketState.ErrorOccured = true;
                    newSocketState.ErrorMesssage = "Timeout: Couldn't Connect With The Server";

                    newSocketState.TheCallback(newSocketState);
                    socket.Close();
                }

                return socket;
            }
            catch (Exception e)
            {
                SocketState errorSocketState = new SocketState(null, nodeConnectedCallback);
                errorSocketState.ErrorOccured = true;
                errorSocketState.ErrorMesssage = e.Message;

                errorSocketState.TheCallback(errorSocketState);
                return null;
            }
        }

        /// <summary>
        /// Connect to a server using the default port for this class.
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="nodeConnectedCallback"></param>
        /// <returns></returns>
        public static Socket ConnectToNetworkNode(string hostName, SocketState.EventProcessor nodeConnectedCallback, int timeoutMs = -1)
        {
            return Networking.ConnectToNetworkNode(hostName, nodeConnectedCallback, Networking.DEFAULT_PORT, timeoutMs);
        }

        /// <summary>
        /// The callback to use when we're finalizing the initial network connect.
        /// </summary>
        /// <param name="ar"></param>
        private static void ConnectedToNetworkNode(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;

            try
            {
                // Complete the connection.
                ss.TheSocket.EndConnect(ar);
            }
            catch (Exception e)
            {
                ss.ErrorOccured = true;
                ss.ErrorMesssage = e.Message;

                ss.TheCallback(ss);
                return;
            }

            ss.SafeToSendRequest = true;

            // Call The Callback To Signal The Connection Is Complete
            ss.TheCallback(ss);
        }
        /// <summary>
        /// Listen for data, using the callback defined in the passed SocketState.
        /// </summary>
        /// <param name="ss"></param>
        public static void listenForData(SocketState ss)
        {
            Networking.listenForData(ss, ss.TheCallback);
        }

        /// <summary>
        /// Listen for data, while explicitly defining what callback we should use when data is recieved.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="dataRecievedCallback"></param>
        public static void listenForData(SocketState ss, SocketState.EventProcessor dataRecievedCallback)
        {
            if (!ss.SafeToSendRequest) { return; }

            ss.TheCallback = dataRecievedCallback;

            // Start listening for a message
            // When a message arrives, handle it on a new thread with ReceiveCallback
            try
            {
                ss.TheSocket.BeginReceive(ss.MessageBuffer, 0, ss.MessageBuffer.Length, SocketFlags.None, Networking.ReceiveCallback, ss);
            }
            catch (Exception e)
            {
                //If An Error Occurs Notify The Socket Owner
                ss.ErrorOccured = true;
                ss.ErrorMesssage = e.Message;

                ss.TheCallback(ss);
                return;
            }
        }

        /// <summary>
        /// The callback to use when we recieve data on the socket. Parcels the data out of our buffer and into 
        /// the SocketState stringGrowableBuffer.
        /// </summary>
        /// <param name="ar"></param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            
            SocketState ss = (SocketState)ar.AsyncState;

            int bytesRead;

            try
            {
                bytesRead = ss.TheSocket.EndReceive(ar);
            }
            catch (Exception e)
            {
                //If An Error Occurs Notify The Socket Owner
                ss.ErrorOccured = true;
                ss.ErrorMesssage = e.Message;

                ss.TheCallback(ss);
                return;
            }

            // socket wants to be closed
            if(bytesRead == 0)
            {
                Disconnect(ss, false);
                return;
            }

            // If the socket is still open
            if (bytesRead > 0)
            {
                lock (ss.StringGrowableBuffer)
                {
                    string theMessage = Encoding.UTF8.GetString(ss.MessageBuffer, 0, bytesRead);
                    // Append the received data to the growable buffer.
                    // It may be an incomplete message, so we need to start building it up piece by piece
                    ss.StringGrowableBuffer.Append(theMessage);
                }

                ss.TheCallback(ss);
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
            string totalData = ss.StringGrowableBuffer.ToString();
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
                lock (ss.StringGrowableBuffer)
                {
                    ss.StringGrowableBuffer.Remove(0, p.Length);
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
            string currentStringsInBuffer = ss.StringGrowableBuffer.ToString();

            lock(ss.StringGrowableBuffer){
                ss.StringGrowableBuffer.Clear();

                foreach (string s in messages)
                    ss.StringGrowableBuffer.Append(s + terminator);

                ss.StringGrowableBuffer.Append(currentStringsInBuffer);
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
            ss.TheSocket.EndSend(ar);
        }
        /// <summary>
        /// Disconnects the specified socket contained in the passed SocketState, using the processorCallback stored in the SocketState.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="reuse"></param>
        public static void Disconnect(SocketState ss, bool reuse)
        {
            Networking.Disconnect(ss, reuse, ss.TheCallback);
        }
        /// <summary>
        /// Disconnects the specified socket contained in the passed SocketState, calling the passed handler when the socket closes.
        /// </summary>
        /// <param name="ss"></param>
        /// <param name="reuse"></param>
        /// <param name="socketClosedHandler"></param>
        public static void Disconnect(SocketState ss, bool reuse, SocketState.EventProcessor socketClosedHandler)
        {
            if (ss.TheSocket == null || !ss.TheSocket.Connected)
            {
                socketClosedHandler(ss);
                return;
            }

            ss.TheCallback = socketClosedHandler;
            ss.SafeToSendRequest = false;
            ss.TheSocket.BeginDisconnect(reuse, DisconnectedCallback, ss);
        }
        /// <summary>
        /// The AsyncCallback that we use when disconnecting.
        /// </summary>
        /// <param name="ar"></param>
        private static void DisconnectedCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;

            try
            {
                ss.TheSocket.EndDisconnect(ar);
            }catch(Exception e)
            {
                ss.ErrorOccured = true;
                ss.ErrorMesssage = e.Message;
            }
            
            ss.TheCallback(ss);
        }
    }
}
