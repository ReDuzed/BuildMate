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
using Terraria.Utilities;
using ReLogic.Graphics;
using BuildMate.Items;
using System.Linq;
using tUserInterface.Extension;

namespace BuildMate
{
    class GlobalWorld : ModSystem
    {
        public override void PostUpdateWorld()
        {
            if (Items.Tile.globalTime > 0)
                Items.Tile.globalTime--;
            else Items.Tile.globalTime = 10620;
        }
        internal static bool draw = false;
        internal static bool enabled = true;
        private Texture2D mouseItemTexture;
        private bool init;
        private List<string> name = new List<string>();
        private List<int> id = new List<int>();
        internal static int menuX = 125, menuY = 75;
        private int timer = 90;
        private Button bTimer;

        public override void OnWorldLoad()/* tModPorter Suggestion: Also override OnWorldUnload, and mirror your worldgen-sensitive data initialization in PreWorldGen */
        {
            //bTimer = new Button(timer.ToString(), Rectangle.Empty);
        }
        public override void PostDrawTiles()
        {
            var player = Main.LocalPlayer;
            Interact.sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            if (draw)
            {
                TileSelect();
                //Item spawn select menu
                //  DONE:  Requires update via Texture2D Asset values
                string text = "Type an item name into chat with this menu open to show the get-item display";
                Vector2 measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(text);
                Utils.DrawBorderString(Interact.sb, text, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y - 10), Color.Gray);
                //Interact.sb.DrawString(FontAssets.MouseText.Value, text, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y - 10), Color.Gray);
                DrawMenus();
                ItemSpawn();
                DrawText(player);
                DrawCursor();
            }
            else
            {
                //Menu hotkey info
                int count = BuildMate.hotkey[0].GetAssignedKeys().Count;
                string text = "Press Key: " + (count > 0 ? BuildMate.hotkey[0].GetAssignedKeys()[0] : "[unassigned]") + " to activate the menu";
                Vector2 measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(text);
                Utils.DrawBorderString(Interact.sb, text, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y - 10), Color.Gray);
                //Interact.sb.DrawString(FontAssets.MouseText.Value, text, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y - 10), Color.Gray);
            }
            Interact.sb.End();
            //  Changed from old style Center drag to tUserInterface
            GUI.menu.box = tUserInterface.Extension.Element.Drag(GUI.menu.box);
            menuX = GUI.menu.box.X;
            menuY = GUI.menu.box.Y;
            if (menuX < 0)
                menuX = 0;
            if (menuX > Main.screenWidth)
                menuX = Main.screenWidth - GUI.menu.box.Width;
            if (menuY < 0)
                menuY = 0;
            if (menuY > Main.screenHeight)
                menuY = Main.screenHeight - GUI.menu.box.Height;
            GUI.acquireItem.active = player.inventory.ToList().Count(t => t != null && t.type == ModContent.ItemType<Items.Tile>()) == 0;
            if (GUI.acquireItem.LeftClick())
            {
                int item = Item.NewItem(Item.GetSource_None(), player.Hitbox, ModContent.ItemType<Items.Tile>());
                if (Main.netMode != 0) NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item);
                GUI.acquireItem.active = false;
            }
        }
        private void DrawCursor()
        {
            if (Items.Tile.tileType >= 0)
            {
                Item item = null;                                                                            
                if (Items.Tile.tileItem) item = GUI.item.FirstOrDefault(t => t.createTile == Items.Tile.tileType);
                else                     item = GUI.item.FirstOrDefault(t => t.createWall == Items.Tile.tileType);
                if (item != null && item.Name != "")
                {
                    mouseItemTexture = TextureAssets.Item[item.type].Value;
                }
            }
            if (!Items.Tile.mouseTexture || mouseItemTexture == null) 
            {
                return;
            }
            Interact.sb.Draw(mouseItemTexture, Main.MouseScreen - new Vector2(mouseItemTexture.Width, 0), Color.White);
        }
        private void DrawText(Player player)
        {
            //Tool info
            var instance = ModContent.GetInstance<Items.Tile>();
            string text = instance.toolName + ": " + instance.toolInfo;
            Vector2 measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(text);
            if (text.Length > 3)
                Utils.DrawBorderString(Interact.sb, text, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y * 2f - 10), Color.Gray);
            //Coordinates
            measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(player.GetModPlayer<GUI>().coordinates);
            Utils.DrawBorderString(Interact.sb, player.GetModPlayer<GUI>().coordinates, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y * 3f - 10), Color.Gray);
            //Tile type
            measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(player.GetModPlayer<GUI>().tileType);
            Utils.DrawBorderString(Interact.sb, player.GetModPlayer<GUI>().tileType, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y * 5f - 10), Color.Gray);
            //Wall type
            measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(player.GetModPlayer<GUI>().wallType);
            Utils.DrawBorderString(Interact.sb, player.GetModPlayer<GUI>().wallType, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y * 4f - 10), Color.Gray);
            //Main.NewText replacement
            if (Items.Tile.mainNewText != null && Items.Tile.mainNewText.Length > 0)
            {
                measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(Items.Tile.mainNewText);
                Utils.DrawBorderString(Interact.sb, Items.Tile.mainNewText, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y * 6f - 10), Color.FromNonPremultiplied(250, 225, 175, 255));
            }
        }
        private void DrawMenus()
        {
            Vector2 measure = default;
            Button[] arrow = new Button[] { GUI.back, GUI.forward };
            int offset = 0;
            Rectangle origin = GUI.menu.box;
            GUI.menu.Draw(false);
            //bTimer.text = Math.Max(timer, 0).ToString();
            //bTimer.Draw(false);
            for (int i = 0; i < arrow.Length; i++)
            {
                measure = new DynamicSpriteFont(24f, 16, 'T').MeasureString(arrow[i].text);
                Rectangle navigation = new Rectangle(origin.X + offset, origin.Y, (int)measure.X, (int)(measure.Y * 1.5f));
                arrow[i].box = navigation;
                arrow[i].Draw();
                offset += (int)measure.X * 2 + 10;
            }
            GUI.page.box = new Rectangle(origin.X + (int)measure.X + 5, origin.Y, (int)measure.X, (int)(measure.Y * 1.5f));
            GUI.page.Draw();
            int width = 0, height = 0;
            for (int i = 0; i < GUI.tab?[GUI.pageIndex].Length; i++)
            {
                measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(GUI.tab?[GUI.pageIndex][i].text);
                GUI.tab[GUI.pageIndex][i].box = new Rectangle(origin.X, 25 + origin.Y + 25 * i, (int)measure.X, (int)(measure.Y * 1.5f));
                GUI.tab[GUI.pageIndex][i].Draw();
                RenderTextBox("Save Schematic", i, GUI.saveBox, GUI.loadBox);
                RenderTextBox("Load Schematic", i, GUI.loadBox, GUI.saveBox);
                width = Math.Max(width, GUI.tab[GUI.pageIndex][i].box.Width + GUI.loadBox.box.Width / 2);
                height = Math.Max(height, 50 + 25 * i);
            }
            //bTimer.box = new Rectangle(menuX + width, menuY + height, 25, 25);
            GUI.menu.box = new Rectangle(menuX, menuY, width + 25, height + 25);
            measure = new DynamicSpriteFont(10f, 16, 'T').MeasureString(GUI.acquireItem.text);
            GUI.acquireItem.box = new Rectangle(Main.screenWidth / 2 - (int)measure.X / 2, Main.screenHeight / 2 + 50, (int)measure.X, (int)(measure.Y * 1.5f));
            GUI.acquireItem.Draw();
        }
        private void RenderTextBox(string text, int index, TextBox box, TextBox other)
        {
            if (GUI.tab[GUI.pageIndex][index].text == text)
            {
                var menu = GUI.tab[GUI.pageIndex][index];
                box.box = new Rectangle(menu.box.X + menu.box.Width + 4, menu.box.Y, 180, menu.box.Height);
                if (box.LeftClick())
                {
                    box.active = !box.active;
                    if (other.active)
                        other.active = false;
                }
                box.UpdateInput();
                box.DrawText();
            }
        }
        private void TileSelect()
        {
            Vector2 mousev = new Vector2(Player.tileTargetX, Player.tileTargetY);
            Vector2[] value = new Vector2[2];
            Color color = Color.Firebrick;
            if (Items.Tile.copyBuffer != null && Items.Tile.copyBuffer.Length > 0)
            {
                value[0] = mousev;
                value[1] = new Vector2(
                    mousev.X + Items.Tile.copyBuffer.GetLength(0), 
                    mousev.Y + Items.Tile.copyBuffer.GetLength(1));
            }
            else
            {
                color = Color.Blue;
                if (Items.Tile.positionX1 < Items.Tile.positionX2)
                {
                    value[0].X = Items.Tile.positionX1;
                    value[1].X = Items.Tile.positionX2;
                }
                else
                {
                    value[1].X = Items.Tile.positionX2;
                    value[0].X = Items.Tile.positionX1;
                }
                if (Items.Tile.positionY1 < Items.Tile.positionY2)
                {
                    value[0].Y = Items.Tile.positionY1;
                    value[1].Y = Items.Tile.positionY2;
                }
                else
                {
                    value[1].Y = Items.Tile.positionY2;
                    value[0].Y = Items.Tile.positionY1;
                }
            }
            value[0] = Terraria.Utils.ToWorldCoordinates(value[0], 0, 0) - Main.screenPosition;
            value[1] = Terraria.Utils.ToWorldCoordinates(value[1], 0, 0) - Main.screenPosition;
            if (Math.Max(value[0].X, 0f) != 0f && Math.Max(value[0].Y, 0f) != 0f &&
                Math.Max(value[1].X, 0f) != 0f && Math.Max(value[1].Y, 0f) != 0f)
                Interact.sb.Draw(TextureAssets.MagicPixel.Value, new Rectangle((int)value[0].X, (int)value[0].Y, (int)(value[1].X - value[0].X), (int)(value[1].Y - value[0].Y)), color * 0.5f);
        }
        private void ItemSpawn()
        {
            Func<string, Icon> search = delegate (string name)
            {
                if (name.Length > 2 && name != null)
                {
                    for (int i = 0; i < GUI.icon.Length; i++)
                    {
                        if (GUI.icon[i].Name.ToLower().Contains(name.ToLower()))
                        {
                            return GUI.icon[i];
                        }
                    }
                }
                return default(Icon);
            };
            if (Main.chatText != null && Main.chatText.Length > 2)
            {
                Icon item = search(Main.chatText);
                if (item != default)
                {
                    int index = GUI.icon.ToList().IndexOf(item);
                    int x = Main.screenWidth / 2 - 100;
                    int y = Main.screenHeight / 2 + 42;
                    if (TextureAssets.Item[item.type].Value.Name.StartsWith("Asynchronously"))
                    {
                        int t = Item.NewItem(Item.GetSource_None(), Vector2.Zero, item.type);
                        if (Main.netMode != 0)
                            NetMessage.SendData(MessageID.SyncItem, -1, -1, null, t);
                    }
                    Interact.sb.Draw(TextureAssets.Item[item.type].Value, new Vector2(x, y), Color.White);
                    Utils.DrawBorderString(Interact.sb, string.Format("{0} {1}", item.Name, item.type), new Vector2(x + 50, y + 4), Color.White);
                    //Interact.sb.DrawString(FontAssets.MouseText.Value, string.Format("{0} {1}", item.Name, item.type), new Vector2(x + 50, y + 4), Color.White);

                    Rectangle grab = new Rectangle(x, y, 48, 48);
                    if (grab.Contains(Main.MouseScreen.ToPoint()))
                    {
                        Utils.DrawBorderString(Interact.sb, "Left/Right click", new Vector2(x, y + 50), Color.White);
                        //Interact.sb.DrawString(FontAssets.MouseText.Value, "Left/Right click", new Vector2(x, y + 50), Color.White);
                        if (Interact.LeftClick() || Interact.RightHold())
                        {
                            //  QuickSpawnItem handles networking
                            Main.LocalPlayer.QuickSpawnItem(Item.GetSource_None(), item.type);
                            //int t = Item.NewItem(Item.GetSource_None(), Main.player[Main.myPlayer].Center, item.type);
                            //if (Main.netMode != 0)
                            //    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, t);
                        }
                    }
                }
            }
        }
    }
}
