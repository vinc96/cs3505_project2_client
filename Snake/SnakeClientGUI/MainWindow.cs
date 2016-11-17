using SnakeModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SnakeClientGUI
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();

            Snake s1 = JsonConvert.DeserializeObject<Snake>("{ \"ID\":1,\"name\":\"Player1\",\"vertices\":[{\"x\":26,\"y\":25},{\"x\":26,\"y\":22},{\"x\":24,\"y\":22},{\"x\":24,\"y\":29},{\"x\":21,\"y\":29}]}");
            Snake s2 = JsonConvert.DeserializeObject<Snake>("{ \"ID\":2,\"name\":\"Player2\",\"vertices\":[{\"x\":2,\"y\":1},{\"x\":148,\"y\":1},{\"x\":148,\"y\":148}]}");
            List<Snake> snakes = new List<Snake>();

            snakes.Add(s1);
            snakes.Add(s2);

            World world = new World(snakes, null, 150, 150);

            snakeDisplayPanel1.updatePanel(world, 0);
        }
    }
}
