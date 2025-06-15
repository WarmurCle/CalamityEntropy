using CalamityEntropy.Content.Particles;
using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class StarSootInjector : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<StarSootSmoke>(), AmmoID.None, singleShotTime: 16, shotVelocity: 18, hasAutoReuse: true);
            Item.DamageType = DamageClass.Magic;
            Item.mana = 16;
            Item.width = 152;
            Item.height = 48;
            Item.useTime = 7;
            Item.damage = 35;
            Item.knockBack = 4f;
            Item.crit = 10;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-8f, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI).ToProj();
            return false;
        }
        public override void HoldItem(Player player)
        {
            if (player.itemTime > 0)
            {
                EParticle.NewParticle(new GlowLightParticle() { lightColor = Color.LightBlue * 0.55f }, player.MountedCenter, (player.itemRotation + (player.direction > 0 ? 0 : MathHelper.Pi)).ToRotationVector2().RotatedByRandom(0.24f) * Main.rand.NextFloat(10, 20) * 2.5f, Color.LightBlue, Main.rand.NextFloat(0.6f, 1.4f), 1, true, BlendState.Additive, 0, 30);
                EParticle.NewParticle(new Smoke() { timeleftmax = 30, Lifetime = 30, scaleStart = 0f, scaleEnd = Main.rand.NextFloat(0.6f, 1.4f) * 0.6f }, player.MountedCenter, (player.itemRotation + (player.direction > 0 ? 0 : MathHelper.Pi)).ToRotationVector2().RotatedByRandom(0.24f) * Main.rand.NextFloat(10, 20) * 2.5f, Color.LightBlue, 1, 1, true, BlendState.Additive, 0);
                EParticle.NewParticle(new Smoke() { timeleftmax = 30, Lifetime = 30, scaleStart = 0f, scaleEnd = Main.rand.NextFloat(0.6f, 1.4f) * 0.6f }, player.MountedCenter, (player.itemRotation + (player.direction > 0 ? 0 : MathHelper.Pi)).ToRotationVector2().RotatedByRandom(0.24f) * Main.rand.NextFloat(10, 20) * 2.5f, Color.LightBlue, 1, 1, true, BlendState.Additive, 0);
                EParticle.NewParticle(new Smoke() { timeleftmax = 30, Lifetime = 30, scaleStart = 0f, scaleEnd = Main.rand.NextFloat(0.6f, 1.4f) * 0.6f }, player.MountedCenter, (player.itemRotation + (player.direction > 0 ? 0 : MathHelper.Pi)).ToRotationVector2().RotatedByRandom(0.24f) * Main.rand.NextFloat(10, 20) * 2.5f, Color.LightBlue, 1, 1, true, BlendState.Additive, 0);
            }
        }
    }
    public class StarSootSmoke : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 40;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.MaxUpdates = 2;
            Projectile.Opacity = 0;
        }
        public override void AI()
        {
            int s = (int)(Utils.Remap(Projectile.timeLeft, 40, 0, 10, 128));
            Projectile.Resize(s, s);
        }

    }
}
