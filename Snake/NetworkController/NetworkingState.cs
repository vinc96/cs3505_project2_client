///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkController
{
    /// <summary>
    /// A template for a networking state object that allows us to associate some given NetworkingOvject with a callback. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="CallDel"></typeparam>
    public class NetworkingState<T, CallDel>
    {
        public T TheNetworkingObject;
        public CallDel TheCallback;

        public bool SafeToSendRequest = false;

        public bool _ErrorOccured = false;
        /// <summary>
        /// Determines whether or not an error has occoured with the associated networking object. sets the connection to 
        /// be killed if it has.
        /// </summary>
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
        /// <summary>
        /// Creates a new NetworkingState object, with the specified object and callback.
        /// </summary>
        /// <param name="aNetworkingObject"></param>
        /// <param name="eventHandler"></param>
        public NetworkingState(T aNetworkingObject, CallDel eventHandler)
        {
            TheNetworkingObject = aNetworkingObject;
            TheCallback = eventHandler;
        }
    }
}
