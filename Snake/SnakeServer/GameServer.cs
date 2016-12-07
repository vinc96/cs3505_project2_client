using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SnakeServer
{
    class GameServer
    {
        ServerSettings settings;

        /// <summary>
        /// Construct a server with the settings file located at settingsPath
        /// </summary>
        /// <param name="settingsPath"></param>
        public GameServer(string settingsPath)
        {
            settings = new ServerSettings(settingsPath);
        }
    }
}
