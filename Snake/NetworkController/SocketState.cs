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
    public class SocketState
    {
        public Socket theSocket;
        public delegate void EventProccessor(SocketState aSocketState);
        public EventProccessor processorCallback;

        public bool safeToSendRequest = false;

        public byte[] messageBuffer = new byte[1024];
        public StringBuilder stringGrowableBuffer = new StringBuilder();

        public SocketState(Socket s, EventProccessor eventProc)
        {
            theSocket = s;
            processorCallback = eventProc;
        }

    }
}
