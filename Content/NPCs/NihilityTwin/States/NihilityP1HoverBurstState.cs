using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>收招判定写在推进段之前,收招会清掉计时,那一帧推进段不执行,原样保留</summary>
    [VaultState((int)NihilityStateIndex.P1HoverBurst, typeof(NihilityStateContext))]
    public class NihilityP1HoverBurstState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P1HoverBurst;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            TrailBurst(ctx);

            if (ctx.Num1 > 0 || CEUtils.getDistance(targetPos, npc.Center) < NihilityDirector.HoverApproachDistance) {
                ctx.Num1++;
            }
            else {
                if (npc.velocity.Length() > NihilityDirector.HoverApproachSpeedCap) {
                    npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * NihilityDirector.HoverApproachSpeedCap;
                }
                npc.velocity = (targetPos - new Vector2(0, NihilityDirector.HoverApproachHeight) - npc.Center) * NihilityDirector.HoverApproachFollow;
                if (CEUtils.getDistance(targetPos, npc.Center + npc.velocity * 2) < NihilityDirector.HoverApproachDistance) {
                    npc.velocity *= NihilityDirector.HoverApproachBrake;
                }
            }

            IVaultState<NihilityStateContext> next = null;
            if (ctx.Num1 > NihilityDirector.HoverDuration) {
                next = EndAttack(ctx);
            }

            if (ctx.Num1 > 0) {
                npc.velocity *= NihilityDirector.HoverDrag;
                npc.velocity += (targetPos - npc.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.HoverThrust;
                cell.velocity = (npc.Center + (targetPos - npc.Center).SafeNormalize(Vector2.UnitX) * NihilityDirector.HoverCellReach - cell.Center) * NihilityDirector.HoverCellLerp;
                if (IsServer && ctx.FrameCounter % NihilityDirector.HoverRingInterval == 0) {
                    float rot = CEUtils.randomRot();
                    for (int i = 0; i < 360; i += NihilityDirector.HoverRingStepDeg) {
                        Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                            (rot + MathHelper.ToRadians(i)).ToRotationVector2() * NihilityDirector.HoverRingSpeed,
                            BulletDamage(ctx), NihilityDirector.BulletKnockback);
                    }
                }
            }

            npc.rotation = npc.velocity.ToRotation();
            return next;
        }
    }
}
