using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Apsychos.Core
{
    /// <summary>
    /// 轮换允许连续重复同一招
    /// 远距离时强制接近,不改动序号,转阶段时把序号置 0
    /// 低血换表时不重置序号,越界才归零
    /// </summary>
    public static class ApsychosRotation
    {
        /// <summary>由注册表创建状态实例(未注册返回 null,框架已打日志)</summary>
        public static IVaultState<ApsychosStateContext> Create(ApsychosStateIndex state)
            => VaultStateRegistry<ApsychosStateContext>.Create((int)state);

        /// <summary>只权威端调,客户端再调一遍会提前清标量</summary>
        public static IVaultState<ApsychosStateContext> Pick(ApsychosStateContext ctx) {
            //对齐原代码:三个标量先无条件清零,再做裁决(计时由新状态的 OnEnter 归零)
            ctx.Num1 = 0f;
            ctx.Num2 = 0f;
            ctx.Num3 = 0f;

            NPC npc = ctx.Npc;
            ApsychosStateIndex next;

            if (npc.HasValidTarget && npc.target.ToPlayer().Distance(npc.Center) > ApsychosDirector.ForceApproachDistance) {
                //隐式行为一:序号不动
                next = ApsychosStateIndex.MoveToTarget;
            }
            else if (ctx.Phase == 1 && npc.life < npc.lifeMax * ApsychosDirector.Phase2LifeRatio) {
                //隐式行为二:置 0 而非自增。转阶段结束后的那次 Pick 会把它推到 1
                next = ApsychosStateIndex.PhaseTrans;
                ctx.AttackIndex = 0;
            }
            else {
                //换表时不重置序号,只在越界时归零
                ApsychosStateIndex[] table = ApsychosDirector.TableFor(ctx.Phase, npc);
                ctx.AttackIndex++;
                if (ctx.AttackIndex >= table.Length) {
                    ctx.AttackIndex = 0;
                }
                next = table[ctx.AttackIndex];
            }

            return Create(next);
        }
    }
}
