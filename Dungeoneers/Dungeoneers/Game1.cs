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
using Dungeoneers.managers;

namespace Dungeoneers
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        MessageManager messager;
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

            dungeon = new Dungeon(spriteDict);
            dungeon.createDungeon();
            dungeon.addPlayer();

            // set screenstate to playing the game
            GameScreenStateManager.CurrentState = ScreenStates.Play;

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

            // update messages
            messager = MessageManager.Instance;
            messager.updateQueue(gameTime.ElapsedGameTime.Milliseconds);

            // player can act
            playerActed = false;
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            HandleInput(currentKeyboardState, gameTime);

            if (playerActed)
            {
                // let the rest of the mobs act
                nextGameStateChange();
                playerActed = false;
            }

            base.Update(gameTime);
        }

        public void nextGameStateChange()
        {
            Vector2 playerPos = dungeon.manager.getPlayerPosition();
            foreach (Entity skeleton in dungeon.manager.getMobList())
            {
                if (dungeon.manager.isMobAliveInList(skeleton.Id))
                {
                    // check if player is close, if so attack!
                    if (dungeon.manager.canAttackPlayer(skeleton.Id))
                    {
                        dungeon.manager.attackPhase(skeleton, dungeon.manager.player);
                    }
                    else
                    {
                        // player moved, so now it's mob's turn
                        skeleton.DoAction("MoveTowardsPlayer", new MoveTowardsPlayerArgs(playerPos.X, playerPos.Y, dungeon.floor, dungeon.manager));
                    }
                }
            }
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
            int sx = (int)((Position)dungeon.manager.StairsUp.GetComponent("Position")).X;
            int sy = (int)((Position)dungeon.manager.StairsUp.GetComponent("Position")).Y;
            Animation sanimation = (Animation)dungeon.manager.StairsUp.GetComponent("Animation");
            spriteBatch.Draw(spriteDict["stairs_up"], new Vector2(sx * (scale * 8), sy * (scale * 8)), sanimation.SourceRect, Color.White, 0f, Vector2.Zero, scale, sanimation.Effects, 0f);

            // draw stairs
            sx = (int)((Position)dungeon.manager.StairsDown.GetComponent("Position")).X;
            sy = (int)((Position)dungeon.manager.StairsDown.GetComponent("Position")).Y;
            sanimation = (Animation)dungeon.manager.StairsDown.GetComponent("Animation");
            spriteBatch.Draw(spriteDict["stairs_down"], new Vector2(sx * (scale * 8), sy * (scale * 8)), sanimation.SourceRect, Color.White, 0f, Vector2.Zero, scale, sanimation.Effects, 0f);

            // TODO draw dead mobs first

            // draw mobs
            foreach (Entity skeleton in dungeon.manager.getMobList())
            {
                int mx = (int)((Position)skeleton.GetComponent("Position")).X;
                int my = (int)((Position)skeleton.GetComponent("Position")).Y;
                if (((Hitpoints)skeleton.GetComponent("Hitpoints")).Alive)
                {
                    Animation manimation = (Animation)skeleton.GetComponent("Animation");
                    spriteBatch.Draw(manimation.SourceTexture, new Vector2(mx * (scale * 8), my * (scale * 8)), manimation.SourceRect, Color.White, 0f, Vector2.Zero, scale, manimation.Effects, 0f);
                }
                else
                {
                    spriteBatch.Draw(spriteDict["item_skull"], new Vector2(mx * (scale * 8), my * (scale * 8)), spriteDict["item_skull"].Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
            }

            // draw player
            int px = (int)((Position)dungeon.manager.player.GetComponent("Position")).X;
            int py = (int)((Position)dungeon.manager.player.GetComponent("Position")).Y;
            Animation panimation = (Animation)dungeon.manager.player.GetComponent("Animation");
            spriteBatch.Draw(panimation.SourceTexture, new Vector2(px * (scale * 8), py * (scale * 8)), panimation.SourceRect, Color.White, 0f, Vector2.Zero, scale, panimation.Effects, 0f);
            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            // draw version
            spriteBatch.DrawString(lofiFont, "Dungeoneers Project 0.3a", new Vector2(0, leftViewport.Height - 15), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);

            // draw messages
            string[] msgList = MessageManager.Instance.getTopMessagesToDisplay();
            if (msgList != null)
            {
                for (int x = 0; x < msgList.Length; x++)
                {
                    spriteBatch.DrawString(lofiFont, msgList[x], new Vector2(2, (x * 15) + 2), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
                }
            }

            // draw message history
            if (GameScreenStateManager.CurrentState == ScreenStates.MessageHistory)
            {
                spriteBatch.Draw(spriteDict["msg_history"], new Vector2(20, 20), spriteDict["msg_history"].Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                MessageManager.Instance.drawMessageHistory(spriteBatch, lofiFont, font_scale);
            }

            spriteBatch.End();

            // right side of screen
            GraphicsDevice.Viewport = rightViewport;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);

            // gui background
            Texture2D gui = spriteDict["gui_background"];
            spriteBatch.Draw(gui, Vector2.Zero, gui.Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);

            // draw ui stuff
            Entity player = dungeon.manager.player;

            Experience exp = player.GetComponent("Experience") as Experience;
            spriteBatch.DrawString(lofiFont, "poemdexter", new Vector2(24, 24), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(lofiFont, "Class Name (" + exp.Current_Level + ")", new Vector2(24, 44), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);


            spriteBatch.DrawString(lofiFont, "EXP: " + exp.Current_EXP + "/" + Meta.ExpLevel[exp.Current_Level], new Vector2(24, 84), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.Draw(spriteDict["ui_bar"], new Vector2(24, 99), spriteDict["ui_bar"].Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            int exp_ctr = (int)Math.Ceiling(((double)exp.Current_LevelEXP / (double)(Meta.ExpLevel[exp.Current_Level] - Meta.ExpLevel[exp.Current_Level - 1])) * 20);
            for (int x = 0; x < exp_ctr; x++)
            {
                spriteBatch.Draw(spriteDict["ui_barpiece_exp"], new Vector2(28 + (8 * x), 99), spriteDict["ui_barpiece_exp"].Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            Hitpoints hp = player.GetComponent("Hitpoints") as Hitpoints;
            spriteBatch.DrawString(lofiFont, "HP: " + hp.Current_HP + "/" + hp.Max_HP, new Vector2(24, 124), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.Draw(spriteDict["ui_bar"], new Vector2(24, 139), spriteDict["ui_bar"].Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            int hp_ctr = (int)Math.Ceiling(((double)hp.Current_HP / (double)hp.Max_HP) * 20);
            for (int x = 0; x < hp_ctr; x++)
            {
                spriteBatch.Draw(spriteDict["ui_barpiece_hp"], new Vector2(28 + (8 * x), 139), spriteDict["ui_barpiece_hp"].Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            Mana mp = player.GetComponent("Mana") as Mana;
            spriteBatch.DrawString(lofiFont, "MP: " + mp.Current_MP + "/" + mp.Max_MP, new Vector2(24, 164), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.Draw(spriteDict["ui_bar"], new Vector2(24, 179), spriteDict["ui_bar"].Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            int mp_ctr = (int)Math.Ceiling(((double)mp.Current_MP / (double)mp.Max_MP) * 20);
            for (int x = 0; x < mp_ctr; x++)
            {
                spriteBatch.Draw(spriteDict["ui_barpiece_mp"], new Vector2(28 + (8 * x), 179), spriteDict["ui_barpiece_mp"].Bounds, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0);
            }

            spriteBatch.DrawString(lofiFont, "STR: 0", new Vector2(24, 224), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(lofiFont, "DEX: 0", new Vector2(104, 224), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(lofiFont, "WIS: 0", new Vector2(184, 224), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            Armor armor = ((Equipment)dungeon.manager.player.GetComponent("Equipment")).Chest;
            spriteBatch.DrawString(lofiFont, "Armor: " + armor.DmgReduction, new Vector2(24, 244), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);

            Weapon weapon = ((Equipment)dungeon.manager.player.GetComponent("Equipment")).MainHand;
            string dice = weapon.Dice_Num + "d" + weapon.Dice_Sides;
            if (weapon.Roll_Mod != 0)
                dice = dice + " + " + weapon.Roll_Mod;
            spriteBatch.DrawString(lofiFont, "Weapon: " + weapon.WName + " (" + dice + ")", new Vector2(24, 264), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);
            spriteBatch.DrawString(lofiFont, "Ranged: -unequipped-", new Vector2(24, 284), Color.White, 0, Vector2.Zero, font_scale, SpriteEffects.None, 0);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        public void HandleInput(KeyboardState keyboard, GameTime gameTime)
        {
            keyboardElapsedTime -= gameTime.ElapsedGameTime.Milliseconds;

            switch (GameScreenStateManager.CurrentState)
            {
                case ScreenStates.Play:
                    if (keyboardElapsedTime <= 0 && !playerActed)
                    {
                        int x = (int)((Position)dungeon.manager.player.GetComponent("Position")).X;
                        int y = (int)((Position)dungeon.manager.player.GetComponent("Position")).Y;

                        // left
                        if ((keyboard.IsKeyDown(Keys.NumPad4) || keyboard.IsKeyDown(Keys.H)) && dungeon.floor[x - 1][y] == 1)
                        {
                            if (dungeon.manager.isMobAliveAtPos(new Vector2(x - 1, y)))
                                dungeon.manager.attackPhase(dungeon.manager.player, dungeon.manager.getAliveMobAt(new Vector2(x - 1, y)));


                            else if (dungeon.manager.getDoor(new Vector2(x - 1, y)) != null)
                            {
                                Entity door = dungeon.manager.getDoor(new Vector2(x - 1, y));
                                if (!(door.GetComponent("Openable") as Openable).Opened)
                                {
                                    (door.GetComponent("Openable") as Openable).Opened = true;
                                    door.DoAction("NextFrameOfAnimation");
                                }
                                else
                                {
                                    dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(-1, 0)));
                                    dungeon.manager.player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("left"));
                                }
                            }
                            else
                            {
                                dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(-1, 0)));
                                dungeon.manager.player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("left"));
                            }
                            keyboardElapsedTime = 200;
                            playerActed = true;
                        }

                        // right
                        else if ((keyboard.IsKeyDown(Keys.NumPad6) || keyboard.IsKeyDown(Keys.L)) && dungeon.floor[x + 1][y] == 1)
                        {
                            if (dungeon.manager.isMobAliveAtPos(new Vector2(x + 1, y)))
                                dungeon.manager.attackPhase(dungeon.manager.player, dungeon.manager.getAliveMobAt(new Vector2(x + 1, y)));

                            else if (dungeon.manager.getDoor(new Vector2(x + 1, y)) != null)
                            {
                                Entity door = dungeon.manager.getDoor(new Vector2(x + 1, y));
                                if (!(door.GetComponent("Openable") as Openable).Opened)
                                {
                                    (door.GetComponent("Openable") as Openable).Opened = true;
                                    door.DoAction("NextFrameOfAnimation");
                                }
                                else
                                {
                                    dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(1, 0)));
                                    dungeon.manager.player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("right"));
                                }
                            }
                            else
                            {
                                dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(1, 0)));
                                dungeon.manager.player.DoAction("ChangeDirectionOfAnimation", new ChangeDirectionOfAnimationArgs("right"));
                            }
                            keyboardElapsedTime = 200;
                            playerActed = true;
                        }

                        // up
                        else if ((keyboard.IsKeyDown(Keys.NumPad8) || keyboard.IsKeyDown(Keys.K)) && dungeon.floor[x][y - 1] == 1)
                        {
                            if (dungeon.manager.isMobAliveAtPos(new Vector2(x, y - 1)))
                                dungeon.manager.attackPhase(dungeon.manager.player, dungeon.manager.getAliveMobAt(new Vector2(x, y - 1)));

                            else if (dungeon.manager.getDoor(new Vector2(x, y - 1)) != null)
                            {
                                Entity door = dungeon.manager.getDoor(new Vector2(x, y - 1));
                                if (!(door.GetComponent("Openable") as Openable).Opened)
                                {
                                    (door.GetComponent("Openable") as Openable).Opened = true;
                                    door.DoAction("NextFrameOfAnimation");
                                }
                                else
                                {
                                    dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(0, -1)));
                                }
                            }
                            else
                            {
                                dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(0, -1)));
                            }
                            keyboardElapsedTime = 200;
                            playerActed = true;
                        }

                        // down
                        else if ((keyboard.IsKeyDown(Keys.NumPad2) || keyboard.IsKeyDown(Keys.J)) && dungeon.floor[x][y + 1] == 1)
                        {
                            if (dungeon.manager.isMobAliveAtPos(new Vector2(x, y + 1)))
                                dungeon.manager.attackPhase(dungeon.manager.player, dungeon.manager.getAliveMobAt(new Vector2(x, y + 1)));

                            else if (dungeon.manager.getDoor(new Vector2(x, y + 1)) != null)
                            {
                                Entity door = dungeon.manager.getDoor(new Vector2(x, y + 1));
                                if (!(door.GetComponent("Openable") as Openable).Opened)
                                {
                                    (door.GetComponent("Openable") as Openable).Opened = true;
                                    door.DoAction("NextFrameOfAnimation");
                                }
                                else
                                {
                                    dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(0, 1)));
                                }
                            }
                            else
                            {
                                dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(0, 1)));
                            }
                            keyboardElapsedTime = 200;
                            playerActed = true;
                        }

                        // northwest
                        else if ((keyboard.IsKeyDown(Keys.NumPad7) || keyboard.IsKeyDown(Keys.Y)) && dungeon.floor[x - 1][y - 1] == 1)
                        {
                            if (dungeon.manager.isMobAliveAtPos(new Vector2(x - 1, y - 1)))
                                dungeon.manager.attackPhase(dungeon.manager.player, dungeon.manager.getAliveMobAt(new Vector2(x - 1, y - 1)));

                            else if (dungeon.manager.getDoor(new Vector2(x - 1, y - 1)) != null)
                            {
                                Entity door = dungeon.manager.getDoor(new Vector2(x - 1, y - 1));
                                if (!(door.GetComponent("Openable") as Openable).Opened)
                                {
                                    (door.GetComponent("Openable") as Openable).Opened = true;
                                    door.DoAction("NextFrameOfAnimation");
                                }
                                else
                                {
                                    dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(-1, -1)));
                                }
                            }
                            else
                            {
                                dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(-1, -1)));
                            }
                            keyboardElapsedTime = 200;
                            playerActed = true;
                        }

                        // northeast
                        else if ((keyboard.IsKeyDown(Keys.NumPad9) || keyboard.IsKeyDown(Keys.U)) && dungeon.floor[x + 1][y - 1] == 1)
                        {
                            if (dungeon.manager.isMobAliveAtPos(new Vector2(x + 1, y - 1)))
                                dungeon.manager.attackPhase(dungeon.manager.player, dungeon.manager.getAliveMobAt(new Vector2(x + 1, y - 1)));

                            else if (dungeon.manager.getDoor(new Vector2(x + 1, y - 1)) != null)
                            {
                                Entity door = dungeon.manager.getDoor(new Vector2(x + 1, y - 1));
                                if (!(door.GetComponent("Openable") as Openable).Opened)
                                {
                                    (door.GetComponent("Openable") as Openable).Opened = true;
                                    door.DoAction("NextFrameOfAnimation");
                                }
                                else
                                {
                                    dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(1, -1)));
                                }
                            }
                            else
                            {
                                dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(1, -1)));
                            }
                            keyboardElapsedTime = 200;
                            playerActed = true;
                        }

                        // southwest
                        else if ((keyboard.IsKeyDown(Keys.NumPad1) || keyboard.IsKeyDown(Keys.B)) && dungeon.floor[x - 1][y + 1] == 1)
                        {
                            if (dungeon.manager.isMobAliveAtPos(new Vector2(x - 1, y + 1)))
                                dungeon.manager.attackPhase(dungeon.manager.player, dungeon.manager.getAliveMobAt(new Vector2(x - 1, y + 1)));

                            else if (dungeon.manager.getDoor(new Vector2(x - 1, y + 1)) != null)
                            {
                                Entity door = dungeon.manager.getDoor(new Vector2(x - 1, y + 1));
                                if (!(door.GetComponent("Openable") as Openable).Opened)
                                {
                                    (door.GetComponent("Openable") as Openable).Opened = true;
                                    door.DoAction("NextFrameOfAnimation");
                                }
                                else
                                {
                                    dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(-1, 1)));
                                }
                            }
                            else
                            {
                                dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(-1, 1)));
                            }
                            keyboardElapsedTime = 200;
                            playerActed = true;
                        }

                        // southeast
                        else if ((keyboard.IsKeyDown(Keys.NumPad3) || keyboard.IsKeyDown(Keys.N)) && dungeon.floor[x + 1][y + 1] == 1)
                        {
                            if (dungeon.manager.isMobAliveAtPos(new Vector2(x + 1, y + 1)))
                                dungeon.manager.attackPhase(dungeon.manager.player, dungeon.manager.getAliveMobAt(new Vector2(x + 1, y + 1)));

                            else if (dungeon.manager.getDoor(new Vector2(x + 1, y + 1)) != null)
                            {
                                Entity door = dungeon.manager.getDoor(new Vector2(x + 1, y + 1));
                                if (!(door.GetComponent("Openable") as Openable).Opened)
                                {
                                    (door.GetComponent("Openable") as Openable).Opened = true;
                                    door.DoAction("NextFrameOfAnimation");
                                }
                                else
                                {
                                    dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(1, 1)));
                                }
                            }
                            else
                            {
                                dungeon.manager.player.DoAction("ChangeDeltaPosition", new ChangePositionArgs(new Vector2(1, 1)));
                            }
                            keyboardElapsedTime = 200;
                            playerActed = true;
                        }

                        // message history
                        else if (keyboard.IsKeyDown(Keys.M))
                        {
                            GameScreenStateManager.CurrentState = ScreenStates.MessageHistory;
                            keyboardElapsedTime = 200;
                        }
                    }
                    break;

                case ScreenStates.MessageHistory:
                    if (keyboardElapsedTime <= 0)
                    {
                        if (keyboard.IsKeyDown(Keys.M) || keyboard.IsKeyDown(Keys.Escape))
                        {
                            GameScreenStateManager.CurrentState = ScreenStates.Play;
                            keyboardElapsedTime = 200;
                        }
                    }
                    break;
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
            spriteDict.Add("gui_background", Content.Load<Texture2D>("gui/ui_background"));
            spriteDict.Add("ui_bar", Content.Load<Texture2D>("gui/ui_bar"));
            spriteDict.Add("ui_barpiece_exp", Content.Load<Texture2D>("gui/ui_barpiece_exp"));
            spriteDict.Add("ui_barpiece_hp", Content.Load<Texture2D>("gui/ui_barpiece_hp"));
            spriteDict.Add("ui_barpiece_mp", Content.Load<Texture2D>("gui/ui_barpiece_mp"));
            spriteDict.Add("shrub", Content.Load<Texture2D>("env/shrub"));
            spriteDict.Add("item_skull", Content.Load<Texture2D>("items/item_skull"));
            spriteDict.Add("msg_history", Content.Load<Texture2D>("gui/msg_history_bg"));
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
            Vector3 playerV = new Vector3(((Position)dungeon.manager.player.GetComponent("Position")).X - 14.5f, ((Position)dungeon.manager.player.GetComponent("Position")).Y - 11f, 0);
            Vector3 viewV = new Vector3(scale * ((Animation)dungeon.manager.StairsUp.GetComponent("Animation")).FrameHeight, scale * ((Animation)dungeon.manager.StairsUp.GetComponent("Animation")).FrameHeight, 0);

            trans = Matrix.CreateTranslation(playerV * -viewV);

            return trans;
        }
    }
}