using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Zyphros : ModItem
    {
        public override bool RangedPrefix()
        {
            return true;
        }
        public override void SetDefaults()
        {
            Item.damage = 720;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 36;
            Item.height = 110;
            Item.useTime = 3;
            Item.useAnimation = 9;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = null;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Arrow;
            Item.autoReuse = true;
            Item.ArmorPenetration = 12;
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.rare = ModContent.RarityType<AbyssalBlue>();

        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 10;

        public override Vector2? HoldoutOffset() => new Vector2(-42, -8);

        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return Main.rand.NextBool(12);
        }

        public override bool? UseItem(Player player)
        {
            CEUtils.PlaySound("zypshot" + Main.rand.Next(1, 3).ToString(), Main.rand.NextFloat(1f, 1.6f), player.Center, 3, 0.3f);
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (Main.rand.NextBool(24))
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ZyphrosCrystal>(), damage, knockback, player.whoAmI, Main.rand.Next(1, 6));
            }
            player.Entropy().itemTime = 3;
            int arrow;
            arrow = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(2)), type, damage, knockback, player.whoAmI);
            arrow.ToProj().Entropy().zypArrow = true;
            arrow.ToProj().ArmorPenetration += 30;
            CEUtils.SyncProj(arrow);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<WyrmTooth>(), 14)
                .AddIngredient(ModContent.ItemType<Drataliornus>(), 1)
                .AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5)
                .AddTile(ModContent.TileType<AbyssalAltarTile>())
                .Register();
        }

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 76f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(28, 0);
            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
    }
}
