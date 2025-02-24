using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
	public class FriendFinder : ModItem
	{
        public override void SetStaticDefaults() {
		}
        public override bool IsLoadingEnabled(Mod mod)
        {
            return false;
        }

        public override void SetDefaults() {
			Item.width = 62;
			Item.height = 70;
            Item.useTime = 22;
            Item.useAnimation = 22;
            Item.useStyle = -1;
            Item.noMelee = true;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
			
		}
        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool? UseItem(Player player)
        {
            if(Main.netMode != NetmodeID.MultiplayerClient)
            {

            }
            return true;
        }

    }
}
