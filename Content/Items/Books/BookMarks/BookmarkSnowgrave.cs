using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookmarkSnowgrave : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Snowgrave");
        public override EBookProjectileEffect getEffect()
        {
            return new SnowgraveBMEffect();
        }
        public override Color tooltipColor => new Color(160, 160, 255);
    }
    public class SnowgraveBMEffect : EBookProjectileEffect
    {
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            int sgtype = ModContent.ProjectileType<Snowgrave>();
            if (projectile.GetOwner().ownedProjectileCounts[sgtype] < 1)
            {
                projectile.GetOwner().Entropy().SnowgraveChargeTime = 20;
                if (projectile.GetOwner().Entropy().SnowgraveCharge >= 1 && projectile.ModProjectile is EBookBaseProjectile ebp && ebp.ShooterModProjectile is EntropyBookHeldProjectile eb)
                {
                    projectile.GetOwner().Entropy().SnowgraveCharge = 0;
                    projectile.GetOwner().Entropy().SnowgraveChargeTime = 0;
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), target.Center, Vector2.Zero, sgtype, eb.CauculateProjectileDamage(0.36f), 0.2f, projectile.owner);
                }
            }
        }
    }
    public class Snowgrave : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Particles/SnowPiece";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Magic, false, -1);
            Projectile.width = 360;
            Projectile.height = 2048;
            Projectile.timeLeft = 360;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
                CEUtils.PlaySound("Snowgrave", 1, Projectile.Center, 2, 4);

            for (float i = 0.2f; i <= 1; i += 0.2f)
            {
                EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(-30, -Math.Abs(i) * 40) + new Vector2((0.4f + 0.6f * (float)(Math.Sin(Main.GameUpdateCount * 0.35f) * 0.5f + 0.5f)) * i * 160, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);
                EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(-30, -Math.Abs(i) * 40) + new Vector2(0, -40) + new Vector2((0.4f + 0.6f * (float)(Math.Sin(Main.GameUpdateCount * 0.35f) * 0.5f + 0.5f)) * i * 160, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);
                EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(30, -Math.Abs(i) * 40) + new Vector2((0.4f + 0.6f * (float)(Math.Sin(Main.GameUpdateCount * 0.35f) * 0.5f + 0.5f)) * i * 160, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);
                EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(30, -Math.Abs(i) * 40) + new Vector2(0, -40) + new Vector2((0.4f + 0.6f * (float)(Math.Sin(Main.GameUpdateCount * 0.35f) * 0.5f + 0.5f)) * i * 160, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);

                EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(-30, -Math.Abs(i) * 40) + new Vector2(-(0.4f + 0.6f * (float)(Math.Sin(Main.GameUpdateCount * 0.35f) * 0.5f + 0.5f)) * i * 160, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);
                EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(-30, -Math.Abs(i) * 40) + new Vector2(0, -40) + new Vector2(-(0.4f + 0.6f * (float)(Math.Sin(Main.GameUpdateCount * 0.35f) * 0.5f + 0.5f)) * i * 160, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);
                EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(30, -Math.Abs(i) * 40) + new Vector2(-(0.4f + 0.6f * (float)(Math.Sin(Main.GameUpdateCount * 0.35f) * 0.5f + 0.5f)) * i * 160, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);
                EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(30, -Math.Abs(i) * 40) + new Vector2(0, -40) + new Vector2(-(0.4f + 0.6f * (float)(Math.Sin(Main.GameUpdateCount * 0.35f) * 0.5f + 0.5f)) * i * 160, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);
            }
            //EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(0, 0) + new Vector2(0, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);
            //EParticle.NewParticle(new SnowPiece() { owner = Projectile.GetOwner() }, Projectile.Center + new Vector2(0, 0) + new Vector2(0, -40) + new Vector2(0, 1024), new Vector2(0, -80), Color.White, 1, 1, true, BlendState.AlphaBlend, 0);

        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.timeLeft < 330;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<GlacialState>(250);
        }
    }
    
}