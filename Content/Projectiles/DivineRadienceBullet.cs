using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class DivineRadienceBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 14;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            drawcount++;
            Projectile.velocity *= 0.98f;
            if (Projectile.ai[2]++ >= 12)
            {
                Projectile.HomingToNPCNearby(3, 0.86f, 210);
            }
            if (Projectile.timeLeft < 25f)
                Projectile.Opacity -= 1 / 25f;
        }
        float drawcount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("CrystalBreak", 1.2f, target.Center, 8, 0.7f);
            float s = 1f;
            for (int i = 0; i < 12; i++)
                GeneralParticleHandler.SpawnParticle(new GlowSparkParticle(target.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(0.6f, 1) * 24 * s, false, 9, 0.024f * Main.rand.NextFloat(0.65f, 1f) * s, Main.rand.NextBool() ? Color.Firebrick : Color.Red, new Vector2(2.4f, 1), true));

        }
        private float PrimitiveWidthFunction(float completionRatio, Vector2 vertex)
        {
            float arrowheadCutoff = 0.36f;
            float width = 39f;
            float minHeadWidth = 0.02f;
            float maxHeadWidth = width;
            if (completionRatio <= arrowheadCutoff)
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(0f, arrowheadCutoff, completionRatio, true));
            return width;
        }
        private Color PrimitiveColorFunction(float completionRatio, Vector2 vertex)
        {
            float endFadeRatio = 0.41f;
            float completionRatioFactor = 2.7f;
            float globalTimeFactor = 5.3f;
            float endFadeFactor = 3.2f;
            float endFadeTerm = Utils.GetLerpValue(0f, endFadeRatio * 0.5f, completionRatio, true) * endFadeFactor;
            float cosArgument = completionRatio * completionRatioFactor - Main.GlobalTimeWrappedHourly * globalTimeFactor + endFadeTerm;
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f;

            float colorLerpFactor = 0.6f;
            Color startingColor = Color.Lerp(ShaderColorOne, ShaderColorTwo, startingInterpolant * colorLerpFactor);

            return Color.Lerp(startingColor, ShaderEndColor, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, endFadeRatio, completionRatio, true))) * Projectile.Opacity;
        }
        private float PrimitiveWidthFunction2(float completionRatio, Vector2 vertex)
        {
            float arrowheadCutoff = 0.36f;
            float width = 18;
            float minHeadWidth = 0.02f;
            float maxHeadWidth = width;
            if (completionRatio <= arrowheadCutoff)
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(0f, arrowheadCutoff, completionRatio, true));
            return width;
        }
        private Color PrimitiveColorFunction2(float completionRatio, Vector2 vertex)
        {
            float endFadeRatio = 0.41f;
            float completionRatioFactor = 2.7f;
            float globalTimeFactor = 5.3f;
            float endFadeFactor = 3.2f;
            float endFadeTerm = Utils.GetLerpValue(0f, endFadeRatio * 0.5f, completionRatio, true) * endFadeFactor;
            float cosArgument = completionRatio * completionRatioFactor - Main.GlobalTimeWrappedHourly * globalTimeFactor + endFadeTerm;
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f;

            float colorLerpFactor = 0.6f;
            Color startingColor = Color.Lerp(ShaderColorOne, ShaderColorTwo, startingInterpolant * colorLerpFactor);

            return Color.Lerp(startingColor, ShaderEndColor, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, endFadeRatio, completionRatio, true))) * 4 * Projectile.Opacity;
        }
        private static Color ShaderColorOne = new Color(237, 66, 66);
        private static Color ShaderColorTwo = new Color(235, 110, 110);
        private static Color ShaderEndColor = new Color(199, 36, 36);
        
        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            Vector2 overallOffset = Projectile.Size * 0.5f;
            overallOffset += Projectile.velocity * 1.4f;
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction2, PrimitiveColorFunction2, (_, _) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]));
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
    }


}