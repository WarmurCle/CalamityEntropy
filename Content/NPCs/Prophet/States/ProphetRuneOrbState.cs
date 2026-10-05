using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Particles.CalamityPorts;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>三个布阵拍写的都是 phase 等于 1 时为 60,否则为 40,后两拍只有二阶段,后两圈始终是九个,原样保留</summary>
    [VaultState((int)ProphetStateIndex.RuneOrb, typeof(ProphetStateContext))]
    public class ProphetRuneOrbState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.RuneOrb;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;
            int phase = ctx.Phase;
            int cd = ctx.Countdown;

            if (cd == ProphetDirector.OrbBlinkBeat && IsServer) {
                Teleport(ctx, target.Center + CEUtils.randomRot().ToRotationVector2() * ProphetDirector.OrbBlinkRadius);
            }

            if (cd == ProphetDirector.OrbRingBeatA) {
                SpawnRing(ctx);
            }
            if (cd == ProphetDirector.OrbRingBeatB && phase > 1) {
                SpawnRing(ctx);
            }
            if (cd == ProphetDirector.OrbRingBeatC && phase > 1) {
                SpawnRing(ctx);
            }

            npc.rotation = npc.velocity.ToRotation();
            npc.velocity += (target.Center - npc.Center).normalize() * ProphetDirector.OrbThrust;
            npc.velocity *= ProphetDirector.OrbDrag;
        }

        /// <summary>一圈锚点:每个锚点两枚 SparkleCal(Calamity CometShard 原配)加一枚静止符文</summary>
        private static void SpawnRing(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            CrystalCue(npc);

            int damage = ProjDamage(ctx);
            float step = ctx.Phase == 1 ? ProphetDirector.OrbAngleStepP1 : ProphetDirector.OrbAngleStepP2;
            for (float i = 0; i < 360; i += step) {
                float rot = MathHelper.ToRadians(i);
                float impactParticleScale = ProphetDirector.OrbSparkleScale;
                Vector2 anchor = npc.Center + rot.ToRotationVector2() * ProphetDirector.OrbRingRadius;
                PRTLoader.NewParticle<PRT_SparkleCal>(anchor, Vector2.Zero, Color.White, impactParticleScale * 1.2f)
                    .Configure(Color.SkyBlue, 12, 0, 4.5f);
                PRTLoader.NewParticle<PRT_SparkleCal>(anchor, Vector2.Zero, Color.SkyBlue, impactParticleScale)
                    .Configure(Color.SkyBlue, 10, 0, 3f);

                //贴图变体是掷骰,写在权威端守卫之内(原代码同样在 netMode 守卫里掷)
                if (IsServer) {
                    Shoot<ProphetRune>(ctx, anchor, Vector2.Zero, damage, 4, npc.whoAmI, rot,
                        Main.rand.Next(ProphetDirector.OrbRuneVariantMin, ProphetDirector.OrbRuneVariantMax));
                }
            }
        }
    }
}
