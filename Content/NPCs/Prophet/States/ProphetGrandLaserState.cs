using CalamityEntropy.Common;
using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>
    /// 四周 250 见方有实心块就作废,倒计时压到 30
    /// 承伤 ×0.5,减伤从 0.12 抬到 0.50,拖人各端都写,只有玩家自己那端算数
    /// </summary>
    [VaultState((int)ProphetStateIndex.GrandLaser, typeof(ProphetStateContext))]
    public class ProphetGrandLaserState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.GrandLaser;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;
            int cd = ctx.Countdown;

            npc.velocity *= ProphetDirector.LaserDrag;
            npc.rotation = (target.Center - npc.Center).ToRotation();

            if (cd == ProphetDirector.LaserSetupBeat) {
                //清场会把场上残留的符文晶体全部打掉,各端都跑,按原写法保留
                int type = ModContent.ProjectileType<RuneCrystalTop>();
                foreach (Projectile p in Main.ActiveProjectiles) {
                    if (p.type == type) {
                        p.Kill();
                    }
                }
                npc.velocity *= 0;

                if (IsServer) {
                    //禁忌档案坐标写入源已删,新世界恒为 (-1,-1)
                    //无效坐标跳过档案馆传送,旧档有效值仍保留原演出
                    if (EDownedBosses.ForbiddenArchiveCenter.X >= 0) {
                        Teleport(ctx, EDownedBosses.GetDungeonArchiveCenterPos() + new Vector2(0, ProphetDirector.LaserArchiveOffsetY));
                    }
                    if (CEUtils.getDistance(npc.Center, target.Center) > ProphetDirector.LaserFallbackDistance) {
                        Teleport(ctx, target.Center - target.velocity.SafeNormalize(-Vector2.UnitY) * ProphetDirector.LaserFallbackRadius);
                    }
                }
            }

            Vector2 focus = npc.Center + new Vector2(0, ProphetDirector.LaserFocusOffsetY);

            if (cd > ProphetDirector.LaserPullUntil) {
                npc.velocity *= ProphetDirector.LaserPullDrag;
                foreach (Player plr in Main.ActivePlayers) {
                    if (plr.Distance(npc.Center) >= ProphetDirector.LaserAffectRadius) {
                        continue;
                    }
                    if (plr.Distance(focus) <= ProphetDirector.LaserPullRadius) {
                        continue;
                    }
                    plr.Entropy().immune = ProphetDirector.LaserPullImmune;
                    plr.wingTime = plr.wingTimeMax;
                    plr.velocity = (focus - plr.Center).normalize() * ProphetDirector.LaserPullSpeed;
                    plr.Center += (focus - plr.Center).normalize() * ProphetDirector.LaserPullStep;
                    //拖人相位 8 颗 RuneParticle/玩家,Additive 40 帧,纯 VFX 不是攻击判定
                    for (int i = 0; i < ProphetDirector.LaserPullParticles; i++) {
                        PRTLoader.NewParticle<PRT_RuneParticle>(CEUtils.randomPoint(plr.getRect()), Vector2.Zero, Color.LightBlue, 1)
                            .Configure(1, true, PRTDrawModeEnum.AdditiveBlend, 0, 40);
                    }
                }
            }

            if (cd > ProphetDirector.LaserWingWindowLow && cd < ProphetDirector.LaserWingWindowHigh) {
                foreach (Player plr in Main.ActivePlayers) {
                    if (plr.Distance(npc.Center) < ProphetDirector.LaserAffectRadius) {
                        //原灾厄的无限飞行位已经删掉,这里每帧把翅膀时间回满,效果相同(player-api)
                        plr.wingTime = plr.wingTimeMax;
                    }
                }
            }

            if (cd == ProphetDirector.LaserFireBeat) {
                //地形检查读的是已同步的图格,各端同算;压缩倒计时是决策,只在权威端广播
                if (CEUtils.CheckSolidTile(npc.Center.getRectCentered(ProphetDirector.LaserBlockedCheckSize, ProphetDirector.LaserBlockedCheckSize))) {
                    ctx.Countdown = ProphetDirector.LaserAbortCountdown;
                    MarkNetUpdate(ctx);
                }
                else {
                    Shoot<FableEye>(ctx, focus, (target.Center - focus).normalize() * ProphetDirector.LaserEyeSpeed,
                        npc.damage / ProphetDirector.EyeDamageDivisor, 4);
                }
            }
        }
    }
}
