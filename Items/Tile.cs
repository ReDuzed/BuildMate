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
        public const string name = "Ancient World Tool", toolTip = "Tool used to handle tiles and walls.";
        public static bool
            mouseTexture = false,
            tileItem = false;
        public static Terraria.Tile[,] copyBuffer;
        public static TileCopy[,] modCopyBuffer;
        public string
            toolInfo, toolName;
        public static string
            mainNewText;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault(name);
            Tooltip.SetDefault(toolTip);
        }
        public void ReassignDefaults(string name, string toolTip)
        {
            toolName = name;
            toolInfo = toolTip;
        }
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.holdStyle = ItemHoldStyleID.HoldUp;
        }
        public override void HoldItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                if (switchTime > 0)
                    switchTime--;
                if (AI[player.whoAmI] < 8)
                {
                    positionX1 = 0;
                    positionX2 = 0;
                    positionY1 = 0;
                    positionY2 = 0;
                }
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
                            mainNewText = "Spawn position set: X " + Math.Round(mousev.X / 16) + " Y " + Math.Round(mousev.Y / 16);
                            //	if NetMessage necessary, it would be message 27		
                            projType = Mod.Find<ModProjectile>("Spawn").Type;
                            proj = Projectile.NewProjectile(Projectile.GetSource_None(), mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 120;
                        }
                        mouseTexture = false;
                        break;
                    case 2:
                    //	tileManage 2, Tile Break
                        projType = Mod.Find<ModProjectile>("TileBreak").Type;
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && globalTime % 4 == 0)
                        {
                            proj = Projectile.NewProjectile(Projectile.GetSource_None(), mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 10;
                        }
                        mouseTexture = false;
                        break;
                    case 3:
                    //	tileManage 3, Wall Break
                        projType = Mod.Find<ModProjectile>("WallBreak").Type;
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && globalTime % 4 == 0)
                        {
                            proj = Projectile.NewProjectile(Projectile.GetSource_None(), mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
                            Main.projectile[proj].timeLeft = 10;
                        }
                        mouseTexture = false;
                        break;
                    case 4:
                    //	tileManage 4, Tile Copy
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseRight && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null && Main.tile[Player.tileTargetX, Player.tileTargetY].HasTile)
                        {
                            switchTime = 1;
                            tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].TileType;
                            player.GetModPlayer<GUI>().tileType = "Tile ID Selected: " + tileType;
                            mouseTexture = true;
                            tileItem = true;
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
                            tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].WallType;
                            player.GetModPlayer<GUI>().wallType = "Wall ID Selected: " + tileType;
                            mouseTexture = true;
                            tileItem = false;
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
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].LiquidAmount = 255;
                            WorldGen.SquareTileFrame((int)mousev.X / 16, (int)mousev.Y / 16, true);
                            if (Main.netMode != 0)
                            {
                                NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                                NetMessage.sendWater((int)mousev.X / 16, (int)mousev.Y / 16);
                            }
                        }
                        if (Main.mouseMiddle && globalTime % 6 == 0)
                        {
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].LiquidAmount = 255;
                            var tile = Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16];
                            tile.LiquidType = LiquidID.Honey;
                            WorldGen.SquareTileFrame((int)mousev.X / 16, (int)mousev.Y / 16, true);
                            if (Main.netMode != 0)
                            {
                                NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                                NetMessage.sendWater((int)mousev.X / 16, (int)mousev.Y / 16);
                            }
                        }
                        if (Main.mouseRight && globalTime % 6 == 0)
                        {
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].LiquidAmount = 255;
                            var tile = Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16];
                            tile.LiquidType = LiquidID.Lava;
                            WorldGen.SquareTileFrame((int)mousev.X / 16, (int)mousev.Y / 16, true);
                            if (Main.netMode != 0)
                            {
                                NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                                NetMessage.sendWater((int)mousev.X / 16, (int)mousev.Y / 16);
                            }
                        }
                        mouseTexture = false;
                        break;
                    case 7:
                    //	tileManage 7, Drain Liquid
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft)
                        {
                            Main.tile[(int)mousev.X / 16, (int)mousev.Y / 16].LiquidAmount = 0;
                            WorldGen.SquareTileFrame((int)mousev.X / 16, (int)mousev.Y / 16, true);
                            if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X / 16) - 1, (int)(mousev.Y / 16) - 1, 3);
                        }
                        mouseTexture = true;
                        break;
                    case 8:
                    //	tileManage 8, Tile Selection Fill
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Corner 1 Set at: X " + positionX1 + " Y " + positionY1;
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Corner 2 Set at: X " + positionX2 + " Y " + positionY2;
                        }
                        if (Main.mouseMiddle && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null)
                        {
                            switchTime = 1;
                            tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].TileType;
                            mainNewText = "Tile ID Selected: " + tileType;
                            mouseTexture = true;
                            tileItem = true;
                        }
                    //  TODO CLICK "Fill All" button
                        //if (BuildMate.hotkey[1].JustPressed)
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
                        if (BuildMate.hotkey[1].JustPressed)
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
                            mainNewText = "Corner 1 Set at: X " + positionX1 + " Y " + positionY1;
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Corner 2 Set at: X " + positionX2 + " Y " + positionY2;
                        }
                    //  TODO CLICK "Clear tiles"
                        if (BuildMate.hotkey[1].JustPressed)
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
                        mouseTexture = false;
                        break;
                    case 10:
                    //	tileManage 10, Wall Selection Fill
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Top Left Corner Set at: X " + positionX1 + " Y " + positionY1;
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2;
                        }
                        if (Main.mouseMiddle && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null)
                        {
                            switchTime = 1;
                            tileType = Main.tile[Player.tileTargetX, Player.tileTargetY].WallType;
                            mainNewText = "Wall ID Selected: " + tileType;
                            mouseTexture = true;
                            tileItem = false;
                        }
                        if (BuildMate.hotkey[1].JustPressed)
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
                            mainNewText = "Top Left Corner Set at: X " + positionX1 + " Y " + positionY1;
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2;
                        }
                        if (BuildMate.hotkey[1].JustPressed)
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
                        mouseTexture = false;
                        break;
                    case 12:
                    //  tileManage 12, Tile Selection Copy
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Top Left Corner Set at: X " + positionX1 + " Y " + positionY1;
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2;
                        }
                        if (Main.mouseMiddle && switchTime <= 0)
                        {
                            if (positionX1 == 0 || positionX2 == 0 || positionY1 == 0 || positionY2 == 0)
                                break;
                            int x = Math.Min(positionX1, positionX2);
                            int y = Math.Min(positionY1, positionY2);
                            int width = Math.Abs(positionX2 - positionX1);
                            int height = Math.Abs(positionY2 - positionY1);
                            copyBuffer = new Terraria.Tile[width, height];
                            for (int i = x; i < x + width; i++)
                            {
                                for (int j = y; j < y + height; j++)
                                {
                                    if (Main.tile[i, j].HasTile)
                                    {
                                        copyBuffer[i - x, j - y] = Main.tile[i, j];
                                    }
                                }
                            }
                            mainNewText = string.Format("{0} tiles copied!", width * height);
                            positionX1 = 0;
                            positionX2 = 0;
                            positionY1 = 0;
                            positionY2 = 0;
                        }
                        if (BuildMate.hotkey[1].JustPressed)
                        {
                            if (copyBuffer != null && copyBuffer.Length > 0)
                            {
                                mousev = new Vector2(Player.tileTargetX, Player.tileTargetY);
                                for (int i = copyBuffer.GetLength(0) - 1; i >= 0 ; i--)
                                {
                                    for (int j = copyBuffer.GetLength(1) - 1; j >= 0 ; j--)
                                    {
                                        if (copyBuffer[i, j] != null)
                                        {
                                            WorldGen.PlaceTile((int)mousev.X + i, (int)mousev.Y + j, copyBuffer[i, j].TileType, true, true);
                                            if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                                        }
                                    }
                                }
                                copyBuffer = null;
                            }
                        }
                        mouseTexture = false;
                        break;
                    case 13: 
                    //  tileManage 13, Wall Selection Copy
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Top Left Corner Set at: X " + positionX1 + " Y " + positionY1;
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2;
                        }
                        if (Main.mouseMiddle && switchTime <= 0)
                        {
                            if (positionX1 == 0 || positionX2 == 0 || positionY1 == 0 || positionY2 == 0)
                                break;
                            int x = Math.Min(positionX1, positionX2);
                            int y = Math.Min(positionY1, positionY2);
                            int width = Math.Abs(positionX2 - positionX1);
                            int height = Math.Abs(positionY2 - positionY1);
                            copyBuffer = new Terraria.Tile[width, height];
                            for (int i = x; i < x + width; i++)
                            {
                                for (int j = y; j < y + height; j++)
                                {
                                    if (Main.tile[i, j].HasTile)
                                    {
                                        copyBuffer[i - x, j - y] = Main.tile[i, j];
                                    }
                                }
                            }
                            mainNewText = string.Format("{0} walls copied!", width * height);
                            positionX1 = 0;
                            positionX2 = 0;
                            positionY1 = 0;
                            positionY2 = 0;
                        }
                        if (BuildMate.hotkey[1].JustPressed)
                        {
                            if (copyBuffer != null && copyBuffer.Length > 0)
                            {
                                mousev = new Vector2(Player.tileTargetX, Player.tileTargetY);
                                for (int i = copyBuffer.GetLength(0) - 1; i >= 0; i--)
                                {
                                    for (int j = copyBuffer.GetLength(1) - 1; j >= 0; j--)
                                    {
                                        if (copyBuffer[i, j] != null)
                                        {
                                            WorldGen.PlaceWall((int)mousev.X + i, (int)mousev.Y + j, copyBuffer[i, j].WallType, true);
                                            if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                                        }
                                    }
                                }
                                copyBuffer = null;
                            }
                        }
                        mouseTexture = false;
                        break;
                    case 14:
                        if (Main.chatText.Length > 0 || GUI.saveBox.text.Length > 0)
                            return;
                    //  tileManage 14, Tile & Wall Selection Copy
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Top Left Corner Set at: X " + positionX1 + " Y " + positionY1;
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2;
                        }
                        if (Main.mouseMiddle && switchTime <= 0)
                        {
                            if (positionX1 == 0 || positionX2 == 0 || positionY1 == 0 || positionY2 == 0)
                                break;
                            int x = Math.Min(positionX1, positionX2);
                            int y = Math.Min(positionY1, positionY2);
                            int width = Math.Abs(positionX2 - positionX1);
                            int height = Math.Abs(positionY2 - positionY1);
                            copyBuffer = new Terraria.Tile[width, height];
                            for (int i = x; i < x + width; i++)
                            {
                                for (int j = y; j < y + height; j++)
                                {
                                    copyBuffer[i - x, j - y] = Main.tile[i, j];
                                    copyBuffer[i - x, j - y].HasTile = Main.tile[i, j].HasTile;
                                    copyBuffer[i - x, j - y].Slope = Main.tile[i, j].Slope;
                                    copyBuffer[i - x, j - y].IsHalfBlock = Main.tile[i, j].IsHalfBlock;
                                }
                            }
                            mainNewText = string.Format("{0} walls copied!", width * height);
                            positionX1 = 0;
                            positionX2 = 0;
                            positionY1 = 0;
                            positionY2 = 0;
                        }
                        if (BuildMate.hotkey[1].JustPressed)
                        {
                            if (copyBuffer != null && copyBuffer.Length > 0)
                            {
                                mousev = new Vector2(Player.tileTargetX, Player.tileTargetY);
                                for (int i = copyBuffer.GetLength(0) - 1; i >= 0; i--)
                                {
                                    for (int j = copyBuffer.GetLength(1) - 1; j >= 0; j--)
                                    {
                                        if (modCopyBuffer != null && modCopyBuffer.GetLength(0) == copyBuffer.GetLength(0) && modCopyBuffer.GetLength(1) == copyBuffer.GetLength(1) && modCopyBuffer[i, j] != null && !modCopyBuffer[i, j].active)
                                            continue;
                                        if (modCopyBuffer == null)
                                        {
                                            if (copyBuffer[i, j].HasTile)
                                            {
                                                WorldGen.PlaceWall((int)mousev.X + i, (int)mousev.Y + j, copyBuffer[i, j].WallType, true);
                                                WorldGen.PlaceTile((int)mousev.X + i, (int)mousev.Y + j, copyBuffer[i, j].TileType, true, true);
                                                WorldGen.SlopeTile((int)mousev.X + i, (int)mousev.Y + j, (int)copyBuffer[i, j].Slope, true);
                                                var tile = Main.tile[(int)mousev.X + i, (int)mousev.Y + j];
                                                tile.IsHalfBlock = copyBuffer[i, j].IsHalfBlock;
                                            }
                                        }
                                        else
                                        {
                                            WorldGen.PlaceWall((int)mousev.X + i, (int)mousev.Y + j, modCopyBuffer[i, j].wall, true);
                                            WorldGen.PlaceTile((int)mousev.X + i, (int)mousev.Y + j, modCopyBuffer[i, j].type, true, true);
                                            WorldGen.SlopeTile((int)mousev.X + i, (int)mousev.Y + j, (int)modCopyBuffer[i, j].slope, true);
                                            var a = new Terraria.WorldBuilding.Actions.SetHalfTile(modCopyBuffer[i, j].halfBlock);
                                            a.Apply(new Point((int)mousev.X + i, (int)mousev.Y + j), (int)mousev.X + i, (int)mousev.Y + j, null);
                                            //var tile = Main.tile[(int)mousev.X + i, (int)mousev.Y + j];
                                            //tile.IsHalfBlock = modCopyBuffer[i, j].halfBlock;
                                        }
                                        WorldGen.SquareTileFrame(i, j);
                                        if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, (int)mousev.X + i - 1, (int)mousev.Y + j - 1, 3);
                                    }
                                }
                                modCopyBuffer = null;
                                copyBuffer = null;
                            }
                        }
                        mouseTexture = false;
                        break;
                    case 15:
                    //	tileManage 15, Tile & Wall Selection Remove
                        Lighting.AddLight((int)(mousev.X / 16), (int)(mousev.Y / 16), 1f, 1f, 1f);
                        if (Main.mouseLeft && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX1 = (int)(Math.Round(mousev.X / 16));
                            positionY1 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Top Left Corner Set at: X " + positionX1 + " Y " + positionY1;
                        }
                        if (Main.mouseRight && switchTime <= 0)
                        {
                            switchTime = 1;
                            positionX2 = (int)(Math.Round(mousev.X / 16));
                            positionY2 = (int)(Math.Round(mousev.Y / 16));
                            mainNewText = "Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2;
                        }
                        if (BuildMate.hotkey[1].JustPressed)
                        {
                            for (int i = positionX1; i < positionX2; i++)
                            {
                                for (int j = positionY1; j < positionY2; j++)
                                {
                                    WorldGen.KillTile(i, j, false, false, true);
                                    WorldGen.KillWall(i, j, false);
                                    WorldGen.SquareTileFrame(i, j);
                                    if (Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i - 1, j - 1, 3);
                                }
                            }
                        }
                        mouseTexture = false;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
