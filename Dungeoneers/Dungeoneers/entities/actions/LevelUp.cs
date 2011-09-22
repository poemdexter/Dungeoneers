using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;
using Dungeoneers.managers;
using Microsoft.Xna.Framework;

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
                    exp.Current_LevelEXP = exp.Current_EXP - Meta.ExpLevel[exp.Current_Level - 1];
                    string name = ((Information)this.Entity.GetComponent("Information") as Information).Username;
                    MessageManager.Instance.addMessage(String.Format("{0} is now level {1}!", name, exp.Current_Level), Color.Yellow);
                }
            }
        }
    }
}
