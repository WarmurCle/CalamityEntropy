using CalamityEntropy.Content.NPCs.Cruiser.Core;
using CalamityEntropy.Content.Projectiles.Cruiser;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>二阶段第一手带着残值起跑,超过 150 第一帧收招,照搬</summary>
    [VaultState((int)CruiserStateIndex.VoidSpike, typeof(CruiserStateContext))]
    public class CruiserVoidSpikeState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.VoidSpike;

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            Player player = ctx.Target;

            npc.velocity = npc.velocity.normalize()
                * (npc.velocity.Length() + (CruiserDirector.SpikeSpeedTarget - npc.velocity.Length()) * CruiserDirector.SpikeSpeedLerp);
            npc.velocity = CEUtils.RotateTowardsAngle(npc.velocity.ToRotation(),
                (player.Center - npc.Center).ToRotation(), CruiserDirector.SpikeTurnRate, false).ToRotationVector2()
                * npc.velocity.Length();

            ctx.ChangeCounter++;
            //等值在这里安全,权威端 ChangeCounter 不被收养,每帧 +1,这一拍全在 IsServer 门内
            if (ctx.ChangeCounter == CruiserDirector.SpikeRingFrameA
                || ctx.ChangeCounter == CruiserDirector.SpikeRingFrameB
                || ctx.ChangeCounter == CruiserDirector.SpikeRingFrameC
                || ctx.ChangeCounter == CruiserDirector.SpikeRingFrameD) {
                if (IsServer) {
                    for (float i = 0; i < 360; i += CruiserDirector.SpikeRingAngleStep) {
                        Shoot(ctx, ModContent.ProjectileType<VoidSpike>(), npc.Center,
                            MathHelper.ToRadians(i).ToRotationVector2() * CruiserDirector.SpikeRingSpeed);
                    }
                    MarkNetUpdate(ctx);
                }
            }
            if (ctx.ChangeCounter > CruiserDirector.SpikeDuration) {
                return NextAttack(ctx);
            }
            return null;
        }
    }
}
