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
    /// defining locations in the snake World.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    class Point
    {
        [JsonProperty]
        private int x;
        [JsonProperty]
        private int y;

        /// <summary>
        /// Represents the X position of this point in 2D space. 
        /// </summary>
        public int PointX
        {
            get
            {
                return this.x;
            }
            set
            {
                this.x = value;
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
            set
            {
                this.y = value;
            }
        }
    }
}
