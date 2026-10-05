using CalamityEntropy.Content.NPCs.SpiritFountain.Core;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.States
{
    /// <summary>前 80 帧进度不涨,柱子轻晃读 GameUpdateCount,各端独立,照搬</summary>
    [VaultState((int)SpiritFountainStateIndex.Boomerang, typeof(SpiritFountainStateContext))]
    public class SpiritFountainBoomerangState : SpiritFountainStateBase
    {
        public override SpiritFountainStateIndex StateIndex => SpiritFountainStateIndex.Boomerang;

        protected override IVaultState<SpiritFountainStateContext> RunBody(SpiritFountainStateContext ctx) {
            SpiritFountain owner = ctx.Owner;
            bool phase3 = ctx.Phase == SpiritFountainDirector.BoomerangPhase3;

            owner.column1.offset.X = float.Lerp(owner.column1.offset.X,
                (float)Math.Cos(Main.GameUpdateCount * SpiritFountainDirector.BoomerangIdleFreq) * SpiritFountainDirector.BoomerangIdleAmp,
                SpiritFountainDirector.BoomerangOffsetLerp);
            owner.column1.rotation = -MathHelper.PiOver2;

            if (Timer > SpiritFountainDirector.BoomerangChargeStartFrame) {
                ctx.Num1 += phase3 ? SpiritFountainDirector.BoomerangStepP3 : SpiritFountainDirector.BoomerangStep;
            }
            else {
                //前 80 帧每帧压回 0,而不是「不涨」:魂环读到的进度必须是 0
                ctx.Num1 = 0;
            }

            if (ctx.Num1 > (phase3 ? SpiritFountainDirector.BoomerangLimitP3 : SpiritFountainDirector.BoomerangLimit)) {
                //换态那一行顺手清进度,收招口不清(原代码就是在这里清的)
                ctx.Num1 = 0;
                return Advance(ctx, StateIndex);
            }
            return null;
        }
    }
}
