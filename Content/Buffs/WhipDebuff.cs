using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using Microsoft.Xna.Framework.Graphics;
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
                if (Main.rand.NextBool(20))
                {
                    modifiers.SetCrit();
                }
            }
            foreach (var t in Tags)
            {
                modifiers.FlatBonusDamage += t.TagDamage * projTagMultiplier;
                modifiers.SourceDamage *= t.TagDamageMult;
                if (Main.rand.NextFloat() < t.CritChance)
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
                if (Main.rand.NextBool(10))
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

            if (projectile.GetOwner().Entropy().shadowRune)
            {
                modifiers.FlatBonusDamage += -(modifiers.FlatBonusDamage.Value * 0.6f);
                modifiers.ArmorPenetration += 128;
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
            if (projectile.npcProj || projectile.trap || !(projectile.minion || projectile.sentry || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]))
                return;


            if (npc.HasBuff<DragonWhipDebuff>())
            {
                if (!Main.rand.NextBool(3))
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, CEUtils.randomRot().ToRotationVector2() * 24, ModContent.ProjectileType<DragonGoldenFire>(), projectile.damage / 3, 1, projectile.owner);
                }
                if (projectile.TryGetOwner(out var owner))
                {
                    owner.Heal((int)MathHelper.Max(damageDone / 4000, 1));
                }
            }
            foreach (var t in Tags)
            {
                if (hit.Crit)
                {
                    if (t.EffectName == "LashingBramblerod")
                    {
                        if (projectile.TryGetOwner(out var owner))
                        {
                            if (owner.ownedProjectileCounts[SilvaVineDRPlayer.VineType] > 0)
                            {
                                foreach (Projectile p in Main.ActiveProjectiles)
                                {
                                    if (p.type == SilvaVineDRPlayer.VineType && p.ModProjectile is SilvaVine sv)
                                    {
                                        if (sv.flowerCount < SilvaVine.MaxFlowers)
                                        {
                                            sv.flowerCount++;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Projectile.NewProjectile(owner.GetSource_FromThis(), owner.position, Vector2.Zero, SilvaVineDRPlayer.VineType, 40, 0, owner.whoAmI);
                            }
                        }
                    }
                    if (t.EffectName == "Crystedge")
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromAI(), npc.Center, CEUtils.randomVec(5.6f), ModContent.ProjectileType<CrystedgeCrystalBig>(), projectile.damage * 3, projectile.knockBack, projectile.owner);
                    }
                    if (t.EffectName == "ForeseeWhip")
                    {
                        int C = 4;
                        foreach (NPC n in Main.ActiveNPCs)
                        {
                            if (!n.friendly && n.CanBeChasedBy(projectile) && n.Distance(npc.Center) < 400 && n != npc)
                            {
                                if (C > 0)
                                {
                                    C--;
                                    int dmg = (int)(damageDone * 0.12f);
                                    projectile.GetOwner().ApplyDamageToNPC(n, dmg, 0, 0, false, projectile.DamageType);
                                    for (float f = 0; f <= 1; f += 0.1f)
                                    {
                                        EParticle.NewParticle(new RuneParticle(), Vector2.Lerp(npc.Center, n.Center, f), CEUtils.randomPointInCircle(0.1f), Color.White, 0.5f, 1, true, BlendState.Additive, 0);
                                    }
                                }
                            }
                        }
                    }
                }
                if (t.EffectName == "MindCorruptor")
                {
                    float rot = CEUtils.randomRot();
                    Projectile.NewProjectile(projectile.GetSource_FromAI(), npc.Center - rot.ToRotationVector2() * 128, rot.ToRotationVector2() * 256 / 10f, ModContent.ProjectileType<CorruptStrike>(), projectile.damage / 12 + 1, 2, projectile.owner);
                }

            }
        }
    }
}
