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
        /// <summary>CueCatchUpGrace 转发 CatchUpGrace,两套窗口不要各调各的</summary>
        public const int CueCatchUpGrace = CEBossCue.CatchUpGrace;

        /// <summary>TimeoutFrames 是超时帧数,默认不限,实战态要给值</summary>
        public virtual int TimeoutFrames => int.MaxValue;

        /// <summary>没目标时不跑 OnUpdate,演出态把 RequiresTarget 覆写成 false</summary>
        public virtual bool RequiresTarget => true;

        /// <summary>覆写 OnEnter 时先调 base</summary>
        public virtual void OnEnter(TCtx ctx) {
            Timer = 0;
            Counter = 0;
        }

        /// <summary>OnUpdate 返回非 null 就换态</summary>
        public abstract IVaultState<TCtx> OnUpdate(TCtx ctx);

        public virtual void OnExit(TCtx ctx) {
        }

        /// <summary>各 Boss 覆写 OnTimeout,让它指回选招口</summary>
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
        /// AdoptNetTiming 必须 public,容差内不动计时,硬对齐会跳过 Timer == N
        /// 计时在这里写入,ReceiveExtraAI 里比较收养前后会恒等,探针不会响
        /// </summary>
        public void AdoptNetTiming(int timer, int counter) {
            CEBossNetDiag.TimingAdopted(StateName, StateId, Timer, timer);
            Timer = CEBossNetMotion.AdoptTimer(Timer, timer);
            Counter = counter;
        }

        /// <summary>CuePassed 要越过宽限才算错过,新代码改用 CEBossCue</summary>
        protected bool CuePassed(int beat) => CEBossCue.Passed(Timer, beat);

        /// <summary>这个重载把判据换成别的已过线帧计数</summary>
        protected static bool CuePassed(float counterValue, int beat) => CEBossCue.Passed(counterValue, beat);

        /// <summary>倒计时用 CuePassedDescending,递增版第一帧会把每拍判成已错过</summary>
        protected static bool CuePassedDescending(float countdown, int beat)
            => CEBossCue.PassedDescending(countdown, beat);

        /// <summary>IsServer 上才做骰点、生成和世界写入,运动各端都跑</summary>
        protected static bool IsServer => !VaultUtils.isClient;

        protected static Vector2 PredictTarget(TCtx ctx, float leadFrames)
            => ctx.Target.Center + ctx.Target.velocity * leadFrames;

        /// <summary>换态同步由 AiSlotNetSync 自带,MarkNetUpdate 给出手锁向这类额外同步</summary>
        protected static void MarkNetUpdate(TCtx ctx) {
            if (IsServer && ctx.Npc != null) {
                ctx.Npc.netUpdate = true;
            }
        }

        /// <summary>两端都调 ResetTiming,脱战计时从 0 起跑</summary>
        public void ResetTiming() {
            Timer = 0;
            Counter = 0;
        }
    }
}
