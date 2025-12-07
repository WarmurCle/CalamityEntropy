using CalamityMod;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items
{
    public interface IGetFromStarterBag
    {
        public bool OwnAble(Player player, ref int count);
    }
    public class StartBagGItem : GlobalItem
    {
        public static bool NameContains(Player player, string str)
        {
            return player.name.ToLower().Contains(str);
        }
        public static List<int> items;
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            if (item.ModItem != null && item.ModItem is StarterBag)
            {
                foreach (int id in items)
                {
                    Item loot = ContentSamples.ItemsByType[id];
                    if (loot.ModItem is IGetFromStarterBag gfsb)
                    {
                        int ItemCount = 1;
                        gfsb.OwnAble(Main.LocalPlayer, ref ItemCount);
                        bool func(DropAttemptInfo info)
                        {
                            int __ = 1;
                            return gfsb.OwnAble(info.player, ref __);
                        }
                        itemLoot.AddIf(func, id, 1, ItemCount, ItemCount);
                    }
                }
            }
        }
    }
}
