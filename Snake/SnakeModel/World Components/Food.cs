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

        private string jsonString;

        /// <summary>
        /// Creates a food object with a null location and an unset ID. 
        /// Useless, except for its use with the JSON classes.
        /// </summary>
        public Food()
        {
            //Do nothing. 
        }

        /// <summary>
        /// Creates a new food with the specified ID located at the specified location.
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="loc"></param>
        public Food(int ID, Point loc) : this()
        {
            this.ID = ID;
            this.loc = loc;
            jsonString = @"{'ID':" + ID + ",'loc':" + loc.ToJson() + "}";
        }
        /// <summary>
        /// "Eats" this food. Effectively sets it location to (-1,-1)
        /// </summary>
        public void eat()
        {
            loc = new Point(-1, -1);
            jsonString = @"{'ID':" + ID + ",'loc':" + loc.ToJson() + "}";
        }

        public string ToJson()
        {
            return jsonString;
        }
    }
}
