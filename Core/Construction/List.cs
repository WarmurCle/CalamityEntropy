using CalamityEntropy.Content.Items.Donator.Scarlet;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.Construction
{
    public class EntropyList: ModSystem
    {
        public static List<int> RogueHammer;
        public override void PostSetupContent()
        {
            RogueHammer =
            [
                ModContent.ItemType<PunishmentHammer>(),
                ModContent.ItemType<FallenHammer>(),
                ModContent.ItemType<NightmareHammer>(),
                ModContent.ItemType<GodsHammer>(),
                //ModContent.ItemType<GrandHammer>()
            ];
        }
        public override void Unload()
        {
            List<int>[] intList =
            [
                RogueHammer
            ];
            for (int i = 0; i < intList.Length; i++)
                intList[i] = null;
        }
    }
}