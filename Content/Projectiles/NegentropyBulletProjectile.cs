using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class NegentropyBulletProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 12; 
            Projectile.height = 12; 
            Projectile.aiStyle = 1; 
            Projectile.friendly = true; Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged; 
            Projectile.penetrate = -1; 
            Projectile.timeLeft = 2400; 
            Projectile.alpha = 255; 
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true; 
            Projectile.tileCollide = false; 
            Projectile.extraUpdates = 5; 
            Projectile.ArmorPenetration = 100;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.light = 0.4f;
            AIType = ProjectileID.Bullet;
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
            for (int i = 0; i < 360; i += 40)
            {
                EParticle.spawnNew(new AbyssalLine() { lx = 2f, xadd = 0.27f }, pos, Vector2.Zero, Color.AliceBlue, 1, 1, true, BlendState.Additive, MathHelper.ToRadians(i));
            }
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            portalTime--;
            if (portalTime == 0)
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