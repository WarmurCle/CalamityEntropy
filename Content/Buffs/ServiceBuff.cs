using CalamityEntropy.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityEntropy.Content.Buffs
{
	public class ServiceBuff : ModBuff
	{
        public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true;
		}
	}
}
