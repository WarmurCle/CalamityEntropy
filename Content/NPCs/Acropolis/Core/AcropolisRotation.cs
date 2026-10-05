using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Acropolis.Core
{
    /// <summary>
    /// 保留原随机权重,不改成表
    /// 第二个骰子写在 &amp;&amp; 左边,门槛不成立也消耗随机数
    /// 被鱼叉拽着时不许起跳,特招冷却 360,单发 160,出招期间照常流逝
    /// </summary>
    public static class AcropolisRotation
    {
        /// <summary>只权威端调,返回 null 是单发,留在行走态</summary>
        public static IVaultState<AcropolisStateContext> Pick(AcropolisStateContext ctx) {
            if (Main.rand.NextBool(AcropolisDirector.BarrageRollDenominator)) {
                return VaultStateRegistry<AcropolisStateContext>.Create((int)AcropolisStateIndex.CannonBarrage);
            }

            //第二个骰子必须先摇再看门槛,与原代码的短路顺序一致
            if (Main.rand.NextBool(AcropolisDirector.JumpRollDenominator) && !ctx.Airborne && ctx.HarpoonOnLauncher) {
                return VaultStateRegistry<AcropolisStateContext>.Create((int)AcropolisStateIndex.JumpShoot);
            }

            //单发:不换态,只推进开火计数。真正的开火与出膛由宿主的 ConsumeShotCue 做,
            //因为原代码的开火点在常态瞄准之后,宿主那一步才刚把枪口转过去
            ctx.TeslaCD = AcropolisDirector.TeslaCDAfterShot;
            ctx.ShotCue++;
            //决策点同步:骰子只在权威端摇,结果必须立刻过线
            ctx.Npc.netUpdate = true;
            return null;
        }
    }
}
