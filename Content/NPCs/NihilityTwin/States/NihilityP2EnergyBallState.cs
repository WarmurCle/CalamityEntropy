using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>三个拍点都是等值判定,计时每帧 +1,不会漏拍,只在权威端生成</summary>
    [VaultState((int)NihilityStateIndex.P2EnergyBall, typeof(NihilityStateContext))]
    public class NihilityP2EnergyBallState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P2EnergyBall;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            if ((ctx.Num1 == NihilityDirector.BallCueFrame1 || ctx.Num1 == NihilityDirector.BallCueFrame2 || ctx.Num1 == NihilityDirector.BallCueFrame3) && IsServer) {
                float rot = CEUtils.randomRot();
                for (int i = 0; i < 360; i += NihilityDirector.BallStepDeg) {
                    Shoot<NihilityEnergyBall>(cell.GetSource_FromThis(), cell.Center, Vector2.Zero,
                        BulletDamage(ctx), NihilityDirector.BulletKnockback,
                        cell.whoAmI, rot + MathHelper.ToRadians(i));
                }
            }

            npc.velocity *= NihilityDirector.BallDrag;
            ctx.Num1++;
            npc.velocity += (targetPos - npc.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.BallThrust;
            npc.rotation = npc.velocity.ToRotation();
            cell.velocity += (targetPos - cell.Center).SafeNormalize(Vector2.UnitX) * NihilityDirector.BallCellThrust;

            if (ctx.Num1 > NihilityDirector.BallDuration) {
                return EndAttack(ctx);
            }
            return null;
        }
    }
}
