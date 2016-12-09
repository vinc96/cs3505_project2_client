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
        public enum Direction { UP = 1, RIGHT, DOWN, LEFT}
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

            CurrentDirection = direction;
            NextDirection = direction; //These must ALWAYS be in a valid state, otherwise an exception is thrown.

            //Add the tail
            switch (direction)
            {
                case Direction.UP:
                    vertices.Add(new Point(head.X, head.Y + length));
                    break;
                case Direction.DOWN:
                    vertices.Add(new Point(head.X, head.Y - length));
                    break;
                case Direction.LEFT:
                    vertices.Add(new Point(head.X + length, head.Y));
                    break;
                case Direction.RIGHT:
                    vertices.Add(new Point(head.X - length, head.Y));
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
                    lastVert = vert;
                    continue; //We need to count between two verticies. This makes 'vert' the point ahead of the tail, and lastVert the tail.
                }
                bool isVertical = (vert.X == lastVert.X);
                bool isIncreasing = isVertical ? (vert.Y - lastVert.Y > 0) : (vert.X - lastVert.X > 0); //True if the section is pointing in the (+) direction. 
                int sectionLength = Math.Max(Math.Abs(vert.Y - lastVert.Y), Math.Abs(vert.X - lastVert.X));
                for (int i = 0; i < sectionLength; i++)
                {
                    if (isVertical)
                    {
                        if (isIncreasing)
                        {
                            //Vertical, pointing down
                            yield return new Point(lastVert.X, lastVert.Y + i);
                        }
                        else
                        {
                            //Vertical, pointing up
                            yield return new Point(lastVert.X, lastVert.Y - i);
                        }
                    }
                    else
                    {
                        if (isIncreasing)
                        {
                            //Horizontal, pointing right
                            yield return new Point(lastVert.X + i, lastVert.Y);
                        }
                        else
                        {
                            //Horizontal, pointing left
                            yield return new Point(lastVert.X - i, lastVert.Y);
                        }
                    }
                }
                lastVert = vert;
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
            bool changingDirection = false;
            if ((int)NextDirection % 2 != (int)CurrentDirection % 2)
            {
                CurrentDirection = NextDirection; //If both directions are odd/even, then they are opposing.
                changingDirection = true;
            }

            int oldHeadIndex = vertices.Count - 1;
            Point oldHead = vertices[oldHeadIndex];

            Point newHead;

            switch (CurrentDirection)
            {
                case Direction.UP:
                    newHead = new Point(oldHead.X, oldHead.Y - 1);
                    break;
                case Direction.DOWN:
                    newHead = new Point(oldHead.X, oldHead.Y + 1);
                    break;
                case Direction.LEFT:
                    newHead = new Point(oldHead.X - 1, oldHead.Y);
                    break;
                case Direction.RIGHT:
                    newHead = new Point(oldHead.X + 1, oldHead.Y);
                    break;
                default:
                    throw new Exception("Invalid Direction");
            }

            if (changingDirection)
            {
                vertices.Add(newHead);
            }
            else
            {
                vertices[oldHeadIndex] = newHead;
            }
        }

        /// <summary>
        /// Returns true if the specified point colldies with this snake false otherwise.
        /// </summary>
        /// <param name="otherPoint"></param>
        /// <returns></returns>
        internal bool Collides(Point otherPoint)
        {
            //Iterate through each sequential pair of verticies to determine if any of them collide with us.
            Point previousVert = null;
            foreach (Point vert in vertices)
            {
                if (ReferenceEquals(previousVert, null))
                {
                    previousVert = vert; //Only executed on the first passthrough of the loop.
                    continue;
                }
                bool isVertical = vert.X == previousVert.X;
                if (isVertical)
                {
                    //We collide if we're on the same x coordinate, at a y pos between this vert and the previous vert.
                    bool collides = (otherPoint.X == vert.X) && ((otherPoint.Y <= vert.Y && otherPoint.Y >= previousVert.Y) || (otherPoint.Y >= vert.Y && otherPoint.Y <= previousVert.Y));
                    if (collides)
                    {
                        return true;
                    }
                }
                else
                {
                    //We collide if we're on the same y coordinate, at a x pos between this vert and the previous vert.
                    bool collides = (otherPoint.Y == vert.Y) && ((otherPoint.X <= vert.X && otherPoint.X >= previousVert.X) || (otherPoint.X >= vert.X && otherPoint.X <= previousVert.X));
                    if (collides)
                    {
                        return true;
                    }
                }
            }

            return false; //If we haven't collided after looking through every pair of verticies, we're clear

        }
        /// <summary>
        /// Returns whether or not this snake is colliding with itself. Needs to be handled separately because our current Collides method isn't designed to
        /// detect if a snake's head is inside of it's body discretely from when the snake's head isn't inside anything.
        /// </summary>
        /// <returns></returns>
        internal bool IsCollidingWithSelf()
        {
            //Iterate through each sequential pair of verticies to determine if any of them collide with us.
            Point previousVert = null;
            foreach (Point vert in vertices)
            {
                if (ReferenceEquals(previousVert, null))
                {
                    previousVert = vert; //Only executed on the first passthrough of the loop.
                    continue;
                }
                bool isVertical = vert.X == previousVert.X;
                if (isVertical)
                {
                    //We collide if we're on the same x coordinate, at a y pos between this vert and the previous vert. Won't return true if we're colliding with the leading vert.
                    //Modified to ignore collisions with leading vert, assuming they'll be caught when  the leading vert becomes previousVert. Head never does, so we won't collide with head.
                    bool collides = (getHead().X == vert.X) && ((getHead().Y < vert.Y && getHead().Y >= previousVert.Y) || (getHead().Y > vert.Y && getHead().Y <= previousVert.Y));
                    if (collides)
                    {
                        return true;
                    }
                }
                else
                {
                    //We collide if we're on the same y coordinate, at a x pos between this vert and the previous vert. Won't return true if we're colliding with the leading vert.
                    //Modified to ignore collisions with leading vert, assuming they'll be caught when  the leading vert becomes previousVert. Head never does, so we won't collide with head.
                    bool collides = (getHead().Y == vert.Y) && ((getHead().X < vert.X && getHead().X >= previousVert.X) || (getHead().X > vert.X && getHead().X <= previousVert.X));
                    if (collides)
                    {
                        return true;
                    }
                }
            }

            return false; //If we haven't collided after looking through every pair of verticies, we're clear

        }



        /// <summary>
        /// Retracts the tail of the snake 1 cell.
        /// </summary>
        internal void RetractTail()
        {
            Point tail = vertices[0]; 
            Point vertBeforeTail = vertices[1];

            bool isVertical = tail.X == vertBeforeTail.X;
            int delta = (isVertical) ? tail.Y - vertBeforeTail.Y : tail.X - vertBeforeTail.X;
            int normlizedDelta = Math.Abs(delta) / delta;

            if (isVertical)
            {
                vertices[0] = new Point(tail.X, tail.Y - normlizedDelta);
            }
            else
            {
                vertices[0] = new Point(tail.X - normlizedDelta, tail.Y);
            }

            if(vertices[0].Equals(vertices[1]))
            {
                vertices.RemoveAt(0);
            }
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
