using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class BloodCodex : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 12;
            Item.useAnimation = Item.useTime = 9;
            Item.crit = 6;
            Item.mana = 3;
            Item.rare = ItemRarityID.Blue;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.width = 40;
            Item.height = 52;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/BCdx").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<BloodCodexHeld>();
        public override int SlotCount => 4;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.CrimtaneBar, 10)
                .AddIngredient(ItemID.Silk, 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class BloodCodexHeld : EntropyBookDrawingAlt
    {
        public override int frameChange => 2;
        public override string OpenAnimationPath => $"{EntropyBook.BaseFolder}/Textures/BloodCodex/Open";
        public override string PageAnimationPath => $"{EntropyBook.BaseFolder}/Textures/BloodCodex/Page";
        public override string UIOpenAnimationPath => $"{EntropyBook.BaseFolder}/Textures/BloodCodex/UI";

        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.lifeSteal++;
            m.armorPenetration += 25;
            return m;
        }
        public override bool Shoot()
        {
            base.Shoot();
            EParticle.NewParticle(new Smoke() { timeleftmax = 30, Lifetime = 30, scaleStart = 0.01f, scaleEnd = Main.rand.NextFloat(0.36f, 0.8f) * 0.6f }, Projectile.Center, Projectile.rotation.ToRotationVector2().RotatedByRandom(randomShootRotMax) * Main.rand.NextFloat(4, 20) * 1f, Color.Red, 0.5f, 1, true, BlendState.Additive, 0);
            EParticle.NewParticle(new Smoke() { timeleftmax = 30, Lifetime = 30, scaleStart = 0.01f, scaleEnd = Main.rand.NextFloat(0.36f, 0.8f) * 0.6f }, Projectile.Center, Projectile.rotation.ToRotationVector2().RotatedByRandom(randomShootRotMax) * Main.rand.NextFloat(4, 20) * 1f, Color.Red, 0.5f, 1, true, BlendState.Additive, 0);
            EParticle.NewParticle(new Smoke() { timeleftmax = 30, Lifetime = 30, scaleStart = 0.01f, scaleEnd = Main.rand.NextFloat(0.36f, 0.8f) * 0.6f }, Projectile.Center, Projectile.rotation.ToRotationVector2().RotatedByRandom(randomShootRotMax) * Main.rand.NextFloat(4, 20) * 1f, Color.Red, 0.5f, 1, true, BlendState.Additive, 0);

            return true;
        }
        public override float randomShootRotMax => 0.05f;
        public override int baseProjectileType => ModContent.ProjectileType<BloodSpray>();
    }
    public class BloodSpray : EBookBaseProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.MaxUpdates = 2;
            Projectile.tileCollide = true;
            Projectile.light = 0.8f;
            Projectile.timeLeft = 400;
            Projectile.penetrate = 1;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for(int i = 0; i < 12; i++)
            {
                GeneralParticleHandler.SpawnParticle(new BloodParticle(Projectile.Center, CEUtils.randomPointInCircle(16), 26, Main.rand.NextFloat(0.6f, 1f), baseColor));
            }
        }
        public override void AI()
        {
            base.AI();
            if (Projectile.localAI[2]++ > 10)
                Projectile.velocity.Y += 0.26f;
            GeneralParticleHandler.SpawnParticle(new BloodParticle(Projectile.Center + Projectile.velocity + CEUtils.randomPointInCircle(5), Projectile.velocity * Main.rand.NextFloat(1f, 1.4f), 22, Main.rand.NextFloat(0.6f, 1f), baseColor));
            if (Projectile.localAI[2] % 2 == 0)
            {
                for (float i = 0; i < 1; i += 0.1f)
                    EParticle.NewParticle(new Smoke() { timeleftmax = 30, Lifetime = 30 }, Projectile.Center - Projectile.velocity * i * 2, Projectile.rotation.ToRotationVector2() * 5 * 1f, new Color(255, 30, 30), 0.04f, 1, true, BlendState.Additive, 0);
            }
        }
        public override Color baseColor => Color.Red;
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff<FlamingBlood>(10 * 60);
        }
    }
}
