using InnoVault.StateMachines;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.Core
{
    /// <summary>
    /// 选招没有轮换表,七个块按顺序 if,各自写死下一手
    /// 血量跌破 66% 插入转阶段,之后十字斩终局循环,不再回主链
    /// 轮换允许连续重复同一招
    /// </summary>
    public static class SpiritFountainRotation
    {
        /// <summary>由注册表创建状态实例(未注册返回 null,框架已打日志)</summary>
        public static IVaultState<SpiritFountainStateContext> Create(SpiritFountainStateIndex state)
            => VaultStateRegistry<SpiritFountainStateContext>.Create((int)state);

        /// <summary>只权威端调,aiTimer 由 OnEnter 归零,Counter 和摇摆故意不清</summary>
        public static IVaultState<SpiritFountainStateContext> Pick(SpiritFountainStateContext ctx, SpiritFountainStateIndex from)
            => Create(Next(from));

        /// <summary>链序映射,逐条对应原代码换态语句里的 <c>ai = AIStyle.X</c></summary>
        public static SpiritFountainStateIndex Next(SpiritFountainStateIndex from) => from switch {
            SpiritFountainStateIndex.SpawnAnimation => SpiritFountainStateIndex.Moving,
            SpiritFountainStateIndex.Moving => SpiritFountainStateIndex.Boomerang,
            SpiritFountainStateIndex.Boomerang => SpiritFountainStateIndex.Lasers,
            SpiritFountainStateIndex.Lasers => SpiritFountainStateIndex.RingFountains,
            SpiritFountainStateIndex.RingFountains => SpiritFountainStateIndex.Moving,
            SpiritFountainStateIndex.PhaseTranse1 => SpiritFountainStateIndex.SpiritSlicing,
            //十字斩不走换态,它在状态体里原地把计时归零重开。这一条只是超时兜底时的去处
            SpiritFountainStateIndex.SpiritSlicing => SpiritFountainStateIndex.SpiritSlicing,
            _ => SpiritFountainStateIndex.Moving,
        };

        /// <summary>顺序 if 不是 else if,换到更靠后同帧续跑读到 0,换回更靠前等下一帧从 1 起跑</summary>
        public static int ChainOrder(SpiritFountainStateIndex index) => index switch {
            SpiritFountainStateIndex.SpawnAnimation => 0,
            SpiritFountainStateIndex.Moving => 2,
            SpiritFountainStateIndex.Boomerang => 3,
            SpiritFountainStateIndex.Lasers => 4,
            SpiritFountainStateIndex.RingFountains => 5,
            SpiritFountainStateIndex.PhaseTranse1 => 6,
            SpiritFountainStateIndex.SpiritSlicing => 7,
            _ => 0,
        };

        /// <summary>同上,取实例的链序;不是本 Boss 的状态就按「最靠后」处理,不续跑</summary>
        public static int ChainOrder(IVaultState<SpiritFountainStateContext> state)
            => state is SpiritFountainStateBase typed ? ChainOrder(typed.StateIndex) : int.MaxValue;

        /// <summary>转阶段插入在出场之后、横扫之前,切出去的演出永远同帧续跑</summary>
        public const int PhaseTriggerOrder = 1;
    }
}
