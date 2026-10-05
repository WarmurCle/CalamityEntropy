using CalamityEntropy.Core.AI;
using InnoVault.StateMachines;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Acropolis.Core
{
    /// <summary>写入 ai[3],走路、步态、冷却、鱼叉不占状态位</summary>
    public enum AcropolisStateIndex
    {
        /// <summary>
        /// 行走(选招口)。地面推进由宿主背景跑,本状态只做两件事:
        /// 冷却归零时骰点选招、单发电球、以及满足条件时请求追高跳
        /// </summary>
        Walk = 0,

        /// <summary>炮击:抬炮 60 帧,之后 140 帧向玩家头顶高抛电球。原 <c>CannonUpAtk = 200</c></summary>
        CannonBarrage = 1,

        /// <summary>跳射:朝玩家方向起跳,滞空期间把炮口压向正下方倾泻电球。原 <c>Jumping + JumpAndShoot = 200</c></summary>
        JumpShoot = 2,

        /// <summary>追高跳:玩家高出本体 200 以上且跳跃冷却透支时,朝玩家弹射上去。原 <c>JumpCD &lt;= -260</c> 那一支</summary>
        Leap = 3,
    }

    /// <summary>拍子用 Timer 区间,不加会归零 Timer 的 beat 枚举</summary>
    public abstract class AcropolisStateBase : CEBossStateBase<AcropolisStateContext>
    {
        public override int StateId => (int)StateIndex;
        public abstract AcropolisStateIndex StateIndex { get; }
        public override string StateName => StateIndex.ToString();

        /// <summary>原代码没有超时兜底,这里统一挂一个到不了的安全网,见 Director 的说明</summary>
        public override int TimeoutFrames => AcropolisDirector.StateTimeoutFrames;

        /// <summary>超时的去处:当成正常收招,回行走</summary>
        protected override IVaultState<AcropolisStateContext> OnTimeout(AcropolisStateContext ctx)
            => BackToWalk(ctx);

        /// <summary>只有权威端换态,客户端返回值被丢掉,返回 null 等包</summary>
        protected static IVaultState<AcropolisStateContext> BackToWalk(AcropolisStateContext ctx)
            => IsServer ? VaultStateRegistry<AcropolisStateContext>.Create((int)AcropolisStateIndex.Walk) : null;

        /// <summary>由注册表创建状态实例(未注册返回 null,框架已打日志)</summary>
        protected static IVaultState<AcropolisStateContext> Create(AcropolisStateIndex state)
            => VaultStateRegistry<AcropolisStateContext>.Create((int)state);

        /// <summary>
        /// 生成敌对弹幕:伤害 <c>NPC.damage / 6.2</c>,击退 4,owner 传 -1。
        /// 客户端不生成,吃随机数的散布也必须在调用前的权威端分支里摇
        /// </summary>
        protected static void Shoot<T>(AcropolisStateContext ctx, Vector2 pos, Vector2 velocity,
            float damageMult = 1f, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f) where T : ModProjectile
            => ctx.Owner.Shoot<T>(pos, velocity, damageMult, ai0, ai1, ai2);
    }
}
