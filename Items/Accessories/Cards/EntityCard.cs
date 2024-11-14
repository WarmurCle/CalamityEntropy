using CalamityEntropy.Buffs;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories.Cards
{
	public class EntityCard : ModItem
	{

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 22;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetAttackSpeed(DamageClass.Generic) *= 1.1f;
            player.GetModPlayer<EModPlayer>().entityCard = true;
        }

        public override void AddRecipes()
        {
        }
    }
}
