using CalamityEntropy.Core.AI;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.LuminarisMoth.Core
{
    /// <summary>写入 ai[3],序号对齐迁移前的 AIStyle</summary>
    public enum LuminarisStateIndex
    {
        /// <summary>绕转喷涡,无接触伤</summary>
        RoundShooting = 0,
        /// <summary>滑出撒刺</summary>
        AstralSpike = 1,
        /// <summary>高空横移,末尾落地拍</summary>
        AboveMovingShooting = 2,
        /// <summary>只掰朝向</summary>
        Waiting1Sec = 3,
        /// <summary>两轮俯冲后追撞</summary>
        Subduction = 4,
        /// <summary>悬停环射</summary>
        StayAboveAndShooting = 5,
        /// <summary>两段锁向直冲</summary>
        Dashing = 6,
        /// <summary>贴身环爆</summary>
        Shoot360 = 7,
        /// <summary>绕场后穿场</summary>
        RoundAndDash = 8,
        /// <summary>四轮砸落,只有砸落段有接触伤</summary>
        SmashDown = 9,
        /// <summary>绕圈射三角弹</summary>
        ShootTriangle = 10,
    }

    /// <summary>拍子用 Countdown 区间,不加会归零 Timer 的 beat 枚举</summary>
    public abstract class LuminarisStateBase : CEBossStateBase<LuminarisStateContext>
    {
        public override int StateId => (int)StateIndex;
        public abstract LuminarisStateIndex StateIndex { get; }
        public override string StateName => StateIndex.ToString();

        /// <summary>原代码无超时,这里挂一个到不了的网</summary>
        public override int TimeoutFrames => LuminarisDirector.StateTimeoutFrames;

        protected override IVaultState<LuminarisStateContext> OnTimeout(LuminarisStateContext ctx)
            => NextAttack(ctx);

        /// <summary>
        /// 体末尾必调,读自减前的值,跌破 0 收招
        /// 客户端 NextAttack 为 null,倒计时继续走负,运动与 -1 帧相同
        /// </summary>
        protected IVaultState<LuminarisStateContext> Tick(LuminarisStateContext ctx, int countdown) {
            ctx.Countdown--;
            return countdown < 0 ? NextAttack(ctx) : null;
        }

        /// <summary>只权威端选招,客户端跑 Pick 会清锚点、弹到零点</summary>
        protected IVaultState<LuminarisStateContext> NextAttack(LuminarisStateContext ctx)
            => IsServer ? LuminarisRotation.Pick(ctx, StateIndex) : null;

        /// <summary>递减时钟,掉到拍点下超过宽限才算错过</summary>
        protected static bool CountdownCuePassed(int countdown, int beat)
            => countdown < beat - CueCatchUpGrace;

        /// <summary>原 Shoot 的 netMode 门,客户端不生成</summary>
        protected static void Shoot<T>(LuminarisStateContext ctx, Vector2 pos, Vector2 velocity,
            float damageMult = 1f, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f) where T : ModProjectile {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                return;
            }
            NPC npc = ctx.Npc;
            int baseDamage = (int)(npc.damage / LuminarisDirector.ProjDamageDivisor);
            Projectile.NewProjectile(npc.GetSource_FromAI(), pos, velocity, ModContent.ProjectileType<T>(),
                (int)(baseDamage * damageMult), LuminarisDirector.ProjKnockback, -1, ai0, ai1, ai2);
        }
    }
}
