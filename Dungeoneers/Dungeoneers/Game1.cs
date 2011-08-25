using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Dungeoneers.dungeon;
using Dungeoneers.entities.components;
using Dungeoneers.entities.actions;

namespace Dungeoneers
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        private int windowHeight = 768;
        private int windowWidth = 1024;
        private float scale = 4.0f;

        Dungeon dungeon;
        int torch_elapsedTime, torch_frameTime;

        Dictionary<string, Texture2D> spriteDict;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = windowHeight;
            graphics.PreferredBackBufferWidth = windowWidth;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            loadSpriteDictionary();

            //int seed = DateTime.Now.Millisecond;
            int seed = 420;

            Console.WriteLine("current seed: " + seed);

            dungeon = new Dungeon(seed, spriteDict);
            dungeon.createDungeon();

            // let's try to make an torch entity that animates
            torch_elapsedTime = 0;
            torch_frameTime = 75;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // this.Exit();
            torch_elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (torch_elapsedTime > torch_frameTime)
            {
                foreach(Entity torch in dungeon.torchList)
                {
                    torch.DoAction("NextFrameOfAnimation");
                    torch_elapsedTime = 0;
                }
            }


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Must have SamplerState.PointClamp to scale the textures appropriately. The rest of the fields are default values.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            drawDungeon(spriteBatch);

            spriteBatch.Draw(spriteDict["skeleton"], new Vector2(24 + (7 * (scale * 8)), 24 + (3 * (scale * 8))), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(spriteDict["bandit"], new Vector2(24 + (9 * (scale * 8)), 24 + (3 * (scale * 8))), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.FlipHorizontally, 0f);

            foreach (Entity torch in dungeon.torchList)
            {
                int x = ((Position)torch.GetComponent("Position")).X;
                int y = ((Position)torch.GetComponent("Position")).Y;
                spriteBatch.Draw(spriteDict["wall_torch"], new Vector2(24 + (x * (scale * 8)), 24 + (y * (scale * 8))), ((Animation)torch.GetComponent("Animation")).SourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void loadSpriteDictionary()
        {
            spriteDict = new Dictionary<string, Texture2D>();

            spriteDict.Add("skeleton", Content.Load<Texture2D>("entity/char_skeleton"));
            spriteDict.Add("bandit", Content.Load<Texture2D>("entity/char_bandit"));
            spriteDict.Add("wall_torch", Content.Load<Texture2D>("env/wall_torch"));
            spriteDict.Add("floor_norm", Content.Load<Texture2D>("env/floor_norm"));
            spriteDict.Add("wall_side_bot", Content.Load<Texture2D>("env/wall_side_bot"));
            spriteDict.Add("wall_exposed", Content.Load<Texture2D>("env/wall_exposed"));
        }

        private void drawDungeon(SpriteBatch batch)
        {
            Texture2D texture;
            int[][] floor = dungeon.floor;
            for (int x = 0; x < floor.Length; x++)
            {
                for (int y = 0; y < floor[x].Length; y++)
                {
                    if (floor[x][y] == 1) texture = spriteDict["floor_norm"];
                    else if (floor[x][y] == 2) texture = spriteDict["wall_exposed"];
                    else if (floor[x][y] == 3) texture = spriteDict["wall_side_bot"];
                    else continue;

                    batch.Draw(texture, new Vector2(24 + (x * (scale * texture.Width)), 24 + (y * (scale * texture.Height))), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
            }
        }
    }
}
