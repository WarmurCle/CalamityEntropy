using CalamityEntropy.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityEntropy.Content.Buffs
{
	public class JailerWhipDebuff : ModBuff
	{
		public static readonly int TagDamage = 7;
        public override string Texture => "CalamityEntropy/Content/Buffs/WhipDebuff";
        public override void SetStaticDefaults() {
			BuffID.Sets.IsATagBuff[Type] = true;
		}
	}
    public class DragonWhipDebuff : ModBuff
    {
        public static readonly int TagDamage = 46;
        public override string Texture => "CalamityEntropy/Content/Buffs/WhipDebuff";
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
    public class CruiserWhipDebuff : ModBuff
    {
        public static readonly int TagDamage = 52;
        public override string Texture => "CalamityEntropy/Content/Buffs/WhipDebuff";
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class WhipDebuffNPC : GlobalNPC
	{
		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
			if (projectile.npcProj || projectile.trap || !(projectile.DamageType == DamageClass.Summon) || ProjectileID.Sets.IsAWhip[projectile.type])
				return;


			var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
			if (npc.HasBuff<JailerWhipDebuff>()) {
				modifiers.FlatBonusDamage += JailerWhipDebuff.TagDamage * projTagMultiplier;
			}
            if (npc.HasBuff<DragonWhipDebuff>())
            {
                modifiers.FlatBonusDamage += DragonWhipDebuff.TagDamage * projTagMultiplier;
            }
            if (npc.HasBuff<CruiserWhipDebuff>())
            {
                modifiers.FlatBonusDamage += CruiserWhipDebuff.TagDamage * projTagMultiplier;
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.npcProj || projectile.trap || !(projectile.DamageType == DamageClass.Summon) || ProjectileID.Sets.IsAWhip[projectile.type])
                return;


            if (npc.HasBuff<DragonWhipDebuff>())
            {
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Util.Util.randomRot().ToRotationVector2() * 24, ModContent.ProjectileType<DragonGoldenFire>(), projectile.damage / 5, 1, projectile.owner);
            }
        }
    }
}
