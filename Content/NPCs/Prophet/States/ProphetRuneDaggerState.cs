using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>除起手瞬移清零外不写速度,保持进招惯性</summary>
    [VaultState((int)ProphetStateIndex.RuneDagger, typeof(ProphetStateContext))]
    public class ProphetRuneDaggerState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.RuneDagger;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;

            npc.rotation = (target.Center - npc.Center).ToRotation();

            if (ctx.Countdown == ProphetDirector.DaggerBlinkBeat && IsServer) {
                Teleport(ctx, target.Center + CEUtils.randomRot().ToRotationVector2()
                    * Main.rand.NextFloat(ProphetDirector.DaggerBlinkRadiusMin, ProphetDirector.DaggerBlinkRadiusMax));
            }

            if (ctx.Countdown > ProphetDirector.DaggerActiveAbove
                && ctx.Countdown % ProphetDirector.DaggerPeriod == 0 && IsServer) {
                Shoot<RuneSword>(ctx, npc.Center, CEUtils.randomPointInCircle(ProphetDirector.DaggerScatter),
                    ProjDamage(ctx), 4);
            }
        }
    }
}
