using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BuildMate.Projectiles
{
	public class Spawn : ModProjectile
	{
	#region set properties
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spawn");
		}
		public override void SetDefaults()
		{
			Projectile.width = 1;
			Projectile.height = 1;
			Projectile.aiStyle = 0;
			Projectile.timeLeft = 120;
			Projectile.friendly = true;
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.scale = 1f;
			Projectile.ownerHitCheck = true;
			Projectile.netUpdate = true;
		}
	#endregion
		public override void AI()
		{
			Lighting.AddLight((int)Projectile.position.X/16, (int)Projectile.position.Y/16, 1f, 1f, 1f);
		}
	}
}