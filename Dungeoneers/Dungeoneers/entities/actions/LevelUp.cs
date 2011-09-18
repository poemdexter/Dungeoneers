using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;

namespace Dungeoneers.entities.actions
{
    class LevelUp : EntityAction
    {
        public LevelUp()
        {
            this.Name = "LevelUp";
        }

        public override void Do()
        {
            if (this.Entity != null)
            {
                Experience exp = this.Entity.GetComponent("Experience") as Experience;
                if (exp != null)
                {
                    exp.Current_Level += 1;
                }
            }
        }
    }
}
