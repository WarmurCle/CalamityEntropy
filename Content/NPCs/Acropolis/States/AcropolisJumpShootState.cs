using CalamityEntropy.Content.NPCs.Acropolis.Core;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Acropolis.States
{
    /// <summary>原 JumpAndShoot,落地后还多打一发</summary>
    [VaultState((int)AcropolisStateIndex.JumpShoot, typeof(AcropolisStateContext))]
    public class AcropolisJumpShootState : AcropolisStateBase
    {
        public override AcropolisStateIndex StateIndex => AcropolisStateIndex.JumpShoot;

        public override void OnEnter(AcropolisStateContext ctx) {
            base.OnEnter(ctx);
            NPC npc = ctx.Npc;
            ctx.TeslaCD = AcropolisDirector.TeslaCDAfterSpecial;
            ctx.Airborne = true;
            ctx.Owner.JumpCD = AcropolisDirector.JumpShootJumpCD;
            ctx.JumpAndShoot = AcropolisDirector.JumpShootFrames;
            ctx.TeslaUpCD = AcropolisDirector.JumpShootTeslaUpInit;
            if (ctx.Target != null) {
                //起跳冲量各端都写:横向的 /scale*scale 在原式里互相抵消,这里保持原写法不化简
                npc.velocity = new Vector2(
                    AcropolisDirector.JumpLaunchSpeedX * Math.Sign(ctx.Target.Center.X - npc.Center.X) / npc.scale,
                    AcropolisDirector.JumpLaunchSpeedY) * npc.scale;
            }
        }

        public override IVaultState<AcropolisStateContext> OnUpdate(AcropolisStateContext ctx) {
            NPC npc = ctx.Npc;
            AcropolisArm cannon = ctx.Cannon;

            //对齐原代码 `JumpAndShoot-- > 0`:自减无条件发生,取自减前的值判分支
            int before = ctx.JumpAndShoot;
            ctx.JumpAndShoot = before - 1;
            if (before > 0) {
                //原代码对这一发连调两次 PointAPos(转向速率翻倍),且转完立刻开火;瞄点沿用挂点设计偏移(原式不乘 dir)
                Vector2 mount = cannon?.MountOffset ?? new Vector2(AcropolisDirector.CannonMountX, AcropolisDirector.CannonMountY);
                ctx.AimCannon(npc.Center + mount * npc.scale + new Vector2(0f, AcropolisDirector.JumpShootAimDrop),
                    AcropolisDirector.JumpShootAimTimes);
                ctx.TeslaUpCD -= ctx.Enrange;
                if (ctx.TeslaUpCD <= 0f) {
                    ctx.TeslaUpCD = AcropolisDirector.JumpShootInterval;
                    //跳射没有后坐,原样保留
                    ctx.QueueCannonShot(AcropolisDirector.JumpShootSpread, AcropolisDirector.JumpShootSpeed,
                        AcropolisDirector.JumpShootProjAi0, 0f);
                }
            }

            //落地由宿主的落地判定清掉腾空标记,本状态下一帧才收招——与原代码的执行顺序一致
            if (!ctx.Airborne) {
                return BackToWalk(ctx);
            }
            return null;
        }
    }
}
