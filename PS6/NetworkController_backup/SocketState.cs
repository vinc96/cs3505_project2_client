///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkController
{
    /// <summary>
    /// A class to contain the state of a networking socket, including the socket itself, buffers, and callbacks.
    /// </summary>
    public class SocketState : NetworkingState<Socket, SocketState.EventProcessor>
    {
        public Socket TheSocket { get { return TheNetworkingObject; } set { TheNetworkingObject = value; } }
        public delegate void EventProcessor(SocketState aSocketState);
        
        public byte[] MessageBuffer = new byte[1024];
        public StringBuilder StringGrowableBuffer = new StringBuilder();

        public SocketState(Socket s, SocketState.EventProcessor eventProc) : base(s, eventProc) { }
    }
}
