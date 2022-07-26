using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
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
        public static Button menu;
        public static Button[] category;
        public static Button[] manager;
        public static string corner0 = "", corner1 = "";
        public string coordinates = "", tileType = "", wallType = "";
        public const int 
            MaxIndex = 17, 
            MaxIndex2 = 2,
            CategoryIndex = 4;
        public static int pageIndex = 0;
        public static Item[] item;
        public static Icon[] icon;
        internal static TextBox saveBox, loadBox;
        public static Button[][] tab;
        public static Button 
            back, forward, page;
        public static Button
            acquireItem;
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (BuildMate.hotkey[0].JustPressed && Main.chatText.Length < 1)
                GlobalWorld.draw = !GlobalWorld.draw;
        }
        public void Init()
        {
            category = new Button[MaxIndex];
            manager = new Button[MaxIndex2];
            string[] text = new string[]
            {
                "Coordinates", "Tile Break", "Wall Break", "Tile Copy", "Wall Copy", "Tile Selection Copy", "Wall Selection Copy", "Tile & Wall Selection Copy", "Place Liquid", "Drain Liquid", "Tile Selection Fill", "Tile Selection Remove", "Wall Selection Fill", "Wall Selection Remove", "Tile & Wall Selection Remove", "Save Schematic", "Load Schematic"
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
            tab = new Button[CategoryIndex][]
            {
                new Button[] { category[0], category[1], category[2], category[3], category[4] },
                new Button[] { category[8], category[9] },
                new Button[] { category[5], category[6], category[10], category[11], category[12], category[13], category[14] },
                new Button[] { category[7], category[15], category[16] }
            };
            back = new Button("<-", Rectangle.Empty);
            forward = new Button("->", Rectangle.Empty);
            page = new Button((pageIndex + 1).ToString(), Rectangle.Empty);
            menu = new Button(" ", Rectangle.Empty);
            acquireItem = new Button("Click to acquire world tool", Rectangle.Empty);
            List<Item> list = new List<Item>();
            List<Icon> list2 = new List<Icon>();
            for (int i = 0; i < TextureAssets.Item.Length; i++)
            {
                int type = Item.NewItem(Item.GetSource_None(), Rectangle.Empty, i, 1, true, 0, false);
                Item item = Main.item[type];
                Item clone = item.Clone();
                list.Add(clone);
                list2.Add(new Icon()
                {
                    Name = item.Name,
                    createTile = item.createTile,
                    createWall = item.createWall,
                    texture = TextureAssets.Item[i].Value,
                    type = item.type
                });
                Main.item[type].TurnToAir();
            }
            item = list.ToArray();
            icon = list2.ToArray();
            saveBox = new TextBox();
            loadBox = new TextBox();
            list.Clear();
            list2.Clear();
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (!init)
            {
                Init();
                init = true;
            }
            if ((bool)back?.LeftClick() && pageIndex > 0)
                pageIndex--;
            if ((bool)forward?.LeftClick() && pageIndex < CategoryIndex - 1)
                pageIndex++;
            page.text = (pageIndex + 1).ToString();
            UIInteract();
        }
        private void UIInteract()
        {
            if (tab == null) return;
            if (pageIndex >= tab.Length || pageIndex < 0)
                pageIndex = 0;
            if (tab[pageIndex] == null || tab[pageIndex].Length == 0)
                return;
            foreach (Button button in tab[pageIndex])
            {
                if (!button.LeftClick()) continue;
                switch (button.text)
                {
                    case "Coordinates":
                        Items.Tile.AI[Player.whoAmI] = 1;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Coordinates", "When used this shows the cursor coordinates; Right Click to move default Map Spawn point");
                        break;
                    case "Tile Break":
                        Items.Tile.AI[Player.whoAmI] = 2;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Break", "Use left click to break the tile at cursor");
                        break;
                    case "Wall Break":
                        Items.Tile.AI[Player.whoAmI] = 3;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Break", "Use left click to break the wall at cursor");
                        break;
                    case "Tile Copy":
                        Items.Tile.AI[Player.whoAmI] = 4;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Copy", "Right click to select tile at cursor, then left click to place that tile type");
                        break;
                    case "Wall Copy":
                        Items.Tile.AI[Player.whoAmI] = 5;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Copy", "Right click to select wall at cursor, then left click to place that wall type");
                        break;
                    case "Place Liquid":
                        Items.Tile.AI[Player.whoAmI] = 6;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Place Liquid", "Left click: water, Middle click: honey, Right click: lava");
                        break;
                    case "Drain Liquid":
                        Items.Tile.AI[Player.whoAmI] = 7;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Drain Liquid", "Left click drains liquid at cursor");
                        break;
                    case "Tile Selection Fill":
                        Items.Tile.AI[Player.whoAmI] = 8;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Selection Fill", "Left/right click places zone, Middle click selects which tile type is used to fill, and then Key: " + BuildMate.hotkey[1].GetAssignedKeys()[0] + " places tiles");
                        break;
                    case "Tile Selection Remove":
                        Items.Tile.AI[Player.whoAmI] = 9;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Selection Remove", "Left/right click places zone, then Key: " + BuildMate.hotkey[1].GetAssignedKeys()[0] + " removes tiles");
                        break;
                    case "Wall Selection Fill":
                        Items.Tile.AI[Player.whoAmI] = 10;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Selection Fill", "Left/right click places zone, Middle click selects which wall type is used to fill, and then Key: " + BuildMate.hotkey[1].GetAssignedKeys()[0] + " places walls");
                        break;
                    case "Wall Selection Remove":
                        Items.Tile.AI[Player.whoAmI] = 11;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Selection Remove", "Left/right click places zone, then Key: " + BuildMate.hotkey[1].GetAssignedKeys()[0] + " removes walls");
                        break;
                    case "Tile Selection Copy":
                        Items.Tile.AI[Player.whoAmI] = 12;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile Selection Copy", "Left/right click places zone, Middle click copies into buffer, then Key: " + BuildMate.hotkey[1].GetAssignedKeys()[0] + " pastes tiles");
                        break;
                    case "Wall Selection Copy":
                        Items.Tile.AI[Player.whoAmI] = 13;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Wall Selection Copy", "Left/right click places zone, Middle click copies into buffer, then Key: " + BuildMate.hotkey[1].GetAssignedKeys()[0] + " pastes walls");
                        break;
                    case "Tile & Wall Selection Copy":
                        Items.Tile.AI[Player.whoAmI] = 14;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile & Wall Selection Copy", "Left/right click places zone, Middle click copies into buffer, then Key: " + BuildMate.hotkey[1].GetAssignedKeys()[0] + " pastes tiles and walls");
                        break;
                    case "Tile & Wall Selection Remove":
                        Items.Tile.AI[Player.whoAmI] = 15;
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Tile & Wall Selection Remove", "Left/right click places zone, then Key: " + BuildMate.hotkey[1].GetAssignedKeys()[0] + " removes tiles and walls");
                        break;
                    case "Save Schematic":
                        if (Items.Tile.copyBuffer != null && GUI.saveBox.text.Length > 0)
                        {
                            using (StreamWriter sw = new StreamWriter(Environment.GetEnvironmentVariable("USERPROFILE") + "\\" + GUI.saveBox.text + ".ini", false))
                            {
                                sw.WriteLine(Items.Tile.copyBuffer.GetLength(0) + " " + Items.Tile.copyBuffer.GetLength(1));
                                for (int j = 0; j < Items.Tile.copyBuffer.GetLength(1); j++)
                                {
                                    for (int i = 0; i < Items.Tile.copyBuffer.GetLength(0); i++)
                                    {
                                        var tile = Items.Tile.copyBuffer[i, j];
                                        if (tile != null)
                                        {                                                                                                     
                                            sw.WriteLine(string.Format("{0} {1} {2} {3} {4} {5},", string.Concat(i, ":", j), tile.TileType, tile.Slope, tile.WallType, tile.HasTile, tile.IsHalfBlock));
                                        }
                                    }
                                }
                            }
                            string info = string.Format("{0} schematic saved at: {1}", GUI.saveBox.text + ".ini", Environment.GetEnvironmentVariable("USERPROFILE"));
                            Main.NewText(info, 250, 225, 175);
                            GUI.saveBox.Clear(false);
                        }
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Save Schematic", "Use [Tile & Wall Selection Copy], copy a set of tiles and walls, type in a file name, then click this button");
                        break;
                    case "Load Schematic":
                        if (Items.Tile.copyBuffer == null && GUI.loadBox.text.Length > 0)
                        {
                            if (!File.Exists(Environment.GetEnvironmentVariable("USERPROFILE") + "\\" + GUI.loadBox.text + ".ini"))
                                return;
                            using (StreamReader sr = new StreamReader(Environment.GetEnvironmentVariable("USERPROFILE") + "\\" + GUI.loadBox.text + ".ini"))
                            {
                                string[] length = sr.ReadLine().TrimEnd(new[] { '\r', '\n' }).Split(' ');
                                int.TryParse(length[0], out int length0);
                                int.TryParse(length[1], out int length1);
                                Items.Tile.copyBuffer = new Terraria.Tile[length0, length1];
                                Items.Tile.modCopyBuffer = new TileCopy[length0, length1];
                                while (!sr.EndOfStream)
                                {
                                    string[] line = sr.ReadLine().TrimEnd(new[] { '\r', '\n' }).Split(' ');
                                    string[] array = line[0].Split(':');

                                    int.TryParse(array[0], out int i);
                                    int.TryParse(array[1], out int j);
                                    ushort.TryParse(line[1], out ushort type);
                                    Enum.TryParse(typeof(SlopeType), line[2], out object slope);
                                    ushort.TryParse(line[3], out ushort wall);
                                    bool.TryParse(line[4], out bool active);
                                    bool.TryParse(line[5], out bool halfBlock);

                                    var tile = new Terraria.Tile();
                                    tile.TileType = type;
                                    tile.Slope = (SlopeType)slope;
                                    tile.WallType = wall;
                                    tile.HasTile = active;
                                    tile.IsHalfBlock = halfBlock;
                                    Items.Tile.copyBuffer[i, j] = tile;
                                    Items.Tile.modCopyBuffer[i, j] = new TileCopy()
                                    {
                                        active = active,
                                        slope = (SlopeType)slope,
                                        halfBlock = halfBlock,
                                        type = type,
                                        wall = wall
                                    };
                                }
                            }
                            string info = string.Format("{0} schematic loaded", GUI.loadBox.text + ".ini");
                            Main.NewText(info, 250, 225, 175);
                            GUI.loadBox.Clear(false);
                        }
                        ModContent.GetInstance<Items.Tile>().ReassignDefaults("Load Schematic", "Type in a file name, then click this button; use [Tile & Wall Selection Copy] to place the schematic");
                        break;
                }
            }
        }
    }
    public class Element
    {
        public Element(Rectangle bounds)
        {
            this.bounds = bounds;
        }
        public bool active;
        public Rectangle bounds;
        public Color color = Color.White;
        public Texture2D texture
        {
            get { return TextureAssets.MagicPixel.Value; }
        }
        private SpriteBatch sb
        {
            get { return Main.spriteBatch; }
        }
        public bool HoverOver()
        {
            return bounds.Contains(Main.MouseScreen.ToPoint());
        }
        public bool LeftClick()
        {
            return HoverOver() && Interact.LeftClick();
        }
        public void Draw()
        {
            sb.Draw(texture, bounds, color);
        }
    }

    public class Icon
    {
        public Texture2D texture;
        public int type;
        public int createTile = -1;
        public int createWall = -1;
        public string Name;
    }   
    public class TileCopy
    {
        public int type;
        public SlopeType slope;
        public int wall;
        public bool active;
        public bool halfBlock;
    }
    public class Interact
    {
        public static bool LeftClick()
        {
            return Main.mouseLeftRelease && Main.mouseLeft;
        }
        public static bool LeftHold()
        {
            return Main.mouseLeft;
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
        public void Clear()
        {
            text = string.Empty;
        }
        public void Clear(bool active)
        {
            text = string.Empty;
            this.active = active;
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
                Main.mapEnabled = false;
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
                            if (n.Length > 2) 
                                continue;
                            text += n;
                        }
                    }
                }
                oldState = keyState;
            }
        }
        public void DrawText()
        {
            sb.Draw(TextureAssets.MagicPixel.Value, box, color);
            Utils.DrawBorderString(sb, text, new Vector2(box.X + 2, box.Y + 1), Color.White);
            //sb.DrawString(FontAssets.MouseText.Value, text, new Vector2(box.X + 2, box.Y + 1), Color.White);
        }
    }
    public class Button
    {
        public string text = "";
        public bool active = true;
        public Color color(bool select = true)
        {
            if (!active)
                return Color.White;
            if (select)
                return box.Contains(Main.MouseScreen.ToPoint()) ? Color.DodgerBlue * 0.67f : Color.DodgerBlue * 0.33f;
            else
            {
                return box.Contains(Main.MouseScreen.ToPoint()) ? Color.Gray * 0.5f : Color.Gray * 0.33f;
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
            return active && box.Contains(Main.MouseScreen.ToPoint()) && Interact.LeftClick();
        }
        public bool LeftHold()
        {
            return active && box.Contains(Main.MouseScreen.ToPoint()) && Interact.LeftHold();
        }
        public Button(string text, Rectangle box, Texture2D texture = null)
        {
            this.texture = texture;
            if (texture == null)
                this.texture = TextureAssets.MagicPixel.Value;
            this.text = text;
            this.box = box;
        }
        public void Draw(bool select = true)
        {
            if (!active)
                return;
            sb.Draw(texture, box, color(select));
            //Utils.DrawBorderString(sb, text, new Vector2(box.X + 2, box.Y + 2), Color.White * 0.90f);
            sb.DrawString(FontAssets.MouseText.Value, text, new Vector2(box.X + 2, box.Y + 2), Color.White * 0.90f);
        }
    }
}
