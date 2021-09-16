using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildMate.Items
{
    public class Tile : ModItem
    {
        public static int[] AI = new int[255];
        public static int
            globalTime = 10620, switchTime = 1, ticks,
            inventory = 0, style = 0, tileManage = 0,
            proj = 0, projType = 0,
            tile = 0, tileType = 0,
            positionX1 = 1200, positionY1 = 1200, positionX2 = 1200, positionY2 = 1200;
        public static string name = "Tile: n/a", toolTip = "Tool used to handle walls.";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(name);
            Tooltip.SetDefault(toolTip);
        }
        public void ReassignDefaults(string name, string toolTip)
        {
            DisplayName.SetDefault(name);
            Tooltip.SetDefault(toolTip);
        }
        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 32;
        }
        public override bool UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (switchTime > 0)
                    switchTime--;
                Vector2 mousev = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);
                switch (AI[player.whoAmI])
                {
                    case 0:
                        goto default;
                    case 1:
                    // 	tileManage 1, Coordinates
                        if (Main.mouseLeft)
                        {
                            player.GetModPlayer<GUI>().coordinates = "Tile position: X " + Math.Round(mousev.X / 16) + " Y " + Math.Round(mousev.Y / 16);
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            Main.spawnTileX = (int)(Math.Round(mousev.X / 16));
                            Main.spawnTileY = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Spawn position set: X " + Math.Round(mousev.X / 16) + " Y " + Math.Round(mousev.Y / 16), 200, 150, 100);
                            //	if NetMessage necessary, it would be message 27		
                            projType = mod.ProjectileType("Spawn");
                            proj = Projectile.NewProjectile(mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 120;
                        }
                        break;
                    case 2:
                    //	tileManage 2, Tile Break
                        projType = mod.ProjectileType("TileBreak");
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && globalTime % 4 == 0)
                        {
                            proj = Projectile.NewProjectile(mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 10;
                        }
                        break;
                    case 3:
                    //	tileManage 3, Wall Break
                        projType = mod.ProjectileType("WallBreak");
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && globalTime % 4 == 0)
                        {
                            proj = Projectile.NewProjectile(mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 10;
                        }
                        break;
                    case 4:
                    //	tileManage 4, Tile Copy
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseRight && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null && Main.tile[Player.tileTargetX, Player.tileTargetY].active())
                        {
                            switchTime = 1;
                            tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].type;
                            player.GetModPlayer<GUI>().tileType = "Tile ID Selected: " + tileType;
                        }
                        if (Main.mouseLeft && globalTime % 6 == 0)
                        {
                            WorldGen.PlaceTile((int)mousev.X / 16, (int)mousev.Y / 16, tileType, false, true, -1, 0);

                            if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                        }
                        break;
                    case 5:
                    //	tileManage 5, Wall Copy
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseRight && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null)
                        {
                            switchTime = 1;
                            tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].wall;
                            player.GetModPlayer<GUI>().wallType = "Wall ID Selected: " + tileType;
                        }
                        if (Main.mouseLeft && globalTime % 6 == 0)
                        {
                            WorldGen.PlaceWall((int)mousev.X / 16, (int)mousev.Y / 16, tileType, false);
                            if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                        }
                        break;
                    case 6:
                    //	tileManage 6, Place Liquid
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && globalTime % 6 == 0)
                        {
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].liquid = 255;
                            WorldGen.SquareTileFrame((int)mousev.X / 16, (int)mousev.Y / 16, true);
                            if (Main.netMode != 0)
                            {
                                NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                                NetMessage.sendWater((int)mousev.X / 16, (int)mousev.Y / 16);
                            }
                        }
                        if (Main.mouseMiddle && globalTime % 6 == 0)
                        {
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].liquid = 255;
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].honey(true);
                            WorldGen.SquareTileFrame((int)mousev.X / 16, (int)mousev.Y / 16, true);
                            if (Main.netMode != 0)
                            {
                                NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                                NetMessage.sendWater((int)mousev.X / 16, (int)mousev.Y / 16);
                            }
                        }
                        if (Main.mouseRight && globalTime % 6 == 0)
                        {
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].liquid = 255;
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].lava(true);
                            WorldGen.SquareTileFrame((int)mousev.X / 16, (int)mousev.Y / 16, true);
                            if (Main.netMode != 0)
                            {
                                NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                                NetMessage.sendWater((int)mousev.X / 16, (int)mousev.Y / 16);
                            }
                        }
                        break;
                    case 7:
                    //	tileManage 7, Drain Liquid
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft)
                        {
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].liquid = 0;
                            WorldGen.SquareTileFrame((int)mousev.X / 16, (int)mousev.Y / 16, true);
                            if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                        }
                        break;
                    case 8:
                    //	tileManage 8, Tile Selection Fill
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Corner 1 Set at: X " + positionX1 + " Y " + positionY1, 250, 225, 175);
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Corner 2 Set at: X " + positionX2 + " Y " + positionY2, 250, 225, 175);
                        }
                        if (Main.mouseMiddle && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null)
                        {
                            switchTime = 1;
                            tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].type;
                            Main.NewText("Tile ID Selected: " + tileType, 200, 150, 100);
                        }
                    //  TODO CLICK "Fill All" button
                        //if (Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
                        //{
                        //    for (int i = positionX1; i < positionX2; i++)
                        //    {
                        //        for (int j = positionY1; j < positionY2; j++)
                        //        {
                        //            //!	default place tile option
                        //            // 	WorldGen.PlaceTile(i, j, tileType, true, true, -1, 0);
                        //            //!	force alternative
                        //            Main.tile[i, j].active(true);
                        //            Main.tile[i, j].type = (ushort)tileType;
                        //            WorldGen.SquareTileFrame(i, j);
                        //            if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                        //        }
                        //    }
                        //}
                    //  TODO CLICK "Fill Empty" button
                        if (Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
                        {
                            for (int i = positionX1; i < positionX2; i++)
                            {
                                for (int j = positionY1; j < positionY2; j++)
                                {
                                    WorldGen.PlaceTile(i, j, tileType, true, false, -1, 0);
                                    WorldGen.SquareTileFrame(i, j);
                                    if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                                }
                            }
                            positionX1 = 0;
                            positionX2 = 0;
                            positionY1 = 0;
                            positionY2 = 0;
                        }
                        break;
                    case 9:
                    //	tileManage 9, Tile Selection Remove
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Corner 1 Set at: X " + positionX1 + " Y " + positionY1, 250, 225, 175);
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Corner 2 Set at: X " + positionX2 + " Y " + positionY2, 250, 225, 175);
                        }
                    //  TODO CLICK "Clear tiles"
                        if (Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
                        {
                            for (int i = positionX1; i < positionX2; i++)
                            {
                                for (int j = positionY1; j < positionY2; j++)
                                {
                                    WorldGen.KillTile(i, j, false, false, true);
                                    WorldGen.SquareTileFrame(i, j);
                                    if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                                }
                            }
                            positionX1 = 0;
                            positionX2 = 0;
                            positionY1 = 0;
                            positionY2 = 0;
                        }
                        break;
                    case 10:
                    //	tileManage 10, Wall Selection Fill
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Top Left Corner Set at: X " + positionX1 + " Y " + positionY1, 250, 225, 175);
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2, 250, 225, 175);
                        }
                        if (Main.mouseMiddle && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null)
                        {
                            switchTime = 1;
                            tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].wall;
                            Main.NewText("Wall ID Selected: " + tileType, 200, 150, 100);
                        }
                        if (Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
                        {
                            for (int i = positionX1; i < positionX2; i++)
                            {
                                for (int j = positionY1; j < positionY2; j++)
                                {
                                    WorldGen.PlaceWall(i, j, tileType, false);
                                    if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                                }
                            }
                        }
                        break;
                    case 11:
                    //	tileManage 11, Wall Selection Remove
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Top Left Corner Set at: X " + positionX1 + " Y " + positionY1, 250, 225, 175);
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            Main.NewText("Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2, 250, 225, 175);
                        }
                        if (Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0)
                        {
                            for (int i = positionX1; i < positionX2; i++)
                            {
                                for (int j = positionY1; j < positionY2; j++)
                                {
                                    WorldGen.KillWall(i, j, false);
                                    WorldGen.SquareTileFrame(i, j);
                                    if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            return true;
        }
    }
}
