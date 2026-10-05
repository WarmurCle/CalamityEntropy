using CalamityEntropy.Core.AI;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Cruiser.Core
{
    /// <summary>
    /// 状态索引,写入 <c>npc.ai[3]</c> 网络同步。
    /// <b>序号与迁移前的 <c>CruiserHead.AIStyle</c> 逐项对齐</b>,方便对照旧代码与旧日志
    /// </summary>
    public enum CruiserStateIndex
    {
        /// <summary>直扑:一路加速撞向玩家,进到 700 + 当前速度就交棒</summary>
        TryToClosePlayer = 0,
        /// <summary>拉开:先散开再回身,第 90 帧甩一次尾鞭(尾部新星)</summary>
        StayAwayAndShootVoidStar = 1,
        /// <summary>绕飞:贴着 600 半径侧向绕圈,每 40 帧甩一次尾鞭</summary>
        AroundPlayerAndShootVoidStar = 2,
        /// <summary>能量球:开局放出一颗挂在本体上的能量球,自己慢速贴近</summary>
        EnergyBall = 3,
        /// <summary>虚空残渣:张嘴蓄 80 帧,一口喷出 80 发残渣,再顺势冲一段</summary>
        VoidResidue = 4,
        /// <summary>转阶段:122 帧演出,计数器是 <c>phaseTrans</c> 而不是状态计时</summary>
        PhaseTransing = 5,
        /// <summary>虚空尖刺:高速盘旋,四次全向 12 发尖刺</summary>
        VoidSpike = 6,
        /// <summary>咬击:咬住玩家拖 20 帧,甩出并沿航线布下刀光阵</summary>
        BiteAndDash = 7,
        /// <summary>巡航:稳速追瞄,100 帧后每帧 1/150 概率收招</summary>
        Cruise = 8,
        /// <summary>裂空吐星:蓄 100 帧一口喷出 80 发虚空星</summary>
        SplittingVoidStar = 9,
        /// <summary>短冲:锁向直冲 38 帧,之后追瞄</summary>
        QuickDash = 10,
        /// <summary>巡游布雷:缓转弯,前 180 帧每 7 帧撒一颗虚空炸弹</summary>
        AroundSpawnVoidBomb = 11,
        /// <summary>虚空激光:瞄准窗后 6 轮定点扫射,每轮自身也顺着光束冲一次</summary>
        VoidLaser = 12,
    }

    /// <summary>
    /// 拍子用 ChangeCounter 区间,不加会归零 Timer 的 beat 枚举
    /// 自增前读一次,自增后再读,阈值照抄
    /// </summary>
    public abstract class CruiserStateBase : CEBossStateBase<CruiserStateContext>
    {
        public override int StateId => (int)StateIndex;
        public abstract CruiserStateIndex StateIndex { get; }
        public override string StateName => StateIndex.ToString();

        /// <summary>原代码没有超时兜底,这里统一挂一个到不了的安全网,见 Director 的说明</summary>
        public override int TimeoutFrames => CruiserDirector.StateTimeoutFrames;

        /// <summary>超时的去处:当成正常收招,走轮换裁决选下一手</summary>
        protected override IVaultState<CruiserStateContext> OnTimeout(CruiserStateContext ctx)
            => NextAttack(ctx);

        /// <summary>只有权威端选招,客户端跑 Pick 会提前清 ChangeCounter,返回 null 等包</summary>
        protected IVaultState<CruiserStateContext> NextAttack(CruiserStateContext ctx)
            => IsServer ? CruiserRotation.Pick(ctx, StateIndex) : null;

        /// <summary>
        /// 生成敌对弹幕。伤害 <c>NPC.damage / 6.9 × 倍率</c>,击退 3,owner 传 -1(全部照搬原 <c>Shoot</c>)。
        /// 客户端不生成
        /// </summary>
        protected static void Shoot(CruiserStateContext ctx, int type, Vector2 pos, Vector2 velocity,
            float damageMult = 1f, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f) {
            if (!IsServer) {
                return;
            }
            NPC npc = ctx.Npc;
            Projectile.NewProjectile(npc.GetSource_FromAI(), pos, velocity, type,
                (int)(npc.damage / CruiserDirector.ProjDamageDivisor * damageMult),
                CruiserDirector.ProjKnockback, -1, ai0, ai1, ai2);
        }
    }
}
