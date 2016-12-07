///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeModel
{
    /// <summary>
    /// A snake object corresponds to a snake in our game. Has methods to get length, color, name, location, etc.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Snake
    {
        public enum Direction { UP = 1, RIGHT = 2,DOWN = 3,LEFT = 4}
        /// <summary>
        /// The ID of this snake. Assigned by the server, all snake IDs are unique in that server.
        /// </summary>
        [JsonProperty]
        public int ID { get; private set; }
        /// <summary>
        /// The name of this snake. Set by the player.
        /// </summary>
        [JsonProperty]
        public string name { get; private set; }

        /// <summary>
        /// All the verticies that make up this snake. Ordered from tail to head, where the last element in the list is the head, and the first is the tail.
        /// </summary>
        [JsonProperty]
        private List<Point> vertices;

        /// <summary>
        /// The direction this snake is traveling. 
        /// </summary>
        public Direction CurrentDirection { get; private set; }
        public Direction NextDirection { get; set; }

        /// <summary>
        /// The length of this snake. Corresponds to the the player's score.
        /// </summary>
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
        /// snakes with different IDs may have the same color, though extremely unlikely (color is derived from String's Hash function, which has pretty
        /// good distribution).
        /// </summary>
        public Color Color
        {
            get
            {
                int hashCode = (ID.ToString() + "This1Is2Different3Salt4").GetHashCode();
                return Color.FromArgb(255, (hashCode & 0x00FF0000) >> 16, (hashCode & 0x0000FF00) >> 8, hashCode & 0x000000FF); //Non-transparent.
            }
            private set { }
        }

        /// <summary>
        /// Creates a new Snake object. Used for deserialization 
        /// </summary>
        public Snake()
        {
            vertices = new List<Point>();
        }

        /// <summary>
        /// Creates a new snake object with the specified ID, name, head and direciton. The snake object will have it's head in the specified location, 
        /// traveling in the specified direction, and will consist of one segment of specified length extending in the opposite direction that we're traveling in.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="name"></param>
        /// <param name="head"></param>
        /// <param name="direction"></param>
        public Snake(int ID, string name, Point head, Direction direction, int length) : this()
        {
            //Add name and ID.
            this.ID = ID;
            this.name = name;
            vertices.Add(head);//Add the head
            //Add the tail
            switch (direction)
            {
                case Direction.UP:
                    vertices.Add(new Point(head.x, head.y + length));
                    break;
                case Direction.DOWN:
                    vertices.Add(new Point(head.x, head.y - length));
                    break;
                case Direction.LEFT:
                    vertices.Add(new Point(head.x + length, head.y));
                    break;
                case Direction.RIGHT:
                    vertices.Add(new Point(head.x - length, head.y));
                    break;
            }
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
        /// Returns an enumerator the contains every single point in this snake, enumerated from tail to head.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Point> getAllPoints()
        {

            Point lastVert = null;
            //First, enumerate the points between 
            foreach (Point vert in vertices)
            {
                if (ReferenceEquals(lastVert, null))
                {
                    continue; //We need to count between two verticies. This makes 'vert' the point ahead of the tail, and lastVert the tail.
                }
                bool isVertical = (Math.Abs(vert.y - lastVert.y) > Math.Abs(vert.x - lastVert.x));
                bool isIncreasing = isVertical ? (vert.y - lastVert.y > 0) : (vert.x - lastVert.x > 0); //True if the section is pointing in the (+) direction. 
                int sectionLength = Math.Max(Math.Abs(vert.y - lastVert.y), Math.Abs(vert.x - lastVert.x));
                for (int i = 0; i < sectionLength; i++)
                {
                    if (isVertical)
                    {
                        if (isIncreasing)
                        {
                            //Vertical, pointing down
                            yield return new Point(lastVert.x, lastVert.y + i);
                        }
                        else
                        {
                            //Vertical, pointing up
                            yield return new Point(lastVert.x, lastVert.y - i);
                        }
                    }
                    else
                    {
                        if (isIncreasing)
                        {
                            //Horizontal, pointing right
                            yield return new Point(lastVert.x + i, lastVert.y);
                        }
                        else
                        {
                            //Horizontal, pointing left
                            yield return new Point(lastVert.x - i, lastVert.y);
                        }
                    }


                }
            }

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

        /// <summary>
        /// Returns the name and length of this snake in the following way: "(name): (length)"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return name + ": " + length;
        }

        /// <summary>
        /// Moves the head of the snake one cell in the direction the snake is traveling.
        /// </summary>
        internal void MoveHead()
        {
            if ((int)NextDirection % 2 != (int)CurrentDirection % 2)
            {
                CurrentDirection = NextDirection; //If both directions are odd/even, then they are opposing. 
            }

            switch (CurrentDirection)
            {
                case Direction.UP:
                    vertices[vertices.Count] = new Point(vertices[vertices.Count].x, vertices[vertices.Count].y - 1);
                    break;
                case Direction.DOWN:
                    vertices[vertices.Count] = new Point(vertices[vertices.Count].x, vertices[vertices.Count].y + 1);
                    break;
                case Direction.LEFT:
                    vertices[vertices.Count] = new Point(vertices[vertices.Count].x - 1, vertices[vertices.Count].y);
                    break;
                case Direction.RIGHT:
                    vertices[vertices.Count] = new Point(vertices[vertices.Count].x + 1, vertices[vertices.Count].y);
                    break;
            }
        }

        /// <summary>
        /// Returns true if this snake's head is coliding with the other snake, false otherwise.
        /// </summary>
        /// <param name="otherSnake"></param>
        /// <returns></returns>
        internal bool Collides(Snake otherSnake)
        {
            foreach (Point p in otherSnake.getAllPoints())
            {
                if (p.Equals(getHead()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Retracts the tail of the snake 1 cell.
        /// </summary>
        internal void RetractTail()
        {
            vertices[0] = getAllPoints().ElementAt(1); //The tail is now the point directly after the tail.
        }
        /// <summary>
        /// "Kills" this snake, setting its verticies to (-1, -1)
        /// </summary>
        public void Kill()
        {
            vertices.Clear();
            vertices.Add(new Point(-1, -1));
        }
    }
}
