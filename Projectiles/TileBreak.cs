using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildMate.Projectiles
{
	public class TileBreak : ModProjectile
	{
	#region set properties
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Tile Break");
		}
		public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.aiStyle = 1;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.tileCollide = true;
			Projectile.ignoreWater = true;
			Projectile.scale = 1f;
			Projectile.ownerHitCheck = true;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.alpha = 255;
			Projectile.netUpdate = true;
		}
	#endregion
		public override void AI()
		{
		//	add light at projectile position
		// 	in retrospect to tile coordinates 
		//!	unnecessary if light added at mouse vector
		//	Lighting.AddLight((int)projectile.position.X/16, (int)projectile.position.Y/16, 1f, 1f, 1f);
		
		//	set variables to locate tile position 
		//	in retrospect to project position
			int i = (int)(Projectile.position.X + (float)(Projectile.width / 2)) / 16;
			int j = (int)(Projectile.position.Y + (float)(Projectile.width / 2)) / 16;
			if (Main.tile[i, j].HasTile) 
			{
			//	if tile is active at the location, run this
				WorldGen.KillTile(i,j,false,false,true);
			//	networking
				if (!Main.tile[i, j].HasTile && Main.netMode != 0)
				{
					NetMessage.SendData(17, -1, -1, null, 0, (float)i, (float)j, 0f, 0, 0, 0);
				}
			}
		}
	}
}