using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Information : Component
    {
        public string Username { get; set; }

        public Information(string name)
        {
            this.Name = "Information";
            this.Username = name;
        }
    }
}