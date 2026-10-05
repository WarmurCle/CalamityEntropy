using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>速度每帧按与玩家差值 ×0.008 直接赋值,不会累积</summary>
    [VaultState((int)ProphetStateIndex.VoidSpike, typeof(ProphetStateContext))]
    public class ProphetVoidSpikeState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.VoidSpike;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;
            int phase = ctx.Phase;

            if (ctx.Countdown == ProphetDirector.SpikeBlinkBeat && IsServer) {
                Teleport(ctx, target.Center + CEUtils.randomRot().ToRotationVector2() * ProphetDirector.SpikeBlinkRadius);
            }

            int period = phase == 1 ? ProphetDirector.SpikePeriodP1 : ProphetDirector.SpikePeriodP2;
            if (ctx.Countdown > ProphetDirector.SpikeActiveAbove && ctx.Countdown % period == 0 && IsServer) {
                //基准角与每根的抖动都是掷骰,全在权威端守卫之内
                float r = CEUtils.randomRot();
                int damage = ProjDamage(ctx);
                float step = phase == 1 ? ProphetDirector.SpikeAngleStepP1 : ProphetDirector.SpikeAngleStepP2;
                for (float i = 0; i < 360; i += step) {
                    Shoot<ProphetVoidSpike>(ctx, npc.Center,
                        (r + MathHelper.ToRadians(i)).ToRotationVector2().RotatedByRandom(ProphetDirector.SpikeScatter) * ProphetDirector.SpikeSpeed,
                        damage, 4, 0, npc.whoAmI);
                }
            }

            npc.velocity = (target.Center - npc.Center) * ProphetDirector.SpikeDriftLerp;
            npc.rotation = npc.velocity.ToRotation();
        }
    }
}
