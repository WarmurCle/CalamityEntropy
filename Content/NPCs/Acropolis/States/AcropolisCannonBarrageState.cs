using CalamityEntropy.Content.NPCs.Acropolis.Core;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Acropolis.States
{
    /// <summary>原 CannonUpAtk,瞄点和开火是声明,弹幕和散布只在权威端</summary>
    [VaultState((int)AcropolisStateIndex.CannonBarrage, typeof(AcropolisStateContext))]
    public class AcropolisCannonBarrageState : AcropolisStateBase
    {
        public override AcropolisStateIndex StateIndex => AcropolisStateIndex.CannonBarrage;

        public override void OnEnter(AcropolisStateContext ctx) {
            base.OnEnter(ctx);
            //原代码在骰点那一支里写的冷却,放到进态执行,客户端换态时也会写上同一个值
            ctx.TeslaCD = AcropolisDirector.TeslaCDAfterSpecial;
        }

        public override IVaultState<AcropolisStateContext> OnUpdate(AcropolisStateContext ctx) {
            //先判收招:原代码是 CannonUpAtk-- > 0,归零那一帧炮口已经回到常态瞄准
            if (Timer >= AcropolisDirector.BarrageFrames) {
                return BackToWalk(ctx);
            }

            //瞄点声明,宿主 Step 时落地;开火排队,Step 之后从转过去的枪口出膛(原代码同一帧内先转后打)
            ctx.AimCannon(ctx.Player.Center + new Vector2(0f, AcropolisDirector.BarrageAimRise));

            if (Timer >= AcropolisDirector.BarrageFireStartFrame) {
                ctx.TeslaUpCD -= ctx.Enrange;
                if (ctx.TeslaUpCD <= 0f) {
                    ctx.TeslaUpCD = AcropolisDirector.BarrageInterval;
                    //反冲与音效各端都补:节拍量已过线,所以各端的开火帧一致
                    ctx.QueueCannonShot(AcropolisDirector.BarrageSpread, AcropolisDirector.BarrageSpeed,
                        AcropolisDirector.BarrageProjAi0, AcropolisDirector.BarrageRecoil);
                }
            }
            return null;
        }
    }
}
