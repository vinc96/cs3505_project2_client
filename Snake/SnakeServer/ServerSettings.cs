///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using SnakeModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
namespace SnakeServer
{   /// <summary>
    /// A ServerSettings object. Contains the settings for this game world,
    /// parsed from a settings.xml file with the following layout:
    /// <SnakeSettings>
    ///     <BoardWidth>Width of the board, in cells</BoardWidth>
    ///     <BoardHeight>Height of the board, in cells</BoardHeight>
    ///     <MSPerFrame>Milliseconds per Frame</MSPerFrame>
    ///     <FoodDensity>Food per Snake</FoodDensity>
    ///     <SnakeRecycleRate>Snake recycle rate</SnakeRecycleRate>
    /// </SnakeSettings>
    /// 
    /// The afformentioned layout is required, but the following additional settings are supported. If 
    /// these settings are not set, they are set to their default values
    /// <MSPerTick> Milliseconds per Server Tick</MSPerTick> (default= equal to MSPerFrame)
    ///         GAMEMODE SETTINGS
    /// </summary>
    public class GameServerSettings
    {
        /// <summary>
        /// The period at which we run game GameUpdates. 
        /// </summary>
        public int MSPerFrame { get; private set; }

        public WorldSettings SnakeWorldSettings { get; private set; }

        /// <summary>
        /// Creates a new WorldSettings object read from the specified filepath, 
        /// throws a relevant IOException if there's a problem finding/reading the file.
        /// </summary>
        /// <param name="filePath"></param>
        public GameServerSettings(string filePath)
        {
            XmlDocument settingsDocument = new XmlDocument();
            settingsDocument.Load(filePath);

            XmlNode settings = settingsDocument["SnakeSettings"];
            
            //Grab our settings
            MSPerFrame = getXmlNodeValueAsInt(settings["MSPerFrame"], 33);

            SnakeWorldSettings = new WorldSettings(settings["WorldSettings"]);
        }
        /// <summary>
        /// A helper method to read settings from an XML node. Returns the value of the node parsed 
        /// as an int if it exists, passes the defined default value if it doesn't. 
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private int getXmlNodeValueAsInt(XmlNode xmlNode, int defaultValue = 0)
        {
            if (xmlNode == null)
            {
                return defaultValue;
            }

            int value = defaultValue;
            int.TryParse(xmlNode.InnerText, out value);

            return value;
        }
    }
}

