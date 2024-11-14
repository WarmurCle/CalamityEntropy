using CalamityEntropy.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityEntropy.Buffs
{
    public class DivingShieldCooldown : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
            Main.debuff[Type] = true;
        }
    }
}