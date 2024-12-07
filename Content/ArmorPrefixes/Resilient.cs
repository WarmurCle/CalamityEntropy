using Terraria;

namespace CalamityEntropy.Content.ArmorPrefixes
{
    public class Resilient : ArmorPrefix
    {
        public override void updateEquip(Player player, Item item)
        {
            player.jumpSpeedBoost += 2f;
        }
        public override int getRollChance()
        {
            return 10;
        }
        public override bool? canApplyTo(Item item)
        {
            if (item.legSlot == -1)
            {
                return false;
            }
            return base.canApplyTo(item);
        }
    }
}
