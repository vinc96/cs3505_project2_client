///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
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
            //Take input, and ignore it.
            while (true) { Console.ReadLine(); }
        }
    }
}
