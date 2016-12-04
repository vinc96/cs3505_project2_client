using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetworkController
{
    public class TcpListenerState: NetworkingState<TcpListener, TcpListenerState.EventProcessor>
    {
        public TcpListener TheTcpListener { get { return TheNetworkingObject;} set { TheNetworkingObject = value; } }
        public delegate void EventProcessor(SocketState aSocketState);

        public TcpListenerState(TcpListener t, TcpListenerState.EventProcessor callback) : base(t, callback) { }
    }
}
