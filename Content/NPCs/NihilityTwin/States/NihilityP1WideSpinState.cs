using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>第 0 层伤害除数是 7 不是 6,细胞挂点整段覆盖速度,不是 1 号的累加</summary>
    [VaultState((int)NihilityStateIndex.P1WideSpin, typeof(NihilityStateContext))]
    public class NihilityP1WideSpinState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P1WideSpin;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            ctx.KeepRotSpeed = true;

            cell.rotation = npc.rotation;
            npc.velocity *= NihilityDirector.WideDrag;
            npc.velocity = (targetPos - npc.Center) * NihilityDirector.WideFollow;
            npc.rotation += ctx.RotSpeed * NihilityDirector.WideRotScale;
            ctx.RotSpeed += NihilityDirector.WideRotAccel;
            ctx.RotSpeed *= NihilityDirector.WideRotDamp;
            cell.velocity *= NihilityDirector.WideCellDrag;
            cell.velocity = (npc.Center + npc.rotation.ToRotationVector2() * NihilityDirector.WideCellOffset - cell.Center) * NihilityDirector.WideCellLerp;

            if (ctx.Num1 > NihilityDirector.WideWindup && ctx.FrameCounter % NihilityDirector.WideFireInterval == 0 && IsServer) {
                float rot = (targetPos - cell.Center).ToRotation();
                for (int i = 0; i < NihilityDirector.WideFanLayers; i++) {
                    if (i > 0) {
                        Shoot<CellBullet>(cell.GetSource_FromThis(),
                            cell.Center + new Vector2(i * NihilityDirector.WideFanBackStep, i * NihilityDirector.WideFanSideStep).RotatedBy(rot),
                            rot.ToRotationVector2() * NihilityDirector.WideFanSpeed,
                            BulletDamage(ctx), NihilityDirector.BulletKnockback);
                        Shoot<CellBullet>(cell.GetSource_FromThis(),
                            cell.Center + new Vector2(i * NihilityDirector.WideFanBackStep, i * -NihilityDirector.WideFanSideStep).RotatedBy(rot),
                            rot.ToRotationVector2() * NihilityDirector.WideFanSpeed,
                            BulletDamage(ctx), NihilityDirector.BulletKnockback);
                    }
                    else {
                        Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                            rot.ToRotationVector2() * NihilityDirector.WideFanSpeed,
                            npc.damage / NihilityDirector.ProjDamageDivisorCenter, NihilityDirector.BulletKnockback);
                    }
                }
                rot = CEUtils.randomRot();
                for (int i = 0; i < 360; i += NihilityDirector.WideRingStepDeg) {
                    Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                        (rot + MathHelper.ToRadians(i)).ToRotationVector2() * NihilityDirector.WideRingSpeed,
                        BulletDamage(ctx), NihilityDirector.BulletKnockback);
                }
            }

            ctx.Num1++;
            if (ctx.Num1 > NihilityDirector.WideDuration) {
                return EndAttack(ctx);
            }
            return null;
        }
    }
}
