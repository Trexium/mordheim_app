using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexedSceneryMobileApp.Models
{
    public class MenuGroup
    {
        public string Id { get; set; }
        public string DisplayText { get; set; }
        public bool Expanded { get; set; }
        public List<MenuItem> Items { get; set; }
        public List<MenuGroup> Groups { get; set; }
    }
}
