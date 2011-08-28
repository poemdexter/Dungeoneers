using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeoneers.framework
{
    public class EntityAction
    {
        public string Name { get; set; }
        public Entity Entity { get; set; }

        public virtual void Do() { }
        public virtual void Do(ActionArgs args) { }
    }
}
