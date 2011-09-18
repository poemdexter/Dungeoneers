using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.action_args;
using Dungeoneers.entities.components;
using Dungeoneers.managers;

namespace Dungeoneers.entities.actions
{
    class TakeDamage : EntityAction
    {
        public TakeDamage()
        {
            this.Name = "TakeDamage";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is TakeDamageArgs)
            {
                Hitpoints hitpoints = this.Entity.GetComponent("Hitpoints") as Hitpoints;
                if (hitpoints != null)
                {
                    hitpoints.Current_HP -= ((TakeDamageArgs)args).Damage;
                    if (hitpoints.Current_HP <= 0)
                    {
                        hitpoints.Alive = false;
                    }
                }
            }
        }
    }
}
