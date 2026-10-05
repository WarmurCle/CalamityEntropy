using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>侧刺原代码漏在 !client 守卫外面,客户端会造幽灵弹,这里走带守卫的出手口</summary>
    [VaultState((int)NihilityStateIndex.P2DashSpiral, typeof(NihilityStateContext))]
    public class NihilityP2DashSpiralState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P2DashSpiral;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            if (ctx.Num1 > NihilityDirector.SpiralReps) {
                ctx.Num2 = 0f;
                return EndAttack(ctx);
            }

            ctx.Num2--;
            if (ctx.Num2 < NihilityDirector.DashSubTimerFloor) {
                ctx.Num1++;
                ctx.Num2 = NihilityDirector.DashSubTimerReset;
                if (ctx.Num1 <= NihilityDirector.SpiralReps) {
                    CEUtils.PlaySound("beast_ghostdash" + Main.rand.Next(1, 5), 1, npc.Center);
                }
            }
            if (ctx.Num2 > 0f) {
                npc.velocity += npc.rotation.ToRotationVector2() * NihilityDirector.DashThrust;
                TrailBurst(ctx);
            }
            else {
                npc.rotation = CEUtils.RotateTowardsAngle(npc.rotation, (targetPos - npc.Center).ToRotation(), NihilityDirector.DashTurnRate, false);
            }
            cell.velocity += (targetPos - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.DashCellThrust;

            if (IsServer) {
                if (ctx.FrameCounter % NihilityDirector.SpiralRingInterval == 0) {
                    float rot = MathHelper.ToRadians((Main.GameUpdateCount * NihilityDirector.SpiralAngleScale) % 360);
                    for (int i = 0; i < 360; i += NihilityDirector.SpiralRingStepDeg) {
                        Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                            (rot + MathHelper.ToRadians(i)).ToRotationVector2() * NihilityDirector.SpiralRingSpeed,
                            BulletDamage(ctx), NihilityDirector.BulletKnockback);
                    }
                }
                if (ctx.FrameCounter % NihilityDirector.SpiralSpikeInterval == 0) {
                    Shoot<CellSpike>(npc.GetSource_FromThis(), npc.Center,
                        (npc.rotation + MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.SpiralSpikeSpeed,
                        BulletDamage(ctx), NihilityDirector.SpikeKnockback);
                    Shoot<CellSpike>(npc.GetSource_FromThis(), npc.Center,
                        (npc.rotation - MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.SpiralSpikeSpeed,
                        BulletDamage(ctx), NihilityDirector.SpikeKnockback);
                }
            }

            npc.velocity *= NihilityDirector.DashDrag;
            return null;
        }
    }
}
