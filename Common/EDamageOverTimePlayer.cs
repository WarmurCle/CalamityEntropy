using CalamityEntropy.Content.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class EDamageOverTimePlayer : ModPlayer
    {
        public override void UpdateBadLifeRegen()
        {
            int damageApply = 0;
            if (Player.HasBuff<LifeOppress>())
            {
                damageApply += 60;
            }
            if (Player.HasBuff<MechanicalTrauma>())
            {
                damageApply += 8;
            }
            if (damageApply > 0)
            {
                if (Player.lifeRegen > 0)
                    Player.lifeRegen = 0;
                Player.lifeRegen -= damageApply;
            }
        }
    }
}
