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
using Dungeoneers.entities.action_args;

namespace Dungeoneers
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        int keyboardElapsedTime;

        Entity player;

        private int windowHeight = 720;
        private int windowWidth = 1280;
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

            int seed = DateTime.Now.Millisecond;
            //int seed = 420;

            Console.WriteLine("current seed: " + seed);

            dungeon = new Dungeon(seed, spriteDict);
            dungeon.createDungeon();

            // set keyboard elapsed time to 0 so we're ready
            keyboardElapsedTime = 0;

            // let's try to make an torch entity that animates
            torch_elapsedTime = 0;
            torch_frameTime = 75;

            // let's add a player
            player = new Entity();
            player.AddComponent(new Position(10, 5));
            player.AddComponent(new Animation(spriteDict["bandit"], 1, false, SpriteEffects.None));
            player.AddAction(new ChangeDeltaPosition());
            player.AddAction(new ChangeDirectionOfAnimation());

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
                foreach (Entity torch in dungeon.torchList)
                {
                    torch.DoAction("NextFrameOfAnimation");
                    torch_elapsedTime = 0;
                }
            }

            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            HandleInput(currentKeyboardState, gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Must have SamplerState.PointClamp to scale the textures appropriately. The rest of the fields are default values.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            drawDungeon(spriteBatch);

            //spriteBatch.Draw(spriteDict["skeleton"], new Vector2(24 + (7 * (scale * 8)), 24 + (3 * (scale * 8))), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // draw player
            int x = ((Position)player.GetComponent("Position")).X;
            int y = ((Position)player.GetComponent("Position")).Y;
            Animation animation = (Animation)player.GetComponent("Animation");
            spriteBatch.Draw(animation.SourceTexture, new Vector2(24 + (x * (scale * 8)), 24 + (y * (scale * 8))), animation.SourceRect, Color.White, 0f, Vector2.Zero, scale, animation.Effects, 0f);

            // draw torches
            foreach (Entity torch in dungeon.torchList)
            {
                x = ((Position)torch.GetComponent("Position")).X;
                y = ((Position)torch.GetComponent("Position")).Y;
                animation = (Animation)torch.GetComponent("Animation");
                spriteBatch.Draw(animation.SourceTexture, new Vector2(24 + (x * (scale * 8)), 24 + (y * (scale * 8))), animation.SourceRect, Color.White, 0f, Vector2.Zero, scale, animation.Effects, 0f);
            }

            // draw doors
            foreach (Entity door in dungeon.doorList)
            {
                x = ((Position)door.GetComponent("Position")).X;
                y = ((Position)door.GetComponent("Position")).Y;
                animation = (Animation)door.GetComponent("Animation");
                spriteBatch.Draw(animation.SourceTexture, new Vector2(24 + (x * (scale * 8)), 24 + (y * (scale * 8))), animation.SourceRect, Color.White, 0f, Vector2.Zero, scale, animation.Effects, 0f);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        // todo, add player entity and movement
        public void HandleInput(KeyboardState keyboard, GameTime gameTime)
        {
            keyboardElapsedTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (currentKeyboardState != previousKeyboardState || (currentKeyboardState == previousKeyboardState && keyboardElapsedTime <= 0))
            {
                if (keyboard.IsKeyDown(Keys.Left))
                {
                    player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(-1, 0)));
                    player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("left"));
                }
                if (keyboard.IsKeyDown(Keys.Right))
                {
                    player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(1, 0)));
                    player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("right"));
                }
                if (keyboard.IsKeyDown(Keys.Up))
                {
                    player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(0, -1)));
                }
                if (keyboard.IsKeyDown(Keys.Down))
                {
                    player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(0, 1)));
                }

                keyboardElapsedTime = 150;
            }
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
            spriteDict.Add("door_wood_ns", Content.Load<Texture2D>("env/door_wood_ns"));
            spriteDict.Add("door_wood_we", Content.Load<Texture2D>("env/door_wood_we"));
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
