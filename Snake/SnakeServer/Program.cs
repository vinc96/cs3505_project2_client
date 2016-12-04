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

            ServerSnakeNetworkController networkingController = new ServerSnakeNetworkController();

            networkingController.startListeningForClients(getInitDataForNewClients, handleRecievedDataFromClient);



            //Take input, and just ignore it.
            while (true)
            {
                networkingController.sendWorldDataToClients("{\"ID\":0,\"name\":\"Danny\",\"vertices\":[{\"x\":35,\"y\":36},{\"x\":20,\"y\":36}]}\n");
            }
            
        }

        static string getInitDataForNewClients(string newPlayerName) // We Probably Need To Send More Data To The Model So We Can Determine What Client Sends What Request
        {
            //Possibly Store Name In a Database
            Console.WriteLine(newPlayerName);

            return "0\n" +
                "150\n" +
                "150\n";
        }

        static void handleRecievedDataFromClient(IList<string> messages) // We Probably Need To Send More Data To The Model So We Can Determine What Client Sends What Request
        {
            foreach(string message in messages)
            {
                Console.WriteLine(message);
            }
        }
    }
}
