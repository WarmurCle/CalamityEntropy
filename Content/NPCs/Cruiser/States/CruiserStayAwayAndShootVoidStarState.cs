using CalamityEntropy.Content.NPCs.Cruiser.Core;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>第 90 帧是尾部新星两个触发口之一</summary>
    [VaultState((int)CruiserStateIndex.StayAwayAndShootVoidStar, typeof(CruiserStateContext))]
    public class CruiserStayAwayAndShootVoidStarState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.StayAwayAndShootVoidStar;

        /// <summary>原 == 90 会被 ±2 收养跨过或重放,改成闩锁加 &gt;= 90,宽限窗压掉中途补放</summary>
        private bool whipCued;

        public override void OnEnter(CruiserStateContext ctx) {
            base.OnEnter(ctx);
            whipCued = false;
        }

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            Player player = ctx.Target;
            Vector2 dir = (player.Center - npc.Center).normalize();

            if (npc.velocity.Length() < CruiserDirector.StayAwaySpeedCap) {
                npc.velocity *= CruiserDirector.StayAwayAccel;
            }
            else {
                npc.velocity *= CruiserDirector.StayAwayDrag;
            }

            ctx.ChangeCounter++;
            if (!whipCued && ctx.ChangeCounter >= CruiserDirector.StayAwayWhipCue) {
                whipCued = true;
                if (!CuePassed(ctx.ChangeCounter, CruiserDirector.StayAwayWhipCue)) {
                    ctx.TailWhipCue = true;
                    MarkNetUpdate(ctx);
                }
            }
            if (ctx.ChangeCounter > CruiserDirector.StayAwayTurnStart) {
                npc.velocity = Vector2.Lerp(npc.velocity, dir * npc.velocity.Length(), CruiserDirector.StayAwayTurnLerp);
                if (npc.velocity.Length() < CruiserDirector.StayAwaySpeedCap) {
                    npc.velocity *= CruiserDirector.StayAwayTurnAccel;
                }
            }
            if (ctx.ChangeCounter > CruiserDirector.StayAwayPushStart) {
                if (npc.velocity.Length() < CruiserDirector.StayAwaySpeedCap) {
                    npc.velocity *= CruiserDirector.StayAwayPushAccel;
                }
                npc.velocity += dir * CruiserDirector.StayAwayPushThrust;
                npc.velocity = Vector2.Lerp(npc.velocity, dir * npc.velocity.Length(), CruiserDirector.StayAwayPushLerp);
                npc.velocity *= CruiserDirector.StayAwayPushDrag;
            }
            if (ctx.ChangeCounter > CruiserDirector.StayAwayDuration) {
                return NextAttack(ctx);
            }
            return null;
        }
    }
}
