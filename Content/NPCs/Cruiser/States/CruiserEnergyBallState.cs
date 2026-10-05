using CalamityEntropy.Content.NPCs.Cruiser.Core;
using CalamityEntropy.Content.Projectiles.Cruiser;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>收招判定写在速度写入之前,收招那帧的推进照样执行</summary>
    [VaultState((int)CruiserStateIndex.EnergyBall, typeof(CruiserStateContext))]
    public class CruiserEnergyBallState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.EnergyBall;

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            Player player = ctx.Target;

            //等值判定安全:拍体只有权威端动作(生成能量球 + netUpdate),
            //而 ChangeCounter 的容差收养只发生在客户端,权威端每帧稳定 +1、必然命中 0
            if (ctx.ChangeCounter == 0) {
                Shoot(ctx, ModContent.ProjectileType<CruiserEnergyBall>(), npc.Center, Vector2.Zero,
                    CruiserDirector.EnergyBallDamageMult, npc.whoAmI);
                MarkNetUpdate(ctx);
            }
            ctx.ChangeCounter++;

            IVaultState<CruiserStateContext> next = null;
            if (ctx.ChangeCounter > CruiserDirector.EnergyBallDuration) {
                next = NextAttack(ctx);
            }
            npc.velocity += (player.Center - npc.Center).normalize()
                * (npc.Distance(player.Center) > CruiserDirector.EnergyBallFarDistance
                    ? CruiserDirector.EnergyBallThrustFar : CruiserDirector.EnergyBallThrustNear);
            npc.velocity *= CruiserDirector.EnergyBallDrag;
            return next;
        }
    }
}
