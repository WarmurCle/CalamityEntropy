using CalamityEntropy.Content.NPCs.Acropolis.Core;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Acropolis.States
{
    /// <summary>原 JumpCD &lt;= -260,不开火,跳射计数为负,落地闸开着</summary>
    [VaultState((int)AcropolisStateIndex.Leap, typeof(AcropolisStateContext))]
    public class AcropolisLeapState : AcropolisStateBase
    {
        public override AcropolisStateIndex StateIndex => AcropolisStateIndex.Leap;

        public override void OnEnter(AcropolisStateContext ctx) {
            base.OnEnter(ctx);
            NPC npc = ctx.Npc;
            ctx.Airborne = true;
            //原代码的追高跳写在地面推进块内部,起跳这一帧悬停控制与横向推进照样跑完
            ctx.KeepGroundedThisFrame = true;
            ctx.Owner.JumpCD = AcropolisDirector.LeapJumpCD;
            if (ctx.Target != null) {
                npc.velocity = new Vector2(
                    AcropolisDirector.LeapSpeedXFactor * (ctx.Target.Center.X - npc.Center.X) / npc.scale,
                    float.Max((ctx.Target.Center.Y - npc.Center.Y) / npc.scale * AcropolisDirector.LeapSpeedYFactor, AcropolisDirector.LeapSpeedYMax))
                    * npc.scale;
            }
        }

        public override IVaultState<AcropolisStateContext> OnUpdate(AcropolisStateContext ctx) {
            if (!ctx.Airborne) {
                return BackToWalk(ctx);
            }
            return null;
        }
    }
}
