using CalamityEntropy.Content.Projectiles;
using Microsoft.Xna.Framework;
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
                if (Main.rand.NextBool(10))
                {
                    modifiers.SetCrit();
                }
			}
            if (npc.HasBuff<DragonWhipDebuff>())
            {
                modifiers.FlatBonusDamage += DragonWhipDebuff.TagDamage * projTagMultiplier;
                if (Main.rand.NextBool(24))
                {
                    modifiers.SetCrit();
                }
            }
            if (npc.HasBuff<CruiserWhipDebuff>())
            {
                modifiers.FlatBonusDamage += CruiserWhipDebuff.TagDamage * projTagMultiplier;
                if (Main.rand.NextBool(9))
                {
                    modifiers.SetCrit();
                }
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            if (projectile.npcProj || projectile.trap || !(projectile.DamageType == DamageClass.Summon) || ProjectileID.Sets.IsAWhip[projectile.type])
                return;


            if (npc.HasBuff<DragonWhipDebuff>())
            {
                if (!Main.rand.NextBool(3))
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Util.Util.randomRot().ToRotationVector2() * 24, ModContent.ProjectileType<DragonGoldenFire>(), projectile.damage / 3, 1, projectile.owner);
                }
                if (projectile.TryGetOwner(out var owner))
                {
                    owner.Heal((int)MathHelper.Max(damageDone / 360, 1));
                }
            }
        }
    }
}
