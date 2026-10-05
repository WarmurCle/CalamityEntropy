using CalamityEntropy.Content.NPCs.SpiritFountain.Core;
using InnoVault.StateMachines;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.States
{
    /// <summary>全链唯一一次换回更靠前的状态,横扫等下一帧从 1 起跑</summary>
    [VaultState((int)SpiritFountainStateIndex.RingFountains, typeof(SpiritFountainStateContext))]
    public class SpiritFountainRingFountainsState : SpiritFountainStateBase
    {
        public override SpiritFountainStateIndex StateIndex => SpiritFountainStateIndex.RingFountains;

        protected override IVaultState<SpiritFountainStateContext> RunBody(SpiritFountainStateContext ctx) {
            ctx.Owner.column1.offset.X *= SpiritFountainDirector.ColumnRetractDamp;
            if (Timer > SpiritFountainDirector.RingFountainsDuration) {
                return Advance(ctx, StateIndex);
            }
            return null;
        }
    }
}
