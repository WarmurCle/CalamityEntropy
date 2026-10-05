using CalamityEntropy.Content.NPCs.SpiritFountain.Core;
using InnoVault.StateMachines;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.States
{
    /// <summary>火力在 SpiritRing,按本状态和 aiTimer 取模</summary>
    [VaultState((int)SpiritFountainStateIndex.Lasers, typeof(SpiritFountainStateContext))]
    public class SpiritFountainLasersState : SpiritFountainStateBase
    {
        public override SpiritFountainStateIndex StateIndex => SpiritFountainStateIndex.Lasers;

        protected override IVaultState<SpiritFountainStateContext> RunBody(SpiritFountainStateContext ctx) {
            ctx.Owner.column1.offset.X *= SpiritFountainDirector.ColumnRetractDamp;
            if (Timer > SpiritFountainDirector.LasersDuration) {
                return Advance(ctx, StateIndex);
            }
            return null;
        }
    }
}
