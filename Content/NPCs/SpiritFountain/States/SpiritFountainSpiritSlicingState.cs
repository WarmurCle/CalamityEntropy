using CalamityEntropy.Content.NPCs.SpiritFountain.Core;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.States
{
    /// <summary>
    /// 终局循环,跑满后在开头 40 帧窗口把计时原地归零,不走换态
    /// 换态会写出相同的 ai[3],客户端察觉不到
    /// 端点由权威端骰一次,随 ExtraAI 过线
    /// </summary>
    [VaultState((int)SpiritFountainStateIndex.SpiritSlicing, typeof(SpiritFountainStateContext))]
    public class SpiritFountainSpiritSlicingState : SpiritFountainStateBase
    {
        /// <summary>终局循环态,不设超时:它本来就该一直跑下去</summary>
        public override int TimeoutFrames => int.MaxValue;

        public override SpiritFountainStateIndex StateIndex => SpiritFountainStateIndex.SpiritSlicing;

        protected override IVaultState<SpiritFountainStateContext> RunBody(SpiritFountainStateContext ctx) {
            NPC npc = ctx.Npc;
            SpiritFountain owner = ctx.Owner;

            npc.dontTakeDamage = false;
            ctx.EyeAlphaTarget = 1;
            int t = SpiritFountainDirector.SlicingPeriod;

            //原 == 1,计时带 ±2 收养,跨过第 1 帧会拿着上一招朝向画满 801 帧
            //写的是常量,每帧重写与只写一次等价,改成区间
            if (Timer >= SpiritFountainDirector.SlicingInitFrame) {
                owner.column1.rotation = -MathHelper.PiOver2;
                owner.column2.rotation = 0;
            }
            //骰点钉死在第 1 帧,权威端 Timer 不被收养,严格 +1,不会被跳过也不会重放
            if (Timer == SpiritFountainDirector.SlicingInitFrame && IsServer) {
                //两柱各自的起始方向:只在权威端骰,结果随 ExtraAI 的 Num 过线
                owner.column1.Num = SpiritFountainDirector.SlicingReach * (Main.rand.NextBool() ? 1 : -1);
                owner.column2.Num = SpiritFountainDirector.SlicingReach * (Main.rand.NextBool() ? 1 : -1);
                MarkNetUpdate(ctx);
            }
            owner.column1.offset.X = float.Lerp(owner.column1.offset.X, owner.column1.Num, SpiritFountainDirector.SlicingOffsetLerp);
            owner.column2.offset.Y = float.Lerp(owner.column2.offset.Y, owner.column2.Num, SpiritFountainDirector.SlicingOffsetLerp);
            if (Timer % t == SpiritFountainDirector.SlicingFlipPhase) {
                owner.column1.Num *= -1;
                owner.column2.Num *= -1;
                //翻向是决策点。权威端这一拍是精确的;客户端万一因收养跨过或重放了它,
                //靠这一包把 Num 直接对回来(Num 在 ExtraAI 里是无容差直取)
                MarkNetUpdate(ctx);
            }

            if (Timer > SpiritFountainDirector.SlicingLoopAfter && Timer % t < SpiritFountainDirector.SlicingLoopWindow) {
                //原代码切到自己再把 aiTimer 归零,状态没变
                //窗口宽 40 帧,±2 跨不过去,发包让客户端跟着重开
                Timer = 0;
                MarkNetUpdate(ctx);
            }
            return null;
        }
    }
}
