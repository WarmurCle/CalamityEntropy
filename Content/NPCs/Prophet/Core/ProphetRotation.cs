using InnoVault.StateMachines;

namespace CalamityEntropy.Content.NPCs.Prophet.Core
{
    /// <summary>
    /// 轮换允许连续重复同一招,八个槽按固定顺序走,其中三个槽当场掷硬币
    /// 只权威端调,客户端再调一遍,序号和倒计时会分叉
    /// </summary>
    public static class ProphetRotation
    {
        /// <summary>由注册表创建状态实例(未注册返回 null,框架已打日志)</summary>
        public static IVaultState<ProphetStateContext> Create(ProphetStateIndex state)
            => VaultStateRegistry<ProphetStateContext>.Create((int)state);

        /// <summary>原末尾 netUpdate 现在由写 ai[3] 的 AiSlotNetSync 完成</summary>
        public static IVaultState<ProphetStateContext> Pick(ProphetStateContext ctx) {
            ctx.AttackIndex++;
            if (ctx.AttackIndex > ProphetDirector.AttackIndexMax) {
                ctx.AttackIndex = 0;
            }
            ProphetStateIndex next = ProphetDirector.AttackFor(ctx.AttackIndex);
            ctx.Countdown = ProphetDirector.DurationFor(next);
            return Create(next);
        }
    }
}
