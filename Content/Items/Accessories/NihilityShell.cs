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
	public class NihilityShell : ModItem
	{
        public static int MaxCount = 2; //×î´ó¼×¿ÇÊý
		public override void SetDefaults() {
			Item.width = 40;
			Item.height = 40;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
			
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().nihShell = true;
        }

        public override void AddRecipes()
        {
        }

        public static void checkDamage(Player player, NPC.HitInfo hitInfo)
        {
            if (hitInfo.Crit)
            {
                if(player.Entropy().nihShellCd <= 0)
                {
                    if (Main.rand.NextBool(6))
                    {
                        if(player.Entropy().nihShellCount < MaxCount)
                        {
                            player.Entropy().nihShellCount++;
                            player.Entropy().nihShellCd = 10 * 60;
                        }
                    }
                }
            }
        }
    }
}
