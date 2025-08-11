using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items;
using CalamityMod.Items.Armor.OmegaBlue;
using CalamityMod.Items.Materials;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Vanity.KM
{
    [AutoloadEquip(EquipType.Head)]
    public class KitsunesMask : ModItem, IDonatorItem
    {
        public string DonatorName => "黯月殇梦";
        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ItemID.Silk, 2)
                .AddIngredient(ItemID.Ruby)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.Loom)
                .Register();
        }
    }
}
