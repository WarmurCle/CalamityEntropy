using CalamityEntropy.Content.Rarities;
using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Lores
{
    public class LoreAcropolis : LoreItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.rare = ModContent.RarityType<AzafureOrange>();
            Item.maxStack = 1;
        }
    }
}
