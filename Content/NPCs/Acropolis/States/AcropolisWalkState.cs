using CalamityEntropy.Content.NPCs.Acropolis.Core;
using InnoVault.StateMachines;

namespace CalamityEntropy.Content.NPCs.Acropolis.States
{
    /// <summary>选招口在地面推进之前骰点,骰到单发就不换态</summary>
    [VaultState((int)AcropolisStateIndex.Walk, typeof(AcropolisStateContext))]
    public class AcropolisWalkState : AcropolisStateBase
    {
        public override AcropolisStateIndex StateIndex => AcropolisStateIndex.Walk;

        /// <summary>选招口本身不该超时,超时兜底只对实招有意义</summary>
        public override int TimeoutFrames => int.MaxValue;

        public override IVaultState<AcropolisStateContext> OnUpdate(AcropolisStateContext ctx) {
            //跨招冷却由宿主每帧按 enrange 扣,这里只判到点。骰子只在权威端摇
            if (ctx.TeslaCD <= 0f && IsServer) {
                IVaultState<AcropolisStateContext> next = AcropolisRotation.Pick(ctx);
                if (next != null) {
                    return next;
                }
            }

            //追高跳:原代码写在地面推进块里的 JumpCD <= -260 那一支
            if (ctx.Grounded && ctx.HarpoonOnLauncher && !ctx.Airborne
                && ctx.Player.Center.Y + AcropolisDirector.LeapHeightGate * ctx.Npc.scale < ctx.Npc.Center.Y
                && ctx.Owner.JumpCD <= AcropolisDirector.LeapReadyJumpCD) {
                return IsServer ? Create(AcropolisStateIndex.Leap) : null;
            }

            return null;
        }
    }
}
