using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeModel
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Food
    {
        [JsonProperty]
        private int ID;

        [JsonProperty]
        private Point loc;
    }
}
