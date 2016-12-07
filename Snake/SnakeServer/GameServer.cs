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

        public void start()
        {
            networkController.startListeningForClients(getInitDataForNewClients, handleRecievedDataFromClient);

            Timer tickLoop = new Timer(settings.MSPerFrame);
            tickLoop.AutoReset = true;

            tickLoop.Elapsed += TickElapsed;

            tickLoop.Start();

        }

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
            gameWorld.UpdateSnakeDirection(clients[client], direction);
        }

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
