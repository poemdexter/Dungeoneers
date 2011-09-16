using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Player_Info : Component
    {
        public string Username { get; set; }

        public Player_Info(string name)
        {
            this.Name = "Player_Info";
            this.Username = name;
        }
    }
}