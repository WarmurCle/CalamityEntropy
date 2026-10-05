using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>唯一切换本体贴图的招,光束跟着 Center 和 rotation,朝向必须过线</summary>
    [VaultState((int)NihilityStateIndex.P2Laser, typeof(NihilityStateContext))]
    public class NihilityP2LaserState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P2Laser;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            if (ctx.Num1 == NihilityDirector.LaserCueFrame) {
                CEUtils.PlaySound("charge", 1, npc.Center);
                CEUtils.PlaySound("charge", 1, npc.Center);
            }
            if (ctx.Num1 < NihilityDirector.LaserAimFrames) {
                npc.velocity = (targetPos - npc.Center) * NihilityDirector.LaserAimFollow;
                npc.rotation = CEUtils.RotateTowardsAngle(npc.rotation, (npc.Center - targetPos).ToRotation(), NihilityDirector.LaserAimRateDeg.ToRadians(), true);
            }
            else {
                if (ctx.Num1 > NihilityDirector.LaserFastTrackFrame) {
                    npc.rotation = CEUtils.RotateTowardsAngle(npc.rotation, (targetPos - npc.Center).ToRotation(), NihilityDirector.LaserFastTrackDeg.ToRadians(), true);
                }
                npc.rotation = CEUtils.RotateTowardsAngle(npc.rotation, (targetPos - npc.Center).ToRotation(), NihilityDirector.LaserSlowTrackRate, false);
            }

            if (ctx.Num1 == NihilityDirector.LaserAimFrames) {
                Shoot<CruiserLaserMouth>(npc.GetSource_FromAI(), npc.Center, Vector2.Zero,
                    (int)(npc.damage / NihilityDirector.LaserDamageDivisor), NihilityDirector.LaserKnockback,
                    npc.whoAmI, NihilityDirector.LaserBeamLifetime);
            }

            if (IsServer && Main.rand.NextBool(NihilityDirector.LaserSprayChance)) {
                Shoot<CellBullet>(cell.GetSource_FromThis(),
                    cell.Center + new Vector2(Main.rand.NextFloat(-NihilityDirector.LaserSprayScatter, NihilityDirector.LaserSprayScatter), Main.rand.NextFloat(-NihilityDirector.LaserSprayScatter, NihilityDirector.LaserSprayScatter)),
                    (cell.Center - targetPos).SafeNormalize(Vector2.UnitX) * NihilityDirector.LaserSpraySpeed,
                    BulletDamage(ctx), NihilityDirector.BulletKnockback);
            }

            cell.velocity += (targetPos - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.LaserCellThrust;
            npc.velocity = (targetPos - npc.Center) * NihilityDirector.LaserFollow;
            ctx.Num1++;
            if (ctx.Num1 > NihilityDirector.LaserDuration) {
                return EndAttack(ctx);
            }
            return null;
        }
    }
}
