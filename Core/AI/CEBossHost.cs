using InnoVault;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Utilities;

namespace CalamityEntropy.Core.AI
{
    /// <summary>
    /// SendExtraAI 必须定长,不许按条件省略字段
    /// 调用方先 WriteTiming,再写累加量,Timer 能自愈,航向积分量不自愈
    /// </summary>
    public static class CEBossHost
    {
        /// <summary>AdoptTimingAtFrameStart 收养同态包的计时,换态包走 HookStateSwapAdoption</summary>
        public static void AdoptTimingAtFrameStart<TCtx>(CEBossNetMotion motion, VaultStateMachine<TCtx> machine)
            where TCtx : CEBossStateContext {
            if (!VaultUtils.isClient || motion == null) {
                return;
            }
            if (machine?.CurrentState is ICEBossNetTiming timed
                && motion.TryTakeTiming(timed.StateId, out int timer, out int counter)) {
                timed.AdoptNetTiming(timer, counter);
            }
        }

        /// <summary>HookStateSwapAdoption 挂在 OnStateChanged 上,换态包在写完计时之后发</summary>
        public static void HookStateSwapAdoption<TCtx>(CEBossNetMotion motion, VaultStateMachine<TCtx> machine)
            where TCtx : CEBossStateContext {
            if (motion == null || machine == null) {
                return;
            }
            machine.OnStateChanged += (_, next, _) => {
                if (VaultUtils.isClient && next is ICEBossNetTiming timed
                    && motion.TryTakeTiming(timed.StateId, out int timer, out int counter)) {
                    timed.AdoptNetTiming(timer, counter);
                }
            };
        }

        /// <summary>Heartbeat 做空闲对账,运动不在客户端跑时不要拉长间隔</summary>
        public static void Heartbeat(NPC npc) {
            if (npc != null && !VaultUtils.isClient && Main.GameUpdateCount % CEBossNetMotion.HeartbeatFrames == 0) {
                npc.netUpdate = true;
            }
        }

        /// <summary>
        /// RunAnchoredPartFrame 只清锚点收敛的平滑,不进预测,也不调 EndFrame
        /// 鱼叉这类挂架时直写位置、脱架后按速度飞的部件要按段切换,切换帧调 ForgetPrediction
        /// 切换点要过线
        /// </summary>
        public static void RunAnchoredPartFrame(NPC npc) {
            if (npc != null) {
                CEBossNetMotion.ClearSmoothing(npc);
            }
        }

        /// <summary>PlaceEntity 直写位置,作废的是被写实体自己的预测</summary>
        public static void PlaceEntity(NPC npc, Vector2 center, CEBossNetMotion motion) {
            if (npc == null) {
                return;
            }
            npc.Center = center;
            motion?.ForgetPrediction();
        }

        /// <summary>SeededRandom 的位置累加别用 Main.rand,sequence 须过线</summary>
        /// <param name="partIndex">单体的 partIndex 填 0</param>
        public static UnifiedRandom SeededRandom(NPC npc, int partIndex, int sequence)
            => new UnifiedRandom((npc == null ? 0 : npc.whoAmI * 7919) + partIndex * 131 + sequence);
    }
}
