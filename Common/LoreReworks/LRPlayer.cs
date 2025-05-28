using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.LoreItems;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common.LoreReworks
{
    public class LRPlayer : ModPlayer
    {
        public override void OnHurt(Player.HurtInfo info)
        {
            if (LoreReworkSystem.Enabled<LoreCrabulon>())
            {
                Player.AddBuff(ModContent.BuffType<Mushy>(), LECabulon.BuffTime * 60);
            }
        }
    }
}
