using CalamityEntropy.Common;
using CalamityEntropy.Content.Rarities;
using CalamityMod.Items.LoreItems;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Lores
{
    public class LoreApsychos : LoreItem
    {
        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
            Item.rare = ItemRarityID.LightRed;
            Item.maxStack = 1;
        }
    }
}
