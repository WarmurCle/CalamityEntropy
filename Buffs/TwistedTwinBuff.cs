using CalamityEntropy.Projectiles;
using CalamityEntropy.Projectiles.TwistedTwin;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityEntropy.Buffs
{
    public class TwistedTwinBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<TwistedTwin>()] > 0){
                player.buffTime[buffIndex] = 18000;
            }
            else{
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}