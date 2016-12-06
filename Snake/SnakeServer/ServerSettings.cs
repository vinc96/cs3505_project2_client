using SnakeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
    class ServerSettings
    {
        /// <summary>
        /// Creates a new WorldSettings object read from the specified filepath, 
        /// throws a relevant IOException if there's a problem finding/reading the file.
        /// </summary>
        /// <param name="filePath"></param>
        public ServerSettings(string filePath)
        {
            XmlDocument settings = new XmlDocument();
            settings.Load(filePath);

            //Grab our settings

            //Build the world settings:
            SnakeWorldSettings = new WorldSettings(settings);


        }

        /// <summary>
        /// The period at which we run game GameUpdates. 
        /// </summary>
        public int MSPerFrame { get; private set; }

           
        public WorldSettings SnakeWorldSettings { get; private set; } 
    }
}

