using InnoVault;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Utilities;

namespace CalamityEntropy.Core.AI
{
    /// <summary>
    /// SendExtraAI 定长,不许按条件省略字段
    /// 先 WriteTiming,再写累加量,Timer 自愈,航向积分量不自愈
    /// </summary>
    public static class CEBossHost
    {
        /// <summary>同态包的计时收养,换态包走 HookStateSwapAdoption</summary>
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

        /// <summary>挂 OnStateChanged,换态包在写完计时之后发</summary>
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

        /// <summary>空闲对账,运动不在客户端跑时不要拉长间隔</summary>
        public static void Heartbeat(NPC npc) {
            if (npc != null && !VaultUtils.isClient && Main.GameUpdateCount % CEBossNetMotion.HeartbeatFrames == 0) {
                npc.netUpdate = true;
            }
        }

        /// <summary>
        /// 锚点收敛只清平滑,不进预测,不调 EndFrame
        /// 鱼叉这类挂架直写、脱架按速度飞的,按段切换,切换帧 ForgetPrediction
        /// 切换点过线
        /// </summary>
        public static void RunAnchoredPartFrame(NPC npc) {
            if (npc != null) {
                CEBossNetMotion.ClearSmoothing(npc);
            }
        }

        /// <summary>直写位置,作废的是被写实体自己的预测</summary>
        public static void PlaceEntity(NPC npc, Vector2 center, CEBossNetMotion motion) {
            if (npc == null) {
                return;
            }
            npc.Center = center;
            motion?.ForgetPrediction();
        }

        /// <summary>位置累加别用 Main.rand,sequence 须过线</summary>
        /// <param name="partIndex">单体填 0</param>
        public static UnifiedRandom SeededRandom(NPC npc, int partIndex, int sequence)
            => new UnifiedRandom((npc == null ? 0 : npc.whoAmI * 7919) + partIndex * 131 + sequence);
    }
}
