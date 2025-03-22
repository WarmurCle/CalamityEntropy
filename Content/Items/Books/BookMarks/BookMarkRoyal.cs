
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkRoyal : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Royal");
        public override EBookProjectileEffect getEffect()
        {
            return new RoyalBMEffect();
        }

        public override Color tooltipColor => Color.SkyBlue;
    }

    public class RoyalBMEffect : EBookProjectileEffect
    {
        public override void OnActive(EntropyBookHeldProjectile book)
        {
            if (book.Projectile.owner == Main.myPlayer)
            {
                book.ShootSingleProjectile(ModContent.ProjectileType<JewelRuby>(), book.Projectile.Center, Vector2.Zero);
                book.ShootSingleProjectile(ModContent.ProjectileType<JewelEmerald>(), book.Projectile.Center, Vector2.Zero);
                book.ShootSingleProjectile(ModContent.ProjectileType<JewelSapphire>(), book.Projectile.Center, Vector2.Zero);
            }
        }
        
    }

    public abstract class RoyalJewel : EBookBaseProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Books/BookMarks/Jewel";

        public override void ApplyHoming()
        {
        }

        public virtual Color getColor => Color.White;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 28;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(Projectile.getDrawData(this.getColor));
            return false;
        }
        public NPC target = null;
        public override void AI()
        {
            Projectile.penetrate = -1;
            if (ShooterModProjectile is EntropyBookHeldProjectile eb)
            {
                if (!eb.active || !ShooterModProjectile.Projectile.active)
                {
                    Projectile.Kill();
                    return;
                }
                else{
                    Projectile.timeLeft = 4;
                }
            }
            else
            {
                Projectile.Kill();
                return;
            }
            if (target == null || !target.active || target.dontTakeDamage || !target.chaseable)
            {
                target = Projectile.FindTargetWithinRange(this.homingRange * 5);
            }
            base.AI();
        }
    }
    public class RubyProj : EBookBaseProjectile
    {
        
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = Projectile.height = 16;
            Projectile.light = 0.3f;
        }
        public override void AI()
        {
            base.AI();
            Projectile.rotation += (Projectile.velocity.X > 0 ? 1 : -1) * 0.2f;
            for(float i = 0; i < 1; i += 0.1f)
            {
                int ruby = Dust.NewDust(Projectile.position - Projectile.velocity * i, Projectile.width, Projectile.height, DustID.GemRuby, Projectile.velocity.X, Projectile.velocity.Y, 90, new Color(), 1.2f);
                Dust dust = Main.dust[ruby];
                dust.noGravity = true;
                dust.velocity *= 0.3f;
            }
        }
    }
    public class JewelRuby : RoyalJewel
    {
        public override Color getColor => new Color(255, 90, 90);
        public override void AI()
        {
            base.AI();
            Vector2 targetPos = Projectile.getOwner().Center + new Vector2(0, -165);
            Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.6f;
            Projectile.velocity *= 0.98f;
            Projectile.rotation = Projectile.velocity.X * 0.06f;
            if(target != null && Main.myPlayer == Projectile.owner)
            {
                if(shootCooldown <= 0)
                {
                    shootCooldown = ((EntropyBookHeldProjectile)ShooterModProjectile).GetShootCd() * 3;
                    ((EntropyBookHeldProjectile)ShooterModProjectile).ShootSingleProjectile(ModContent.ProjectileType<RubyProj>(), Projectile.Center, (target.Center - Projectile.Center), 0.2f, shotSpeedMul: 0.4f);
                }
            }
            if(shootCooldown > 0)
            {
                shootCooldown--;
            }
        }
        public int shootCooldown = 0;
    }
    public class JewelEmerald : RoyalJewel
    {
        public override Color getColor => new Color(90, 255, 90);
        public int dashTime = 0;
        public override void AI()
        {
            base.AI();
            Vector2 targetPos = Projectile.getOwner().Center + new Vector2(0, -120);
            if (dashTime > 0)
            {
                dashTime--;
                Projectile.rotation += (Projectile.velocity.X > 0 ? 1 : -1) * 0.4f;
                for (float i = 0; i < 1; i += 0.1f)
                {
                    int d = Dust.NewDust(Projectile.position - Projectile.velocity * i, Projectile.width, Projectile.height, DustID.GemEmerald, Projectile.velocity.X, Projectile.velocity.Y, 90, new Color(), 1.2f);
                    Dust dust = Main.dust[d];
                    dust.noGravity = true;
                    dust.velocity *= 0.6f;
                }
            }
            else
            {
                Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.6f;
                Projectile.velocity *= 0.98f;
                Projectile.rotation = Projectile.velocity.X * 0.06f;
            }
            if (target != null && Main.myPlayer == Projectile.owner)
            {
                if (dashCooldown <= 0 && dashTime <= 0)
                {
                    dashCooldown = ((EntropyBookHeldProjectile)ShooterModProjectile).GetShootCd() * 12;
                    dashTime = 60;
                    Projectile.velocity = (target.Center - Projectile.Center).normalize() * 20;
                }
            }

            if (dashCooldown > 0)
            {
                dashCooldown--;
            }
        }
        public int dashCooldown = 0;
    }
    public class JewelSapphire : RoyalJewel
    {
        public override Color getColor => new Color(90, 90, 255);
        public override void AI()
        {
            base.AI();
            Vector2 targetPos = Projectile.getOwner().Center;
            Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.06f;
            if (Projectile.Distance(targetPos) > 260)
            {
                Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.4f;
                Projectile.velocity *= 0.98f;
                Projectile.rotation = Projectile.velocity.X * 0.06f;
            }
            Projectile.velocity *= 0.992f;
            float dist = 128;
            for (float i = 0; i < 360; i += 1)
            {
                Vector2 p = Projectile.Center + MathHelper.ToRadians(i).ToRotationVector2() * dist;
                Dust dust = Dust.NewDustPerfect(p, 229);
                dust.position = p;
                dust.scale = 0.7f;
                dust.noGravity = true;
                dust.velocity = Projectile.velocity;
            }
        }
    }
}