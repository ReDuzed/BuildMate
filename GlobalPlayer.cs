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

namespace BuildMate
{
	public class GlobalPlayer : ModPlayer
	{
		public static bool init;
		public static int
			globalTime = 10620, switchTime = 90, ticks,
			inventory = 0, style = 0, tileManage = 0,
			proj = 0, projType = 0,
			tile = 0, tileType = 0,
			positionX1 = 1200, positionY1 = 1200, positionX2 = 1200, positionY2 = 1200;
		public static bool 
			enabled = false, 
			flip = false, flipInv = false, overwrite = false;
		public static string[] 
			INV = new string[] { "Tier 1 Armor" , "Tier 2 Armor" , "Tier 3 Armor" , "Throwables & Explosives" , "Flails & Spears" , "Bows & Guns" , "Magic Weapons" , "Swords" , "Phase Weapons" , "Tools" , "Movement Accessories" , "Combat Accessories" , "Miscellaneous Accessories" , "Christmas" , "Miscellaneous" , "Vanity" , "Consumables, Potions" , "Materials: Part 1, Bars" , "Materials: Part 2, Souls" , "Mechanical, Wire" , "Ammo" , "Alchemy, Plants" , "Statues: Useful" , "Statues: Useless" , "Crafting Workshops" , "Decorations" , "Music Boxes" , "Lighting, Torches" , "Ore & Gems" , "Wall"  , "Soil & Blocks, Seeds" , "Bricks" },
			TILE = new string[] { "Coordinates" , "Tile Break" , "Wall Break" , "Tile Copy" , "Wall Copy" , "Place Liquid" , "Drain Liquid" , "Tile Selection Fill" , "Tile Selection Remove" , "Wall Selection Fill" , "Wall Selection Remove" };
		public override void Initialize()
		{
		//	necessary for multiplayer
			if(Main.netMode == 0) {
			enabled 	= false;
			flip 		= false;
			flipInv		= false;
		
			style 		= 0;
			inventory	= 0;
			tileManage	= 0;
			
			proj		= 0;
			projType	= 0;
			
			tile		= 0;
			tileType	= 0;
		//	make sure default values are in a map's range index	
			positionX1	= 1200;
			positionY1	= 1200;
			positionX2	= 1200;
			positionY2	= 1200;
		//	RESET movement variables
			Player.defaultGravity 	= 0.4f;
			player.ignoreWater		= false;
			player.merman			= false;
			player.hideMerman		= false;
			}
		}
		public override void PreUpdate()
		{
		//	usage for toggling events such as key presses
									globalTime++;
			if(switchTime > 0)		switchTime--;
			if(globalTime > 10800){	globalTime = 0;
				//	controls tip
					Main.NewText("WARNING: Overwrites all of inventory", 250, 75, 50);
					Main.NewText("| Controls: 'I' = on, 'O' = off, 'P' = enable overwriting inventory", 130, 200, 120);
					Main.NewText("| Use Arrows Keys", 130, 200, 120);
					Main.NewText("| Item/Wall Fill Features use...", 130, 200, 120);
					Main.NewText("| Left, Middle, and Right mouse buttons and...", 130, 200, 120);
					Main.NewText("| 'X' to activate the fill command!", 130, 200, 120); }
			if(Main.netMode != 0 && globalTime == 0) Main.NewText("Flying Disabled, Multiplayer Detected", 200, 125, 75);
		//	turn on/off build features
			if (!Main.drawingPlayerChat) {
				if(!enabled && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.I) < 0){
					enabled 		= true;
					Main.NewText("Building Tools Enabled", 130, 200, 120);
				}
				if(enabled && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.O) < 0){
					enabled 		= false;
					Main.NewText("Building Tools Disabled", 200, 160, 120);
				}
				if (Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.P) < 0 && switchTime == 0){
					overwrite 		= !overwrite;
					Main.NewText("Inventory overwriting " + (overwrite ? "enabled" : "disabled"), overwrite ? new Color(130, 200, 120) : new Color(200, 160, 120));
					switchTime = 60;
				}
			}
			if(enabled){
			#region movement
			//	movement variables
				Player.defaultGravity 	= Main.netMode != 0 ? Player.defaultGravity : 0f;
				player.ignoreWater		= true;
				player.merman			= true;
				player.hideMerman		= true;
			//	mod controls - WASD
			
			//  example netcode
			//	might be message 13 instead of 65
			//	if(Main.netMode == 1)	NetMessage.SendData(13, -1, -1, null, 0, player.whoAmI, 0f, 0f, 1, 0, 0); 
			
			//	flying only if in singleplayer
				if(player.controlUp && Main.netMode == 0) player.position.Y -= 5; 
				if(player.controlLeft && Main.netMode == 0) player.position.X -= 5;
				if(player.controlDown && Main.netMode == 0) player.position.Y += 5;
				if(player.controlRight && Main.netMode == 0) player.position.X += 5;
			
			//	negate neutral movement
				if(!player.controlUp && !player.controlDown && Main.netMode == 0) player.velocity.Y = 0;
				if(!player.controlLeft && !player.controlRight && Main.netMode == 0) player.velocity.X = 0;
			#endregion
			
			//	using flip bool to keep resets clean
				flip 	= false;
				if(!flipInv && overwrite){
					//	RESET inventory before setting default
						for(int i = 5; i < player.inventory.Length-10; i++) player.inventory[i].SetDefaults(0);
					
						player.inventory[0].SetDefaults(ItemID.CopperPickaxe);
						player.inventory[1].SetDefaults(ItemID.CopperAxe);
						player.inventory[2].SetDefaults(ItemID.CopperHammer);
						player.inventory[3].SetDefaults(ItemID.BluePhaseblade);
						player.inventory[4].SetDefaults(ItemID.IvyWhip);
						player.armor[0].SetDefaults(ItemID.Sunglasses);
						player.armor[4].SetDefaults(ItemID.CloudinaBalloon);
						player.armor[5].SetDefaults(ItemID.SpectreBoots);
						player.armor[6].SetDefaults(ItemID.ObsidianHorseshoe);
						flipInv = true;
				}
			
			//	enumerating styles via arrow keys
				if(switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Up) < 0){
					if(style == 0){
						switchTime = 60;
						Main.NewText("Tile Manager", 250, 225, 175);
						style = 1;
					}
				}
				else if(switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Down) < 0){
					if(style == 1){
						switchTime = 60;
						Main.NewText("Inventory Manager", 250, 225, 175);
						style = 0;	
					}
				}
				
				if(style == 0 && overwrite){
			#region inventory
				//	using depreciation to return to previous inventory values
					if(inventory >= 0 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Left) < 0){
							switchTime = 45;
							if(inventory > 0) inventory	-= 1;
							Main.NewText("Inventory management rollback: " + INV[inventory], 200, 150, 100);
							if(inventory == 0) inventory -= 1;
					//	using array length to loop to top
						if(inventory < 0){
							inventory = INV.Length-1;
							Main.NewText("Inventory management looped: " + INV[inventory], 200, 150, 100);
						}
					}	
				//	using incrementing values to determine inventory
					if(inventory == 0 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 1;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region	teir 1 armor
				//	RESET inventory before refill		
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.CopperHelmet);
				player.inventory[12].SetDefaults(ItemID.IronHelmet);
				player.inventory[13].SetDefaults(ItemID.SilverHelmet);
				player.inventory[14].SetDefaults(ItemID.GoldHelmet);
				player.inventory[15].SetDefaults(ItemID.MiningHelmet);
				player.inventory[21].SetDefaults(ItemID.CopperChainmail);
				player.inventory[22].SetDefaults(ItemID.IronChainmail);
				player.inventory[23].SetDefaults(ItemID.SilverChainmail);
				player.inventory[24].SetDefaults(ItemID.GoldChainmail);
				player.inventory[25].SetDefaults(ItemID.MiningShirt);
				player.inventory[31].SetDefaults(ItemID.CopperGreaves);
				player.inventory[32].SetDefaults(ItemID.IronGreaves);
				player.inventory[33].SetDefaults(ItemID.SilverGreaves);
				player.inventory[34].SetDefaults(ItemID.GoldGreaves);
				player.inventory[35].SetDefaults(ItemID.MiningPants);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 1 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 2;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region tier 2 armor
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.WoodenHammer);
				player.inventory[11].SetDefaults(ItemID.MeteorHelmet);
				player.inventory[12].SetDefaults(ItemID.ShadowHelmet);
				player.inventory[13].SetDefaults(ItemID.NecroHelmet);
				player.inventory[14].SetDefaults(ItemID.JungleHat);
				player.inventory[15].SetDefaults(ItemID.MoltenHelmet);
				player.inventory[21].SetDefaults(ItemID.MeteorSuit);
				player.inventory[22].SetDefaults(ItemID.ShadowScalemail);
				player.inventory[23].SetDefaults(ItemID.NecroBreastplate);
				player.inventory[24].SetDefaults(ItemID.JungleShirt);
				player.inventory[25].SetDefaults(ItemID.MoltenBreastplate);
				player.inventory[31].SetDefaults(ItemID.MeteorLeggings);
				player.inventory[32].SetDefaults(ItemID.ShadowGreaves);
				player.inventory[33].SetDefaults(ItemID.NecroGreaves);
				player.inventory[34].SetDefaults(ItemID.JunglePants);
				player.inventory[35].SetDefaults(ItemID.MoltenGreaves);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 2 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 3;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region tier 3 armor
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.CobaltHat);
				player.inventory[12].SetDefaults(ItemID.CobaltHelmet);
				player.inventory[13].SetDefaults(ItemID.MythrilHood);
				player.inventory[14].SetDefaults(ItemID.MythrilHelmet);
				player.inventory[15].SetDefaults(ItemID.AdamantiteHeadgear);
				player.inventory[16].SetDefaults(ItemID.AdamantiteHelmet);
				player.inventory[17].SetDefaults(ItemID.HallowedHeadgear);
				player.inventory[18].SetDefaults(ItemID.HallowedHelmet);
				player.inventory[21].SetDefaults(ItemID.CobaltBreastplate);
				player.inventory[22].SetDefaults(ItemID.CobaltMask);
				player.inventory[23].SetDefaults(ItemID.MythrilChainmail);
				player.inventory[24].SetDefaults(ItemID.MythrilHat);
				player.inventory[25].SetDefaults(ItemID.AdamantiteBreastplate);
				player.inventory[26].SetDefaults(ItemID.AdamantiteMask);
				player.inventory[27].SetDefaults(ItemID.HallowedPlateMail);
				player.inventory[28].SetDefaults(ItemID.HallowedMask);
				player.inventory[31].SetDefaults(ItemID.CobaltLeggings);
				player.inventory[33].SetDefaults(ItemID.MythrilGreaves);
				player.inventory[35].SetDefaults(ItemID.AdamantiteLeggings);
				player.inventory[37].SetDefaults(ItemID.HallowedGreaves);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 3 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 4;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region throwables & explosives
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.VilePowder);
				player.inventory[12].SetDefaults(ItemID.Shuriken);
				player.inventory[13].SetDefaults(ItemID.Bone);
				player.inventory[14].SetDefaults(ItemID.SpikyBall);
				player.inventory[15].SetDefaults(ItemID.ThrowingKnife);
				player.inventory[16].SetDefaults(ItemID.PoisonedKnife);
				player.inventory[21].SetDefaults(ItemID.Dynamite);
				player.inventory[22].SetDefaults(ItemID.Grenade);
				player.inventory[23].SetDefaults(ItemID.Bomb);
				player.inventory[24].SetDefaults(ItemID.StickyBomb);
				player.inventory[25].SetDefaults(ItemID.Explosives);
				player.inventory[31].SetDefaults(ItemID.Flamarang);
				player.inventory[32].SetDefaults(ItemID.ThornChakram);
				player.inventory[33].SetDefaults(ItemID.WoodenBoomerang);
				player.inventory[34].SetDefaults(ItemID.EnchantedBoomerang);
				player.inventory[35].SetDefaults(ItemID.LightDisc);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 4 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 5;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region flails & spears
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.Harpoon);
				player.inventory[12].SetDefaults(ItemID.BallOHurt);
				player.inventory[13].SetDefaults(ItemID.BlueMoon);
				player.inventory[14].SetDefaults(ItemID.Sunfury);
				player.inventory[15].SetDefaults(ItemID.DaoofPow);
				player.inventory[20].SetDefaults(ItemID.Spear);
				player.inventory[21].SetDefaults(ItemID.Trident);
				player.inventory[22].SetDefaults(ItemID.DarkLance);
				player.inventory[23].SetDefaults(ItemID.CobaltNaginata);
				player.inventory[24].SetDefaults(ItemID.MythrilHalberd);
				player.inventory[25].SetDefaults(ItemID.AdamantiteGlaive);
				player.inventory[26].SetDefaults(ItemID.Gungnir);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 5 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 6;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region bows & guns
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.Blowpipe);
				player.inventory[12].SetDefaults(ItemID.FlintlockPistol);
				player.inventory[13].SetDefaults(ItemID.Musket);
				player.inventory[14].SetDefaults(ItemID.Handgun);
				player.inventory[15].SetDefaults(ItemID.Minishark);
				player.inventory[16].SetDefaults(ItemID.Megashark);
				player.inventory[17].SetDefaults(ItemID.PhoenixBlaster);
				player.inventory[18].SetDefaults(ItemID.Sandgun);
				player.inventory[19].SetDefaults(ItemID.Shotgun);
				player.inventory[20].SetDefaults(ItemID.SpaceGun);
				player.inventory[21].SetDefaults(ItemID.StarCannon);
				player.inventory[22].SetDefaults(ItemID.Flamethrower);
				player.inventory[23].SetDefaults(ItemID.ClockworkAssaultRifle);
				player.inventory[24].SetDefaults(ItemID.WoodenBow);
				player.inventory[25].SetDefaults(ItemID.CopperBow);
				player.inventory[26].SetDefaults(ItemID.IronBow);
				player.inventory[27].SetDefaults(ItemID.SilverBow);
				player.inventory[28].SetDefaults(ItemID.GoldBow);
				player.inventory[30].SetDefaults(ItemID.DemonBow);
				player.inventory[31].SetDefaults(ItemID.MoltenFury);
				player.inventory[32].SetDefaults(ItemID.CobaltRepeater);
				player.inventory[33].SetDefaults(ItemID.MythrilRepeater);
				player.inventory[34].SetDefaults(ItemID.AdamantiteRepeater);
				player.inventory[35].SetDefaults(ItemID.HallowedRepeater);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 6 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 7;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region magic weapons
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.FlowerofFire);
				player.inventory[12].SetDefaults(ItemID.Vilethorn);
				player.inventory[13].SetDefaults(ItemID.MagicMissile);
				player.inventory[14].SetDefaults(ItemID.Flamelash);
				player.inventory[15].SetDefaults(ItemID.WaterBolt);
				player.inventory[16].SetDefaults(ItemID.DemonScythe);
				player.inventory[17].SetDefaults(ItemID.CrystalStorm);
				player.inventory[18].SetDefaults(ItemID.CursedFlames);
				player.inventory[20].SetDefaults(ItemID.AquaScepter);
				player.inventory[21].SetDefaults(ItemID.LaserRifle);
				player.inventory[22].SetDefaults(ItemID.MagicDagger);
				player.inventory[23].SetDefaults(ItemID.MagicalHarp);
				player.inventory[24].SetDefaults(ItemID.RainbowRod);
				player.inventory[25].SetDefaults(ItemID.IceRod);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 7 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 8;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region swords
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.WoodenSword);
				player.inventory[12].SetDefaults(ItemID.CopperShortsword);
				player.inventory[13].SetDefaults(ItemID.CopperBroadsword);
				player.inventory[14].SetDefaults(ItemID.IronShortsword);
				player.inventory[15].SetDefaults(ItemID.IronBroadsword);
				player.inventory[16].SetDefaults(ItemID.SilverShortsword);
				player.inventory[17].SetDefaults(ItemID.SilverBroadsword);
				player.inventory[18].SetDefaults(ItemID.GoldShortsword);
				player.inventory[19].SetDefaults(ItemID.GoldBroadsword);
				player.inventory[20].SetDefaults(ItemID.NightsEdge);
				player.inventory[21].SetDefaults(ItemID.LightsBane);
				player.inventory[22].SetDefaults(ItemID.Starfury);
				player.inventory[23].SetDefaults(ItemID.Muramasa);
				player.inventory[24].SetDefaults(ItemID.BladeofGrass);
				player.inventory[25].SetDefaults(ItemID.FieryGreatsword);
				player.inventory[30].SetDefaults(ItemID.CobaltSword);
				player.inventory[31].SetDefaults(ItemID.MythrilSword);
				player.inventory[32].SetDefaults(ItemID.AdamantiteSword);
				player.inventory[33].SetDefaults(ItemID.BreakerBlade);
				player.inventory[34].SetDefaults(ItemID.Excalibur);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 8 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 9;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region phase weapons
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.WhitePhaseblade);
				player.inventory[12].SetDefaults(ItemID.BluePhaseblade);
				player.inventory[13].SetDefaults(ItemID.RedPhaseblade);
				player.inventory[14].SetDefaults(ItemID.PurplePhaseblade);
				player.inventory[15].SetDefaults(ItemID.GreenPhaseblade);
				player.inventory[16].SetDefaults(ItemID.YellowPhaseblade);
				player.inventory[21].SetDefaults(ItemID.WhitePhasesaber);
				player.inventory[22].SetDefaults(ItemID.BluePhasesaber);
				player.inventory[23].SetDefaults(ItemID.RedPhasesaber);
				player.inventory[24].SetDefaults(ItemID.PurplePhasesaber);
				player.inventory[25].SetDefaults(ItemID.GreenPhasesaber);
				player.inventory[26].SetDefaults(ItemID.YellowPhasesaber);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 9 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 10;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region tools
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[2].SetDefaults(ItemID.Drax);
				player.inventory[3].SetDefaults(ItemID.PurificationPowder);
				player.inventory[4].SetDefaults(ItemID.HolyWater);
				player.inventory[5].SetDefaults(ItemID.UnholyWater);
				player.inventory[6].SetDefaults(ItemID.EmptyBucket);
				player.inventory[7].SetDefaults(ItemID.WaterBucket);
				player.inventory[8].SetDefaults(ItemID.LavaBucket);
				player.inventory[11].SetDefaults(ItemID.CopperPickaxe);
				player.inventory[12].SetDefaults(ItemID.IronPickaxe);
				player.inventory[13].SetDefaults(ItemID.SilverPickaxe);
				player.inventory[14].SetDefaults(ItemID.GoldPickaxe);
				player.inventory[15].SetDefaults(ItemID.NightmarePickaxe);
				player.inventory[16].SetDefaults(ItemID.MoltenPickaxe);
				player.inventory[17].SetDefaults(ItemID.CobaltDrill);
				player.inventory[18].SetDefaults(ItemID.MythrilDrill);
				player.inventory[19].SetDefaults(ItemID.AdamantiteDrill);
				player.inventory[20].SetDefaults(ItemID.CopperAxe);
				player.inventory[21].SetDefaults(ItemID.IronAxe);
				player.inventory[22].SetDefaults(ItemID.SilverAxe);
				player.inventory[23].SetDefaults(ItemID.GoldAxe);
				player.inventory[24].SetDefaults(ItemID.WarAxeoftheNight);
				player.inventory[25].SetDefaults(ItemID.MeteorHamaxe);
				player.inventory[26].SetDefaults(ItemID.MoltenHamaxe);
				player.inventory[27].SetDefaults(ItemID.CobaltChainsaw);
				player.inventory[28].SetDefaults(ItemID.MythrilChainsaw);
				player.inventory[29].SetDefaults(ItemID.AdamantiteChainsaw);
				player.inventory[30].SetDefaults(ItemID.WoodenHammer);
				player.inventory[31].SetDefaults(ItemID.CopperHammer);
				player.inventory[32].SetDefaults(ItemID.IronHammer);
				player.inventory[33].SetDefaults(ItemID.SilverHammer);
				player.inventory[34].SetDefaults(ItemID.GoldHammer);
				player.inventory[35].SetDefaults(ItemID.TheBreaker);
				player.inventory[36].SetDefaults(ItemID.Pwnhammer);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 10 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 11;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region movement accessories
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.Aglet);
				player.inventory[2].SetDefaults(ItemID.AnkletoftheWind);
				player.inventory[3].SetDefaults(ItemID.HermesBoots);
				player.inventory[4].SetDefaults(ItemID.RocketBoots);
				player.inventory[5].SetDefaults(ItemID.AngelWings);
				player.inventory[6].SetDefaults(ItemID.DemonWings);
				player.inventory[7].SetDefaults(ItemID.SpectreBoots);
				player.inventory[8].SetDefaults(ItemID.LuckyHorseshoe);
				player.inventory[9].SetDefaults(ItemID.ObsidianHorseshoe);
				player.inventory[11].SetDefaults(ItemID.CloudinaBottle);
				player.inventory[12].SetDefaults(ItemID.ShinyRedBalloon);
				player.inventory[13].SetDefaults(ItemID.CloudinaBalloon);
				player.inventory[14].SetDefaults(ItemID.Flipper);
				player.inventory[15].SetDefaults(ItemID.DivingHelmet);
				player.inventory[16].SetDefaults(ItemID.DivingGear);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion			
					}
					if(inventory == 11 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 12;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region combat accessories
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.CobaltShield);
				player.inventory[2].SetDefaults(ItemID.FeralClaws);
				player.inventory[3].SetDefaults(ItemID.ObsidianSkull);
				player.inventory[4].SetDefaults(ItemID.Shackle);
				player.inventory[5].SetDefaults(ItemID.ObsidianShield);
				player.inventory[6].SetDefaults(ItemID.StarCloak);
				player.inventory[7].SetDefaults(ItemID.TitanGlove);
				player.inventory[8].SetDefaults(ItemID.CrossNecklace);
				player.inventory[11].SetDefaults(ItemID.BandofRegeneration);
				player.inventory[12].SetDefaults(ItemID.BandofStarpower);
				player.inventory[13].SetDefaults(ItemID.NaturesGift);
				player.inventory[14].SetDefaults(ItemID.ManaFlower);
				player.inventory[15].SetDefaults(ItemID.PhilosophersStone);
				player.inventory[16].SetDefaults(ItemID.RangerEmblem);
				player.inventory[17].SetDefaults(ItemID.SorcererEmblem);
				player.inventory[18].SetDefaults(ItemID.WarriorEmblem);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 12 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 13;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region miscellaneous accessories
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.DepthMeter);
				player.inventory[2].SetDefaults(ItemID.CopperWatch);
				player.inventory[3].SetDefaults(ItemID.SilverWatch);
				player.inventory[4].SetDefaults(ItemID.GoldWatch);
				player.inventory[5].SetDefaults(ItemID.Compass);
				player.inventory[6].SetDefaults(ItemID.GPS);
				player.inventory[11].SetDefaults(ItemID.Ruler);
				player.inventory[12].SetDefaults(ItemID.Toolbelt);
				player.inventory[13].SetDefaults(ItemID.MoonCharm);
				player.inventory[14].SetDefaults(ItemID.NeptunesShell);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 13 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 14;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region Christmas
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[3].SetDefaults(ItemID.YellowPresent);
				player.inventory[4].SetDefaults(ItemID.BluePresent);
				player.inventory[5].SetDefaults(ItemID.GreenPresent);
				player.inventory[6].SetDefaults(ItemID.SantaHat);
				player.inventory[7].SetDefaults(ItemID.SantaShirt);
				player.inventory[8].SetDefaults(ItemID.SantaPants);
				player.inventory[11].SetDefaults(ItemID.RedLight);
				player.inventory[12].SetDefaults(ItemID.BlueLight);
				player.inventory[13].SetDefaults(ItemID.GreenLight);
				player.inventory[14].SetDefaults(ItemID.SnowGlobe);
				player.inventory[20].SetDefaults(ItemID.CandyCaneBlock);
				player.inventory[21].SetDefaults(ItemID.GreenCandyCaneBlock);
				player.inventory[22].SetDefaults(ItemID.SnowBlock);
				player.inventory[23].SetDefaults(ItemID.SnowBrick);
				player.inventory[25].SetDefaults(ItemID.CandyCaneWall);
				player.inventory[26].SetDefaults(ItemID.GreenCandyCaneWall);
				player.inventory[27].SetDefaults(ItemID.SnowBrickWall);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 14 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 15;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region miscellaneous
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.GrapplingHook);
				player.inventory[2].SetDefaults(ItemID.DualHook);
				player.inventory[3].SetDefaults(ItemID.DirtRod);
				player.inventory[4].SetDefaults(ItemID.GuideVoodooDoll);
				player.inventory[5].SetDefaults(ItemID.ShadowOrb);
				player.inventory[6].SetDefaults(ItemID.FairyBell);
				player.inventory[7].SetDefaults(ItemID.MagicMirror);
				player.inventory[8].SetDefaults(ItemID.WhoopieCushion);
				player.inventory[9].SetDefaults(ItemID.Boulder);
				player.inventory[11].SetDefaults(ItemID.GoblinBattleStandard);
				player.inventory[12].SetDefaults(ItemID.SuspiciousLookingEye);
				player.inventory[13].SetDefaults(ItemID.WormFood);
				player.inventory[14].SetDefaults(ItemID.SlimeCrown);
				player.inventory[15].SetDefaults(ItemID.MechanicalEye);
				player.inventory[16].SetDefaults(ItemID.MechanicalWorm);
				player.inventory[17].SetDefaults(ItemID.MechanicalSkull);
				player.inventory[18].SetDefaults(ItemID.GoldenKey);
				player.inventory[19].SetDefaults(ItemID.ShadowKey);
				player.inventory[20].SetDefaults(ItemID.CopperCoin);
				player.inventory[21].SetDefaults(ItemID.SilverCoin);
				player.inventory[22].SetDefaults(ItemID.GoldCoin);
				player.inventory[23].SetDefaults(ItemID.PlatinumCoin);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 15 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 16;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region vanity
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.Goggles);
				player.inventory[2].SetDefaults(ItemID.Sunglasses);
				player.inventory[3].SetDefaults(ItemID.JungleRose);
				player.inventory[4].SetDefaults(ItemID.FishBowl);
				player.inventory[5].SetDefaults(ItemID.Robe);
				player.inventory[6].SetDefaults(ItemID.MimeMask);
				player.inventory[7].SetDefaults(ItemID.BunnyHood);
				player.inventory[8].SetDefaults(ItemID.SummerHat);
				player.inventory[9].SetDefaults(ItemID.RobotHat);
				player.inventory[11].SetDefaults(ItemID.ArchaeologistsHat);
				player.inventory[12].SetDefaults(ItemID.PlumbersHat);
				player.inventory[13].SetDefaults(ItemID.TopHat);
				player.inventory[14].SetDefaults(ItemID.FamiliarWig);
				player.inventory[15].SetDefaults(ItemID.RedHat);
				player.inventory[16].SetDefaults(ItemID.NinjaHood);
				player.inventory[17].SetDefaults(ItemID.HerosHat);
				player.inventory[18].SetDefaults(ItemID.ClownHat);
				player.inventory[19].SetDefaults(ItemID.WizardHat);
				player.inventory[21].SetDefaults(ItemID.ArchaeologistsJacket);
				player.inventory[22].SetDefaults(ItemID.PlumbersShirt);
				player.inventory[23].SetDefaults(ItemID.TuxedoShirt);
				player.inventory[24].SetDefaults(ItemID.FamiliarShirt);
				player.inventory[25].SetDefaults(ItemID.TheDoctorsShirt);
				player.inventory[26].SetDefaults(ItemID.NinjaShirt);
				player.inventory[27].SetDefaults(ItemID.HerosShirt);
				player.inventory[28].SetDefaults(ItemID.ClownShirt);
				player.inventory[29].SetDefaults(ItemID.GoldCrown);
				player.inventory[31].SetDefaults(ItemID.ArchaeologistsPants);
				player.inventory[32].SetDefaults(ItemID.PlumbersPants);
				player.inventory[33].SetDefaults(ItemID.TuxedoPants);
				player.inventory[34].SetDefaults(ItemID.FamiliarPants);
				player.inventory[35].SetDefaults(ItemID.TheDoctorsPants);
				player.inventory[36].SetDefaults(ItemID.NinjaPants);
				player.inventory[37].SetDefaults(ItemID.HerosPants);
				player.inventory[38].SetDefaults(ItemID.ClownPants);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 16 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 17;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region consumables, potions
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.LesserHealingPotion);
				player.inventory[2].SetDefaults(ItemID.LesserManaPotion);
				player.inventory[3].SetDefaults(ItemID.LesserRestorationPotion);
				player.inventory[4].SetDefaults(ItemID.HealingPotion);
				player.inventory[5].SetDefaults(ItemID.ManaPotion);
				player.inventory[6].SetDefaults(ItemID.RestorationPotion);
				player.inventory[7].SetDefaults(ItemID.GreaterHealingPotion);
				player.inventory[8].SetDefaults(ItemID.GreaterManaPotion);
				player.inventory[11].SetDefaults(ItemID.ArcheryPotion);
				player.inventory[12].SetDefaults(ItemID.BattlePotion);
				player.inventory[13].SetDefaults(ItemID.FeatherfallPotion);
				player.inventory[14].SetDefaults(ItemID.GillsPotion);
				player.inventory[15].SetDefaults(ItemID.GravitationPotion);
				player.inventory[16].SetDefaults(ItemID.HunterPotion);
				player.inventory[17].SetDefaults(ItemID.InvisibilityPotion);
				player.inventory[18].SetDefaults(ItemID.IronskinPotion);
				player.inventory[19].SetDefaults(ItemID.MagicPowerPotion);
				player.inventory[20].SetDefaults(ItemID.ManaRegenerationPotion);
				player.inventory[21].SetDefaults(ItemID.NightOwlPotion);
				player.inventory[22].SetDefaults(ItemID.ObsidianSkinPotion);
				player.inventory[23].SetDefaults(ItemID.RegenerationPotion);
				player.inventory[24].SetDefaults(ItemID.ShinePotion);
				player.inventory[25].SetDefaults(ItemID.SpelunkerPotion);
				player.inventory[26].SetDefaults(ItemID.SwiftnessPotion);
				player.inventory[27].SetDefaults(ItemID.ThornsPotion);
				player.inventory[28].SetDefaults(ItemID.WaterWalkingPotion);
				player.inventory[30].SetDefaults(ItemID.Mushroom);
				player.inventory[31].SetDefaults(ItemID.GlowingMushroom);
				player.inventory[32].SetDefaults(ItemID.Ale);
				player.inventory[33].SetDefaults(ItemID.BowlofSoup);
				player.inventory[34].SetDefaults(ItemID.Goldfish);
				player.inventory[36].SetDefaults(ItemID.FallenStar);
				player.inventory[37].SetDefaults(ItemID.LifeCrystal);
				player.inventory[38].SetDefaults(ItemID.ManaCrystal);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 17 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 18;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region materials: part 1, bars
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[3].SetDefaults(ItemID.Cobweb);
				player.inventory[4].SetDefaults(ItemID.Silk);
				player.inventory[5].SetDefaults(ItemID.Gel);
				player.inventory[6].SetDefaults(ItemID.Lens);
				player.inventory[7].SetDefaults(ItemID.BlackLens);
				player.inventory[8].SetDefaults(ItemID.Chain);
				player.inventory[9].SetDefaults(ItemID.Hook);
				player.inventory[11].SetDefaults(ItemID.ShadowScale);
				player.inventory[12].SetDefaults(ItemID.TatteredCloth);
				player.inventory[13].SetDefaults(ItemID.Leather);
				player.inventory[14].SetDefaults(ItemID.RottenChunk);
				player.inventory[15].SetDefaults(ItemID.WormTooth);
				player.inventory[16].SetDefaults(ItemID.Cactus);
				player.inventory[17].SetDefaults(ItemID.WaterBucket);
				player.inventory[18].SetDefaults(ItemID.LavaBucket);
				player.inventory[19].SetDefaults(ItemID.VilePowder);
				player.inventory[20].SetDefaults(ItemID.Stinger);
				player.inventory[21].SetDefaults(ItemID.Feather);
				player.inventory[22].SetDefaults(ItemID.Vine);
				player.inventory[23].SetDefaults(ItemID.JungleSpores);
				player.inventory[24].SetDefaults(ItemID.SharkFin);
				player.inventory[25].SetDefaults(ItemID.AntlionMandible);
				player.inventory[26].SetDefaults(ItemID.IllegalGunParts);
				player.inventory[27].SetDefaults(ItemID.Glowstick);
				player.inventory[28].SetDefaults(ItemID.GreenDye);
				player.inventory[29].SetDefaults(ItemID.BlackDye);
				player.inventory[30].SetDefaults(ItemID.CopperBar);
				player.inventory[31].SetDefaults(ItemID.IronBar);
				player.inventory[32].SetDefaults(ItemID.SilverBar);
				player.inventory[33].SetDefaults(ItemID.GoldBar);
				player.inventory[34].SetDefaults(ItemID.DemoniteBar);
				player.inventory[35].SetDefaults(ItemID.MeteoriteBar);
				player.inventory[36].SetDefaults(ItemID.HellstoneBar);
				player.inventory[37].SetDefaults(ItemID.CobaltBar);
				player.inventory[38].SetDefaults(ItemID.MythrilBar);
				player.inventory[39].SetDefaults(ItemID.AdamantiteBar);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion			
					}
					if(inventory == 18 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 19;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region	materials: part 2, souls
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.Bell);
				player.inventory[12].SetDefaults(ItemID.Harp);
				player.inventory[13].SetDefaults(ItemID.SpellTome);
				player.inventory[14].SetDefaults(ItemID.CursedFlame);
				player.inventory[15].SetDefaults(ItemID.DarkShard);
				player.inventory[16].SetDefaults(ItemID.LightShard);
				player.inventory[17].SetDefaults(ItemID.PixieDust);
				player.inventory[18].SetDefaults(ItemID.UnicornHorn);
				player.inventory[20].SetDefaults(ItemID.SoulofFlight);
				player.inventory[21].SetDefaults(ItemID.SoulofFright);
				player.inventory[22].SetDefaults(ItemID.SoulofLight);
				player.inventory[23].SetDefaults(ItemID.SoulofMight);
				player.inventory[24].SetDefaults(ItemID.SoulofNight);
				player.inventory[25].SetDefaults(ItemID.SoulofSight);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 19 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 20;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region mechanical, wire
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[4].SetDefaults(ItemID.Wrench);
				player.inventory[5].SetDefaults(ItemID.WireCutter);
				player.inventory[6].SetDefaults(ItemID.Switch);
				player.inventory[7].SetDefaults(ItemID.ActiveStoneBlock);
				player.inventory[8].SetDefaults(ItemID.InactiveStoneBlock);
				player.inventory[11].SetDefaults(ItemID.Wire);
				player.inventory[12].SetDefaults(ItemID.Lever);
				player.inventory[13].SetDefaults(ItemID.BrownPressurePlate);
				player.inventory[14].SetDefaults(ItemID.GrayPressurePlate);
				player.inventory[15].SetDefaults(ItemID.GreenPressurePlate);
				player.inventory[16].SetDefaults(ItemID.RedPressurePlate);
				player.inventory[17].SetDefaults(ItemID.Timer1Second);
				player.inventory[18].SetDefaults(ItemID.Timer3Second);
				player.inventory[19].SetDefaults(ItemID.Timer5Second);
				player.inventory[20].SetDefaults(ItemID.InletPump);
				player.inventory[21].SetDefaults(ItemID.OutletPump);
				player.inventory[22].SetDefaults(ItemID.DartTrap);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 20 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 21;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region ammo
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.WoodenArrow);
				player.inventory[12].SetDefaults(ItemID.FlamingArrow);
				player.inventory[13].SetDefaults(ItemID.UnholyArrow);
				player.inventory[14].SetDefaults(ItemID.JestersArrow);
				player.inventory[15].SetDefaults(ItemID.HellfireArrow);
				player.inventory[16].SetDefaults(ItemID.HolyArrow);
				player.inventory[17].SetDefaults(ItemID.CursedArrow);
				player.inventory[20].SetDefaults(ItemID.Seed);
				player.inventory[21].SetDefaults(ItemID.MusketBall);
				player.inventory[22].SetDefaults(ItemID.SilverBullet);
				player.inventory[23].SetDefaults(ItemID.MeteorShot);
				player.inventory[24].SetDefaults(ItemID.CrystalBullet);
				player.inventory[25].SetDefaults(ItemID.CursedBullet);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 21 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 22;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region alchemy, plants
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[3].SetDefaults(ItemID.ClayPot);
				player.inventory[4].SetDefaults(ItemID.BottledWater);
				player.inventory[5].SetDefaults(ItemID.Bottle);
				player.inventory[8].SetDefaults(ItemID.Acorn);
				player.inventory[9].SetDefaults(ItemID.Sunflower);
				player.inventory[11].SetDefaults(ItemID.BlinkrootSeeds);
				player.inventory[12].SetDefaults(ItemID.DaybloomSeeds);
				player.inventory[13].SetDefaults(ItemID.FireblossomSeeds);
				player.inventory[14].SetDefaults(ItemID.MoonglowSeeds);
				player.inventory[15].SetDefaults(ItemID.DeathweedSeeds);
				player.inventory[16].SetDefaults(ItemID.WaterleafSeeds);
				player.inventory[21].SetDefaults(ItemID.Blinkroot);
				player.inventory[22].SetDefaults(ItemID.Daybloom);
				player.inventory[23].SetDefaults(ItemID.Fireblossom);
				player.inventory[24].SetDefaults(ItemID.Moonglow);
				player.inventory[25].SetDefaults(ItemID.Deathweed);
				player.inventory[26].SetDefaults(ItemID.Waterleaf);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 22 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 23;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region statues: useful
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.BatStatue);
				player.inventory[2].SetDefaults(ItemID.BirdStatue);
				player.inventory[3].SetDefaults(ItemID.BombStatue);
				player.inventory[4].SetDefaults(ItemID.BunnyStatue);
				player.inventory[5].SetDefaults(ItemID.ChestStatue);
				player.inventory[6].SetDefaults(ItemID.CrabStatue);
				player.inventory[7].SetDefaults(ItemID.FishStatue);
				player.inventory[8].SetDefaults(ItemID.HeartStatue);
				player.inventory[9].SetDefaults(ItemID.JellyfishStatue);
				player.inventory[11].SetDefaults(ItemID.KingStatue);
				player.inventory[12].SetDefaults(ItemID.MushroomStatue);
				player.inventory[13].SetDefaults(ItemID.PiranhaStatue);
				player.inventory[14].SetDefaults(ItemID.QueenStatue);
				player.inventory[15].SetDefaults(ItemID.SkeletonStatue);
				player.inventory[16].SetDefaults(ItemID.SlimeStatue);
				player.inventory[17].SetDefaults(ItemID.StarStatue);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 23 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 24;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region statues: useless
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.AngelStatue);
				player.inventory[2].SetDefaults(ItemID.AnvilStatue);
				player.inventory[3].SetDefaults(ItemID.AxeStatue);
				player.inventory[4].SetDefaults(ItemID.BoomerangStatue);
				player.inventory[5].SetDefaults(ItemID.BootStatue);
				player.inventory[6].SetDefaults(ItemID.BowStatue);
				player.inventory[7].SetDefaults(ItemID.CorruptStatue);
				player.inventory[8].SetDefaults(ItemID.CrossStatue);
				player.inventory[9].SetDefaults(ItemID.EyeballStatue);
				player.inventory[11].SetDefaults(ItemID.GargoyleStatue);
				player.inventory[12].SetDefaults(ItemID.GloomStatue);
				player.inventory[13].SetDefaults(ItemID.GoblinStatue);
				player.inventory[14].SetDefaults(ItemID.HammerStatue);
				player.inventory[15].SetDefaults(ItemID.HornetStatue);
				player.inventory[16].SetDefaults(ItemID.ImpStatue);
				player.inventory[17].SetDefaults(ItemID.PickaxeStatue);
				player.inventory[18].SetDefaults(ItemID.PillarStatue);
				player.inventory[19].SetDefaults(ItemID.PotStatue);
				player.inventory[20].SetDefaults(ItemID.PotionStatue);
				player.inventory[21].SetDefaults(ItemID.ReaperStatue);
				player.inventory[22].SetDefaults(ItemID.ShieldStatue);
				player.inventory[23].SetDefaults(ItemID.SpearStatue);
				player.inventory[24].SetDefaults(ItemID.SunflowerStatue);
				player.inventory[25].SetDefaults(ItemID.SwordStatue);
				player.inventory[26].SetDefaults(ItemID.TreeStatue);
				player.inventory[27].SetDefaults(ItemID.WomanStatue);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 24 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 25;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region	crafting workshops
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[2].SetDefaults(ItemID.WoodenChair);
				player.inventory[3].SetDefaults(ItemID.WoodenTable);
				player.inventory[4].SetDefaults(ItemID.WorkBench);
				player.inventory[5].SetDefaults(ItemID.Sawmill);
				player.inventory[6].SetDefaults(ItemID.Keg);
				player.inventory[7].SetDefaults(ItemID.CookingPot);
				player.inventory[8].SetDefaults(ItemID.IronAnvil);
				player.inventory[9].SetDefaults(ItemID.MythrilAnvil);
				player.inventory[11].SetDefaults(ItemID.Furnace);
				player.inventory[12].SetDefaults(ItemID.Hellforge);
				player.inventory[13].SetDefaults(ItemID.AdamantiteForge);
				player.inventory[14].SetDefaults(ItemID.Loom);
				player.inventory[15].SetDefaults(ItemID.Bookcase);
				player.inventory[16].SetDefaults(ItemID.TinkerersWorkshop);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 25 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 26;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region decorations
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[1].SetDefaults(ItemID.CopperHammer);
				player.inventory[2].SetDefaults(ItemID.WoodenDoor);
				player.inventory[3].SetDefaults(ItemID.WoodenChair);
				player.inventory[4].SetDefaults(ItemID.WoodenTable);
				player.inventory[5].SetDefaults(ItemID.Bed);
				player.inventory[6].SetDefaults(ItemID.Sign);
				player.inventory[7].SetDefaults(ItemID.Tombstone);
				player.inventory[8].SetDefaults(ItemID.Book);
				player.inventory[9].SetDefaults(ItemID.Bookcase);
				player.inventory[11].SetDefaults(ItemID.AngelStatue);
				player.inventory[12].SetDefaults(ItemID.Toilet);
				player.inventory[13].SetDefaults(ItemID.Bathtub);
				player.inventory[14].SetDefaults(ItemID.Bench);
				player.inventory[15].SetDefaults(ItemID.Piano);
				player.inventory[16].SetDefaults(ItemID.GrandfatherClock);
				player.inventory[17].SetDefaults(ItemID.Dresser);
				player.inventory[18].SetDefaults(ItemID.Throne);
				player.inventory[19].SetDefaults(ItemID.PinkVase);
				player.inventory[20].SetDefaults(ItemID.Bowl);
				player.inventory[21].SetDefaults(ItemID.Mannequin);
				player.inventory[22].SetDefaults(ItemID.Mug);
				player.inventory[23].SetDefaults(ItemID.Coral);
				player.inventory[24].SetDefaults(ItemID.CrystalShard);
				player.inventory[25].SetDefaults(ItemID.Spike);
				player.inventory[26].SetDefaults(ItemID.RedBanner);
				player.inventory[27].SetDefaults(ItemID.GreenBanner);
				player.inventory[28].SetDefaults(ItemID.BlueBanner);
				player.inventory[29].SetDefaults(ItemID.YellowBanner);
				player.inventory[30].SetDefaults(ItemID.Chest);
				player.inventory[31].SetDefaults(ItemID.GoldChest);
				player.inventory[32].SetDefaults(ItemID.ShadowChest);
				player.inventory[33].SetDefaults(ItemID.Barrel);
				player.inventory[34].SetDefaults(ItemID.TrashCan);
				player.inventory[36].SetDefaults(ItemID.Safe);
				player.inventory[37].SetDefaults(ItemID.PiggyBank);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 26 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 27;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region music boxes
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.MusicBox);
				player.inventory[12].SetDefaults(ItemID.MusicBoxBoss1);
				player.inventory[13].SetDefaults(ItemID.MusicBoxBoss2);
				player.inventory[14].SetDefaults(ItemID.MusicBoxBoss3);
				player.inventory[15].SetDefaults(ItemID.MusicBoxCorruption);
				player.inventory[16].SetDefaults(ItemID.MusicBoxEerie);
				player.inventory[17].SetDefaults(ItemID.MusicBoxJungle);
				player.inventory[18].SetDefaults(ItemID.MusicBoxNight);
				player.inventory[19].SetDefaults(ItemID.MusicBoxOverworldDay);
				player.inventory[20].SetDefaults(ItemID.MusicBoxTheHallow);
				player.inventory[21].SetDefaults(ItemID.MusicBoxTitle);
				player.inventory[22].SetDefaults(ItemID.MusicBoxUnderground);
				player.inventory[23].SetDefaults(ItemID.MusicBoxUndergroundCorruption);
				player.inventory[24].SetDefaults(ItemID.MusicBoxUndergroundHallow);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 27 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 28;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region lighting, torches
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[3].SetDefaults(ItemID.Torch);
				player.inventory[4].SetDefaults(ItemID.Candle);
				player.inventory[5].SetDefaults(ItemID.WaterCandle);
				player.inventory[6].SetDefaults(ItemID.Candelabra);
				player.inventory[7].SetDefaults(ItemID.SkullLantern);
				player.inventory[8].SetDefaults(ItemID.TikiTorch);
				player.inventory[9].SetDefaults(ItemID.LampPost);
				player.inventory[11].SetDefaults(ItemID.CursedTorch);
				player.inventory[12].SetDefaults(ItemID.DemonTorch);
				player.inventory[13].SetDefaults(ItemID.BlueTorch);
				player.inventory[14].SetDefaults(ItemID.GreenTorch);
				player.inventory[15].SetDefaults(ItemID.PurpleTorch);
				player.inventory[16].SetDefaults(ItemID.RedTorch);
				player.inventory[17].SetDefaults(ItemID.WhiteTorch);
				player.inventory[18].SetDefaults(ItemID.YellowTorch);
				player.inventory[20].SetDefaults(ItemID.CopperChandelier);
				player.inventory[21].SetDefaults(ItemID.SilverChandelier);
				player.inventory[22].SetDefaults(ItemID.GoldChandelier);
				player.inventory[23].SetDefaults(ItemID.ChainLantern);
				player.inventory[24].SetDefaults(ItemID.ChineseLantern);
				player.inventory[25].SetDefaults(ItemID.DiscoBall);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 28 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 29;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region ore & gems
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[11].SetDefaults(ItemID.CopperOre);
				player.inventory[12].SetDefaults(ItemID.IronOre);
				player.inventory[13].SetDefaults(ItemID.SilverOre);
				player.inventory[14].SetDefaults(ItemID.GoldOre);
				player.inventory[15].SetDefaults(ItemID.DemoniteOre);
				player.inventory[16].SetDefaults(ItemID.Meteorite);
				player.inventory[17].SetDefaults(ItemID.Obsidian);
				player.inventory[18].SetDefaults(ItemID.Hellstone);
				player.inventory[20].SetDefaults(ItemID.CobaltOre);
				player.inventory[21].SetDefaults(ItemID.MythrilOre);
				player.inventory[22].SetDefaults(ItemID.AdamantiteOre);
				player.inventory[30].SetDefaults(ItemID.Amethyst);
				player.inventory[31].SetDefaults(ItemID.Diamond);
				player.inventory[32].SetDefaults(ItemID.Emerald);
				player.inventory[33].SetDefaults(ItemID.Ruby);
				player.inventory[34].SetDefaults(ItemID.Sapphire);
				player.inventory[35].SetDefaults(ItemID.Topaz);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 29 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 30;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region wall
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[3].SetDefaults(ItemID.DirtWall);
				player.inventory[4].SetDefaults(ItemID.StoneWall);
				player.inventory[5].SetDefaults(ItemID.WoodWall);
				player.inventory[6].SetDefaults(ItemID.GrayBrickWall);
				player.inventory[7].SetDefaults(ItemID.RedBrickWall);
				player.inventory[8].SetDefaults(ItemID.GlassWall);
				player.inventory[9].SetDefaults(ItemID.PlankedWall);
				player.inventory[11].SetDefaults(ItemID.CopperBrickWall);
				player.inventory[12].SetDefaults(ItemID.SilverBrickWall);
				player.inventory[13].SetDefaults(ItemID.GoldBrickWall);
				player.inventory[14].SetDefaults(ItemID.ObsidianBrickWall);
				player.inventory[15].SetDefaults(ItemID.PinkBrickWall);
				player.inventory[16].SetDefaults(ItemID.GreenBrickWall);
				player.inventory[17].SetDefaults(ItemID.BlueBrickWall);
				player.inventory[20].SetDefaults(ItemID.CobaltBrickWall);
				player.inventory[21].SetDefaults(ItemID.IridescentBrickWall);
				player.inventory[22].SetDefaults(ItemID.MythrilBrickWall);
				player.inventory[23].SetDefaults(ItemID.PearlstoneBrickWall);
				player.inventory[24].SetDefaults(ItemID.MudstoneBrickWall);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 30 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							inventory 	= 31;
							Main.NewText("" + INV[inventory-1], 250, 225, 175);
				#region soil & blocks, seeds
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[3].SetDefaults(ItemID.Torch);
				player.inventory[4].SetDefaults(ItemID.WoodPlatform);
				player.inventory[5].SetDefaults(ItemID.Wood);
				player.inventory[6].SetDefaults(ItemID.DirtBlock);
				player.inventory[7].SetDefaults(ItemID.SandBlock);
				player.inventory[8].SetDefaults(ItemID.ClayBlock);
				player.inventory[9].SetDefaults(ItemID.MudBlock);
				player.inventory[11].SetDefaults(ItemID.AshBlock);
				player.inventory[12].SetDefaults(ItemID.SiltBlock);
				player.inventory[13].SetDefaults(ItemID.StoneBlock);
				player.inventory[14].SetDefaults(ItemID.EbonstoneBlock);
				player.inventory[15].SetDefaults(ItemID.PearlstoneBlock);
				player.inventory[16].SetDefaults(ItemID.PearlsandBlock);
				player.inventory[17].SetDefaults(ItemID.EbonsandBlock);
				player.inventory[20].SetDefaults(ItemID.GrassSeeds);
				player.inventory[21].SetDefaults(ItemID.JungleGrassSeeds);
				player.inventory[22].SetDefaults(ItemID.MushroomGrassSeeds);
				player.inventory[23].SetDefaults(ItemID.CorruptSeeds);
				player.inventory[24].SetDefaults(ItemID.HallowedSeeds);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
					if(inventory == 31 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
							switchTime 	= 45;
							Main.NewText("" + INV[inventory], 250, 225, 175);
							inventory 	= 0;
				#region	bricks
				//	RESET inventory before refill
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].SetDefaults(0);
				player.inventory[5].SetDefaults(ItemID.GrayBrick);
				player.inventory[6].SetDefaults(ItemID.RedBrick);
				player.inventory[7].SetDefaults(ItemID.Glass);
				player.inventory[8].SetDefaults(ItemID.Wood);
				player.inventory[9].SetDefaults(ItemID.WoodenBeam);
				player.inventory[11].SetDefaults(ItemID.CopperBrick);
				player.inventory[12].SetDefaults(ItemID.SilverBrick);
				player.inventory[13].SetDefaults(ItemID.GoldBrick);
				player.inventory[14].SetDefaults(ItemID.ObsidianBrick);
				player.inventory[15].SetDefaults(ItemID.HellstoneBrick);
				player.inventory[16].SetDefaults(ItemID.PinkBrick);
				player.inventory[17].SetDefaults(ItemID.GreenBrick);
				player.inventory[18].SetDefaults(ItemID.BlueBrick);
				player.inventory[20].SetDefaults(ItemID.CobaltBrick);
				player.inventory[21].SetDefaults(ItemID.DemoniteBrick);
				player.inventory[22].SetDefaults(ItemID.IridescentBrick);
				player.inventory[23].SetDefaults(ItemID.MythrilBrick);
				player.inventory[24].SetDefaults(ItemID.PearlstoneBrick);
				player.inventory[25].SetDefaults(ItemID.MudstoneBlock);
				//	set items to maximum stack
				for(int i = 0; i < player.inventory.Length; i++) player.inventory[i].stack = player.inventory[i].maxStack;
				#endregion
					}
				#endregion
				}
			#region tile editing
				if(style == 1){
			#region menu
				//	using depreciation to return to previous inventory values
					if(tileManage >= 0 && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Left) < 0){
							switchTime = 45;
							if(tileManage > 0) tileManage	-= 1;
							Main.NewText("Rollback: " + TILE[tileManage], 200, 150, 100);
							if(tileManage == 0) tileManage -= 1;
					//	using array length to loop to top
						if(tileManage < 0){
							tileManage = TILE.Length-1;
							Main.NewText("Loop: " + TILE[tileManage], 200, 150, 100);
						}
					}	
				//	incrementing using right arrow key
					if(tileManage < TILE.Length && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
						switchTime = 45;
						tileManage += 1;
						Main.NewText("" + TILE[tileManage-1], 250, 225, 175);
					}
					if(tileManage == TILE.Length && switchTime <= 0 && Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.Right) < 0){
						switchTime = 45;
						tileManage = 1;
						Main.NewText("Looped: " + TILE[tileManage-1], 250, 225, 175);
					}
			#endregion
			#region tile manage tools			
				//	designate variables for tileManage functions
				//	set vectors for mouse position
					Vector2 mousev = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);

				// 	tileManage 1, Coordinates
					if(tileManage == 1 && Main.mouseLeft && globalTime%2 == 0){
						Main.NewText		("Tile position: X " + Math.Round(mousev.X/16) + " Y " + Math.Round(mousev.Y/16), 200, 150, 100);
					}
					if(tileManage == 1 && Main.mouseRight && switchTime <= 0){
						switchTime		 	= 45;
						Main.spawnTileX 	= (int)(Math.Round(mousev.X/16));
						Main.spawnTileY 	= (int)(Math.Round(mousev.Y/16));
						Main.NewText		("Spawn position set: X " + Math.Round(mousev.X/16) + " Y " + Math.Round(mousev.Y/16), 200, 150, 100);
				//	if NetMessage necessary, it would be message 27		
						projType			= mod.ProjectileType("Spawn");
						proj				= Projectile.NewProjectile(mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
						Main.projectile[proj].timeLeft = 120;
					}
				//	tileManage 2, Tile Break
					if(tileManage == 2){
						projType 	= mod.ProjectileType("TileBreak");
									Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseLeft && globalTime%4 == 0){
							proj	= Projectile.NewProjectile(mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
									Main.projectile[proj].timeLeft = 10;
						}
					}
				//	tileManage 3, Wall Break
					if(tileManage == 3){
						projType 	= mod.ProjectileType("WallBreak");
									Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseLeft && globalTime%4 == 0){
							proj	= Projectile.NewProjectile(mousev.X, mousev.Y, 0f, 0f, projType, 0, 0f, Main.myPlayer);
									Main.projectile[proj].timeLeft = 10;
						}
					}
				//	tileManage 4, Tile Copy
					if(tileManage == 4){
						Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseRight && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null && Main.tile[Player.tileTargetX, Player.tileTargetY].active()){
							switchTime 	= 45;
							tileType 	= Main.tile[Player.tileTargetX, Player.tileTargetY].type;
							Main.NewText("Tile ID Selected: " + tileType, 200, 150, 100);
						}
						if(Main.mouseLeft && globalTime%6 == 0){
							WorldGen.PlaceTile((int)mousev.X/16, (int)mousev.Y/16, tileType, false, true, -1, 0);

							if(Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X/16)-1, (int)(mousev.Y/16)-1, 3);
						}
					}
				//	tileManage 5, Wall Copy
					if(tileManage == 5){
						Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseRight && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null){
							switchTime 	= 45;
							tileType 	= Main.tile[Player.tileTargetX, Player.tileTargetY].wall;
							Main.NewText("Wall ID Selected: " + tileType, 200, 150, 100);
						}
						if(Main.mouseLeft && globalTime%6 == 0){
							WorldGen.PlaceWall((int)mousev.X/16, (int)mousev.Y/16, tileType, false);
							if(Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X/16)-1, (int)(mousev.Y/16)-1, 3);
						}
					}
				//	tileManage 6, Place Liquid
					if(tileManage == 6){
						Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseLeft && globalTime%6 == 0){
							Main.tile[(int)mousev.X/16, (int)mousev.Y/16].liquid = 255;
							WorldGen.SquareTileFrame((int)mousev.X/16, (int)mousev.Y/16, true);
							if(Main.netMode != 0) {
								NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X/16)-1, (int)(mousev.Y/16)-1, 3);
								NetMessage.sendWater((int)mousev.X/16, (int)mousev.Y/16);
							}
						}
						if(Main.mouseMiddle && globalTime%6 == 0){
							Main.tile[(int)mousev.X/16, (int)mousev.Y/16].liquid = 255;
							Main.tile[(int)mousev.X/16, (int)mousev.Y/16].honey(true);
							WorldGen.SquareTileFrame((int)mousev.X/16, (int)mousev.Y/16, true);
							if(Main.netMode != 0) {
								NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X/16)-1, (int)(mousev.Y/16)-1, 3);
								NetMessage.sendWater((int)mousev.X/16, (int)mousev.Y/16);
							}
						}
						if(Main.mouseRight && globalTime%6 == 0){
							Main.tile[(int)mousev.X/16, (int)mousev.Y/16].liquid = 255;
							Main.tile[(int)mousev.X/16, (int)mousev.Y/16].lava(true);
							WorldGen.SquareTileFrame((int)mousev.X/16, (int)mousev.Y/16, true);
							if(Main.netMode != 0) {
								NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X/16)-1, (int)(mousev.Y/16)-1, 3);
								NetMessage.sendWater((int)mousev.X/16, (int)mousev.Y/16);
							}
						}
					}
					//	tileManage 7, Drain Liquid
					if(tileManage == 7){
						Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseLeft){
							Main.tile[(int)mousev.X/16, (int)mousev.Y/16].liquid = 0;
							WorldGen.SquareTileFrame((int)mousev.X/16, (int)mousev.Y/16, true);
							if(Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, (int)(mousev.X/16)-1, (int)(mousev.Y/16)-1, 3);
						}
					}
					//	tileManage 8, Tile Selection Fill
					
					if(tileManage == 8){
						Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseLeft && switchTime <= 0){
							switchTime = 90;
							positionX1 = (int)(Math.Round(mousev.X/16));
							positionY1 = (int)(Math.Round(mousev.Y/16));
							Main.NewText("Top Left Corner Set at: X " + positionX1 + " Y " + positionY1, 250, 225, 175);
						}
						if(Main.mouseMiddle && switchTime <= 0){
							switchTime = 90;
							positionX2 = (int)(Math.Round(mousev.X/16));
							positionY2 = (int)(Math.Round(mousev.Y/16));
							Main.NewText("Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2, 250, 225, 175);
						}	
						if(Main.mouseRight && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null){
							switchTime 	= 45;
							tileType 	= Main.tile[Player.tileTargetX, Player.tileTargetY].type;
							Main.NewText("Tile ID Selected: " + tileType, 200, 150, 100);
						}
						if(Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0){
							for(int i = positionX1; i < positionX2; i++){
							for(int j = positionY1; j < positionY2; j++){
							//!	default place tile option
							// 	WorldGen.PlaceTile(i, j, tileType, true, true, -1, 0);
							//!	force alternative
								Main.tile[i, j].active(true);
								Main.tile[i, j].type = (ushort)tileType;
								WorldGen.SquareTileFrame(i, j);
								if(Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i-1, j-1, 3);
								}
							}
						}
					}
					//	tileManage 9, Tile Selection Remove
					if(tileManage == 9){
						Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseLeft && switchTime <= 0){
							switchTime = 90;
							positionX1 = (int)(Math.Round(mousev.X/16));
							positionY1 = (int)(Math.Round(mousev.Y/16));
							Main.NewText("Top Left Corner Set at: X " + positionX1 + " Y " + positionY1, 250, 225, 175);
						}
						if(Main.mouseMiddle && switchTime <= 0){
							switchTime = 90;
							positionX2 = (int)(Math.Round(mousev.X/16));
							positionY2 = (int)(Math.Round(mousev.Y/16));
							Main.NewText("Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2, 250, 225, 175);
						}	
						if(Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0){
							for(int i = positionX1; i < positionX2; i++){
							for(int j = positionY1; j < positionY2; j++){
								WorldGen.KillTile(i, j, false, false, true);
								WorldGen.SquareTileFrame(i, j);
								if(Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i-1, j-1, 3);
								}
							}
						}
					}
					//	tileManage 10, Wall Selection Fill
					if(tileManage == 10){
						Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseLeft && switchTime <= 0){
							switchTime = 90;
							positionX1 = (int)(Math.Round(mousev.X/16));
							positionY1 = (int)(Math.Round(mousev.Y/16));
							Main.NewText("Top Left Corner Set at: X " + positionX1 + " Y " + positionY1, 250, 225, 175);
						}
						if(Main.mouseMiddle && switchTime <= 0){
							switchTime = 90;
							positionX2 = (int)(Math.Round(mousev.X/16));
							positionY2 = (int)(Math.Round(mousev.Y/16));
							Main.NewText("Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2, 250, 225, 175);
						}	
						if(Main.mouseRight && switchTime <= 0 && Main.tile[Player.tileTargetX, Player.tileTargetY] != null){
							switchTime 	= 45;
							tileType 	= Main.tile[Player.tileTargetX, Player.tileTargetY].wall;
							Main.NewText("Wall ID Selected: " + tileType, 200, 150, 100);
						}
						if(Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0){
							for(int i = positionX1; i < positionX2; i++){
							for(int j = positionY1; j < positionY2; j++){
								WorldGen.PlaceWall(i, j, tileType, false);
								if(Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i-1, j-1, 3);
								}
							}
						}
					}
					//	tileManage 11, Wall Selection Remove
					if(tileManage == 11){
						Lighting.AddLight((int)(mousev.X/16), (int)(mousev.Y/16), 1f, 1f, 1f);
						if(Main.mouseLeft && switchTime <= 0){
							switchTime = 90;
							positionX1 = (int)(Math.Round(mousev.X/16));
							positionY1 = (int)(Math.Round(mousev.Y/16));
							Main.NewText("Top Left Corner Set at: X " + positionX1 + " Y " + positionY1, 250, 225, 175);
						}
						if(Main.mouseMiddle && switchTime <= 0){
							switchTime = 90;
							positionX2 = (int)(Math.Round(mousev.X/16));
							positionY2 = (int)(Math.Round(mousev.Y/16));
							Main.NewText("Bottom Right Corner Set at: X " + positionX2 + " Y " + positionY2, 250, 225, 175);
						}	
						if(Main.GetKeyState((int)Microsoft.Xna.Framework.Input.Keys.X) < 0){
							for(int i = positionX1; i < positionX2; i++){
							for(int j = positionY1; j < positionY2; j++){
								WorldGen.KillWall(i, j, false);
								WorldGen.SquareTileFrame(i, j);
								if(Main.netMode != 0) NetMessage.SendTileSquare(Main.myPlayer, i-1, j-1, 3);
								}
							}
						}
					}
			#endregion
				}
			#endregion
			}
			else
			{
			//	run Initialize to reset defaults
			//	activate only once using flip bool
				if(!flip){
					if(Main.netMode == 0) Initialize();
					else
					{
						enabled 	= false;
						flip 		= false;
						flipInv		= false;
					
						style 		= 0;
						inventory	= 0;
						tileManage	= 0;
						
						proj		= 0;
						projType	= 0;
						
						tile		= 0;
						tileType	= 0;
					//	make sure default values are in a map's range index	
						positionX1	= 1200;
						positionY1	= 1200;
						positionX2	= 1200;
						positionY2	= 1200;
					//	RESET movement variables
						Player.defaultGravity 	= 0.4f;
						player.ignoreWater		= false;
						player.merman			= false;
						player.hideMerman		= false;
					}
					flip = true;
				}
			}
		}
	}
}