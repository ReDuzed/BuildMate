using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildMate.Projectiles
{
	public class WallBreak : ModProjectile
	{
	#region set properties
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wall Break");
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
		//		unnecessary if light added at mouse vector
		//	Lighting.AddLight((int)projectile.position.X/16, (int)projectile.position.Y/16, 1f, 1f, 1f);
		
		//	set variables to locate tile position 
		//	in retrospect to project position
			int i = (int)(Projectile.position.X + (float)(Projectile.width / 2)) / 16;
			int j = (int)(Projectile.position.Y + (float)(Projectile.width / 2)) / 16;
			if (Main.tile[i, j].WallType != null) 
			{
			//	if wall is active at the location, run this
				WorldGen.KillWall(i, j, false);
			//	networking
				if (Main.tile[i, j].WallType == 0 && Main.netMode != 0)
				{
					NetMessage.SendData(17, -1, -1, null, 2, (float)i, (float)j, 0f, 0, 0, 0);
				}
			}
		}
	}
}