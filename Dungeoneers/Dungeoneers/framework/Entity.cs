using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;

namespace Dungeoneers
{
    public class Entity
    {
        public int Id { get; set; }

        private Dictionary<string, Component> components = new Dictionary<string, Component>();
        private Dictionary<string, EntityAction> actions = new Dictionary<string, EntityAction>();

        public void AddComponent(Component component)
        {
            this.components.Add(component.Name, component);
        }

        public void RemoveComponent(string name)
        {
            this.components.Remove(name);
        }

        public Component GetComponent(string name)
        {
            return (this.components.ContainsKey(name)) ? components[name] : null;
        }

        public void AddAction(EntityAction action)
        {
            action.Entity = this;
            this.actions.Add(action.Name, action);
        }

        public void RemoveAction(string name)
        {
            this.actions.Remove(name);
        }

        public void DoAction(string name)
        {
            if (this.actions.ContainsKey(name))
            {
                this.actions[name].Do();
            }
        }

        public void DoAction(string name, ActionArgs arg)
        {
            if (this.actions.ContainsKey(name))
            {
                this.actions[name].Do(arg);
            }
        }
    }
}
