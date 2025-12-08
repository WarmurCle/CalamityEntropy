using CalamityEntropy.Content.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class EDamageOverTimeNPC : GlobalNPC
    {
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            int damageApply = 0;
            if (npc.HasBuff<LifeOppress>())
                if (npc.lifeRegen > 0)
                    npc.lifeRegen = 0;
            if (npc.HasBuff<LifeOppress>())
            {
                damageApply += 1600;
            }
            if (npc.HasBuff<MechanicalTrauma>())
            {
                damageApply += 20;
            }
            damage += damageApply * 2;
            npc.lifeRegen -= damageApply * 2;
        }
    }
}
