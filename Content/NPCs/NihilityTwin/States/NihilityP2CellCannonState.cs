using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>
    /// 蓄力期每两帧额外 +1,发射拍落在 &lt; 100 与 == 100 上,双跳不会发生
    /// 朝向写进 Num3,原 ai[3] 留给状态号,所以挪成要过线的字段
    /// </summary>
    [VaultState((int)NihilityStateIndex.P2CellCannon, typeof(NihilityStateContext))]
    public class NihilityP2CellCannonState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P2CellCannon;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            ctx.KeepRotSpeed = true;

            if (ctx.Num1 > 0 || CEUtils.getDistance(cell.Center, npc.Center) < NihilityDirector.CannonDockDistance) {
                ctx.Num1++;
            }
            else {
                npc.velocity *= NihilityDirector.CannonDockDrag;
                cell.velocity += (npc.Center - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.CannonDockPull;
            }

            if (ctx.Num1 > 0 && ctx.Num1 < NihilityDirector.CannonChargeFrames) {
                npc.velocity *= NihilityDirector.CannonDrag;
                npc.velocity = (targetPos - npc.Center) * NihilityDirector.CannonFollow;
                npc.rotation += ctx.RotSpeed;
                ctx.RotSpeed += NihilityDirector.CannonRotAccel;
                ctx.RotSpeed *= NihilityDirector.CannonRotDamp;
                ctx.Owner.PlaceCell(npc.Center + new Vector2(NihilityDirector.CannonCellOffset, 0).RotatedBy(npc.rotation));
                cell.velocity *= 0;
                if (ctx.FrameCounter % NihilityDirector.CannonDoubleTickInterval == 0) {
                    ctx.Num1++;
                }
            }

            if (ctx.Num1 == NihilityDirector.CannonChargeFrames) {
                npc.rotation = (targetPos - npc.Center).ToRotation();
                cell.velocity = npc.rotation.ToRotationVector2() * NihilityDirector.CannonLaunchSpeed;
                npc.rotation += MathHelper.Pi;
                ctx.Owner.PlaceCell(npc.Center + new Vector2(NihilityDirector.CannonCellOffset, 0).RotatedBy(npc.rotation));
                ctx.Num3 = npc.rotation;
                MarkNetUpdate(ctx);
            }

            if (ctx.Num1 > NihilityDirector.CannonChargeFrames && ctx.Num1 < NihilityDirector.CannonRecoilEndFrame) {
                ctx.RotSpeed *= NihilityDirector.CannonRecoilRotDamp;
                npc.rotation += ctx.RotSpeed;
                cell.velocity = ctx.Num3.ToRotationVector2() * NihilityDirector.CannonTrailSpeed;
                if (ctx.FrameCounter % NihilityDirector.CannonFireInterval == 0 && IsServer) {
                    Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                        (cell.velocity.ToRotation() + MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.CannonBulletSpeed,
                        BulletDamage(ctx), NihilityDirector.BulletKnockback);
                    Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                        (cell.velocity.ToRotation() - MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.CannonBulletSpeed,
                        BulletDamage(ctx), NihilityDirector.BulletKnockback);
                }
            }

            if (ctx.Num1 > NihilityDirector.CannonRecallFrame) {
                cell.velocity *= NihilityDirector.CannonCellDrag;
                cell.velocity += (targetPos - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.CannonCellThrust;
            }

            if (ctx.Num1 > NihilityDirector.CannonDuration) {
                return EndAttack(ctx);
            }
            return null;
        }
    }
}
