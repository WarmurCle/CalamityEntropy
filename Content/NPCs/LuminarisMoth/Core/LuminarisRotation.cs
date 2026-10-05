using InnoVault.StateMachines;

namespace CalamityEntropy.Content.NPCs.LuminarisMoth.Core
{
    /// <summary>
    /// 轮换允许连续重复同一招,读的是自增前的序号,开局第一手是 0 号槽
    /// 先选招再判越界,6 号槽出招、下一轮才回 0
    /// 转二阶段不重置序号
    /// </summary>
    public static class LuminarisRotation
    {
        public static IVaultState<LuminarisStateContext> Create(LuminarisStateIndex state)
            => VaultStateRegistry<LuminarisStateContext>.Create((int)state);

        /// <summary>只权威端调,门在 NextAttack</summary>
        /// <param name="current">二阶段表外沿用上一手,现有范围到不了</param>
        public static IVaultState<LuminarisStateContext> Pick(LuminarisStateContext ctx, LuminarisStateIndex current) {
            //锚点和标量先清零,再裁决
            ctx.Vec1 = Vector2.Zero;
            ctx.Vec2 = Vector2.Zero;
            ctx.Num1 = 0f;
            ctx.Num2 = 0f;
            ctx.Num3 = 0f;

            LuminarisStateIndex next = SetAISyyle(ctx, current);
            //原代码 SetAISyyle() 返回后紧跟的 AIRound++
            ctx.AttackIndex++;
            ctx.Countdown = LuminarisDirector.DurationOf(next);
            if (ctx.Npc != null) {
                //原 pick 块末尾的 netUpdate。决策点:换招
                ctx.Npc.netUpdate = true;
            }
            return Create(next);
        }

        /// <summary>一阶段把序号限制在 0 到 6,二阶段用 if 重映射,越界时 round 大于等于 6 就写成 -1</summary>
        private static LuminarisStateIndex SetAISyyle(LuminarisStateContext ctx, LuminarisStateIndex current) {
            int round = ctx.AttackIndex;
            LuminarisStateIndex next = current;

            if (ctx.Phase == 1) {
                //原 `ai = (AIStyle)AIRound;`——一阶段的出招序列就是枚举的前七项
                next = (LuminarisStateIndex)round;
            }
            else {
                if (round == 0) {
                    next = LuminarisStateIndex.Shoot360;
                }
                if (round == 1) {
                    next = LuminarisStateIndex.SmashDown;
                }
                if (round == 2) {
                    next = LuminarisStateIndex.RoundAndDash;
                }
                if (round == 3) {
                    next = LuminarisStateIndex.Subduction;
                }
                if (round == 4) {
                    next = LuminarisStateIndex.ShootTriangle;
                }
                if (round == 5) {
                    next = LuminarisStateIndex.AstralSpike;
                }
                if (round == 6) {
                    next = LuminarisStateIndex.Dashing;
                }
            }

            //越界处理:置 -1 而非 0,因为紧接着还有一次自增
            if (round >= 6) {
                ctx.AttackIndex = -1;
            }
            return next;
        }
    }
}
