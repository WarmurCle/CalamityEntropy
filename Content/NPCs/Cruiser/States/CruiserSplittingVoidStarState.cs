using CalamityEntropy.Content.NPCs.Cruiser.Core;
using CalamityEntropy.Content.Projectiles.Cruiser;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>这一招和残渣同型,蓄力到 100,第 100 帧喷出虚空星,收招提前到 140</summary>
    [VaultState((int)CruiserStateIndex.SplittingVoidStar, typeof(CruiserStateContext))]
    public class CruiserSplittingVoidStarState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.SplittingVoidStar;

        /// <summary>原 == 20 和 == 100 是各端本地音,±2 收养会跨过,改成闩锁</summary>
        private bool windupCued;
        private bool burstCued;

        public override void OnEnter(CruiserStateContext ctx) {
            base.OnEnter(ctx);
            windupCued = false;
            burstCued = false;
        }

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            Player player = ctx.Target;
            Vector2 dir = (player.Center - npc.Center).normalize();

            //自增前:嘴部开合与蓄力音
            if (ctx.ChangeCounter < CruiserDirector.SplitMouthOpenUntil) {
                ctx.MouthRot += CruiserDirector.SplitMouthOpenRate;
            }
            else if (ctx.ChangeCounter < CruiserDirector.SplitMouthCloseUntil) {
                ctx.MouthRot += CruiserDirector.SplitMouthCloseRate;
            }
            if (!windupCued && CuePassed(ctx.ChangeCounter, CruiserDirector.SplitSoundFrame)) {
                windupCued = true;
            }
            if (!windupCued && ctx.ChangeCounter >= CruiserDirector.SplitSoundFrame) {
                windupCued = true;
                CEUtils.PlaySound("voidSound", CruiserDirector.SplitSoundPitch, npc.Center);
            }

            ctx.ChangeCounter++;

            if (ctx.ChangeCounter < CruiserDirector.SplitBurstFrame
                && npc.Distance(player.Center) > CruiserDirector.SplitApproachDistance) {
                npc.velocity *= CruiserDirector.SplitFarDrag;
                npc.velocity += dir * CruiserDirector.SplitFarThrust;
            }
            else {
                npc.velocity *= CruiserDirector.SplitNearDrag;
                npc.velocity += dir * CruiserDirector.SplitNearThrust;
            }
            if (!burstCued && CuePassed(ctx.ChangeCounter, CruiserDirector.SplitBurstFrame)) {
                burstCued = true;
            }
            if (!burstCued && ctx.ChangeCounter >= CruiserDirector.SplitBurstFrame) {
                burstCued = true;
                if (IsServer) {
                    for (int i = 0; i < CruiserDirector.SplitBurstCount; i++) {
                        Shoot(ctx, ModContent.ProjectileType<VoidStar>(), npc.Center,
                            npc.velocity.normalize().RotatedByRandom(CruiserDirector.SplitBurstSpread)
                                * CruiserDirector.SplitBurstSpeed
                                * Main.rand.NextFloat(CruiserDirector.SplitBurstSpeedMin, CruiserDirector.SplitBurstSpeedMax),
                            CruiserDirector.SplitDamageMult);
                    }
                    MarkNetUpdate(ctx);
                }
                CEUtils.PlaySound("CruiserSpit", 1.2f, npc.Center);
                CEUtils.PlaySound("VoidBomb", 1.1f, npc.Center);
                CEUtils.PlaySound("VoidBomb", 1.1f, npc.Center);
                CEUtils.PlaySound("VoidBomb", 1.1f, npc.Center);
                CEUtils.PlaySound("vbuse", 1, npc.Center);
            }
            if (ctx.ChangeCounter > CruiserDirector.SplitDuration) {
                return NextAttack(ctx);
            }
            return null;
        }
    }
}
