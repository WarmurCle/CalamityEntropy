using CalamityEntropy.Content.NPCs.Prophet.Core;
using CalamityEntropy.Content.Projectiles.Prophet;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Prophet.States
{
    /// <summary>除瞬移清零外不写速度,整数层 0.8 倍、半整数层 0.5 倍</summary>
    [VaultState((int)ProphetStateIndex.RuneTorrentFan, typeof(ProphetStateContext))]
    public class ProphetRuneTorrentFanState : ProphetStateBase
    {
        public override ProphetStateIndex StateIndex => ProphetStateIndex.RuneTorrentFan;

        protected override void RunAttack(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            Player target = ctx.Target;
            float difficult = ctx.Difficult;
            int phase = ctx.Phase;

            npc.rotation = (target.Center - npc.Center).ToRotation();

            if (ctx.Countdown == ProphetDirector.TorrentFanBlinkBeat) {
                CrystalCue(npc);
                if (IsServer) {
                    Teleport(ctx, target.Center + target.velocity.SafeNormalize(CEUtils.randomRot().ToRotationVector2())
                        * ProphetDirector.TorrentFanBlinkRadius / difficult);
                }
            }

            if (ctx.Countdown == ProphetDirector.TorrentFanFireBeat) {
                int damage = ProjDamage(ctx);
                Vector2 aim = (target.Center - npc.Center).normalize();
                float spread = phase == 1 ? ProphetDirector.TorrentFanSpreadP1 : ProphetDirector.TorrentFanSpreadP2;
                int layers = phase == 1 ? ProphetDirector.TorrentFanLayersP1 : ProphetDirector.TorrentFanLayersP2;
                float halfLayers = phase == 1 ? ProphetDirector.TorrentFanHalfLayersP1 : ProphetDirector.TorrentFanHalfLayersP2;

                //外层:判定是 <=,所以含正中那一发共 layers + 1 层。正中那一发的 ai0 是 6,两侧是 5
                for (int i = 0; i <= layers; i++) {
                    if (i == 0) {
                        Shoot<RuneTorrent>(ctx, npc.Center, aim * difficult * ProphetDirector.TorrentFanSpeedOuter,
                            damage, 4, ProphetDirector.TorrentFanMaxSpeedCenter * difficult, ProphetDirector.TorrentFanAi1);
                    }
                    else {
                        Shoot<RuneTorrent>(ctx, npc.Center,
                            aim.RotatedBy(i * spread) * difficult * ProphetDirector.TorrentFanSpeedOuter,
                            damage, 4, ProphetDirector.TorrentFanMaxSpeedSide * difficult, ProphetDirector.TorrentFanAi1);
                        Shoot<RuneTorrent>(ctx, npc.Center,
                            aim.RotatedBy(i * -spread) * difficult * ProphetDirector.TorrentFanSpeedOuter,
                            damage, 4, ProphetDirector.TorrentFanMaxSpeedSide * difficult, ProphetDirector.TorrentFanAi1);
                    }
                }

                //内插层用半整数层号,飞得更慢,填在外层的缝里
                for (float i = 0.5f; i <= halfLayers; i++) {
                    Shoot<RuneTorrent>(ctx, npc.Center,
                        aim.RotatedBy(i * spread) * difficult * ProphetDirector.TorrentFanSpeedInner,
                        damage, 4, ProphetDirector.TorrentFanMaxSpeedSide * difficult, ProphetDirector.TorrentFanAi1);
                    Shoot<RuneTorrent>(ctx, npc.Center,
                        aim.RotatedBy(i * -spread) * difficult * ProphetDirector.TorrentFanSpeedInner,
                        damage, 4, ProphetDirector.TorrentFanMaxSpeedSide * difficult, ProphetDirector.TorrentFanAi1);
                }
            }
        }
    }
}
