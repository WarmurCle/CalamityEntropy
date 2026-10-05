using CalamityEntropy.Content.NPCs.VoidDestroyer.Core;
using CalamityEntropy.Content.Projectiles.VoidDestroyer;
using InnoVault.StateMachines;
using System;
using Terraria;

namespace CalamityEntropy.Content.NPCs.VoidDestroyer.States
{
    /// <summary>偶数平面横冲,奇数从 Z 1.5 穿平面,毒刺在穿平面那帧,节拍由陆龟弹幕自管</summary>
    [VaultState((int)VDStateIndex.GreenJungle, typeof(VDStateContext))]
    public class VDGreenJungleState : VDStateBase
    {
        public override string StateName => "GreenJungle";
        public override VDStateIndex StateIndex => VDStateIndex.GreenJungle;
        public override Vector2 AnchorFor(VDStateContext ctx) => ctx.Target.Center + VDDirector.JungleHoverOffset;

        public override IVDState OnUpdate(VDStateContext ctx) {
            Timer++;
            ctx.CoreColorTarget = VDVfx.JungleGreen;
            DeclareHoldRelative(ctx, VDDirector.JungleHoverOffset, 0.1f, 0.3f, 36f);
            int dashes = VDDirector.JungleDashes(ctx.Phase);
            int duration = VDDirector.JungleSpawnFrame + VDDirector.JungleTail;
            for (int k = 0; k < dashes; k++) {
                duration += VDDirector.TortoiseCycleFrames(k);
            }

            ctx.CoreGlow = Math.Max(ctx.CoreGlow, MathHelper.Clamp(Timer / (float)VDDirector.JungleSpawnFrame, 0f, 1f));
            if (Timer < VDDirector.JungleSpawnFrame && Timer % 3 == 0) {
                ConvergeSparks(ctx, VDVfx.JungleGreen, 80f, 170f, 0.1f);
            }
            if (Timer == VDDirector.JungleSpawnFrame) {
                ctx.CoreGlow = 1f;
                ctx.WingPulse = 1f;
                ctx.RimFlash = 1f;
                VDVfx.Sound("VoidAnticipation", 0.8f, ctx.Npc.Center, 3, 0.9f);
                if (IsServer) {
                    if (Main.getGoodWorld) {
                        Shoot<VDHoloTortoise>(ctx, ctx.Npc.Center, Vector2.Zero, VDDirector.DmgHoloBeast, ctx.Npc.whoAmI, ctx.Npc.target, -1);
                        Shoot<VDHoloTortoise>(ctx, ctx.Npc.Center, Vector2.Zero, VDDirector.DmgHoloBeast, ctx.Npc.whoAmI, ctx.Npc.target, 1);
                    }
                    else {
                        Shoot<VDHoloTortoise>(ctx, ctx.Npc.Center, Vector2.Zero, VDDirector.DmgHoloBeast, ctx.Npc.whoAmI, ctx.Npc.target, 0);
                    }
                }
            }
            if (Timer >= duration) {
                return EndAttack(ctx);
            }
            return null;
        }
    }
}
