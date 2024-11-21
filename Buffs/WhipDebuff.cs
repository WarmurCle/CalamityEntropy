using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Buffs
{
	public class JailerWhipDebuff : ModBuff
	{
		public static readonly int TagDamage = 8;
        public override string Texture => "CalamityEntropy/Buffs/WhipDebuff";
        public override void SetStaticDefaults() {
			BuffID.Sets.IsATagBuff[Type] = true;
		}
	}



	public class ExampleWhipDebuffNPC : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
			if (projectile.npcProj || projectile.trap || !projectile.IsMinionOrSentryRelated)
				return;


			var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
			if (npc.HasBuff<JailerWhipDebuff>()) {
				modifiers.FlatBonusDamage += JailerWhipDebuff.TagDamage * projTagMultiplier;
			}

		}
	}
}
