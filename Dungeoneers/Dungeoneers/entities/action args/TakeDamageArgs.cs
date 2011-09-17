using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers.entities.action_args
{
    class TakeDamageArgs : ActionArgs
    {
        public int Damage { get; set; }

        public TakeDamageArgs(int damage)
        {
            this.Damage = damage;
        }
    }
}
