using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Kinanition : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 36;
            Item.height = 110;
            Item.useTime = 19;
            Item.useAnimation = 5;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item5;
            Item.shoot = ProjectileID.WoodenArrowFriendly;
            Item.shootSpeed = 46f;
            Item.useAmmo = AmmoID.Arrow;
            Item.autoReuse = true;
            Item.ArmorPenetration = 12;
            Item.value = 8000;
            Item.rare = ItemRarityID.Purple;

        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 16;

        public override Vector2? HoldoutOffset() => new Vector2(-42, 0);

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.shoot = ModContent.ProjectileType<LightningSpear>();
            }
            else
            {
                Item.shoot = ProjectileID.WoodenArrowFriendly;
            }
            return base.UseItem(player);
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2f)
            {
                Item.knockBack = 6f;
                SoundStyle usd = new SoundStyle("CalamityMod/Sounds/Item/RealityRuptureStealth", SoundType.Sound);
                usd.Volume = 0.4f;
                Item.UseSound = usd;

                Item.shootSpeed = 60f;
                Item.useAmmo = AmmoID.None;
                Item.autoReuse = true;
                Item.useTime = 36;
                Item.useAnimation = 36;
                Item.shoot = ModContent.ProjectileType<LightningSpear>();
            }
            else
            {
                Item.knockBack = 6f;
                Item.UseSound = SoundID.Item5;
                Item.shootSpeed = 42f;
                Item.autoReuse = true;
                Item.useAmmo = AmmoID.Arrow;
                Item.useTime = 7;
                Item.useAnimation = 40;
                Item.shoot = ProjectileID.WoodenArrowFriendly;
            }
            return base.CanUseItem(player);
        }
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            if (player.itemAnimation < 38)
            {
                return false;
            }
            return base.CanConsumeAmmo(ammo, player);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, type, damage * 12, knockback, player.whoAmI);
            }
            else
            {
                if (player.itemAnimation < player.itemAnimationMax / 2)
                {

                    return false;
                }
                SoundEngine.PlaySound(Item.UseSound);
                if (CalamityUtils.CheckWoodenAmmo(type, player))
                    type = ProjectileID.WoodenArrowFriendly;

                int j = 2;
                var var = Main.rand;
                for (int i = 0; i < 2 + player.Entropy().WeaponBoost; i++)
                {
                    if (i == 0)
                    {
                    }
                    else
                    {
                        int arrow;
                        arrow = Projectile.NewProjectile(source, position + player.itemRotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * j + player.itemRotation.ToRotationVector2() * Main.rand.Next(4, 16), velocity.RotatedBy(MathHelper.ToRadians((float)Main.rand.Next(0, 7) - 3f)) * 2, type, damage / 2, knockback, player.whoAmI);
                        Main.projectile[arrow].Entropy().Lightning = true;
                        Main.projectile[arrow].penetrate = 5;
                        arrow = Projectile.NewProjectile(source, position + player.itemRotation.ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * j + player.itemRotation.ToRotationVector2() * Main.rand.Next(4, 16), velocity.RotatedBy(MathHelper.ToRadians((float)Main.rand.Next(0, 7) - 3f)) * 2, type, damage / 2, knockback, player.whoAmI);
                        Main.projectile[arrow].Entropy().Lightning = true;
                        Main.projectile[arrow].penetrate = 5;
                    }
                }
            }

            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<Barinade>(), 1).
                AddIngredient(ModContent.ItemType<Barinautical>(), 1).
                AddIngredient(ModContent.ItemType<Lumenyl>(), 20).
                AddIngredient(ModContent.ItemType<LifeAlloy>(), 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
