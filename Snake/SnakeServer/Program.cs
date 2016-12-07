using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server...");
            //Well, whadayathink! start the server!
            
            GameServer server = new GameServer(@"..\..\..\Resources\GameSettings.xml");
            
            server.start();

            while (true) { Console.ReadLine(); }
        }
    }
}
