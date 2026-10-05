using CalamityEntropy.Content.NPCs.Cruiser.Core;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles.Cruiser;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>
    /// 瞄准窗用 LaserAim 计时,扫射用 ChangeCounter 计时,瞄准期间不推进 ChangeCounter
    /// LaserAim++ &lt; 35 与自增后 &gt; 36,第 36 帧是空帧
    /// 宿主在本状态不把 rotation 覆写成速度朝向
    /// </summary>
    [VaultState((int)CruiserStateIndex.VoidLaser, typeof(CruiserStateContext))]
    public class CruiserVoidLaserState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.VoidLaser;

        /// <summary>
        /// 不能写 ChangeCounter % 46 == K,±2 收养会跨过直写速度的拍
        /// 改成轮次单调加拍内闩锁,权威端每帧 +1 时与原等值逐帧等价
        /// </summary>
        private int cycleStarted = -1;
        private bool firedThisCycle;
        private bool brakedThisCycle;

        public override void OnEnter(CruiserStateContext ctx) {
            base.OnEnter(ctx);
            cycleStarted = -1;
            firedThisCycle = false;
            brakedThisCycle = false;
        }

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            Player player = ctx.Target;

            //自增后的值参与第二个判据,所以这里必须先自增再比较(原 localAI[2]++ 的写法)
            int aimBefore = ctx.LaserAim;
            ctx.LaserAim++;
            if (aimBefore < CruiserDirector.LaserAimFrames) {
                npc.rotation = CEUtils.RotateTowardsAngle(npc.rotation,
                    (player.Center + player.velocity * CruiserDirector.LaserAimLeadFrames * CruiserDirector.LaserLeadFactor - npc.Center).ToRotation(),
                    CruiserDirector.LaserAimRotateRate, false);
                npc.velocity *= CruiserDirector.LaserAimDrag;
                npc.velocity += npc.rotation.ToRotationVector2() * CruiserDirector.LaserAimBackThrust;
            }
            if (ctx.LaserAim > CruiserDirector.LaserActiveFrom) {
                //轮次单调推出,不用等值判定;beat 是轮内帧号
                int cycle = ctx.ChangeCounter / CruiserDirector.LaserCycle;
                int beat = ctx.ChangeCounter % CruiserDirector.LaserCycle;
                int u = (int)Utils.Remap(
                    CruiserDirector.LaserCycle * cycle,
                    0, CruiserDirector.LaserCycles * CruiserDirector.LaserCycle,
                    CruiserDirector.LaserLeadHigh, CruiserDirector.LaserLeadLow);

                //原 cc % 46 == 0:重锁朝向 + 速度归一化到 1(近乎停住,预告好读)
                if (cycle > cycleStarted) {
                    cycleStarted = cycle;
                    firedThisCycle = false;
                    brakedThisCycle = false;
                    //中途加入并且已经越过本轮起拍很久时,静默跳过这一拍的演出,只认闩锁
                    if (!CuePassed(beat, 0)) {
                        if (ctx.ChangeCounter > 1) {
                            npc.rotation = (player.Center + player.velocity * u * CruiserDirector.LaserLeadFactor - npc.Center).ToRotation();
                        }
                        npc.velocity = npc.rotation.ToRotationVector2();
                        MarkNetUpdate(ctx);
                        if (!Main.dedServ) {
                            //每轮双层预告粒子,lifetime 传 -1 靠手动删,跟 46 tick 的激光帧对齐
                            PRTLoader.NewParticle<PRT_CruiserWarn>(npc.Center, Vector2.Zero, Color.White, CruiserDirector.LaserWarnScaleBig)
                                .Configure(1, true, PRTDrawModeEnum.AdditiveBlend, npc.rotation, -1);
                            PRTLoader.NewParticle<PRT_CruiserWarn>(npc.Center, Vector2.Zero, Color.White, CruiserDirector.LaserWarnScaleSmall)
                                .Configure(1, true, PRTDrawModeEnum.AdditiveBlend, npc.rotation, -1);
                        }
                    }
                }
                //原 cc % 46 == u:开火并顺着光束冲出去
                if (!firedThisCycle && beat >= u) {
                    firedThisCycle = true;
                    if (!CuePassed(beat, u)) {
                        Shoot(ctx, ModContent.ProjectileType<CruiserLaser2>(), npc.Center,
                            npc.rotation.ToRotationVector2() * CruiserDirector.LaserBeamSpeed, ai0: npc.whoAmI);
                        //冲刺是运动,各端都跑;上面的弹幕生成只在权威端
                        npc.velocity = npc.rotation.ToRotationVector2()
                            * ((CEUtils.getDistance(npc.Center, player.Center) + CruiserDirector.LaserDashDistanceBonus)
                                / (CruiserDirector.LaserDashDivisorBase - u));
                    }
                }
                //原 cc % 46 == 45:刹回速度 4。beat 上限就是 45,所以这里只需要闩锁
                if (!brakedThisCycle && beat >= CruiserDirector.LaserCycleBrakeFrame) {
                    brakedThisCycle = true;
                    npc.velocity = npc.velocity.normalize() * CruiserDirector.LaserCycleBrakeSpeed;
                }
                ctx.ChangeCounter++;
                if (ctx.ChangeCounter >= CruiserDirector.LaserCycles * CruiserDirector.LaserCycle) {
                    ResetAim(ctx);
                    return NextAttack(ctx);
                }
            }
            return null;
        }

        /// <summary>超时兜底也要把瞄准窗计时清掉,否则下一次激光会跳过瞄准窗直接开火</summary>
        protected override IVaultState<CruiserStateContext> OnTimeout(CruiserStateContext ctx) {
            ResetAim(ctx);
            return base.OnTimeout(ctx);
        }

        /// <summary>只权威端清,客户端先归零会在等包时重新进瞄准窗</summary>
        private static void ResetAim(CruiserStateContext ctx) {
            if (IsServer) {
                ctx.LaserAim = 0;
            }
        }
    }
}
