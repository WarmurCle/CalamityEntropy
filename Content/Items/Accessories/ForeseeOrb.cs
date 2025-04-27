using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Utilities;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class ForeseeOrb : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.accessory = true;

        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Entropy().foreseeOrbItem = Item;
            if (!player.HasBuff<ShatteredOrb>())
            {
                player.GetDamage(DamageClass.Generic) += 0.2f;
            }
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.HasBuff<ShatteredOrb>())
                {
                    TextureAssets.Item[Type] = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Accessories/ForeseeOrbBreak");
                }
                else
                {
                    TextureAssets.Item[Type] = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Accessories/ForeseeOrb");
                }
            }
        }

    }
}