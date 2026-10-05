using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>每次闪现都是权威端掷骰并当场过线</summary>
    [VaultState((int)ProphetStateIndex.RingBlink, typeof(ProphetStateContext))]
    public class ProphetRingBlinkState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.RingBlink;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;
            float difficult = ctx.Difficult;
            int phase = ctx.Phase;

            if (ctx.Countdown > ProphetDirector.RingBlinkUntil) {
                int period = phase == 1 ? ProphetDirector.RingBlinkPeriodP1 : ProphetDirector.RingBlinkPeriodP2;
                if (ctx.Countdown % period == 0) {
                    if (IsServer) {
                        Teleport(ctx, target.Center + CEUtils.randomRot().ToRotationVector2()
                            * ProphetDirector.RingBlinkRadius / difficult);
                    }
                    CEUtils.PlaySound("crystedge_spawn_crystal", Main.rand.NextFloat(0.8f, 1.2f), npc.Center);
                    Shoot<RuneTorrent>(ctx, npc.Center,
                        (target.Center - npc.Center).normalize() * difficult * ProphetDirector.RingTorrentSpeed,
                        ProjDamage(ctx), 4, ProphetDirector.RingTorrentMaxSpeed * difficult);
                    npc.rotation = (target.Center - npc.Center).ToRotation();
                }
            }
            else {
                npc.rotation = npc.velocity.ToRotation();
                npc.velocity *= ProphetDirector.RingChaseDrag;
                npc.velocity += (target.Center - npc.Center).normalize() * ProphetDirector.RingChaseThrust;
            }
        }
    }
}
