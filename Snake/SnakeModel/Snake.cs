using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Snake
    {
        [JsonProperty]
        private int ID;

        [JsonProperty]
        private string name;

        [JsonProperty]
        private List<Point> vertices;

        /// <summary>
        /// Returns an ordered enumberable containing all the verticies of this snakes. Right now, returns a copy of our 
        /// current List in an attempt to restrict access to the private instance variable.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Point> getVerticies()
        {
            return new List<Point>(vertices);
        }

        /// <summary>
        /// Returns the name of this snake.
        /// </summary>
        /// <returns></returns>
        public string getName()
        {
            return name;
        }

        /// <summary>
        /// Returns the numeric ID for this snake.
        /// </summary>
        /// <returns></returns>
        public int getID()
        {
            return ID;
        }
}

    
}
