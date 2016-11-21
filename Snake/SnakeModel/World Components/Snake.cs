using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Snake
    {
        [JsonProperty]
        public int ID { get; private set; }

        [JsonProperty]
        public string name { get; private set;}

        public int length
        {
            get
            {
                int length = 0;
                Point previousVert = null;
                foreach (Point vert in vertices)
                {
                    if (ReferenceEquals(previousVert, null))
                    {
                        previousVert = vert;
                        continue;
                    }
                    length += Math.Max(Math.Abs(previousVert.PointX - vert.PointX), Math.Abs(previousVert.PointY - vert.PointY));

                    previousVert = vert;
                }
                return length;
            }
            private set
            {
                //Do nothing
            }
        }

        /// <summary>
        /// Returns a color for this snake, defined by this snake's ID. Two snakes with the same ID will have the same color. However, Two 
        /// snakes with different IDs may have the same color, though unlikely (Essentially, the color is a modified Hash).
        /// </summary>
        public Color Color {
            get {
                int hashCode = (ID.ToString() + "This1Is2Different3Salt4").GetHashCode();
                return Color.FromArgb(255, (hashCode & 0x00FF0000) >> 16, (hashCode & 0x0000FF00) >> 8, hashCode & 0x000000FF); //Non-transparent.
            }
            private set {}
        }

        /// <summary>
        /// All the verticies that make up this snake. Ordered from tail to head, where the last element in the list is the head, and the first is the tail.
        /// </summary>
        [JsonProperty]
        private List<Point> vertices;

        public Snake()
        {
            vertices = new List<Point>();
        }

        /// <summary>
        /// Returns an ordered enumberable containing all the verticies of this snakes. 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Point> getVerticies()
        {
            //Protects the internals of this class, but just awful complexity (esp. considering how often it's used.) High priority to fix.
            return vertices; 
        }

        /// <summary>
        /// Returns the point location of the head of this snake. 
        /// </summary>
        /// <returns></returns>
        public Point getHead()
        {
            //We can just return the point, as they're immutable.
            return vertices[vertices.Count - 1];
        }
    }

    
}
