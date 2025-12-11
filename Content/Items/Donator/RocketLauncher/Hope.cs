using CalamityEntropy.Content.Items.Donator.RocketLauncher.Ammo;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items.Materials;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher
{
    public class Hope : ModItem
    {
        public static int MaxStick => 2;
        public static int ExplodeRadius => 60;
        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CharredMissleProj>(), BaseMissleProj.AmmoType, singleShotTime: 52, shotVelocity: 20f, hasAutoReuse: true);
            Item.width = 90;
            Item.height = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 16;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item61;
            Item.value = Item.buyPrice(silver: 30); 
            Item.rare = ItemRarityID.Orange;
            Item.Entropy().tooltipStyle = 8;
            Item.Entropy().strokeColor = Color.DarkBlue;
            Item.Entropy().NameColor = new Color(80, 220, 255);
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
            return new Vector2(-27f, -4f);
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AerialiteBar>(15)
                .AddIngredient<BloodSample>(10)
                .AddIngredient<OsseousRemains>(5)
                .AddIngredient(ItemID.Vertebrae, 10)
                .AddTile(TileID.Anvils)
                .Register();
            CreateRecipe()
                .AddIngredient<AerialiteBar>(15)
                .AddIngredient<RottenMatter>(10)
                .AddIngredient<OsseousRemains>(5)
                .AddIngredient(ItemID.RottenChunk, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position += (new Vector2(54, -16) * new Vector2(1, player.direction)).RotatedBy(velocity.ToRotation());
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, MaxStick, ExplodeRadius);
            p.ToProj().Entropy().applyBuffs.Add(BuffID.OnFire3);
            return false;
        }
    }
}
