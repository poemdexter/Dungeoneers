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
        SpriteFont lofiFont;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;
        int keyboardElapsedTime;

        Viewport leftViewport, rightViewport;

        private int windowHeight = 720;
        private int windowWidth = 1280;
        private float scale = 4.0f;
        private float font_scale = 2.0f;

        private bool playerActed;

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
            lofiFont = Content.Load<SpriteFont>("font/lofi_font");
            loadSpriteDictionary();

            leftViewport = new Viewport(0, 0, 960, 720);
            rightViewport = new Viewport(960, 0, 320, 720);


            int seed = DateTime.Now.Millisecond;
            //int seed = 203;

            Console.WriteLine("current seed: " + seed);

            dungeon = new Dungeon(seed, spriteDict);
            dungeon.createDungeon();
            dungeon.addPlayer();

            // set keyboard elapsed time to 0 so we're ready
            keyboardElapsedTime = 0;

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
                foreach (Entity torch in dungeon.torchList)
                {
                    torch.DoAction("NextFrameOfAnimation");
                    torch_elapsedTime = 0;
                }
            }

            // player can act
            playerActed = false;
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            HandleInput(currentKeyboardState, gameTime);

            if (playerActed)
            {
                // let the rest of the mobs act
                nextGameStateChange();
            }

            base.Update(gameTime);
        }

        public void nextGameStateChange()
        {
            // player moved, so now it's mob's turn
            dungeon.skeleton.DoAction("MoveTowardsPlayer", 
                new MoveTowardsPlayerArgs(((Position)dungeon.player.GetComponent("Position")).X,
                                          ((Position)dungeon.player.GetComponent("Position")).Y, 
                                          dungeon.floor));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // left side of screen
            GraphicsDevice.Viewport = leftViewport;
            // Must have SamplerState.PointClamp to scale the textures appropriately. The rest of the fields are default values.
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, getMatrixTranslation());

            drawDungeon(spriteBatch);

            // draw torches
            foreach (Entity torch in dungeon.torchList)
            {
                int x = (int)((Position)torch.GetComponent("Position")).X;
                int y = (int)((Position)torch.GetComponent("Position")).Y;
                Animation animation = (Animation)torch.GetComponent("Animation");
                spriteBatch.Draw(animation.SourceTexture, new Vector2(x * (scale * 8), y * (scale * 8)), animation.SourceRect, Color.White, 0f, Vector2.Zero, scale, animation.Effects, 0f);
            }

            // draw doors
            foreach (Entity door in dungeon.manager.getDoorList())
            {
                int x = (int)((Position)door.GetComponent("Position")).X;
                int y = (int)((Position)door.GetComponent("Position")).Y;
                Animation animation = (Animation)door.GetComponent("Animation");
                spriteBatch.Draw(animation.SourceTexture, new Vector2(x * (scale * 8), y * (scale * 8)), animation.SourceRect, Color.White, 0f, Vector2.Zero, scale, animation.Effects, 0f);
            }

            // draw stairs
            int sx = (int)((Position)dungeon.StairsUp.GetComponent("Position")).X;
            int sy = (int)((Position)dungeon.StairsUp.GetComponent("Position")).Y;
            Animation sanimation = (Animation)dungeon.StairsUp.GetComponent("Animation");
            spriteBatch.Draw(spriteDict["stairs_up"], new Vector2(sx * (scale * 8), sy * (scale * 8)), sanimation.SourceRect, Color.White, 0f, Vector2.Zero, scale, sanimation.Effects, 0f);

            // draw stairs
            sx = (int)((Position)dungeon.StairsDown.GetComponent("Position")).X;
            sy = (int)((Position)dungeon.StairsDown.GetComponent("Position")).Y;
            sanimation = (Animation)dungeon.StairsDown.GetComponent("Animation");
            spriteBatch.Draw(spriteDict["stairs_down"], new Vector2(sx * (scale * 8), sy * (scale * 8)), sanimation.SourceRect, Color.White, 0f, Vector2.Zero, scale, sanimation.Effects, 0f);

            // draw player
            int px = (int)((Position)dungeon.player.GetComponent("Position")).X;
            int py = (int)((Position)dungeon.player.GetComponent("Position")).Y;
            Animation panimation = (Animation)dungeon.player.GetComponent("Animation");
            spriteBatch.Draw(panimation.SourceTexture, new Vector2(px * (scale * 8), py * (scale * 8)), panimation.SourceRect, Color.White, 0f, Vector2.Zero, scale, panimation.Effects, 0f);

            // draw mobs
            int mx = (int)((Position)dungeon.skeleton.GetComponent("Position")).X;
            int my = (int)((Position)dungeon.skeleton.GetComponent("Position")).Y;
            Animation manimation = (Animation)dungeon.skeleton.GetComponent("Animation");
            spriteBatch.Draw(manimation.SourceTexture, new Vector2(mx * (scale * 8), my * (scale * 8)), manimation.SourceRect, Color.White, 0f, Vector2.Zero, scale, manimation.Effects, 0f);

            spriteBatch.End();

            // draw version
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            spriteBatch.DrawString(lofiFont, "Dungeoneers Project 0.1a", new Vector2(0, leftViewport.Height - 15), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.End();

            // right side of screen
            GraphicsDevice.Viewport = rightViewport;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            // gui background
            Texture2D gui = spriteDict["gui_background"];
            spriteBatch.Draw(gui, Vector2.Zero, gui.Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // draw ui stuff
            spriteBatch.DrawString(lofiFont, "poemdexter", new Vector2(24, 24), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(lofiFont, "Bandit", new Vector2(24, 44), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(lofiFont, "LVL: 1", new Vector2(24, 64), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(lofiFont, "EXP: 0", new Vector2(24, 84), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void HandleInput(KeyboardState keyboard, GameTime gameTime)
        {
            keyboardElapsedTime -= gameTime.ElapsedGameTime.Milliseconds;

            if (keyboardElapsedTime <= 0)
            {
                int x = (int)((Position)dungeon.player.GetComponent("Position")).X;
                int y = (int)((Position)dungeon.player.GetComponent("Position")).Y;

                if (keyboard.IsKeyDown(Keys.Left) && dungeon.floor[x - 1][y] == 1)
                {
                    if (dungeon.manager.getDoor(new Vector2(x - 1, y)) != null)
                    {
                        Entity door = dungeon.manager.getDoor(new Vector2(x - 1, y));
                        if (!(door.GetComponent("Openable") as Openable).Opened)
                        {
                            (door.GetComponent("Openable") as Openable).Opened = true;
                            door.DoAction("NextFrameOfAnimation");
                        }
                        else
                        {
                            dungeon.player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(-1, 0)));
                            dungeon.player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("left"));
                        }
                    }
                    else
                    {
                        dungeon.player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(-1, 0)));
                        dungeon.player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("left"));
                    }
                    keyboardElapsedTime = 150;
                    playerActed = true;
                }
                else if (keyboard.IsKeyDown(Keys.Right) && dungeon.floor[x + 1][y] == 1)
                {
                    if (dungeon.manager.getDoor(new Vector2(x + 1, y)) != null)
                    {
                        Entity door = dungeon.manager.getDoor(new Vector2(x + 1, y));
                        if (!(door.GetComponent("Openable") as Openable).Opened)
                        {
                            (door.GetComponent("Openable") as Openable).Opened = true;
                            door.DoAction("NextFrameOfAnimation");
                        }
                        else
                        {
                            dungeon.player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(1, 0)));
                            dungeon.player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("right"));
                        }
                    }
                    else
                    {
                        dungeon.player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(1, 0)));
                        dungeon.player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("right"));
                    }
                    keyboardElapsedTime = 150;
                    playerActed = true;
                }
                else if (keyboard.IsKeyDown(Keys.Up) && dungeon.floor[x][y - 1] == 1)
                {
                    if (dungeon.manager.getDoor(new Vector2(x, y - 1)) != null)
                    {
                        Entity door = dungeon.manager.getDoor(new Vector2(x, y - 1));
                        if (!(door.GetComponent("Openable") as Openable).Opened)
                        {
                            (door.GetComponent("Openable") as Openable).Opened = true;
                            door.DoAction("NextFrameOfAnimation");
                        }
                        else
                        {
                            dungeon.player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(0, -1)));
                        }
                    }
                    else
                    {
                        dungeon.player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(0, -1)));
                    }
                    keyboardElapsedTime = 150;
                    playerActed = true;
                }
                else if (keyboard.IsKeyDown(Keys.Down) && dungeon.floor[x][y + 1] == 1)
                {
                    if (dungeon.manager.getDoor(new Vector2(x, y + 1)) != null)
                    {
                        Entity door = dungeon.manager.getDoor(new Vector2(x, y + 1));
                        if (!(door.GetComponent("Openable") as Openable).Opened)
                        {
                            (door.GetComponent("Openable") as Openable).Opened = true;
                            door.DoAction("NextFrameOfAnimation");
                        }
                        else
                        {
                            dungeon.player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(0, 1)));
                        }
                    }
                    else
                    {
                        dungeon.player.DoAction("ChangeDeltaPosition", new ChangeDeltaPositionArgs(new Vector2(0, 1)));
                    }
                    keyboardElapsedTime = 150;
                    playerActed = true;
                }
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
            spriteDict.Add("stairs_up", Content.Load<Texture2D>("env/stairs_up"));
            spriteDict.Add("stairs_down", Content.Load<Texture2D>("env/stairs_down"));
            spriteDict.Add("gui_background", Content.Load<Texture2D>("gui/gui_background"));
            spriteDict.Add("shrub", Content.Load<Texture2D>("env/shrub"));
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
                    // else if (floor[x][y] == 0) texture = spriteDict["shrub"];
                    else continue;

                    batch.Draw(texture, new Vector2(x * (scale * texture.Width), y * (scale * texture.Height)), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
            }
        }

        private Matrix getMatrixTranslation()
        {
            Matrix trans;

            // X - ( viewport width / (frameheight * scale) ) / 2 ish
            Vector3 playerV = new Vector3(((Position)dungeon.player.GetComponent("Position")).X - 14.5f, ((Position)dungeon.player.GetComponent("Position")).Y - 11f, 0);
            Vector3 viewV = new Vector3(scale * ((Animation)dungeon.StairsUp.GetComponent("Animation")).FrameHeight, scale * ((Animation)dungeon.StairsUp.GetComponent("Animation")).FrameHeight, 0);

            trans = Matrix.CreateTranslation(playerV * -viewV);

            return trans;
        }
    }
}