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
                damageApply += 4501;
            }
            var dict = DotBuff.InstanceByType();
            for (int i = 0; i < npc.buffType.Length; i++)
            {
                if (npc.buffTime[i] > 0)
                {
                    if (dict.TryGetValue(npc.buffType[i], out var res) && res is DotBuff)
                    {
                        damageApply += res.DamageEnemiesPerSec;
                    }
                }
            }
            
            damageApply = (int)(damageApply * npc.Entropy().DebuffDamageMult());
            damage += damageApply;
            npc.lifeRegen -= damageApply * 2;
        }
    }
}
