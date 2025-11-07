using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.MainHammer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.Scarlet
{
    public class GodsHammer: BaseHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<GodsHammerProj>();
        public override void ExSD()
        {
            Item.width = Item.height = 86;
            Item.damage = 457;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 20f;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.value = Item.buyPrice(gold: 12);
        }
        //临时写一下，用于调试
        public override void UpdateInventory(Player player)
        {
            Item.damage = 457;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<NightmareHammer>().
                AddIngredient(ItemID.FragmentNebula, 15).
                AddIngredient<CosmiliteBar>(15).
                AddIngredient<AscendantSpiritEssence>(10).
                AddCondition(Condition.NearShimmer).
                Register();
        }
    }
}