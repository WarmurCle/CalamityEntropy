using CalamityEntropy.Content.NPCs.Cruiser.Core;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>开局锁向之后不再追瞄,锁向在权威端打一次 netUpdate</summary>
    [VaultState((int)CruiserStateIndex.QuickDash, typeof(CruiserStateContext))]
    public class CruiserQuickDashState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.QuickDash;

        /// <summary>原 == 0 会被收养跨过,冲刺方向会来自残留朝向,改成闩锁加宽限窗</summary>
        private bool aimLocked;

        public override void OnEnter(CruiserStateContext ctx) {
            base.OnEnter(ctx);
            aimLocked = false;
        }

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            Player player = ctx.Target;

            if (!aimLocked) {
                aimLocked = true;
                if (!CuePassed(ctx.ChangeCounter, 0)) {
                    npc.rotation = (player.Center - npc.Center).ToRotation();
                    MarkNetUpdate(ctx);
                }
            }
            ctx.ChangeCounter++;

            if (ctx.ChangeCounter > CruiserDirector.QuickDashThrustFrames) {
                npc.velocity *= CruiserDirector.QuickDashChaseDrag;
                npc.velocity += (player.Center - npc.Center).normalize() * CruiserDirector.QuickDashChaseThrust;
            }
            else {
                npc.velocity += npc.rotation.ToRotationVector2() * CruiserDirector.QuickDashThrust;
            }
            if (ctx.ChangeCounter > CruiserDirector.QuickDashDuration) {
                return NextAttack(ctx);
            }
            return null;
        }
    }
}
