﻿///Written by Josh Christensen (u0978248) and Nathan Veillon (u0984669) 
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
        public int ID { get; private set; }

        [JsonProperty]
        public Point loc { get; private set; }
    }
}
