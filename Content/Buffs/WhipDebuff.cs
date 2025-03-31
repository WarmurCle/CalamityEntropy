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
        公共 静态 只读 int 标签伤害 = 30;
        公共 覆盖字符串纹理 => "CalamityEntropy/Content/Buffs/鞭击减益"输入：;
        公共 覆盖 无返回值 SetStaticDefaults()
        {
            BuffID.Sets.IsATagBuff[Type] = 真;
        }
    }

    公共 类 鞭打减益NPC : 全局NPC
    {
        公共 覆盖 无 修改命中投射物(NPC npc, 投射物 projectile, 参考 NPC.命中修正符 modifiers)
        {
            如果 (项目物是NPC投射物 || 项目物是陷阱 || 项目物的伤害类型不是召唤伤害 || 项目物是鞭子)
                返回;

            如果 (项目物.尝试获取所有者(输出 变量 所有者))
            输入：
                如果 (Main.rand.Next(0, 100) < owner.Entropy().summonCrit)
                {
                    modifiers.SetCrit();
                }
            }

            var projTagMultiplier = ProjectileID.Sets.SummonTagDamageMultiplier[projectile.type];
            如果 (角色有技能<监狱鞭打减益>())
            输入：            {
                modifiersFlatBonusDamage += JailerWhipDebuffTagDamage * projTagMultiplier;
                如果 (Main.rand.NextBool(10))
                {
                    modifiers.SetCrit();
                }
            }
            如果 (角色.有技能<龙鞭减益>())
            输入：
                modifiersFlatBonusDamage += DragonWhipDebuffTagDamage * projTagMultiplier;
                如果 (Main.rand.NextBool(50))
                {
                    modifiers.SetCrit();
                }
            }
            如果 (角色技能.有技能<轻巡鞭打减益>())
            输入：
                modifiersFlatBonusDamage += 舰船鞭打减益标签伤害 * proj标签乘数;
                如果 (Main.rand.NextBool(15))
                {
                    modifiers.SetCrit();
                }
            }
            如果 (角色.有技能<龙鞭诅咒>())
            输入：
                modifiersFlatBonusDamage += WyrmWhipDebuffTagDamage * projTagMultiplier;
                modifiers.SourceDamage += WyrmWhipDebuff.TagDamageMul * projTagMultiplier;
                如果 (Main.rand.NextBool(8))
                {
                    modifiers.SetCrit();
                }
            }
        }

        公共 覆盖 无返回值 OnHitByProjectile(NPC npc, Projectile projectile, NPC.打击信息 hit, int damageDone)
        {
            如果 (项目物是NPC投射物 || 项目物是陷阱 || 项目物的伤害类型不是召唤伤害 || 项目物是鞭子)
                返回;


            如果 (角色.有技能<龙鞭减益>())
            输入：
                如果 (!Main.rand.NextBool(10))
                {
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, Util.Util.randomRot().ToRotationVector2() * 24, ModContent.ProjectileType<DragonGoldenFire>(), projectile.damage / 8, 1, projectile.owner);
                }
                如果 (项目物.尝试获取所有者(输出 变量 所有者))
                {
                    owner.Heal((int)MathHelper.Max(damageDone / 3000, 1));
                }
            }
        }
    }
输入：}
