using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeoneers.managers
{
    class MessageManager
    {
        private static MessageManager instance;

        public static MessageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MessageManager();
                }
                return instance;
            }
        }

        private MessageManager() { }



    }
}
