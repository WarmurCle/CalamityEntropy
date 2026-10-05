using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>开场给全体玩家 60 帧无敌,出射角取 GameUpdateCount,生成在权威端</summary>
    [VaultState((int)NihilityStateIndex.P1Orbit, typeof(NihilityStateContext))]
    public class NihilityP1OrbitState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P1Orbit;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            if (ctx.Num1 == 0) {
                //原代码连播两次同一条起手音,原样保留
                CEUtils.PlaySound("charge", 1, npc.Center);
                CEUtils.PlaySound("charge", 1, npc.Center);
                npc.rotation = (npc.Center - targetPos).ToRotation();
                //无敌帧要在各端各自写:玩家受击判定本来就跑在自己那台机器上
                foreach (Player player in Main.ActivePlayers) {
                    player.Entropy().immune = NihilityDirector.OrbitGraceFrames;
                }
            }
            ctx.Num1++;

            Vector2 t = npc.rotation.ToRotationVector2() * NihilityDirector.OrbitRadius;
            if (ctx.Num1 < NihilityDirector.OrbitReleaseFrame) {
                npc.velocity = (targetPos + t - npc.Center) * NihilityDirector.OrbitFollow;
                cell.velocity = (targetPos - t - cell.Center) * NihilityDirector.OrbitFollow;
            }
            else {
                npc.velocity.Y -= NihilityDirector.OrbitRiseAccel;
                cell.velocity.Y -= NihilityDirector.OrbitRiseAccel;
            }

            if (IsServer && ctx.Num1 > NihilityDirector.OrbitFireStart && ctx.Num1 < NihilityDirector.OrbitReleaseFrame
                && Main.rand.NextBool(NihilityDirector.OrbitFireChance)) {
                if (Main.rand.NextBool(NihilityDirector.OrbitSplitChance)) {
                    Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                        (Main.GameUpdateCount * NihilityDirector.OrbitBulletSpin).ToRotationVector2() * NihilityDirector.OrbitBulletSpeed,
                        BulletDamage(ctx), NihilityDirector.BulletKnockback);
                }
                else {
                    Shoot<NihilityFire>(npc.GetSource_FromThis(), npc.Center,
                        (Main.GameUpdateCount * NihilityDirector.OrbitFireSpin).ToRotationVector2() * NihilityDirector.OrbitFireSpeed,
                        BulletDamage(ctx), NihilityDirector.BulletKnockback);
                }
            }

            IVaultState<NihilityStateContext> next = null;
            if (ctx.Num1 > NihilityDirector.OrbitDuration) {
                next = EndAttack(ctx);
            }
            npc.rotation += MathHelper.ToRadians(NihilityDirector.OrbitSelfSpinDeg);
            return next;
        }
    }
}
