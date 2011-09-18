using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.action_args;
using Dungeoneers.entities.components;

namespace Dungeoneers.entities.actions
{
    class GainExperience : EntityAction
    {
        public GainExperience()
        {
            this.Name = "GainExperience";
        }

        public override void Do(ActionArgs args)
        {
            if (this.Entity != null && args != null && args is GainExperienceArgs)
            {
                Experience exp = this.Entity.GetComponent("Experience") as Experience;
                if (exp != null)
                {
                    exp.Current_EXP += ((GainExperienceArgs)args).Amount;
                    if (exp.Current_EXP >= Meta.ExpLevel[exp.Current_Level])
                        this.Entity.DoAction("LevelUp");
                }
            }
        }
    }
}
