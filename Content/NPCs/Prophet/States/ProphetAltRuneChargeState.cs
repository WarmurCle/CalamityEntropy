using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>角度上限写的是 358 不是 360,一阶段 72 度只排 5 个,二阶段 60 度排 6 个,原样保留</summary>
    [VaultState((int)ProphetStateIndex.AltRuneCharge, typeof(ProphetStateContext))]
    public class ProphetAltRuneChargeState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.AltRuneCharge;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;
            float difficult = ctx.Difficult;
            int phase = ctx.Phase;
            int cd = ctx.Countdown;

            if (cd == ProphetDirector.AltBlinkBeat && IsServer) {
                Teleport(ctx, target.Center + CEUtils.randomRot().ToRotationVector2()
                    * ProphetDirector.AltBlinkRadius / difficult);
            }

            if (cd == ProphetDirector.AltRuneBeat && IsServer) {
                //原代码这一块漏了 netMode 守卫,客户端会各自多生成一圈只存在于本地的符文;
                //按联机契约收归权威端,弹幕本身由服务端广播
                float r = CEUtils.randomRot();
                int damage = ProjDamage(ctx);
                float step = phase == 1 ? ProphetDirector.AltRuneStepP1 : ProphetDirector.AltRuneStepP2;
                for (float i = 0; i < ProphetDirector.AltRuneAngleLimit; i += step) {
                    Shoot<ProphetRuneAlt>(ctx, npc.Center, Vector2.Zero, damage, 2, npc.whoAmI,
                        r + MathHelper.ToRadians(i),
                        Main.rand.Next(ProphetDirector.OrbRuneVariantMin, ProphetDirector.OrbRuneVariantMax));
                }
            }

            if (cd == ProphetDirector.AltChargeBeatA || cd == ProphetDirector.AltChargeBeatB || cd == ProphetDirector.AltChargeBeatC) {
                //锁向 + 一帧定速:运动,各端都跑。朝向是持久累加量,决策点当场过线
                npc.rotation = (PredictTarget(ctx, ProphetDirector.AltChargeLeadFrames) - npc.Center).ToRotation();
                npc.ai[1] = ProphetDirector.AltChargeFrames;
                npc.velocity = npc.rotation.ToRotationVector2() * difficult
                    * (phase == 1 ? ProphetDirector.AltChargeSpeedP1 : ProphetDirector.AltChargeSpeedP2)
                    * ProphetDirector.AltChargeSpeedBase;
                MarkNetUpdate(ctx);
            }

            if (npc.ai[1] > 0) {
                npc.ai[1]--;
                npc.velocity *= ProphetDirector.AltChargeDrag;
            }
            else {
                npc.velocity *= ProphetDirector.AltBrakeDrag;
            }
        }
    }
}
