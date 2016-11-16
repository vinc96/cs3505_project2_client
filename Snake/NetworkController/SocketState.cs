using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkController
{
    public class SocketState
    {
        public Socket theSocket;
        public delegate void EventProccessor(SocketState aSocketState);
        public EventProccessor processorCallback;

        public byte[] messageBuffer = new byte[1024];
        public StringBuilder stringGrowableBuffer = new StringBuilder();

        public SocketState(Socket s, EventProccessor eventProc)
        {
            theSocket = s;
            processorCallback = eventProc;
        }

    }
}
