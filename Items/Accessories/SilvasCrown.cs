using CalamityEntropy.Buffs;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories
{
	public class SilvasCrown : ModItem
	{

		public override void SetDefaults() {
			Item.width = 42;
			Item.height = 42;
            Item.value = CalamityGlobalItem.RarityLightRedBuyPrice;
            Item.rare = ModContent.RarityType<GlowGreen>();
            Item.accessory = true;
            
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().defenseDamageRatio = 0;
            player.Calamity().nextHitDealsDefenseDamage = false;
            
            player.Entropy().SCrown = true;
            player.statLifeMax2 += 50;
        }

    }
}
