using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityMod.Rarities;
using System.Collections.Generic;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs;
using Terraria.Graphics.Effects;
using ReLogic.Content;
using System.IO;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityMod.Buffs.DamageOverTime;
namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookmarkPactOfDecay : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ModContent.RarityType<Violet>();
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("PactOfDecay");
        public override Color tooltipColor => new Color(160, 6, 6);
        public override EBookProjectileEffect getEffect()
        {
            return new DecayPactBMEffect();
        }
    }

    public class DecayPactBMEffect : EBookProjectileEffect
    {
        public override void OnActive(EntropyBookHeldProjectile book)
        {
            int projtype = ModContent.ProjectileType<DecayPactMaelstrom>();
            book.ShootSingleProjectile(projtype, book.Projectile.Center, (Main.MouseWorld - book.Projectile.Center), 0.05f, 1);
        }
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 460);
        }
    }

    public class DecayPactMaelstrom : EBookBaseProjectile
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/white";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.localNPCHitCooldown = 3;
            Projectile.Opacity = 0;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(targetPos);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            targetPos = reader.ReadVector2();
        }
        NPC target = null;
        Vector2 targetPos = Vector2.Zero;
        public override void AI()
        {
            Projectile.penetrate = -1;
            base.AI();
            if (target == null || !target.active || target.dontTakeDamage)
            {
                target = Projectile.FindTargetWithinRange(2000);
            }
            if (Main.myPlayer == Projectile.owner)
            {
                targetPos = (target == null ? Main.MouseWorld : target.Center);
            }
            if(Util.Util.getDistance(Projectile.Center, targetPos) > 120)
            {
                Projectile.velocity += (targetPos - Projectile.Center).normalize() * 2f;
                Projectile.velocity *= 0.92f;
            }
            if(Util.Util.getDistance(Projectile.Center, Projectile.getOwner().Center) > 2600)
            {
                target = null;
                Projectile.Center = Projectile.getOwner().Center;
            }
            if(Projectile.owner.ToPlayer().GetModPlayer<CapricornBookmarkRecordPlayer>().EBookUsingTime > 1 && Projectile.timeLeft >= 19)
            {
                Projectile.timeLeft = 20;
            }
            if(Projectile.timeLeft == 20)
            {
                if(Projectile.Opacity < 1)
                {
                    Projectile.Opacity += 0.05f;
                }
            }
            else
            {
                Projectile.Opacity -= 0.05f;
            }
            Projectile.pushByOther(0.6f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(6))
            {
                base.OnHitNPC(target, hit, damageDone);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Util.Util.DrawGlow(Projectile.Center, Color.Black * Projectile.Opacity, Projectile.scale * 10 * Projectile.Opacity, false);
            Texture2D screamTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ScreamyFace", AssetRequestMode.ImmediateLoad).Value;
            lightColor.R = (byte)(255 * Projectile.Opacity);
            Main.spriteBatch.End();
            Effect shieldEffect = Filters.Scene["CalamityMod:HellBall"].GetShader().Shader;
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

            float noiseScale = 0.6f;
            Projectile.scale *= 0.8f;
            shieldEffect.Parameters["time"].SetValue(Main.GameUpdateCount / 60f * 0.6f);
            shieldEffect.Parameters["blowUpPower"].SetValue(3.2f);
            shieldEffect.Parameters["blowUpSize"].SetValue(0.4f);
            shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

            float opacity = Projectile.Opacity;
            shieldEffect.Parameters["shieldOpacity"].SetValue(opacity);
            shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

            Color edgeColor = Color.Black * opacity;
            Color shieldColor = Color.Lerp(Color.Red, Color.Magenta, 0.5f) * opacity;

            shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
            shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

            Vector2 pos = Projectile.Center - Main.screenPosition;

            float scale = 0.715f;
            Main.spriteBatch.Draw(screamTex, pos, null, Color.White, 0, screamTex.Size() * 0.5f, scale * Projectile.scale * Projectile.Opacity, 0, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D vortexTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SoulVortex").Value;
            Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/LargeBloom").Value;
            for (int i = 0; i < 10; i++)
            {
                float angle = MathHelper.TwoPi * i / 3f + Main.GlobalTimeWrappedHourly * MathHelper.TwoPi;
                Color outerColor = Color.Lerp(Color.Red, Color.Magenta, i * 0.15f);
                Color drawColor = Color.Lerp(outerColor, Color.Black, i * 0.2f) * 0.5f;
                drawColor.A = 0;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;

                drawPosition += (angle + Main.GlobalTimeWrappedHourly * i / 16f).ToRotationVector2() * 6f;
                Main.EntitySpriteDraw(vortexTexture, drawPosition, null, drawColor * Projectile.Opacity, -angle + MathHelper.PiOver2, vortexTexture.Size() * 0.5f, (Projectile.scale * (1 - i * 0.05f)) * Projectile.Opacity, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition, null, Color.Black * Projectile.Opacity, Projectile.rotation, centerTexture.Size() * 0.5f, (Projectile.scale * 0.9f) * Projectile.Opacity, SpriteEffects.None, 0);
            Projectile.scale /= 0.8f;
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
    }
}