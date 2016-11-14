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
    }
}
