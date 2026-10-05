using InnoVault;
using InnoVault.StateMachines;

namespace CalamityEntropy.Core.AI
{
    /// <summary>
    /// Timer 是状态内帧计时,Counter 是总龄,都过线
    /// Timer++ 在子类 OnUpdate 之后,提前会让节拍整体偏一帧
    /// </summary>
    public abstract class CEBossStateBase<TCtx> : VaultState<TCtx>, ICEBossNetTiming
        where TCtx : CEBossStateContext
    {
        /// <summary>转发 CatchUpGrace,两套窗口不要各调各的</summary>
        public const int CueCatchUpGrace = CEBossCue.CatchUpGrace;

        /// <summary>超时帧,默认不限,实战态要给值</summary>
        public virtual int TimeoutFrames => int.MaxValue;

        /// <summary>没目标不跑体,演出态覆写 false</summary>
        public virtual bool RequiresTarget => true;

        /// <summary>覆写先调 base</summary>
        public virtual void OnEnter(TCtx ctx) {
            Timer = 0;
            Counter = 0;
        }

        /// <summary>非 null 换态</summary>
        public abstract IVaultState<TCtx> OnUpdate(TCtx ctx);

        public virtual void OnExit(TCtx ctx) {
        }

        /// <summary>各 Boss 覆写,指回选招口</summary>
        protected virtual IVaultState<TCtx> OnTimeout(TCtx ctx) => null;

        public sealed override void OnEnter(VaultStateMachine<TCtx> machine, TCtx ctx) {
            OnEnter(ctx);
        }

        public sealed override IVaultState<TCtx> OnUpdate(VaultStateMachine<TCtx> machine, TCtx ctx) {
            Counter++;
            IVaultState<TCtx> next = null;
            if (!RequiresTarget || ctx.TargetValid) {
                next = OnUpdate(ctx);
            }
            if (next == null && Counter > TimeoutFrames) {
                next = OnTimeout(ctx);
            }
            Timer++;
            return next;
        }

        public sealed override void OnExit(VaultStateMachine<TCtx> machine, TCtx ctx) {
            OnExit(ctx);
        }

        /// <summary>
        /// 接口必须 public,容差内不动,硬对齐会跳过 Timer == N
        /// 计时在这里落盘,ReceiveExtraAI 里比收养前后恒等,探针不会响
        /// </summary>
        public void AdoptNetTiming(int timer, int counter) {
            CEBossNetDiag.TimingAdopted(StateName, StateId, Timer, timer);
            Timer = CEBossNetMotion.AdoptTimer(Timer, timer);
            Counter = counter;
        }

        /// <summary>越过宽限才算错过,新代码用 CEBossCue</summary>
        protected bool CuePassed(int beat) => CEBossCue.Passed(Timer, beat);

        /// <summary>判据换别的已过线帧计数</summary>
        protected static bool CuePassed(float counterValue, int beat) => CEBossCue.Passed(counterValue, beat);

        /// <summary>倒计时用,递增版第一帧会把每拍判成已错过</summary>
        protected static bool CuePassedDescending(float countdown, int beat)
            => CEBossCue.PassedDescending(countdown, beat);

        /// <summary>骰点、生成、世界写入,运动各端都跑</summary>
        protected static bool IsServer => !VaultUtils.isClient;

        protected static Vector2 PredictTarget(TCtx ctx, float leadFrames)
            => ctx.Target.Center + ctx.Target.velocity * leadFrames;

        /// <summary>换态由 AiSlotNetSync 自带,这里给出手锁向之类</summary>
        protected static void MarkNetUpdate(TCtx ctx) {
            if (IsServer && ctx.Npc != null) {
                ctx.Npc.netUpdate = true;
            }
        }

        /// <summary>两端都调,脱战从 0 起跑</summary>
        public void ResetTiming() {
            Timer = 0;
            Counter = 0;
        }
    }
}
