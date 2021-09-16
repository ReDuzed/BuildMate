using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.GameInput;
using ReLogic.Graphics;
using BuildMate.Items;

namespace BuildMate
{
    public class GUI : ModPlayer
    {
        public bool draw = false;
        public bool init = false;
        public Button selected;
        public static Button[] category;
        public static Button[] manager;
        public static string corner0, corner1;
        public string coordinates, tileType, wallType;
        public const int MaxIndex = 13, MaxIndex2 = 2;
        public void Init()
        {
            category = new Button[MaxIndex];
            manager = new Button[MaxIndex2];
            string[] text = new string[]
            {
                "Coordinates", "Tile Break", "Wall Break", "Tile Copy", "Wall Copy", "Tile & Wall Copy", "Place Liquid", "Drain Liquid", "Tile Selection Fill", "Tile Selection Remove", "Wall Selectin Fill", "Wall Selection Remove", "Tile & Wall Selection Remove"
            };
            for (int i = 0; i < MaxIndex; i++)
            {
                var box = new Button(text[i], Rectangle.Empty);
                category[i] = box;
            }
            string[] text2 = new string[] { "Tile", "Inventory" };
            for (int i = 0; i < MaxIndex2; i++)
            {
                var box = new Button(text2[i], Rectangle.Empty);
                manager[i] = box;
            }
        }
        public override void ModifyDrawInfo(ref PlayerDrawInfo drawInfo)
        {
            UIInteract();
        }
        private void UIInteract()
        {
            if (category == null || category?.Length == 0)
                return;
            foreach (Button button in category)
            {
                switch (button.text)
                {
                    case "Coordinates":
                        Items.Tile.AI[player.whoAmI] = 1;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Coordinates", "When used this shows the cursor coordinates");
                        break;
                    case "Tile Break":
                        Items.Tile.AI[player.whoAmI] = 2;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Break", "Use left click to break the tile at cursor");
                        break;
                    case "Wall Break":
                        Items.Tile.AI[player.whoAmI] = 3;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Break", "Use left click to break the wall at cursor");
                        break;
                    case "Tile Copy":
                        Items.Tile.AI[player.whoAmI] = 4;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Copy", "Right click to select tile at cursor, then left click to place that tile type");
                        break;
                    case "Wall Copy":
                        Items.Tile.AI[player.whoAmI] = 5;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Copy", "Right click to select wall at cursor, then left click to place that wall type");
                        break;
                    case "Place Liquid":
                        Items.Tile.AI[player.whoAmI] = 6;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Place Liquid", "Left click: water, Middle click: honey, Right click: lava");
                        break;
                    case "Drain Liquid":
                        Items.Tile.AI[player.whoAmI] = 7;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Drain Liquid", "Left click drains liquid at cursor");
                        break;
                    case "Tile Selection Fill":
                        Items.Tile.AI[player.whoAmI] = 8;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Selection Fill", "Left/right click places zone, Middle click selects which tile type is used to fill, and then Key: 'X' places tiles");
                        break;
                    case "Tile Selection Remove":
                        Items.Tile.AI[player.whoAmI] = 9;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Selection Remove", "Left/right click places zone, then Key: 'X' removes tiles");
                        break;
                    case "Wall Selection Fill":
                        Items.Tile.AI[player.whoAmI] = 10;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Selection Fill", "Left/right click places zone, Middle click selects which wall type is used to fill, and then Key: 'X' places walls");
                        break;
                    case "Wall Selection Remove":
                        Items.Tile.AI[player.whoAmI] = 11;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Selection Remove", "Left/right click places zone, then Key: 'X' removes walls");
                        break;
                }
            }
        }
        
    }

    public class Interact
    {
        public static bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        public static bool RightHold()
        {
            return Main.mouseRight;
        }
        public static bool KeyPress(Keys key)
        {
            return Main.oldKeyState.IsKeyUp(key) && Main.keyState.IsKeyDown(key);
        }
        public static bool KeyHold(Keys key)
        {
            return Main.keyState.IsKeyDown(key);
        }
        public static SpriteBatch sb => Main.spriteBatch;
    }

    public class TextBox
    {
        public bool active;
        public string text = "";
        public Color color
        {
            get { return active ? Color.DodgerBlue * 0.67f : Color.DodgerBlue * 0.33f; }
        }
        public Rectangle box;
        private KeyboardState oldState;
        private KeyboardState keyState
        {
            get { return Keyboard.GetState(); }
        }
        private SpriteBatch sb
        {
            get { return Main.spriteBatch; }
        }
        public TextBox()
        {
        }
        public TextBox(Rectangle box)
        {
            this.box = box;
        }
        
        public bool LeftClick()
        {
            return box.Contains(Main.MouseScreen.ToPoint()) && Interact.LeftClick();
        }
        public bool HoverOver()
        {
            return box.Contains(Main.MouseScreen.ToPoint());
        }
        public void UpdateInput()
        {
            if (active)
            {
                foreach (Keys key in keyState.GetPressedKeys())
                {
                    if (oldState.IsKeyUp(key))
                    {
                        if (key == Keys.F3)
                            return;
                        if (key == Keys.Back)
                        {
                            if (text.Length > 0)
                                text = text.Remove(text.Length - 1);
                            oldState = keyState;
                            return;
                        }
                        else if (key == Keys.Space)
                            text += " ";
                        else if (key == Keys.OemPeriod)
                            text += ".";
                        else if (text.Length < 24 && key != Keys.OemPeriod)
                        {
                            string n = key.ToString().ToLower();
                            if (n.StartsWith("d") && n.Length == 2)
                                n = n.Substring(1);
                            text += n;
                        }
                    }
                }
                oldState = keyState;
            }
        }
        public void DrawText()
        {
            sb.Draw(Main.magicPixel, box, color);
            sb.DrawString(Main.fontMouseText, text, new Vector2(box.X + 2, box.Y + 1), Color.White);
        }
    }
    public class Button
    {
        public string text = "";
        public Color color(bool select = true)
        {
            if (select)
                return box.Contains(Main.MouseScreen.ToPoint()) ? Color.DodgerBlue * 0.67f : Color.DodgerBlue * 0.33f;
            else
            {
                return box.Contains(Main.MouseScreen.ToPoint()) ? Color.White : Color.White * 0.67f;
            }
        }
        public Rectangle box;
        private SpriteBatch sb
        {
            get { return Main.spriteBatch; }
        }
        public Texture2D texture;
        public bool LeftClick()
        {
            return box.Contains(Main.MouseScreen.ToPoint()) && Interact.LeftClick();
        }
        public Button(string text, Rectangle box, Texture2D texture = null)
        {
            this.texture = texture;
            if (texture == null)
                this.texture = Main.magicPixel;
            this.text = text;
            this.box = box;
        }
        public void Draw(bool select = true)
        {
            sb.Draw(texture, box, color(select));
            sb.DrawString(Main.fontMouseText, text, new Vector2(box.X + 2, box.Y + 2), Color.White * 0.90f);
        }
    }
}
