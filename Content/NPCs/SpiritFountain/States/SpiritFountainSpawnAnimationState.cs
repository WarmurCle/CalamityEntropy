using CalamityEntropy.Common;
using CalamityEntropy.Content.NPCs.SpiritFountain.Core;
using CalamityEntropy.Content.Particles;
using InnoVault.PRT;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.States
{
    /// <summary>
    /// 300 帧聚魂期间整帧提前收工,计时压回 0,脱战和眼睛插值都跳过
    /// 落点各端都跑,魂环只在权威端,无目标也照演
    /// </summary>
    [VaultState((int)SpiritFountainStateIndex.SpawnAnimation, typeof(SpiritFountainStateContext))]
    public class SpiritFountainSpawnAnimationState : SpiritFountainStateBase
    {
        public override SpiritFountainStateIndex StateIndex => SpiritFountainStateIndex.SpawnAnimation;

        /// <summary>出场闪光的本地一次性闸。纯表现,不过线:它的作用就是「本端第一次跑到这里」</summary>
        private bool shineFired;

        public override void OnEnter(SpiritFountainStateContext ctx) {
            base.OnEnter(ctx);
            shineFired = false;
        }

        protected override IVaultState<SpiritFountainStateContext> RunBody(SpiritFountainStateContext ctx) {
            NPC npc = ctx.Npc;
            SpiritFountain owner = ctx.Owner;

            //原代码这一整块在 if (ai == SpawnAnimation) 里,配对的 else 才写 starePoint = 本地玩家,
            //所以演出期间那条 else 不执行
            ctx.StareAtLocalPlayer = false;

            if (!ctx.SetPos) {
                owner.column1.rotation = -MathHelper.PiOver2;
                npc.Opacity = 0;
                ctx.SetPos = true;
                //禁忌档案坐标写入源已删,新世界恒为 (-1,-1),pos.X < 10 回退到玩家
                //旧档有效坐标仍用档案馆位置
                Vector2 pos = EDownedBosses.GetDungeonArchiveCenterPos();
                npc.Center = pos.X < SpiritFountainDirector.ArchivePosValidX
                    ? (npc.HasValidTarget ? npc.target.ToPlayer().Center : Main.player[0].Center)
                    : pos;
                ctx.StarePoint = npc.Center;
                //落点是决策点:各端各算一次可能差几像素,让权威端立刻发一包由原版位置同步对齐
                MarkNetUpdate(ctx);
            }

            //原 == 300,过线后带 ±2 收养,等值会被跨过,客户端永远看不到出场闪光
            //改成首帧本地闸,权威端首帧倒计时就是 300,等价
            //倒计时已走完不补放
            if (!shineFired && ctx.GatheringAnimation > 0) {
                shineFired = true;
                if (IsLocal) {
                    //出场首帧双 Shine,lifetime 320 的一次性大粒子
                    PRT_ShineParticle shine1 = PRTLoader.NewParticle<PRT_ShineParticle>(npc.Center, Vector2.Zero, Color.AliceBlue, SpiritFountainDirector.GatheringShineScale1);
                    shine1.flag = true;
                    shine1.Configure(1, true, PRTDrawModeEnum.AdditiveBlend, 0, SpiritFountainDirector.GatheringShineLife);
                    PRT_ShineParticle shine2 = PRTLoader.NewParticle<PRT_ShineParticle>(npc.Center, Vector2.Zero, Color.White, SpiritFountainDirector.GatheringShineScale2);
                    shine2.flag = true;
                    shine2.Configure(1, true, PRTDrawModeEnum.AdditiveBlend, 0, SpiritFountainDirector.GatheringShineLife);
                }
            }

            //自减写在判据里:无论是否还在聚魂期都会减,演出后半段它一路走进负数,照搬
            if (ctx.GatheringAnimation-- > 0) {
                if (ctx.GatheringAnimation > SpiritFountainDirector.GatheringSpiritStopAt && IsLocal) {
                    //归魂概率 0.2~1 随倒计时递减,后半段才密起来
                    float chance = SpiritFountainDirector.GatheringSpiritChanceBase
                        + (1 - (ctx.GatheringAnimation - SpiritFountainDirector.GatheringSpiritChanceOffset) / SpiritFountainDirector.GatheringSpiritChanceSpan);
                    if (Main.rand.NextFloat() < chance) {
                        float rr = CEUtils.randomRot();
                        PRT_HomingSpiritParticle spirit = PRTLoader.NewParticle<PRT_HomingSpiritParticle>(
                            npc.Center + rr.ToRotationVector2() * SpiritFountainDirector.GatheringSpiritRadius,
                            rr.ToRotationVector2().RotatedByRandom(SpiritFountainDirector.GatheringSpiritScatter)
                                .RotatedBy(SpiritFountainDirector.GatheringSpiritSwirl * (Main.rand.NextBool() ? 1 : -1))
                                * Main.rand.NextFloat(SpiritFountainDirector.GatheringSpiritSpeedMin, SpiritFountainDirector.GatheringSpiritSpeedMax),
                            Color.AliceBlue, 1);
                        spirit.TargetPos = npc.Center;
                        spirit.Configure(1, true, PRTDrawModeEnum.AdditiveBlend, 0, -1);
                    }
                }
                //原代码在这里 return:计时压回 0,并且把尾声(脱战判定 + 眼睛插值 + 摇摆清零)一起跳过
                Timer = 0;
                ctx.HaltFrame = true;
                return null;
            }

            if (Timer > SpiritFountainDirector.SpawnStareStartFrame) {
                ctx.StarePoint = Vector2.Lerp(ctx.StarePoint, Main.LocalPlayer.Center, SpiritFountainDirector.SpawnStareLerp);
            }
            if (npc.Opacity < 1 && ctx.EyeAlpha >= SpiritFountainDirector.SpawnEyeAlphaGate) {
                npc.Opacity += SpiritFountainDirector.SpawnOpacityStep;
            }
            else {
                if (ctx.EyeAlpha < SpiritFountainDirector.SpawnEyeAlphaGate) {
                    ctx.EyeAlpha += SpiritFountainDirector.SpawnEyeAlphaStep;
                }
            }
            owner.column1.alpha = npc.Opacity * SpiritFountainDirector.SpawnColumnAlphaFactor;
            if (Timer > SpiritFountainDirector.SpawnSlowDownFrame) {
                ctx.FountainSpeed = float.Lerp(ctx.FountainSpeed, SpiritFountainDirector.SpawnFountainSpeedTarget, SpiritFountainDirector.SpawnFountainSpeedLerp);
            }

            IVaultState<SpiritFountainStateContext> next = null;
            if (Timer > SpiritFountainDirector.SpawnEndFrame) {
                npc.Opacity = 1;
                owner.column1.alpha = SpiritFountainDirector.SpawnEndColumnAlpha;
                ctx.EyeAlpha = SpiritFountainDirector.SpawnEndEyeAlpha;
                ctx.FountainSpeed = SpiritFountainDirector.SpawnFountainSpeedTarget;
                next = Advance(ctx, StateIndex);
                if (ctx.SpawnSpirits) {
                    ctx.SpawnSpirits = false;
                    ctx.CenterRing = (int)Math.Ceiling(owner.SpiritCount / 2f);
                    owner.SpawnRingSet(0);
                }
            }
            //原代码这一行在换态语句之后、块结束之前,所以换态那一帧也要走
            ctx.EyeAlphaTarget = ctx.EyeAlpha;
            return next;
        }
    }
}
