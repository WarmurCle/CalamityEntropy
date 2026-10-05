using CalamityEntropy.Content.NPCs.Cruiser.Core;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using InnoVault.StateMachines;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>
    /// 等咬中时 ChangeCounter 停在 0,原代码没有上限
    /// 撞墙把计数跳到 60,跳过甩出和刀光阵
    /// 玩家位置和速度各端都写,玩家自己的客户端那次才算数
    /// </summary>
    [VaultState((int)CruiserStateIndex.BiteAndDash, typeof(CruiserStateContext))]
    public class CruiserBiteAndDashState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.BiteAndDash;

        /// <summary>
        /// 原 == 20 有一次硬刹,漏掉客户端会带着 80 继续飞
        /// 改成闩锁加 &gt;= 20,撞墙必须先消费闩锁,否则会误放刀光阵
        /// </summary>
        private bool launchDone;

        public override void OnEnter(CruiserStateContext ctx) {
            base.OnEnter(ctx);
            launchDone = false;
        }

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            Player player = ctx.Target;
            IVaultState<CruiserStateContext> next = null;

            //这个 == 0 是窗口,不是一次性拍,收养跨过它是对的
            if (ctx.ChangeCounter == 0) {
                //清场:自家三种散弹一律抹掉,免得拖拽段被自己的弹幕糊满
                List<int> clearTypes = new List<int>
                {
                    ModContent.ProjectileType<VoidStar>(),
                    ModContent.ProjectileType<VoidResidue>(),
                    ModContent.ProjectileType<VoidSpike>()
                };
                foreach (Projectile p in Main.ActiveProjectiles) {
                    if (clearTypes.Contains(p.type)) {
                        p.Kill();
                    }
                }
                npc.velocity *= CruiserDirector.BiteApproachDrag;
                npc.velocity += (player.Center - npc.Center).normalize() * CruiserDirector.BiteApproachThrust;
                if (CEUtils.getDistance(npc.Center + npc.rotation.ToRotationVector2() * CruiserDirector.BiteGrabOffset,
                        player.Center) < CruiserDirector.BiteGrabRange) {
                    ctx.ChangeCounter++;
                    player.velocity *= 0;
                    player.Center = npc.Center + npc.rotation.ToRotationVector2() * CruiserDirector.BiteGrabOffset;
                    MarkNetUpdate(ctx);
                }
            }
            else {
                ctx.ChangeCounter++;
                if (ctx.ChangeCounter < CruiserDirector.BiteDragFrames) {
                    ctx.MouthRot += CruiserDirector.BiteMouthRate;
                    npc.velocity = npc.velocity.normalize()
                        * (npc.velocity.Length()
                            + (CruiserDirector.BiteDragSpeedTarget - npc.velocity.Length()) * CruiserDirector.BiteDragSpeedLerp);

                    if (CEUtils.getDistance(npc.Center + npc.rotation.ToRotationVector2() * CruiserDirector.BiteGrabOffset,
                            player.Center) < CruiserDirector.BiteGrabRange) {
                        player.velocity *= 0;
                        player.Entropy().immune = CruiserDirector.BiteImmuneFrames;
                        player.Center = npc.Center + npc.rotation.ToRotationVector2() * CruiserDirector.BiteGrabOffset;
                    }
                    if (!CEUtils.isAir(npc.Center + npc.rotation.ToRotationVector2() * CruiserDirector.BiteWallProbe)) {
                        ctx.ChangeCounter = CruiserDirector.BiteWallSkipTo;
                        //撞墙不甩出:预先把甩出拍标记为已消费,复现原代码「跳到 60 后 == 20 不再成立」
                        launchDone = true;
                    }
                }
                else {
                    Vector2 targetPos = player.Center
                        + (npc.Center - player.Center).normalize().RotatedBy(CruiserDirector.BiteOrbitAngle) * CruiserDirector.BiteOrbitRadius;
                    npc.velocity += (targetPos - npc.Center).normalize() * CruiserDirector.BiteOrbitThrust;
                    npc.velocity *= CruiserDirector.BiteOrbitDrag;
                    if (ctx.ChangeCounter > CruiserDirector.BiteDuration) {
                        next = NextAttack(ctx);
                    }
                }
                //中途加入已经越过甩出拍很久:静默记账,不补演出(基类 CuePassed 的标准用法)
                if (!launchDone && CuePassed(ctx.ChangeCounter, CruiserDirector.BiteDragFrames)) {
                    launchDone = true;
                }
                //原代码把这一段写在 if/else 之外,所以计数正好等于 20 那一帧,绕飞与甩出会同帧执行
                if (!launchDone && ctx.ChangeCounter >= CruiserDirector.BiteDragFrames) {
                    launchDone = true;
                    if (CEUtils.getDistance(npc.Center + npc.rotation.ToRotationVector2() * CruiserDirector.BiteLaunchProbe,
                            player.Center) < CruiserDirector.BiteGrabRange) {
                        player.velocity = npc.velocity * CruiserDirector.BiteLaunchSpeedMult;
                        player.Entropy().CruiserAntiGravTime = CruiserDirector.BiteAntiGravFrames;
                    }

                    if (IsServer) {
                        int slashType = ModContent.ProjectileType<CruiserSlash>();
                        for (int i = 1; i < CruiserDirector.BiteSlashRows; i++) {
                            for (int j = -(CruiserDirector.BiteSlashColumns - 1); j < CruiserDirector.BiteSlashColumns; j++) {
                                if (j == 0) {
                                    Shoot(ctx, slashType,
                                        npc.Center + npc.velocity.normalize() * CruiserDirector.BiteSlashSpacing * i,
                                        npc.velocity);
                                }
                                else {
                                    Shoot(ctx, slashType,
                                        npc.Center + npc.velocity.normalize().RotatedBy(CruiserDirector.BiteSlashAngleStep * j)
                                            * CruiserDirector.BiteSlashSpacing * i,
                                        npc.velocity.RotatedBy(CruiserDirector.BiteSlashAngleStep * j));
                                }
                            }
                        }
                        MarkNetUpdate(ctx);
                    }
                    npc.velocity *= CruiserDirector.BiteSlashSelfDrag;
                }
            }
            return next;
        }
    }
}
