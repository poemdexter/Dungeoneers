using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.components
{
    class Openable : Component
    {
        public bool Opened { get; set; }

        public Openable()
        {
            this.Name = "Openable";
            this.Opened = false;
        }
    }
}
