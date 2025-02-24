using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
	public class NegentropyBulletProjectile : ModProjectile
	{
		public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults() {
			Projectile.width = 12; // The width of projectile hitbox
			Projectile.height = 12; // The height of projectile hitbox
			Projectile.aiStyle = 1; // The ai style of the projectile, please reference the source code of Terraria
			Projectile.friendly = true; // Can the projectile deal damage to enemies?
			Projectile.hostile = false; // Can the projectile deal damage to the player?
			Projectile.DamageType = DamageClass.Ranged; // Is the projectile shoot by a ranged weapon?
			Projectile.penetrate = -1; // How many monsters the projectile can penetrate. (OnTileCollide below also decrements penetrate for bounces as well)
			Projectile.timeLeft = 4000; // The live time for the projectile (60 = 1 second, so 600 is 10 seconds)
			Projectile.alpha = 255; // The transparency of the projectile, 255 for completely transparent. (aiStyle 1 quickly fades the projectile in) Make sure to delete this if you aren't using an aiStyle that fades in. You'll wonder why your projectile is invisible.
			Projectile.light = 0.5f; // How much light emit around the projectile
			Projectile.ignoreWater = true; // Does the projectile's speed be influenced by water?
			Projectile.tileCollide = false; // Can the projectile collide with tiles?
			Projectile.extraUpdates = 5; // Set to above 0 if you want the projectile to update multiple time in a frame
			Projectile.ArmorPenetration = 100;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 10;
			Projectile.light = 0.4f;
			AIType = ProjectileID.Bullet; // Act exactly like default Bullet
		}
		public int portalcount = 2;
        public override bool PreDraw(ref Color lightColor)
        {
			lightColor = Color.White;
			Texture2D tex = Projectile.getTexture();
			Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(tex.Width, tex.Height / 2), Projectile.scale, SpriteEffects.None, 0);
			return false;
        }
        public void portalParticle(Vector2 pos)
        {
            for (int i = 0; i < 360; i += 20)
            {
                EParticle.spawnNew(new AbyssalLine() { lx = 0.27f, xadd = 0.27f }, pos, Vector2.Zero, Color.AliceBlue, 1, 1, true, BlendState.Additive, MathHelper.ToRadians(i));
            }
        }
		
        public override void AI()
        {
			Projectile.rotation = Projectile.velocity.ToRotation();
			portalTime--;
			if(portalTime == 0)
			{
				Projectile.damage = ((int)(Projectile.damage * 0.5f));
				NPC target = ptarget;
                portalParticle(Projectile.Center);
                Util.Util.PlaySound("portal_emerge", 1.6f, Projectile.Center, 12, 0.22f);
                Projectile.Center = target.Center + Util.Util.randomRot().ToRotationVector2() * 400;
                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.velocity.Length();
                Projectile.netUpdate = true;
                portalParticle(Projectile.Center);
            }
        }
		public NPC ptarget = null;
		public int portalTime = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
			Projectile.damage = (int)(Projectile.damage * 0.9f);
			Util.Util.PlaySound("ystn_hit", 1.6f, Projectile.Center, 1, 0.24f);
            if (portalcount > 0 && portalTime < 0)
			{
                portalcount--;
				portalTime = 28;
				ptarget = target;
			}
        }
    }
}