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
			projectile.width = 1;
			projectile.height = 1;
			projectile.aiStyle = 0;
			projectile.timeLeft = 120;
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.scale = 1f;
			projectile.ownerHitCheck = true;
			projectile.netUpdate = true;
		}
	#endregion
		public override void AI()
		{
			Lighting.AddLight((int)projectile.position.X/16, (int)projectile.position.Y/16, 1f, 1f, 1f);
		}
	}
}