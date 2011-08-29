using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Dungeoneers.entities.components;

namespace Dungeoneers.entities.actions
{
    class ChangeStateOfOpenable : EntityAction
    {
        public ChangeStateOfOpenable()
        {
            this.Name = "ChangeStateOfOpenable";
        }

        public override void Do()
        {
            if (this.Entity != null)
            {
                Openable openable = this.Entity.GetComponent("Openable") as Openable;
                if (openable != null)
                {
                    if (!openable.Opened)
                    {
                        openable.Opened = true;
                    }
                }
            }
        }
    }
}
