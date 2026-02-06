using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class SpectralWhispers : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 8;
            Item.useAnimation = Item.useTime = 38;
            Item.crit = 4;
            Item.mana = 11;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.width = 40;
            Item.height = 52;
            Item.shootSpeed = 32;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/SW").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<SpectralWhispersHeld>();
        public override int SlotCount => 1;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DemoniteBar, 10)
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class SpectralWhispersHeld : EntropyBookDrawingAlt
    {
        public override int frameChange => 4;
        public override string OpenAnimationPath => $"{EntropyBook.BaseFolder}/Textures/SpectralWhispers/Open";
        public override string PageAnimationPath => $"{EntropyBook.BaseFolder}/Textures/SpectralWhispers/Page";
        public override string UIOpenAnimationPath => $"{EntropyBook.BaseFolder}/Textures/SpectralWhispers/UI";

        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.armorPenetration += 40;
            return m;
        }
        public override void AI()
        {
            base.AI();
            Projectile.GetOwner().Calamity().mouseWorldListener = true;
        }
        public override bool Shoot()
        {
            base.Shoot();
            base.Shoot();
            base.Shoot();
            return true;
        }
        public override float randomShootRotMax => 0;
        public override int baseProjectileType => ModContent.ProjectileType<WhisperingSpike>();
    }
    public class WhisperingSpike : EBookBaseProjectile
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.MaxUpdates = 2;
            Projectile.tileCollide = true;
            Projectile.light = 0.35f;
            Projectile.timeLeft = 800;
            Projectile.penetrate = 1;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 9 * Projectile.scale, Projectile.Center - Projectile.rotation.ToRotationVector2() * 9 * Projectile.scale, targetHitbox, (int)(Projectile.scale * 12));
        }
        public int Penetrate = -1;
        public override bool PreAI()
        {
            if (Projectile.localAI[0]++ == 1)
            {
                Penetrate = Projectile.penetrate;
                Projectile.penetrate = -1;
            }
            return base.PreAI();
        }
        public override void ApplyHoming()
        {
            if(ShouldUpdatePosition())
                base.ApplyHoming();
        }
        public override void AI()
        {
            base.AI();
            HideTime--;
            
            if (Projectile.localAI[0] == 1)
            {
                CEUtils.PlaySound("SoulSpawn" + Main.rand.Next(2).ToString(), Main.rand.NextFloat(0.8f, 1.2f), Projectile.Center);
                Projectile.Center -= Projectile.velocity.normalize().RotatedByRandom(1.6f) * 120;
            }
            if (HideTime == 0)
            {
                for(int i = 0; i < 12; i++)
                {
                    GeneralParticleHandler.SpawnParticle(new CalamityMod.Particles.CritSpark(Projectile.Center, CEUtils.randomPointInCircle(9), color * 4, color, Main.rand.NextFloat(0.5f, 1.2f) * Projectile.scale, 18, 0.3f));
                }
            }
            if (stick > 0)
            {
                NPC t = ((int)Projectile.ai[2]).ToNPC();
                Projectile.tileCollide = false;
                if (stick == 1)
                    Projectile.Center = t.Center + stickOffset;
                size *= 1.016f;
                if (size >= 2.4f)
                    Projectile.Kill();
                if (!t.active)
                    stick = 2;
            }
            if (Projectile.localAI[0] < 24)
            {
                Projectile.position += Projectile.GetOwner().velocity / 2;
                Projectile.velocity = (Projectile.GetOwner().Calamity().mouseWorld - Projectile.Center).normalize() * Projectile.velocity.Length();
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public int HideTime = Main.rand.Next(1, 16);
        public override Color baseColor => new Color(42, 40, 82);
        public float size = 1;
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 offset = Vector2.Zero;
            if (HideTime > 0)
                return false;
            Texture2D tex1 = Projectile.GetTexture();
            Texture2D tex2 = this.getTextureAlt();
            float scale = size * Projectile.scale;
            CEUtils.DrawGlow(Projectile.Center, color, scale * 1.4f);
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex1, Projectile.Center + offset - Main.screenPosition, null, color * 4f, Projectile.rotation + MathHelper.PiOver2, tex1.Size() / 2, scale * 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex1, Projectile.Center + offset - Main.screenPosition, null, Color.White, Projectile.rotation + MathHelper.PiOver2, tex1.Size() / 2, scale * 0.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex2, Projectile.Center + offset - Main.screenPosition, null, color, Projectile.rotation, tex2.Size() / 2, Projectile.scale * (size - 1) * 5f, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.GetOwner(), Projectile.Center, Projectile.damage, 130, Projectile.DamageType).ArmorPenetration = Projectile.ArmorPenetration + 10;
            for (float i = 0; i <= 1; i += 0.05f)
            {
                GeneralParticleHandler.SpawnParticle(new CustomPulse(Projectile.Center, Vector2.Zero, Color.Lerp(color * 4.5f, color, i) * 0.8f, "CalamityMod/Particles/FlameExplosion", Vector2.One, Main.rand.NextFloat(-10, 10), 0.005f, i * 0.165f, (int)((1.2f - i) * 20)));
            }
            SoundEngine.PlaySound(SoundID.Item122 with { PitchRange = (1.2f, 1.6f) }, Projectile.Center);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Penetrate--;
            if (Penetrate == 0)
            {
                Projectile.ai[2] = target.whoAmI;
                stick = 1;
                stickOffset = Projectile.Center - target.Center;
                CEUtils.SyncProj(Projectile.whoAmI);
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stick);
            writer.WriteVector2(stickOffset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stick = reader.ReadInt32();
            stickOffset = reader.ReadVector2();
        }
        public override bool? CanHitNPC(NPC target)
        {
            return ShouldUpdatePosition();
        }
        public override bool ShouldUpdatePosition()
        {
            if (stick > 0 || Projectile.localAI[0] < 24)
                return false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = oldVelocity;
            if (stick == -1)
            {
                stick = 2;
            }
            CEUtils.SyncProj(Projectile.whoAmI);
            return false;
        }
        public Vector2 stickOffset = Vector2.Zero;
        public int stick = -1;
    }
}
