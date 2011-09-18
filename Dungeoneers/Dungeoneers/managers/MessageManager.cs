using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Dungeoneers.managers
{
    class MessageManager
    {
        private static MessageManager instance;
        private Queue<Message> messageQueue;

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

        private MessageManager()
        {
            messageQueue = new Queue<Message>();
        }

        public void addMessage(string message)
        {
            messageQueue.Enqueue(new Message(message, 5000));
        }

        public Message[] getMessageQueueCopy()
        {
            return messageQueue.ToArray();
        }

        public void updateQueue(int timeElapsed)
        {
            foreach (Message msg in messageQueue.ToList())
            {
                msg.TimeCreated -= timeElapsed;
                if (msg.TimeCreated <= 0)
                    messageQueue.Dequeue();
            }
        }
    }

    class Message
    {
        public string Msg { get; set; }
        public int TimeCreated { get; set; }

        public Message(string msg, int timeCreated)
        {
            Msg = msg;
            TimeCreated = timeCreated;
        }
    }
}
