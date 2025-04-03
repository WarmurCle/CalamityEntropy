using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class Revelation : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.damage = 210;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 16;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice / 4; Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<RevelationThrow>();
            Item.shootSpeed = 45f;
            Item.DamageType = CUtil.rogueDC;
            Item.rare = ItemRarityID.Red;
            Item.Entropy().tooltipStyle = 3;
            Item.Entropy().NameColor = new Color(160, 0, 0);
            Item.Entropy().stroke = true;
            Item.Entropy().strokeColor = new Color(90, 0, 0);
            Item.Entropy().HasCustomStrokeColor = true;
            Item.Entropy().HasCustomNameColor = true;
        }

        public override void ModifyStatsExtra(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
                type = ModContent.ProjectileType<RevelationMelee>();
        }


        public override float StealthDamageMultiplier => 4f;
        public override float StealthVelocityMultiplier => 1f;
        public override float StealthKnockbackMultiplier => 4f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<MeldConstruct>(), 5);
            recipe.AddIngredient(ModContent.ItemType<TwistingNether>(), 2);
            recipe.AddIngredient(ModContent.ItemType<ShardofAntumbra>(), 1);
            recipe.AddIngredient(ItemID.DeathSickle, 1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
