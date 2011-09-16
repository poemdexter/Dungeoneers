using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Player_Class : Component
    {
        public string ClassName { get; set; }

        public Player_Class(string name)
        {
            this.Name = "Player_Class";
            this.ClassName = name;
        }
    }
}