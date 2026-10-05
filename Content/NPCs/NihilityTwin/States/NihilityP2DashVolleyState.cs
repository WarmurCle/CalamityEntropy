using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>Num1 是冲刺次数不是帧计时,Num2 进状态时不清,第一次冲刺的相位取决于上一手残值,原样保留</summary>
    [VaultState((int)NihilityStateIndex.P2DashVolley, typeof(NihilityStateContext))]
    public class NihilityP2DashVolleyState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P2DashVolley;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            if (ctx.Num1 > NihilityDirector.DashVolleyReps) {
                ctx.Num2 = 0f;
                return EndAttack(ctx);
            }

            ctx.Num2--;
            if (ctx.Num2 < NihilityDirector.DashSubTimerFloor) {
                ctx.Num1++;
                ctx.Num2 = NihilityDirector.DashSubTimerReset;
                if (ctx.Num1 <= NihilityDirector.DashVolleyReps) {
                    //音效选号吃随机数,各端各选各的;不参与任何判定,保持在原位置
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
                if (ctx.FrameCounter % NihilityDirector.DashRingInterval == 0) {
                    float rot = CEUtils.randomRot();
                    for (int i = 0; i < 360; i += NihilityDirector.DashRingStepDeg) {
                        Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                            (rot + MathHelper.ToRadians(i)).ToRotationVector2() * NihilityDirector.DashRingSpeed,
                            BulletDamage(ctx), NihilityDirector.BulletKnockback);
                    }
                }
                if (ctx.FrameCounter % NihilityDirector.DashSpikeInterval == 0) {
                    Shoot<CellSpike>(npc.GetSource_FromThis(),
                        npc.Center + new Vector2(Main.rand.NextFloat(-NihilityDirector.DashSpikeScatter, NihilityDirector.DashSpikeScatter), Main.rand.NextFloat(-NihilityDirector.DashSpikeScatter, NihilityDirector.DashSpikeScatter)),
                        (npc.rotation + MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.DashSpikeSpeed,
                        BulletDamage(ctx), NihilityDirector.SpikeKnockback);
                    Shoot<CellSpike>(npc.GetSource_FromThis(),
                        npc.Center + new Vector2(Main.rand.NextFloat(-NihilityDirector.DashSpikeScatter, NihilityDirector.DashSpikeScatter), Main.rand.NextFloat(-NihilityDirector.DashSpikeScatter, NihilityDirector.DashSpikeScatter)),
                        (npc.rotation - MathHelper.PiOver2).ToRotationVector2() * NihilityDirector.DashSpikeSpeed,
                        BulletDamage(ctx), NihilityDirector.SpikeKnockback);
                }
            }

            npc.velocity *= NihilityDirector.DashDrag;
            return null;
        }
    }
}
