using System;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityEntropy.Content.Items.Accessories
{
	public class MawOfTheVoid : ModItem
	{
        public static int Damage = 225;  		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().mawOfVoid = true;
        }

        public override void AddRecipes()
        {
        }
    }
}
