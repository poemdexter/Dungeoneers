using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dungeoneers.framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Dungeoneers.entities.components
{
    class Animation : Component
    {
        public Texture2D SourceTexture { get; set; }
        public int CurrentFrame { get; set; }
        public int FrameCount { get; set; }
        public int FrameHeight { get; set; }
        public Rectangle SourceRect { get; set; }
        public bool Looping { get; set; }
        public SpriteEffects Effects { get; set; }

        public Animation(Texture2D sourceTexture, int frameCount, bool looping, SpriteEffects effects)
        {
            this.Name = "Animation";
            this.SourceTexture = sourceTexture;
            this.FrameCount = frameCount;
            this.Effects = effects;
            this.Looping = looping;
            this.CurrentFrame = 0;
            this.FrameHeight = this.SourceTexture.Height;
            this.SourceRect = new Rectangle(this.CurrentFrame * this.FrameHeight, 0, this.FrameHeight, this.FrameHeight);
        }
    }
}
