using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Aquashard : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.damage = 22;
            Item.ArmorPenetration = 6;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.ArmorPenetration = 86;
            Item.knockBack = 1f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<AquashardThrow>();
            Item.shootSpeed = 36f;
            Item.DamageType = CUtil.rogueDC;
        }



        public override float StealthDamageMultiplier => 1.2f;
        public override float StealthVelocityMultiplier => 1.5f;
        public override float StealthKnockbackMultiplier => 3f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    p.ToProj().netUpdate = true;
                    p.ToProj().penetrate = 5;
                }
                return false;
            }
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<SeaPrism>(), 8)
                .AddIngredient(ModContent.ItemType<PearlShard>(), 4)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
