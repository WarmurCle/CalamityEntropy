using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>计时必须先进玩家 1200 以内才起跳,Nz 扣本体速度,原代码没同步,补进 ExtraAI</summary>
    [VaultState((int)NihilityStateIndex.P1CellLance, typeof(NihilityStateContext))]
    public class NihilityP1CellLanceState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P1CellLance;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            if (ctx.Num1 > 0 || CEUtils.getDistance(targetPos, npc.Center) < NihilityDirector.LanceApproachDistance) {
                ctx.Num1++;
            }
            else {
                npc.velocity = (targetPos - npc.Center) * NihilityDirector.LanceApproachFollow;
            }

            if (ctx.Num1 > 0) {
                if (ctx.Num1 < NihilityDirector.LanceWindupFrames) {
                    npc.velocity *= NihilityDirector.LanceWindupDrag;
                    cell.velocity = (npc.Center + (targetPos - npc.Center).SafeNormalize(Vector2.UnitX) * NihilityDirector.LanceCellReach - cell.Center) * NihilityDirector.LanceCellLerp;
                    ctx.Nz = (targetPos - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.LanceAimSpeed;
                }
                else if (ctx.Num1 < NihilityDirector.LanceThrustFrames) {
                    Vector2 j = ctx.Nz * (ctx.Num1 / (float)NihilityDirector.LanceThrustFrames);
                    cell.velocity += j * NihilityDirector.LanceCellAccel;
                    npc.velocity -= j * NihilityDirector.LanceBodyRecoil;
                    if (ctx.FrameCounter % NihilityDirector.LanceFireInterval == 0 && IsServer) {
                        Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                            (cell.velocity.ToRotation() + MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.LanceBulletSpeed,
                            BulletDamage(ctx), NihilityDirector.BulletKnockback);
                        Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                            (cell.velocity.ToRotation() - MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.LanceBulletSpeed,
                            BulletDamage(ctx), NihilityDirector.BulletKnockback);
                    }
                }
                else {
                    //同样是先乘阻尼再整段覆盖,前一句不起作用,照搬
                    npc.velocity *= NihilityDirector.LanceRecoverDrag;
                    npc.velocity = (targetPos - npc.Center) * NihilityDirector.LanceRecoverFollow;
                    cell.velocity = (npc.Center - cell.Center) * NihilityDirector.LanceCellRecall;
                }
            }

            npc.rotation = (npc.Center - cell.Center).ToRotation();

            if (ctx.Num1 > NihilityDirector.LanceDuration) {
                return EndAttack(ctx);
            }
            return null;
        }
    }
}
