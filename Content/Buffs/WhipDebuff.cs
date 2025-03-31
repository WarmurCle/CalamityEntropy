using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Buffs
{
    public class JailerWhipDebuff : ModBuff
    {
        public static readonly int TagDamage = 5;
        public override string Texture => "CalamityEntropy/Content/Buffs/WhipDebuff";
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
    public class DragonWhipDebuff : ModBuff
    {
        public static readonly int TagDamage = 5;
        public override string Texture => "CalamityEntropy/Content/Buffs/WhipDebuff";
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
    public class WyrmWhipDebuff : ModBuff
    {
        public static readonly float TagDamageMul = 0.15f;
        public static readonly int TagDamage = 90;
        public override string Texture => "CalamityEntropy/Content/Buffs/WhipDebuff";
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }
    public class CruiserWhipDebuff : ModBuff
    {
        public static readonly int TagDamage = 30;
        public override string Texture => "CalamityEntropy/Content/Buffs/WhipDebuff";
        public override void SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = true;
        }
    }

    public class WhipDebuffNPC : GlobalNPC
    {
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (projectile.npcProj || projectile.trap || !(projectile.DamageType == DamageClass.Summon) || ProjectileID.Sets.IsAWhip[projectile.type])
                return;

            if (projectile.TryGetOwner(out var owner))
            {
                if (Main.rand.Next(0, 100) < owner.Entropy().summonCrit)
                {
                    modifiers.SetCrit();
                }
            }

            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            if (npc.HasBuff<JailerWhipDebuff>())
            {
                modifiers.FlatBonusDamage += JailerWhipDebuff.TagDamage * projTagMultiplier;
                if (Main.rand.NextBool(100))
                {
                    modifiers.SetCrit();
                }
            }
            if (npc.HasBuff<DragonWhipDebuff>())
            {
                modifiers.FlatBonusDamage += DragonWhipDebuff.TagDamage * projTagMultiplier;
                if (Main.rand.NextBool(50))
                {
                    modifiers.SetCrit();
                }
            }
            if (npc.HasBuff<CruiserWhipDebuff>())
            {
                modifiers.FlatBonusDamage += CruiserWhipDebuff.TagDamage * projTagMultiplier;
                if (Main.rand.NextBool(15))
                {
                    modifiers.SetCrit();
                }
            }
            if (npc.HasBuff<WyrmWhipDebuff>())
            {
                modifiers.FlatBonusDamage += WyrmWhipDebuff.TagDamage * projTagMultiplier;
                modifiers.SourceDamage += WyrmWhipDebuff.TagDamageMul * projTagMultiplier;
                if (Main.rand.NextBool(8))
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
                if (!Main.rand.NextBool(10))
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Util.Util.randomRot().ToRotationVector2() * 24, ModContent.ProjectileType<DragonGoldenFire>(), projectile.damage / 8, 1, projectile.owner);
                }
                if (projectile.TryGetOwner(out var owner))
                {
                    owner.Heal((int)MathHelper.Max(damageDone / 3000, 1));
                }
            }
        }
    }
}
