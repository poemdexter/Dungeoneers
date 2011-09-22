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
        private List<Message> messageHistory;

        private int PageNumber { get; set; }
        private const int HISTORY_LINES = 29;

        public bool PageUp { get; set; }
        public bool PageDown { get; set; }

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
            messageHistory = new List<Message>();
        }

        public void resetPageNumber()
        {
            PageNumber = 1;
        }

        public void addMessage(string message)
        {
            messageQueue.Enqueue(new Message(message, 5000));
            messageHistory.Add(new Message(message, 5000));
        }

        public void addMessage(string message, Color color)
        {
            messageQueue.Enqueue(new Message(message, 5000, color));
            messageHistory.Add(new Message(message, 5000, color));
        }

        public List<Message> getTopMessagesToDisplay()
        {
            List<Message> messages = messageQueue.ToList();
            if (messages.Count > 0)
            {
                int lines = Math.Min(5, messages.Count);
                List<Message> temp = new List<Message>();
                for (int a = lines; a > 0; a--)
                    temp.Add(messages[messages.Count - a]);

                return temp;
            }
            return null;
        }

        public List<Message> getPageOfHistory()
        {
            // we're at the very earliest
            if (messageHistory.ToList().Count - 1 < HISTORY_LINES)
            {
                PageUp = false;  // can't go up (into the past)
                if (PageNumber == 1)
                    PageDown = false; // can't go down since we're on first page
                else
                    PageDown = true;
                return messageHistory.ToList();
            }
            else
            {
                List<Message> temp = messageHistory.ToList();
                int start = Math.Max(0,temp.Count - (HISTORY_LINES * PageNumber));
                if (start == 0)  // can't go up since we're at very beginning of history
                    PageUp = false;
                else
                    PageUp = true;
                if (PageNumber == 1)  // can't go down since we're on first page
                    PageDown = false;
                else
                    PageDown = true;
                return temp.Skip(start).Take(HISTORY_LINES).ToList();
                
            }
        }

        public void tryPageUp()
        {
            if (PageUp)
                PageNumber++;
        }

        public void tryPageDown()
        {
            if (PageDown)
                PageNumber--;
        }

        // for message history state
        public void drawMessageHistory(SpriteBatch spriteBatch, SpriteFont font, float scale)
        {
            List<Message> history = getPageOfHistory();
            int n = 0;
            foreach(Message msg in history)
            {
                spriteBatch.DrawString(font, msg.Msg, new Vector2(40, 40 + (15*n)), msg.TextColor, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
                n++;
            }

            // draw << >> for player showing pages available
            if (PageUp)
                spriteBatch.DrawString(font, "<<", new Vector2(690, 475), Color.GreenYellow, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            if (PageDown)
                spriteBatch.DrawString(font, ">>", new Vector2(710, 475), Color.GreenYellow, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
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
        public Color TextColor { get; set; }

        public Message(string msg, int timeCreated)
        {
            Msg = msg;
            TimeCreated = timeCreated;
            TextColor = Color.White;
        }

        public Message(string msg, int timeCreated, Color color)
        {
            Msg = msg;
            TimeCreated = timeCreated;
            TextColor = color;
        }
    }
}
