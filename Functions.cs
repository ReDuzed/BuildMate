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

namespace BuildMate
{
    public class Functions
    {
		private Player player;
		public Functions(Player player)
        {
			this.player = player;
        }
        public void ResetInventory()
		{                           
		//	RESET inventory before setting default
			for (int i = 5; i < player.inventory.Length - 10; i++) player.inventory[i].SetDefaults(0);

			player.inventory[0].SetDefaults(ItemID.CopperPickaxe);
			player.inventory[1].SetDefaults(ItemID.CopperAxe);
			player.inventory[2].SetDefaults(ItemID.CopperHammer);
			player.inventory[3].SetDefaults(ItemID.BluePhaseblade);
			player.inventory[4].SetDefaults(ItemID.IvyWhip);
			player.armor[0].SetDefaults(ItemID.Sunglasses);
			player.armor[4].SetDefaults(ItemID.CloudinaBalloon);
			player.armor[5].SetDefaults(ItemID.SpectreBoots);
			player.armor[6].SetDefaults(ItemID.ObsidianHorseshoe);
		}
    }
}
