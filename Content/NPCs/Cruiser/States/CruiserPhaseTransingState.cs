using CalamityEntropy.Content.NPCs.Cruiser.Core;
using CalamityEntropy.Content.Particles;
using InnoVault.PRT;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>转阶段不用自己的计时,也不自己收招,到 122 直接写入 VoidSpike,不清 changeCounter,OnUpdate 永远返回 null</summary>
    [VaultState((int)CruiserStateIndex.PhaseTransing, typeof(CruiserStateContext))]
    public class CruiserPhaseTransingState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.PhaseTransing;

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            if (npc.velocity.Length() < CruiserDirector.PhaseTransSpeedFloor) {
                npc.velocity *= CruiserDirector.PhaseTransAccel;
            }
            else {
                npc.velocity *= CruiserDirector.PhaseTransDrag;
            }

            if (!Main.dedServ && ctx.Owner != null) {
                //每骨节每帧一颗,节数多时能堆几百颗,对齐原转场密度(骨节坐标读 Rigs2D 链骨)
                int count = ctx.Owner.ChainPointCount;
                for (int i = 0; i < count; i++) {
                    Vector2 p = ctx.Owner.ChainPoint(i);
                    PRT_Void vpt = PRTLoader.NewParticle<PRT_Void>(p,
                        CEUtils.randomPointInCircle(CruiserDirector.PhaseTransParticleScatter), Color.White, 1f);
                    vpt.Opacity = Main.rand.NextFloat(CruiserDirector.PhaseTransParticleOpacityMin, CruiserDirector.PhaseTransParticleOpacityMax);
                    vpt.shape = 4;
                }
            }
            return null;
        }
    }
}
