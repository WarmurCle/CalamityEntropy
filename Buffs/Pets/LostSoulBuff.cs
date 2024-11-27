using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityEntropy.Projectiles.Pets;
using CalamityEntropy.Projectiles.Pets.DoG;
using CalamityEntropy.Projectiles.Pets.StormWeaver;
using CalamityEntropy.Projectiles.Pets.Eater;
namespace CalamityEntropy.Buffs.Pets
{
    public class LostSoulBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<LostSoulProj>());
		}
    }
}
