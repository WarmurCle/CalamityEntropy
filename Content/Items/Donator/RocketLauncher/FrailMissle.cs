using CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher
{
    public class FrailMissle : ModItem
    {
        public static int MaxStick => 3;
        public static int ExplodeRadius => 120;
        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CharredMissleProj>(), BaseMissleProj.AmmoType, singleShotTime: 70, shotVelocity: 25f, hasAutoReuse: true);
            Item.width = 90;
            Item.height = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 40;
            Item.knockBack = 4f;
            Item.UseSound = CEUtils.GetSound("aprclaunch", 1.3f);
            Item.value = Item.buyPrice(silver: 30);
            Item.rare = ItemRarityID.Green;
            Item.Entropy().tooltipStyle = 8;
            Item.Entropy().strokeColor = new Color(40, 0, 0);
            Item.Entropy().NameColor = new Color(160, 0, 0);
            Item.Entropy().NameLightColor = Color.Black;
        }

        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 76f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = -HoldoutOffset().Value;
            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.5)
                rotation += (-0.2f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-20f, -4f);
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        #endregion
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position += (new Vector2(64, -16) * new Vector2(1, player.direction)).RotatedBy(velocity.ToRotation());
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, MaxStick, ExplodeRadius);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<PurifiedGel>(10)
                .AddIngredient<BloodOrb>(6)
                .AddIngredient<SparkSpreader>()
                .AddTile(TileID.Anvils)
                .Register();
        }


    }
}
