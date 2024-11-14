using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityEntropy.Projectiles.Pets.StormWeaver;
namespace CalamityEntropy.Buffs.Pets
{
    public class Rainstorm : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<StormWeaverPet>());
		}
    }
}
