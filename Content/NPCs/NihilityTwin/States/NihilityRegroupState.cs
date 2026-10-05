using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>计时在块首自增,Num1 走到 81 收手,一共跑满 81 帧</summary>
    [VaultState((int)NihilityStateIndex.Regroup, typeof(NihilityStateContext))]
    public class NihilityRegroupState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.Regroup;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            ctx.Num1++;
            npc.velocity *= NihilityDirector.RegroupDrag;
            cell.velocity *= NihilityDirector.RegroupDrag;
            if (ctx.Phase == 1) {
                if (CEUtils.getDistance(cell.Center, npc.Center) > NihilityDirector.RegroupCellLeash) {
                    cell.velocity += (npc.Center - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.RegroupCellPullP1;
                }
            }
            else {
                cell.velocity += (targetPos - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.RegroupCellPushP2;
            }
            npc.velocity += (targetPos - npc.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.RegroupThrust;
            npc.rotation = npc.velocity.ToRotation();
            ctx.Owner.SpawnParticle(ctx.Owner.buttom);

            if (ctx.Num1 > NihilityDirector.RegroupFrames) {
                return NextAttack(ctx);
            }
            return null;
        }
    }
}
