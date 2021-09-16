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

namespace BuildMate.Items
{
    class GlobalWorld : ModWorld
    {
        public override void PostUpdate()
        {
            if (Tile.globalTime > 0)
                Tile.globalTime--;
            else Tile.globalTime = 10620;
        }
        bool draw;
        public override void PostDrawTiles()
        {
            var player = Main.player[Main.myPlayer];
            draw = !player.controlInv;
            if (draw)
            {
                Interact.sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                TileSelect();
                DrawMenus();
                DrawText(player);
                Interact.sb.End();
            }
        }
        private void DrawText(Player player)
        {
            Vector2 measure = new DynamicSpriteFont(1f, 16, 'O').MeasureString(player.GetModPlayer<GUI>().coordinates);
            Interact.sb.DrawString(Main.fontMouseText, player.GetModPlayer<GUI>().coordinates, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y), Color.Gray);
            measure = new DynamicSpriteFont(1f, 16, 'O').MeasureString(player.GetModPlayer<GUI>().tileType);
            Interact.sb.DrawString(Main.fontMouseText, player.GetModPlayer<GUI>().tileType, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y * 3f), Color.Gray);
            measure = new DynamicSpriteFont(1f, 16, 'O').MeasureString(player.GetModPlayer<GUI>().wallType);
            Interact.sb.DrawString(Main.fontMouseText, player.GetModPlayer<GUI>().wallType, new Vector2(Main.screenWidth / 2 - measure.X / 2, Main.screenHeight - measure.Y * 2f), Color.Gray);
        }
        private void DrawMenus()
        {
            Vector2 measure = default;
            for (int i = 0; i < GUI.category?.Length; i++)
            {
                measure = new DynamicSpriteFont(1f, 16, 'O').MeasureString(GUI.category[i].text);
                GUI.category[i].box = new Rectangle(150, 80 + 40 * i, (int)measure.X, (int)measure.Y);
                GUI.category[i].Draw();
            }
        }
        private void TileSelect()
        {
            Vector2 region = Vector2.Zero;
            float[] value = new float[4];
            if (Items.Tile.positionX1 < Items.Tile.positionX2)
            {
                value[0] = Items.Tile.positionX1 * 16;
                value[2] = Items.Tile.positionX2 * 16;
            }
            else
            {
                value[0] = Items.Tile.positionX2 * 16;
                value[2] = Items.Tile.positionX1 * 16;
            }
            if (Items.Tile.positionY1 < Items.Tile.positionY2)
            {
                value[1] = Items.Tile.positionY1 * 16;
                value[3] = Items.Tile.positionY2 * 16;
            }
            else
            {
                value[1] = Items.Tile.positionY2 * 16;
                value[3] = Items.Tile.positionY1 * 16;
            }
            Interact.sb.Draw(Main.magicPixel, new Rectangle((int)value[0], (int)value[1], (int)(value[2] - Main.screenPosition.X), (int)(value[3] - Main.screenPosition.Y)), Color.Blue * 0.5f);
        }
    }
}
