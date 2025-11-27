using CalamityEntropy.Content.Buffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                damageApply += 9600;
            }
            if (npc.HasBuff<MechanicalTrauma>())
            {
                damageApply += 34;
            }
            damage += damageApply * 2;
            npc.lifeRegen -= damageApply * 2;
        }
    }
}
