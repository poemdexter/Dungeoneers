using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Dungeoneers.managers
{
    class MessageManager
    {
        private static MessageManager instance;
        private Queue<Message> messageQueue;
        private List<string> messageHistory;
        private int PageNumber { get; set; }
        private const int HISTORY_LINES = 29;

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
            messageHistory = new List<string>();
        }

        public void resetPageNumber()
        {
            PageNumber = 1;
        }

        public void addMessage(string message)
        {
            messageQueue.Enqueue(new Message(message, 5000));
            messageHistory.Add(message);
        }

        public string[] getTopMessagesToDisplay()
        {
            Message[] temp = messageQueue.ToArray();
            if (temp.Length > 0)
            {
                string[] ret = new string[Math.Min(5, temp.Length)];
                for (int x = 0; x < Math.Min(5, temp.Length); x++)
                {
                    ret[x] = temp[temp.Length - Math.Min((5 - x), temp.Length)].Msg;
                }
                return ret;
            }
            return null;
        }

        public string[] getFirstPageOfHistory()
        {
            PageNumber = 1;
            if (messageHistory.ToArray().Length - 1 < HISTORY_LINES)
            {
                string[] temp = messageHistory.ToArray();
                Array.Reverse(temp);
                return temp;
            }
            else
                return null;
        }

        // for message history state
        public void drawMessageHistory(SpriteBatch spriteBatch, SpriteFont font, float scale)
        {
            for (int x = 0; x < HISTORY_LINES; x++)
            {
                spriteBatch.DrawString(font, "poemdexter line 1", new Vector2(40, 40 + (15*x)), Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
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
        public Vector2 Position { get; set; }

        public Message(string msg, int timeCreated)
        {
            Msg = msg;
            TimeCreated = timeCreated;
            Position = Vector2.Zero;
        }

        public Message(string msg, int timeCreated, Vector2 position)
        {
            Msg = msg;
            TimeCreated = timeCreated;
            Position = position;
        }
    }
}
