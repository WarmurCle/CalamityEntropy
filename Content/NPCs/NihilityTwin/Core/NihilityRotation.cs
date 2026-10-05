using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.Core
{
    /// <summary>
    /// 出招本来就是随机的,允许连续重复同一招,只在小细胞超过 8 时重掷
    /// 掷点收归权威端,原代码客户端也会掷,结果经 ai[3] 过线
    /// </summary>
    public static class NihilityRotation
    {
        /// <summary>由注册表创建状态实例(未注册返回 null,框架已打日志)</summary>
        public static IVaultState<NihilityStateContext> Create(NihilityStateIndex state)
            => VaultStateRegistry<NihilityStateContext>.Create((int)state);

        /// <summary>换招时只清 aicounter,Num2、Num3、Nz 和 ChaseTimer 不清,残值带进下一手</summary>
        public static IVaultState<NihilityStateContext> Regroup(NihilityStateContext ctx) {
            ctx.Num1 = 0;
            return Create(NihilityStateIndex.Regroup);
        }

        /// <summary>
        /// 随机选下一手(原 <c>randomAI</c>)。<b>只该由权威端调用</b>:它带副作用(清计数、吃随机数)。
        /// 抑制条件与原代码同构,包括「重掷」是整段重来而不是换一个面
        /// </summary>
        public static IVaultState<NihilityStateContext> Pick(NihilityStateContext ctx) {
            ctx.Num1 = 0;
            int roll = Main.rand.Next(NihilityDirector.AttackRollFaces);
            if (ctx.Phase == 2 && roll == NihilityDirector.SplitRoll
                && NihilityDirector.CountSmallCells(ModContent.NPCType<ChaoticCellSmall>()) > NihilityDirector.SpawnCellCap) {
                //原代码在这里递归调用 randomAI():整段重掷,而不是在剩下六面里挑
                return Pick(ctx);
            }
            return Create(NihilityDirector.StateFor(ctx.Phase, roll));
        }
    }
}
