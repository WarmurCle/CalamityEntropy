using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Armor;
using CalamityEntropy.Content.Rarities;
using CalamityEntropy.Content.Tiles;
using CalamityEntropy.Core.CalamityRef;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    public class ReincarnationBadge : ModItem
    {
        public override void SetDefaults() {
            Item.width = 98;
            Item.height = 60;
            Item.value = Item.buyPrice(platinum: 1, gold: 50);
            Item.rare = ModContent.RarityType<NihilityBlue>();
            Item.accessory = true;

        }
        public override void ModifyTooltips(List<TooltipLine> list) {
            // 脱离灾厄:灾厄动态饰品键位并入自有 AccessoryAbilityHotKey(player-api.md §2)
            list.Replace("[KEY]", EModPlayer.AccessoryAbilityHotKey.TooltipKeyHint());
        }

        public override void UpdateAccessory(Player player, bool hideVisual) {
            player.Entropy().reincarnationBadge = true;
        }

        public override void AddRecipes() {
            if (CECal.CalChainReady(CEID.Item_AscendantInsignia, CEID.Item_AscendantSpiritEssence, CEID.Tile_CosmicAnvil)) {
                CreateRecipe().AddIngredient(CEID.Item_AscendantInsignia)
                .AddIngredient(CEID.Item_AscendantSpiritEssence, 4)
                .AddTile(CEID.Tile_CosmicAnvil).Register();
                return;
            }
            //脱离灾厄时,灾厄升华勋章改成原版飞升徽记,徽记是那条灾厄配方原来的物品,合成站改成远古操纵机
            CreateRecipe().AddIngredient(ItemID.EmpressFlightBooster)
                .AddIngredient<VoidBar>(5)
                .AddTile<VoidWellTile>().Register();
        }
    }
}
