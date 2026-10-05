using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>基准角原各端各抽,现在收归权威端随 ExtraAI 过线,0.5° 自转各端都跑</summary>
    [VaultState((int)NihilityStateIndex.P1Circle, typeof(NihilityStateContext))]
    public class NihilityP1CircleState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P1Circle;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            cell.velocity *= NihilityDirector.CircleCellDrag;
            cell.velocity += (targetPos - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.CircleCellThrust;
            cell.ai[2] = NihilityDirector.CircleCellGlow;
            npc.velocity = npc.rotation.ToRotationVector2() * NihilityDirector.CircleSpeed;
            npc.rotation = CEUtils.RotateTowardsAngle(npc.rotation, (cell.Center - npc.Center).ToRotation(), NihilityDirector.CircleTurnRate, false);

            if (ctx.Num1 == NihilityDirector.CircleRingRollFrame && IsServer) {
                ctx.Num2 = CEUtils.randomRot();
                MarkNetUpdate(ctx);
            }
            ctx.Num2 += MathHelper.ToRadians(NihilityDirector.CircleRingSpinDeg);
            ctx.Num1++;

            if (IsServer) {
                if (Main.GameUpdateCount % NihilityDirector.CircleSpikeInterval == 0) {
                    Shoot<CellSpike>(npc.GetSource_FromThis(), npc.Center,
                        (npc.rotation + MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.CircleSpikeSpeed,
                        BulletDamage(ctx), NihilityDirector.SpikeKnockback);
                    Shoot<CellSpike>(npc.GetSource_FromThis(), npc.Center,
                        (npc.rotation - MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.CircleSpikeSpeed,
                        BulletDamage(ctx), NihilityDirector.SpikeKnockback);
                }
                if (ctx.Num1 < NihilityDirector.CircleBurstFrame) {
                    if (Main.GameUpdateCount % NihilityDirector.CircleRingInterval == 0) {
                        for (int i = 0; i < 360; i += NihilityDirector.CircleRingStepDeg) {
                            Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                                (ctx.Num2 + MathHelper.ToRadians(i)).ToRotationVector2() * NihilityDirector.CircleRingSpeed,
                                BulletDamage(ctx), NihilityDirector.BulletKnockback);
                        }
                    }
                }
                else {
                    for (int i = 0; i < 360; i += NihilityDirector.CircleRingStepDeg) {
                        if (Main.rand.NextBool(NihilityDirector.CircleBurstChance)) {
                            Shoot<CellBullet>(cell.GetSource_FromThis(),
                                cell.Center + new Vector2(Main.rand.Next(-NihilityDirector.CircleBurstScatter, NihilityDirector.CircleBurstScatter), Main.rand.Next(-NihilityDirector.CircleBurstScatter, NihilityDirector.CircleBurstScatter)),
                                (ctx.Num2 + MathHelper.ToRadians(i)).ToRotationVector2() * NihilityDirector.CircleBurstSpeed,
                                BulletDamage(ctx), NihilityDirector.BulletKnockback);
                        }
                    }
                }
            }

            if (ctx.Num1 > NihilityDirector.CircleDuration) {
                return EndAttack(ctx);
            }
            return null;
        }
    }
}
