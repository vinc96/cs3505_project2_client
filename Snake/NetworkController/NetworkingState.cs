using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkController
{
    public class NetworkingState<T, CallDel>
    {
        public T TheNetworkingObject;
        public CallDel TheCallback;

        public bool SafeToSendRequest = false;

        public bool _ErrorOccured = false;

        public bool ErrorOccured
        {
            get { return _ErrorOccured; }
            set
            {
                SafeToSendRequest = SafeToSendRequest && !value;
                _ErrorOccured = value;
            }
        }

        public string ErrorMesssage;

        public NetworkingState(T aNetworkingObject, CallDel eventHandler)
        {
            TheNetworkingObject = aNetworkingObject;
            TheCallback = eventHandler;
        }
    }
}
