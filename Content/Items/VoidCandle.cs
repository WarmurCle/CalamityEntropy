using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public class VoidCandle : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 42;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(platinum: 2);
            Item.rare = ItemRarityID.Pink;
            Item.createTile = ModContent.TileType<VoidCandleTile>();
            Item.buffType = ModContent.BuffType<VoidCandleBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(600);
        }
        
    }
}
