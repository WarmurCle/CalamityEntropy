using CalamityEntropy.Core.CalamityRef;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    /// <summary>
    /// 替代灾厄新手包;开包内容走 StartBagGItem 按 IGetFromStarterBag 注入
    /// MagicStorage/ImproveGame 便利物品由本类条件注入
    /// 首次进世界的发放在 EModPlayer.OnEnterWorld,看 ServerConfig.ExtraItemsInStarterBag
    /// </summary>
    public class EntropyStarterBag : ModItem
    {
        // 暂用彩票箱贴图占位，正式贴图画好后换回同名资源
        public override string Texture => "CalamityEntropy/Content/Items/LotteryBox";

        public override void SetDefaults() {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 1;
            Item.consumable = true;
            Item.rare = ItemRarityID.White;
        }

        public override bool CanRightClick() => true;

        public override void ModifyItemLoot(ItemLoot itemLoot) {
            if (CERef.Has) {
                return;
            }
            StartBagGItem.AddConvenienceMods(itemLoot);
        }
    }
}
