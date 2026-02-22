using CalamityEntropy.Content.Rarities;
using CalamityMod.Items.LoreItems;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Lores
{
    public class LuminarisLore : LoreItem
    {
        public static float wingTimeAddition = 0.05f;
        
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ModContent.RarityType<Lunarblight>();
            Item.maxStack = 1;
        }
    }
}
