using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Nemesis
{
    public class Nemesis : ModItem, IDevItem
    {
        public string DevName => "锯角";
        private int fireIndex;
        public override void SetDefaults()
        {
            Item.height = 154;
            Item.width = 154;
            Item.damage = 360;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = Item.useTime = 18;
            Item.scale = 1;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.UseSound = null;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<NemesisProj>();
            Item.shootSpeed = 18f;
            Item.SetKnifeHeld<NemesisHeld>();
            fireIndex = 0;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position
            , Vector2 velocity, int type, int damage, float knockback)
        {
            int newLevel = 0;
            if (++fireIndex > 6)
            {
                newLevel = 1;
                fireIndex = 0;
            }
            if (player.altFunctionUse == 2)
            {
                newLevel = 2;
            }
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, newLevel);
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<TheBurningSky>()
                .AddIngredient<GalactusBlade>()
                .AddIngredient<FadingRunestone>()
                .AddTile<VoidWellTile>()
                .Register();
        }
    }
}
