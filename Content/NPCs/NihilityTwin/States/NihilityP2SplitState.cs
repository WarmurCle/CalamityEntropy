using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>小细胞超过 8 在选招被重掷,NewNPC 原没有权威端守卫,这里补上</summary>
    [VaultState((int)NihilityStateIndex.P2Split, typeof(NihilityStateContext))]
    public class NihilityP2SplitState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P2Split;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            if (ctx.Num1 == NihilityDirector.SpawnCueFrame && IsServer) {
                for (int i = 0; i < NihilityDirector.SpawnCellCount; i++) {
                    NPC.NewNPC(npc.GetSource_FromAI(), (int)cell.Center.X, (int)cell.Center.Y,
                        ModContent.NPCType<ChaoticCellSmall>(), 0, cell.whoAmI);
                }
            }
            npc.rotation = npc.velocity.ToRotation();
            ctx.Num1++;
            cell.velocity += (targetPos - cell.Center).SafeNormalize(Vector2.Zero) * NihilityDirector.SplitCellThrust;

            if (IsServer && ctx.FrameCounter % NihilityDirector.SplitRingInterval == 0) {
                float rot = CEUtils.randomRot();
                for (int i = 0; i < 360; i += NihilityDirector.SplitRingStepDeg) {
                    Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                        (rot + MathHelper.ToRadians(i)).ToRotationVector2() * NihilityDirector.SplitRingSpeed,
                        BulletDamage(ctx), NihilityDirector.BulletKnockback);
                }
            }

            npc.velocity *= NihilityDirector.SplitDrag;
            if (ctx.Num1 > NihilityDirector.SplitDuration) {
                return EndAttack(ctx);
            }
            return null;
        }
    }
}
