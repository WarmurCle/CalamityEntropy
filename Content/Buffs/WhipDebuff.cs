using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using System.Collections.Generic;
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
        public static readonly int TagDamage = 15;
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
    public class WhipTag
    {
        public int TagDamage;
        public float TagDamageMult;
        public float CritChance;
        public int TimeLeft = 0;
        public string ItemFullName;
        public string EffectName;
        public WhipTag(string name, int tick, int tagDamage, float tagDamageMult, float Crit = 0, string effectName = "")
        {
            ItemFullName = name;
            TimeLeft = tick;
            TagDamage = tagDamage;
            TagDamageMult = tagDamageMult;
            this.CritChance = Crit;
            EffectName = effectName;
        }
    }
    public class WhipDebuffNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public List<WhipTag> Tags = new List<WhipTag>();
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
                if (Main.rand.NextBool(10))
                {
                    modifiers.SetCrit();
                }
            }
            foreach(var t in Tags)
            {
                modifiers.FlatBonusDamage += t.TagDamage * projTagMultiplier;
                modifiers.SourceDamage *= t.TagDamageMult;
                if (Main.rand.NextFloat() < t.CritChance)
                {
                    modifiers.SetCrit();
                    if(t.EffectName == "Crystedge")
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromAI(), npc.Center, Util.Util.randomVec(5.6f), ModContent.ProjectileType<CrystedgeCrystalBig>(), projectile.damage * 3, projectile.knockBack, projectile.owner);
                    }
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
                if (Main.rand.NextBool(10))
                {
                    modifiers.SetCrit();
                }
            }
            if (npc.HasBuff<WyrmWhipDebuff>())
            {
                modifiers.FlatBonusDamage += WyrmWhipDebuff.TagDamage * projTagMultiplier;
                modifiers.SourceDamage += WyrmWhipDebuff.TagDamageMul * projTagMultiplier;
                if (Main.rand.NextBool(6))
                {
                    modifiers.SetCrit();
                }
            }
        }
        public override bool PreAI(NPC npc)
        {
            for (int i = Tags.Count - 1; i >= 0; i--)
            {
                if (--Tags[i].TimeLeft <= 0)
                {
                    Tags.RemoveAt(i);
                }
            }
            return base.PreAI(npc);
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
                    owner.Heal((int)MathHelper.Max(damageDone / 3000, 1));
                }
            }
        }
    }
}
