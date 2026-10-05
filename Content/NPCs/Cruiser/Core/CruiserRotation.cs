using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Cruiser.Core
{
    /// <summary>
    /// 不设防复读,6/19 是哨兵,上一手是拉开就退一格改直扑,那一格走两次
    /// 二阶段每次选招都重压 defense 50 和减伤 0.42,转阶段收尾直写 VoidSpike,不清计数
    /// 残值超过 150 时第一手尖刺第一帧收招,照搬,阶段判据读当帧血量
    /// </summary>
    public static class CruiserRotation
    {
        /// <summary>由注册表创建状态实例(未注册返回 null,框架已打日志)</summary>
        public static IVaultState<CruiserStateContext> Create(CruiserStateIndex state)
            => VaultStateRegistry<CruiserStateContext>.Create((int)state);

        /// <summary>只权威端调,门在 NextAttack</summary>
        /// <param name="from">哨兵要读它,原代码读的是还没改的 ai</param>
        public static IVaultState<CruiserStateContext> Pick(CruiserStateContext ctx, CruiserStateIndex from) {
            //对齐原代码:计数先无条件清零,再做裁决(netUpdate 由 AiSlotNetSync 在写状态号时打)
            ctx.ChangeCounter = 0;

            NPC npc = ctx.Npc;
            if (ctx.Phase == 1) {
                ctx.AttackIndex++;
                if (ctx.AttackIndex > CruiserDirector.Phase1.Length - 1) {
                    ctx.AttackIndex = 0;
                }
                CruiserStateIndex slot = CruiserDirector.Phase1[ctx.AttackIndex];
                if (slot == CruiserStateIndex.PhaseTransing) {
                    //隐式行为一:哨兵槽
                    if (from == CruiserStateIndex.StayAwayAndShootVoidStar) {
                        ctx.AttackIndex--;
                        slot = CruiserStateIndex.TryToClosePlayer;
                    }
                    else {
                        slot = Main.rand.NextBool() ? CruiserStateIndex.EnergyBall : CruiserStateIndex.VoidResidue;
                    }
                }
                return Create(slot);
            }

            //隐式行为二:二阶段每次选招都重压护甲与减伤
            npc.defense = CruiserDirector.DefensePhase2;
            ctx.Owner.DamageReduction = CruiserDirector.DRPhase2;
            ctx.AttackIndex++;
            if (ctx.AttackIndex >= CruiserDirector.Phase2.Length) {
                ctx.AttackIndex = 0;
            }
            return Create(CruiserDirector.Phase2[ctx.AttackIndex]);
        }
    }
}
