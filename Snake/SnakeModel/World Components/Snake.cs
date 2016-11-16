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
        /// <summary>
        /// All the verticies that make up this snake. Ordered from tail to head, where the last element in the list is the head, and the first is the tail.
        /// </summary>
        [JsonProperty]
        private List<Point> vertices;

        /// <summary>
        /// Returns an ordered enumberable containing all the verticies of this snakes. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Point> getVerticies()
        {
            //Protects the internals of this class, but just awful complexity (esp. considering how often it's used.) High priority to fix.
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
        /// <summary>
        /// Returns the point location of the head of this snake. 
        /// </summary>
        /// <returns></returns>
        internal Point getHead()
        {
            //We can just return the point, as they're immutable.
            return vertices[vertices.Count];
        }
    }

    
}
