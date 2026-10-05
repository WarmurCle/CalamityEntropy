using CalamityEntropy.Content.NPCs.VoidDestroyer.Core;
using CalamityEntropy.Content.Projectiles.VoidDestroyer;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace CalamityEntropy.Content.NPCs.VoidDestroyer.States
{
    /// <summary>远角贯穿、近角越肩、平面角直飞,闪现期间计时暂停,自带传送</summary>
    [VaultState((int)VDStateIndex.TeleportFire, typeof(VDStateContext))]
    public class VDTeleportFireState : VDStateBase
    {
        public override string StateName => "TeleportFire";
        public override VDStateIndex StateIndex => VDStateIndex.TeleportFire;

        private int cornerStep;

        public override void OnEnter(VDStateContext ctx) {
            base.OnEnter(ctx);
            cornerStep = 0;
        }

        /// <summary>当前角的深度(收招拍停在最后一角的深度上,由 hub 的落定拍拉回)</summary>
        private float CurrentDepth(VDStateContext ctx) {
            int corners = VDDirector.TeleportFireCorners(ctx.Phase);
            return VDDirector.TeleportFireDepth(Math.Min(cornerStep, corners - 1));
        }

        public override IVDState OnUpdate(VDStateContext ctx) {
            Timer++;
            NPC npc = ctx.Npc;
            int corners = VDDirector.TeleportFireCorners(ctx.Phase);
            float depth = CurrentDepth(ctx);
            ctx.Depth = depth;
            if (cornerStep < corners) {
                if (Timer == 1 && IsServer) {
                    ctx.CornerIndex = VDDirector.TeleportFireOrder[cornerStep % VDDirector.TeleportFireOrder.Length];
                    //角位是表观位置:按本角深度换算成世界坐标,远角的世界位置在更远处、近角在更近处,画出来都在玩家 480px 外的角上
                    Vector2 anchor = DepthAnchor(ctx, VDVfx.CornerDirs[ctx.CornerIndex] * VDDirector.TeleportFireOffset, depth);
                    ctx.Owner.StartBlink(anchor);
                }
                //落地后 18 帧核心蓄力 + 汇聚流再出手:落地即预告
                float charge = MathHelper.Clamp(Timer / (float)VDDirector.TeleportFireShotFrame, 0f, 1f);
                ctx.CoreGlow = Math.Max(ctx.CoreGlow, charge);
                ctx.RimCharge = charge;
                if (Timer < VDDirector.TeleportFireShotFrame && Timer % 2 == 0) {
                    ConvergeSparks(ctx, VDVfx.VoidPurple, 50f, 110f, 0.14f);
                }
                if (Timer == VDDirector.TeleportFireShotFrame) {
                    Fire(ctx, depth);
                }
                if (Timer > VDDirector.TeleportFireShotFrame) {
                    ctx.CoreGlow = 0f;
                }
                if (Timer >= VDDirector.TeleportFireCornerFrames) {
                    cornerStep++;
                    ResetTimer();
                    MarkNetUpdate(ctx);
                }
                return null;
            }
            //收招:停在最后一角(深度由 hub 落定拍拉回平面)
            ctx.CoreGlow = 0f;
            if (Timer >= VDDirector.TeleportFireTail) {
                return EndAttack(ctx);
            }
            return null;
        }

        /// <summary>出手:平面角直飞扇;深度角朝垂直于角→玩家方向排开的 5 个落点射纵深弹(远角贯穿、近角越肩),40 帧到平面</summary>
        private void Fire(VDStateContext ctx, float depth) {
            Vector2 core = ctx.Owner.CorePos;
            int half = VDDirector.TeleportFireBolts / 2;
            if (Math.Abs(depth) < 0.01f) {
                Vector2 dir = (ctx.Target.Center - core).SafeNormalize(Vector2.UnitY);
                MuzzleCue(ctx, dir, 4f, "CruiserSpit", 0.85f);
                for (int i = -half; i <= half; i++) {
                    Shoot<VDVoidBolt>(ctx, core, dir.RotatedBy(MathHelper.ToRadians(VDDirector.TeleportFireSpreadDeg * i)) * VDDirector.TeleportFireBoltSpeed, VDDirector.DmgVoidBolt, VDVoidBolt.ModeStraight);
                }
                return;
            }
            Vector2 predicted = PredictTarget(ctx, VDDirector.TeleportFireZLead);
            Vector2 toTarget = (predicted - core).SafeNormalize(Vector2.UnitY);
            Vector2 perp = toTarget.RotatedBy(MathHelper.PiOver2);
            MuzzleCue(ctx, toTarget, 4f, "CruiserSpit", depth > 0f ? 0.7f : 1f, 1f);
            int mode = depth > 0f ? VDVoidBolt.ModeZPierce : VDVoidBolt.ModeZFromNear;
            for (int i = -half; i <= half; i++) {
                Vector2 landing = predicted + perp * (i * VDDirector.TeleportFireLandSpacing);
                (Vector2 vel, float zVel) = AimThroughPlane(core, depth, landing, VDDirector.TeleportFireZFrames);
                ShootDepth<VDVoidBolt>(ctx, core, vel, VDDirector.DmgVoidBolt, depth, zVel, 0f, mode);
            }
        }
    }
}
