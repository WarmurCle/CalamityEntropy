using CalamityEntropy.Content.NPCs.SpiritFountain.Core;
using CalamityEntropy.Content.Particles.CalamityPorts;
using CalamityEntropy.Content.Projectiles.SpiritFountainShoots;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.States
{
    /// <summary>三阶段本体可以被直接打,摇摆相位和幅度过线,弹幕只在权威端</summary>
    [VaultState((int)SpiritFountainStateIndex.Moving, typeof(SpiritFountainStateContext))]
    public class SpiritFountainMovingState : SpiritFountainStateBase
    {
        public override SpiritFountainStateIndex StateIndex => SpiritFountainStateIndex.Moving;

        protected override IVaultState<SpiritFountainStateContext> RunBody(SpiritFountainStateContext ctx) {
            NPC npc = ctx.Npc;
            SpiritFountain owner = ctx.Owner;
            float enrage = ctx.Enrage;
            int phase = ctx.Phase;

            //只有本状态保留摇摆量,其余状态由宿主按原代码挂在本块上的 else 清零
            ctx.KeepMovingSway = true;

            ctx.FountainSpeed = float.Lerp(ctx.FountainSpeed, SpiritFountainDirector.MovingFountainSpeedTarget, SpiritFountainDirector.MovingFountainSpeedLerp);
            //三目两侧都是 1,等价于没有分支。原作者大概本来想给三阶段提速,照搬保留
            ctx.MCounter += SpiritFountainDirector.MovingSwayStep * enrage * (phase == SpiritFountainDirector.MovingDamageablePhase ? 1f : 1);
            ctx.MAmp = float.Lerp(ctx.MAmp, 1, SpiritFountainDirector.MovingAmpLerp * enrage);
            ctx.C1LastPos = owner.column1.offset.X;

            owner.column1.offset.X = float.Lerp(owner.column1.offset.X,
                (float)Math.Cos(ctx.MCounter) * SpiritFountainDirector.MovingSwayRange * ctx.MAmp,
                SpiritFountainDirector.MovingOffsetLerp);
            if (Timer > SpiritFountainDirector.MovingTiltStartFrame) {
                owner.column1.rotation = (ctx.C1LastPos - owner.column1.offset.X)
                    * (Main.zenithWorld ? SpiritFountainDirector.MovingTiltFactorZenith : SpiritFountainDirector.MovingTiltFactor)
                    * (1 + phase * SpiritFountainDirector.MovingTiltPhaseFactor) - MathHelper.PiOver2;
            }
            owner.column1.alpha = float.Lerp(owner.column1.alpha, SpiritFountainDirector.MovingColumnAlpha, SpiritFountainDirector.MovingColumnAlphaLerp);

            if (phase == 1) {
                if (ctx.GlobalCounter % (int)(SpiritFountainDirector.MovingP1Interval / enrage) == 0 && IsServer) {
                    //随机初速只在权威端骰,结果随弹幕本体过线;客户端不空转 Main.rand
                    Shoot<SpiritBullet>(ctx, npc.Center, CEUtils.randomRot().ToRotationVector2() * SpiritFountainDirector.MovingP1Speed, 1, npc.whoAmI);
                }
            }
            if (phase == 2) {
                if (ctx.GlobalCounter % (int)(SpiritFountainDirector.MovingP2Interval / enrage) == 0) {
                    Vector2 dir = (ctx.GlobalCounter * SpiritFountainDirector.MovingP2Phase).ToRotationVector2() * SpiritFountainDirector.MovingP2Speed;
                    Shoot<SpiritBullet>(ctx, npc.Center, dir, 1, npc.whoAmI, SpiritFountainDirector.MovingP2Ai1);
                    Shoot<SpiritBullet>(ctx, npc.Center, -dir, 1, npc.whoAmI, SpiritFountainDirector.MovingP2Ai1);
                }
            }
            if (phase == SpiritFountainDirector.MovingDamageablePhase) {
                ctx.EyeAlphaTarget = 1;
                npc.dontTakeDamage = false;

                if (ctx.GlobalCounter % (int)(SpiritFountainDirector.MovingP3Interval / enrage) == 0) {
                    for (float i = 0; i < SpiritFountainDirector.MovingP3SweepEnd; i += SpiritFountainDirector.MovingP3SweepStep) {
                        float rt = ctx.GlobalCounter * SpiritFountainDirector.MovingP3Phase + MathHelper.ToRadians(i);
                        Shoot<SpiritBullet>(ctx, npc.Center - rt.ToRotationVector2() * SpiritFountainDirector.MovingP3Radius / enrage,
                            rt.ToRotationVector2() * SpiritFountainDirector.MovingP3Speed, 1, npc.whoAmI,
                            SpiritFountainDirector.MovingP3Ai1, SpiritFountainDirector.MovingP3Ai2);
                        if (!IsLocal) {
                            continue;
                        }
                        for (float d = 0; d < 1; d += SpiritFountainDirector.MovingP3LineStep) {
                            //预警线铺满整条弹道,每 45 度扇面 40 颗量级
                            float dmx = SpiritFountainDirector.MovingP3Radius / enrage;
                            Vector2 top = npc.Center + rt.ToRotationVector2() * (d * dmx);
                            Vector2 sparkVelocity2 = (npc.Center - top) * SpiritFountainDirector.MovingP3LineVelFactor;
                            top += CEUtils.randomPointInCircle(SpiritFountainDirector.MovingP3LineScatter);
                            int sparkLifetime2 = SpiritFountainDirector.MovingP3LineLife;
                            float sparkScale2 = Main.rand.NextFloat(SpiritFountainDirector.MovingP3LineScaleMin, SpiritFountainDirector.MovingP3LineScaleMax);
                            Color sparkColor2 = Color.Lerp(Color.AliceBlue, Color.SkyBlue, Main.rand.NextFloat(0, 1));
                            PRTLoader.NewParticle<PRT_LineCal>(top, sparkVelocity2, sparkColor2, sparkScale2).Configure(false, sparkLifetime2);
                        }
                    }
                }
            }

            if (Timer > SpiritFountainDirector.MovingDuration) {
                return Advance(ctx, StateIndex);
            }
            return null;
        }
    }
}
