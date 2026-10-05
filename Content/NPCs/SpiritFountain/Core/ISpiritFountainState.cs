using CalamityEntropy.Core.AI;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.Core
{
    /// <summary>写入 ai[3],序号对齐重构前的 AIStyle,SpiritRing 原来引用旧枚举</summary>
    public enum SpiritFountainStateIndex
    {
        /// <summary>出场演出:300 帧聚魂 → 显形 → 放出一号柱魂环</summary>
        SpawnAnimation = 0,
        /// <summary>横扫:一号柱左右大幅摆动,按阶段叠加三套弹幕</summary>
        Moving = 1,
        /// <summary>回旋:柱子收回中线,魂环按 Index 依次脱柱扑人</summary>
        Boomerang = 2,
        /// <summary>激光:本体只收柱子,火力全在魂环的扫射上</summary>
        Lasers = 3,
        /// <summary>落环喷泉:本体只收柱子,魂环落地聚成冲击波</summary>
        RingFountains = 4,
        /// <summary>转阶段演出:二号柱亮起,全场免伤 71 帧</summary>
        PhaseTranse1 = 5,
        /// <summary>十字斩:双柱交替横扫竖扫,终局循环态,不再退出</summary>
        SpiritSlicing = 6,
    }

    /// <summary>
    /// 拍子用 Timer 区间判断,不加会归零 Timer 的 beat 枚举
    /// 原 AI 没有没目标就不跑的判断,RequiresTarget 整族关掉
    /// </summary>
    public abstract class SpiritFountainStateBase : CEBossStateBase<SpiritFountainStateContext>
    {
        public override int StateId => (int)StateIndex;
        public abstract SpiritFountainStateIndex StateIndex { get; }
        public override string StateName => StateIndex.ToString();

        /// <summary>原代码整条 AI 没有目标门槛,原样保留</summary>
        public override bool RequiresTarget => false;

        /// <summary>原代码没有超时兜底,这里统一挂一个到不了的安全网,见 Director 的说明</summary>
        public override int TimeoutFrames => SpiritFountainDirector.StateTimeoutFrames;

        /// <summary>状态体本帧看到的 aiTimer,基类 Timer++ 在状态体之后,部件读 Timer 会错一帧</summary>
        public int BodyTimer { get; private set; }

        public override void OnEnter(SpiritFountainStateContext ctx) {
            base.OnEnter(ctx);
            BodyTimer = 0;
        }

        public sealed override IVaultState<SpiritFountainStateContext> OnUpdate(SpiritFountainStateContext ctx) {
            IVaultState<SpiritFountainStateContext> next = RunBody(ctx);
            //状态体自己可能动过 Timer(转阶段的额外自增、十字斩的原地归零),所以在它跑完之后取值
            BodyTimer = Timer;
            return next;
        }

        /// <summary>状态主体。返回非 null 请求换态,返回 null 留在本状态</summary>
        protected abstract IVaultState<SpiritFountainStateContext> RunBody(SpiritFountainStateContext ctx);

        /// <summary>超时的去处:当成正常收招,走线性链的下一手</summary>
        protected override IVaultState<SpiritFountainStateContext> OnTimeout(SpiritFountainStateContext ctx)
            => Advance(ctx, StateIndex);

        /// <summary>换到更靠前的状态要等下一帧,开头自增让首帧读到 1 不是 0</summary>
        public void AdoptDeferredEntry() {
            Timer++;
        }

        /// <summary>只有权威端真的换态,客户端等 ai[3]</summary>
        protected static IVaultState<SpiritFountainStateContext> Advance(SpiritFountainStateContext ctx, SpiritFountainStateIndex from)
            => IsServer ? SpiritFountainRotation.Pick(ctx, from) : null;

        /// <summary>damage / 6 整数除法再乘倍率,owner -1,客户端不生成</summary>
        protected static void Shoot<T>(SpiritFountainStateContext ctx, Vector2 pos, Vector2 velocity,
            float damageMult = 1f, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f) where T : ModProjectile {
            ctx.Owner?.Shoot(ModContent.ProjectileType<T>(), pos, velocity, damageMult, ai0, ai1, ai2);
        }

        /// <summary>本地端(含单机)。粒子、音效、滤镜只在这里做</summary>
        protected static bool IsLocal => !Main.dedServ;
    }
}
