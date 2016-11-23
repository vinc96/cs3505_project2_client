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
    /// A food object. Has an ID (unique among food), and a location.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Food
    {
        /// <summary>
        /// The ID for this food. 
        /// </summary>
        [JsonProperty]
        public int ID { get; private set; }
        /// <summary>
        /// The location of the food.
        /// </summary>
        [JsonProperty]
        public Point loc { get; private set; }
    }
}
