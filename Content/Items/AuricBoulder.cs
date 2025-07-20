using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Tiles;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class AuricBoulder : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
            ItemID.Sets.SortingPriorityMaterials[Type] = 119;
        }

        public override void SetDefaults()
        {

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(gold: 4);
            Item.rare = ModContent.RarityType<Violet>();
            if (ModLoader.HasMod("MoreBoulders") && ModLoader.TryGetMod("MoreBoulders", out var mbMod))
            {
                Item.shoot = ModContent.ProjectileType<AuricBoulderProjFriendly>();
                Item.shootSpeed = 14;

                Item.damage = 1800;
                Item.ammo = 540;
                Item.crit = 6;
                Item.DamageType = mbMod.Find<DamageClass>("RollingDamage");
            }
            Item.DefaultToPlaceableTile(ModContent.TileType<AuricBoulderTile>(), 0);
            Item.width = 32;
            Item.height = 32;
        }

        public override bool CanShoot(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<AuricBar>(), 1).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
