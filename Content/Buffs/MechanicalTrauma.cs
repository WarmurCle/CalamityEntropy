using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs
{
    public class MechanicalTrauma : DotBuff
    {
        public override int DamagePlayerPerSec => 4;
        public override int DamageEnemiesPerSec => 20;
    }
}