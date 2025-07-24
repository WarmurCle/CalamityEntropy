using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class BrambleShoot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 280;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            Projectile.rotation += 0.16f;
            NPC target = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 1400);
            if (target != null && drawcount > 9)
            {
                Projectile.velocity *= 0.94f;
                Vector2 v = target.Center - Projectile.Center;
                v.Normalize();

                Projectile.velocity += v * 3;
            }
            drawcount++;
        }
        float drawcount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, new Color(0, 255, 0), new Vector2(2f, 2f), 0, 0f, 0.2f, 8);
            GeneralParticleHandler.SpawnParticle(pulse);
        }
        public override string Texture => CEUtils.WhiteTexPath;
        private float PrimitiveWidthFunction(float completionRatio)
        {
            float arrowheadCutoff = 0.36f;
            float width = 39f;
            float minHeadWidth = 0.02f;
            float maxHeadWidth = width;
            if (completionRatio <= arrowheadCutoff)
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(0f, arrowheadCutoff, completionRatio, true));
            return width * 0.7f;
        }
        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LawnGreen, Color.Green, MathHelper.Clamp(completionRatio * 0.8f, 0f, 1f)) * base.Projectile.Opacity;
        }

        public float WidthFunction(float completionRatio)
        {
            float num = 8f;
            float num2 = ((!(completionRatio < 0.1f)) ? MathHelper.Lerp(num, 0f, Utils.GetLerpValue(0.1f, 1f, completionRatio, clamped: true)) : ((float)Math.Sin(completionRatio / 0.1f * (MathF.PI / 2f)) * num + 0.1f));
            return num2 * base.Projectile.Opacity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            base.Projectile.oldPos[0] = base.Projectile.position + base.Projectile.velocity.SafeNormalize(Vector2.Zero) * 50f;
            PrimitiveRenderer.RenderTrail(base.Projectile.oldPos, new PrimitiveSettings(WidthFunction, ColorFunction, (float _) => base.Projectile.Size * 0.5f + base.Projectile.velocity), 80);
            Texture2D value = CEUtils.getExtraTex("Leaf");
            Main.EntitySpriteDraw(value, base.Projectile.Center - Main.screenPosition, null, Color.White, base.Projectile.rotation, value.Size() * 0.5f, base.Projectile.scale, SpriteEffects.None);
            return false;
        }
    }


}