///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using NetworkController;
using SnakeModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace SnakeServer
{
    /// <summary>
    /// The object containing the entire server. On construction and start, this object spawns all the threads needed to run
    /// a SnakeServer.
    /// </summary>
    class GameServer
    {
        GameServerSettings settings;
        Dictionary<SocketState, int> clients;
        World gameWorld;
        ServerSnakeNetworkController networkController;

        /// <summary>
        /// Construct a server with the settings file located at settingsPath
        /// </summary>
        /// <param name="settingsPath"></param>
        public GameServer(string settingsPath)
        {
            settings = new GameServerSettings(settingsPath);
            clients = new Dictionary<SocketState, int>();
            gameWorld = new World(settings.SnakeWorldSettings);
            networkController = new ServerSnakeNetworkController();
           
        }
        /// <summary>
        /// Starts the GameServer. Spawns a timer that runs our GameUpdate function, and sends data to clients.
        /// </summary>
        public void start()
        {
            networkController.startListeningForClients(getInitDataForNewClients, handleRecievedDataFromClient);

            Timer tickLoop = new Timer(settings.MSPerFrame);
            tickLoop.AutoReset = true;

            tickLoop.Elapsed += TickElapsed;

            tickLoop.Start();

        }
        /// <summary>
        /// The function we call when one server tick has elapsed. Executes a GameUpdate, then takes JSON and sends it to clients.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TickElapsed(object sender, ElapsedEventArgs e)
        {
            string worldJson;
            lock (gameWorld)
            {
                gameWorld.GameUpdate();
                worldJson = gameWorld.ToJson();
            }

            networkController.sendWorldDataToClients(worldJson);
        }
        /// <summary>
        /// Called when we're initializing a new client (on the client's connection)
        /// Allocates a new snake for the connecting client, and sends the client the world data once the snake has been allocated.
        /// </summary>
        /// <param name="newPlayerName"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private string getInitDataForNewClients(string newPlayerName, SocketState client) // We Probably Need To Send More Data To The Model So We Can Determine What Client Sends What Request
        {
            //Possibly Store Name In a Database
            Console.WriteLine(newPlayerName);

            int newSnakeId;
            lock (gameWorld)
            {
                newSnakeId = gameWorld.AllocateNewSnake(newPlayerName);
            }

            //Associate our client with its snake ID.
            clients.Add(client, newSnakeId);

            return newSnakeId + "\n" +
                    gameWorld.Size.X + "\n" +
                    gameWorld.Size.Y + "\n";

        }
        /// <summary>
        /// Handles direction change commands from clients.
        /// </summary>
        /// <param name="messages"></param>
        /// <param name="client"></param>
        private void handleRecievedDataFromClient(IList<string> messages, SocketState client) // We Probably Need To Send More Data To The Model So We Can Determine What Client Sends What Request
        {
            bool isValid = validateClientMessages(messages);

            if (!isValid)
            {
                //Disconnect the offending socket
                return;
            }

            string directionString = messages.Last().Trim("()".ToCharArray());

            int direction = int.Parse(directionString);
            lock (gameWorld)
            {
                gameWorld.UpdateSnakeDirection(clients[client], direction);
            }
        }
        /// <summary>
        /// Ensures that if a client sends bad data, then they get disconnection. Returns false if the passed messages are invalid.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        private bool validateClientMessages(IEnumerable<string> messages)
        {
            string[] validStrings = { "(1)", "(2)", "(3)", "(4)" };
            foreach(string message in messages)
            {
                if (!validStrings.Contains(message))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
