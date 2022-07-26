using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using BuildMate;

namespace BuildMate
{
	public class NPC_global : GlobalNPC
	{
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (GlobalWorld.enabled) {
					spawnRate = 0;
					maxSpawns = 0;
			}
			else {
					spawnRate = 575;
					maxSpawns = 6;
			}
		}
	}
}
