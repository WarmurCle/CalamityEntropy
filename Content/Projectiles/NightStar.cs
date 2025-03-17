using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class NightStar : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.friendly = true;
            Projectile.light = 1f;
            Projectile.timeLeft = 240;
            Projectile.ArmorPenetration = 16;
        }
        List<Vector2> odp = new List<Vector2>();
        public override void AI()
        {
            base.AI();
            Projectile.rotation = Projectile.velocity.ToRotation();
            for (int i = 0; i < 3; i++) {
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.3f + Util.Util.randomVec(2), color, 40, 0.6f, 1, 0.2f, true, 0, true));
            }
        }
        public override void PostAI()
        {
            base.PostAI();
            odp.Add(Projectile.Center);
            if(odp.Count > 20)
            {
                odp.RemoveAt(0);
            }
        }
        public override void ApplyHoming()
        {
            if (++Projectile.ai[2] > 12)
            {
                base.ApplyHoming();
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            odp.Add(Projectile.Center);
            Texture2D tex =Util.Util.getExtraTex("StarTexture");
            
            Main.spriteBatch.UseBlendState(BlendState.Additive);

            Main.spriteBatch.EnterShaderRegion(BlendState.Additive);
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/StreakSolid"));
            GameShaders.Misc["CalamityMod:ArtAttack"].Apply();
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth1, TrailColor1, (float _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);
            
            GameShaders.Misc["CalamityMod:ArtAttack"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak2"));
            PrimitiveRenderer.RenderTrail(odp, new PrimitiveSettings(TrailWidth2, TrailColor2, (float _) => Vector2.Zero, smoothen: true, pixelate: false, GameShaders.Misc["CalamityMod:ArtAttack"]), 180);

            Main.spriteBatch.ExitShaderRegion();


            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
            Util.Util.DrawGlow(Projectile.Center, color, Projectile.scale * 1.2f);
            Util.Util.DrawGlow(Projectile.Center, Color.White, Projectile.scale * 0.8f);

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Main.GlobalTimeWrappedHourly, tex.Size() / 2f, Projectile.scale * 0.6f, SpriteEffects.None, 0);
            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);

            odp.RemoveAt(odp.Count - 1);

            return false;
        }
        public override Color baseColor => new Color(60, 60, 255);
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            for(int i = 0; i < 12; i++)
            {
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.3f + Util.Util.randomVec(6), color, 40, 0.8f, 1, 0.2f, true, 0, true));
            }
        }
        public Color TrailColor1(float completionRatio)
        {
            Color result = this.color * Projectile.Opacity * (completionRatio);
            return result;
        }
        public float TrailWidth1(float completionRatio)
        {
            return 26 * (completionRatio) * Projectile.scale;
        }
        public Color TrailColor2(float completionRatio)
        {
            Color result = Color.White * Projectile.Opacity * (completionRatio);
            return result;
        }
        public float TrailWidth2(float completionRatio)
        {
            return 18 * (completionRatio) * Projectile.scale;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 12; i++)
            {
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.3f + Util.Util.randomVec(6), color, 40, 0.8f, 1, 0.2f, true, 0, true));
            }
            for (int i = 0; i < 6; i++)
            {
                EParticle.spawnNew(new WindParticle(), Projectile.Center, Vector2.Zero, new Color(80, 80, 255), 2, 1, true, BlendState.Additive, Util.Util.randomRot());
            }
        }
    }
}