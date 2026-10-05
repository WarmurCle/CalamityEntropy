using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>二阶段闪现周期 46、开火周期 50,故意不同周期</summary>
    [VaultState((int)ProphetStateIndex.RuneVolley, typeof(ProphetStateContext))]
    public class ProphetRuneVolleyState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.RuneVolley;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;
            float difficult = ctx.Difficult;
            int phase = ctx.Phase;

            //运动:各端都跑
            npc.velocity *= ProphetDirector.VolleyDrag;
            npc.velocity += (npc.Center - target.Center).normalize().RotatedBy(MathHelper.PiOver2)
                * (npc.Center.X < target.Center.X ? ProphetDirector.VolleyOrbitAccel : -ProphetDirector.VolleyOrbitAccel);
            npc.rotation = CEUtils.RotateTowardsAngle(npc.rotation, (target.Center - npc.Center).ToRotation(),
                ProphetDirector.VolleyRotateRate, false);

            int blinkPeriod = phase == 1 ? ProphetDirector.VolleyBlinkPeriodP1 : ProphetDirector.VolleyBlinkPeriodP2;
            //掷骰只在权威端:落点随包过线
            if (ctx.Countdown >= ProphetDirector.VolleyActiveUntil && ctx.Countdown % blinkPeriod == 0 && IsServer) {
                Teleport(ctx, target.Center + target.velocity.SafeNormalize(CEUtils.randomRot().ToRotationVector2())
                    * ProphetDirector.VolleyBlinkRadius / difficult);
            }

            int firePeriod = phase == 1 ? ProphetDirector.VolleyFirePeriodP1 : ProphetDirector.VolleyFirePeriodP2;
            int firePhase = phase == 1 ? ProphetDirector.VolleyFirePhaseP1 : ProphetDirector.VolleyFirePhaseP2;
            if (ctx.Countdown >= ProphetDirector.VolleyActiveUntil && ctx.Countdown % firePeriod == firePhase) {
                CrystalCue(npc);

                int damage = ProjDamage(ctx);
                Vector2 aim = (target.Center - npc.Center).normalize();
                float spread = phase == 1 ? ProphetDirector.VolleySpreadP1 : ProphetDirector.VolleySpreadP2;
                int layers = phase == 1 ? ProphetDirector.VolleyLayersP1 : ProphetDirector.VolleyLayersP2;
                //判定是 <=,所以含正中那一发共 layers + 1 层
                for (int i = 0; i <= layers; i++) {
                    if (i == 0) {
                        Shoot<RuneTorrent>(ctx, npc.Center, aim * difficult, damage, 4,
                            ProphetDirector.VolleyTorrentMaxSpeed * difficult, ProphetDirector.VolleyTorrentAi1);
                    }
                    else {
                        Shoot<RuneTorrent>(ctx, npc.Center, aim.RotatedBy(i * spread) * difficult, damage, 4,
                            ProphetDirector.VolleyTorrentMaxSpeed * difficult, ProphetDirector.VolleyTorrentAi1);
                        Shoot<RuneTorrent>(ctx, npc.Center, aim.RotatedBy(i * spread * -1) * difficult, damage, 4,
                            ProphetDirector.VolleyTorrentMaxSpeed * difficult, ProphetDirector.VolleyTorrentAi1);
                    }
                }

                //后坐是运动,各端都写;弹幕生成已在 Shoot 里守卫
                npc.velocity += (npc.Center - target.Center).normalize() * ProphetDirector.VolleyRecoil;
                MarkNetUpdate(ctx);
            }
        }
    }
}
