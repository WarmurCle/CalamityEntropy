using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>高于 50 时 40 帧一循环,余数 20 放闪电,50 以下停在绕行窗</summary>
    [VaultState((int)ProphetStateIndex.RuneImpact, typeof(ProphetStateContext))]
    public class ProphetRuneImpactState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.RuneImpact;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;
            int phase = ctx.Phase;
            int cd = ctx.Countdown;

            bool orbit = true;
            //原代码写的是 if (余数 > 20) { 空块 } else { ... };这里直接取反,语义一致
            if (cd > ProphetDirector.ImpactActiveAbove) {
                if (cd % ProphetDirector.ImpactPeriod <= ProphetDirector.ImpactFireRemainder) {
                    orbit = false;
                    if (cd % ProphetDirector.ImpactPeriod == ProphetDirector.ImpactFireRemainder) {
                        int damage = ProjDamage(ctx);
                        Vector2 aim = (target.Center - npc.Center).normalize();
                        float spread = phase == 1 ? ProphetDirector.ImpactSpreadP1 : ProphetDirector.ImpactSpreadP2;
                        int layers = phase == 1 ? ProphetDirector.ImpactLayersP1 : ProphetDirector.ImpactLayersP2;
                        //判定是 <=,所以含正中那一发共 layers + 1 层
                        for (int i = 0; i <= layers; i++) {
                            if (i == 0) {
                                Shoot<ProphetLightning>(ctx, npc.Center, aim * ProphetDirector.ImpactBoltSpeed, damage, 4);
                            }
                            else {
                                Shoot<ProphetLightning>(ctx, npc.Center,
                                    aim.RotatedBy(i * spread) * ProphetDirector.ImpactBoltSpeed, damage, 4);
                                Shoot<ProphetLightning>(ctx, npc.Center,
                                    aim.RotatedBy(i * -spread) * ProphetDirector.ImpactBoltSpeed, damage, 4);
                            }
                        }
                    }
                    npc.velocity *= ProphetDirector.ImpactBrakeDrag;
                }
            }

            if (orbit) {
                npc.velocity = (target.Center + (npc.Center - target.Center).normalize() * ProphetDirector.ImpactOrbitRadius
                    - npc.Center) * ProphetDirector.ImpactOrbitLerp;
                npc.rotation = CEUtils.RotateTowardsAngle(npc.rotation, npc.velocity.ToRotation(),
                    ProphetDirector.ImpactOrbitRotate, false);
            }
        }
    }
}
