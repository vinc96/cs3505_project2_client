///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeModel
{
    /// <summary>
    /// A Point represents a single point in 2D space. Simply defined as a helper class to ease organization when it comes to 
    /// defining locations in the snake World. Points are immutable. 
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Point
    {
        /// <summary>
        /// The x coordinate of this point.
        /// </summary>
        [JsonProperty]
        public int x { get; private set; }
        /// <summary>
        /// The y coordinate of this point.
        /// </summary>
        [JsonProperty]
        public int y { get; private set; }

        /// <summary>
        /// Creates a new point with the specified coordinates.
        /// </summary>
        /// <param name="pointX"></param>
        /// <param name="pointY"></param>
        public Point(int pointX, int pointY)
        {
            x = pointX;
            y = pointY;
        }

        /// <summary>
        /// Represents the X position of this point in 2D space. 
        /// </summary>
        public int PointX
        {
            get
            {
                return this.x;
            }
        }

        /// <summary>
        /// Represents the Y position of this point in 2D space. 
        /// </summary>
        public int PointY
        {
            get
            {
                return this.y;
            }
        }

        /// <summary>
        /// Our special implemementation of the Equals method. Returns true if the two points have the same coordinates.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //If the object is null, then we aren't equal.
            if (ReferenceEquals(obj, null))
            {
                return false;
            }
            //If the object isn't a point, then we aren't equal
            if (!obj.GetType().Equals(typeof(Point)))
            {
                return false;
            }
            Point otherPoint = (Point) obj; //It's safe to assume that this is a point now.
            return (this.x == otherPoint.x && this.y == otherPoint.y);//Points are equal if their locations are the same.
        }
    }
}
