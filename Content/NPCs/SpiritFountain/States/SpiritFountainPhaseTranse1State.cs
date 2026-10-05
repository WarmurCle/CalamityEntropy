using CalamityEntropy.Content.NPCs.SpiritFountain.Core;
using InnoVault.StateMachines;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.States
{
    /// <summary>计时每帧额外再加一次,所以走 1、3、5,140 的门槛实际 71 帧就到,原样保留</summary>
    [VaultState((int)SpiritFountainStateIndex.PhaseTranse1, typeof(SpiritFountainStateContext))]
    public class SpiritFountainPhaseTranse1State : SpiritFountainStateBase
    {
        public override SpiritFountainStateIndex StateIndex => SpiritFountainStateIndex.PhaseTranse1;

        protected override IVaultState<SpiritFountainStateContext> RunBody(SpiritFountainStateContext ctx) {
            SpiritFountain owner = ctx.Owner;

            ctx.DontTakeDmg = true;
            owner.column1.offset *= 0;
            owner.column2.alpha = float.Lerp(owner.column2.alpha, SpiritFountainDirector.TransColumn2Alpha, SpiritFountainDirector.TransColumn2AlphaLerp);
            owner.column2.id = 1;
            //块内的第二次自增:基类在状态体之后还会再加一次,合起来每帧 +2
            Timer++;

            if (Timer > SpiritFountainDirector.TransDuration) {
                ctx.DontTakeDmg = false;
                IVaultState<SpiritFountainStateContext> next = Advance(ctx, StateIndex);
                owner.column2.alpha = SpiritFountainDirector.TransColumn2Alpha;
                return next;
            }
            return null;
        }
    }
}
