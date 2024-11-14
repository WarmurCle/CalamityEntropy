using CalamityEntropy.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityEntropy.Buffs
{
    public class VoidStorm : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<VoidMark>()] > 0){
                player.buffTime[buffIndex] = 18000;
            }
            else{
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}