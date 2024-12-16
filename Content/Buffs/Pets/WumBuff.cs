using CalamityEntropy.Content.Projectiles.Pets.DarkFissure;
using CalamityEntropy.Content.Projectiles.Pets.DoG;
using CalamityEntropy.Content.Projectiles.Pets.Signus;
using CalamityEntropy.Content.Projectiles.Pets.StormWeaver;
using CalamityEntropy.Content.Projectiles.Pets.WUPPO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs.Pets
{
    public class WumBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
			Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
			player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<WuppoPet>());
        }
    }
}
