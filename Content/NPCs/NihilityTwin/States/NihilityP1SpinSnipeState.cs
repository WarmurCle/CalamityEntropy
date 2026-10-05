using CalamityEntropy.Content.NPCs.NihilityTwin.Core;
using CalamityEntropy.Content.Projectiles;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.States
{
    /// <summary>
    /// RotSpeed 与一阶段 4 号、二阶段 1 号共用,离开这三手由宿主清零
    /// 先 *= 0.98 再整段覆盖成位置弹簧,前一句不起作用,照搬
    /// 原 counter % 1 == 0 恒真,这里略去
    /// </summary>
    [VaultState((int)NihilityStateIndex.P1SpinSnipe, typeof(NihilityStateContext))]
    public class NihilityP1SpinSnipeState : NihilityStateBase
    {
        public override NihilityStateIndex StateIndex => NihilityStateIndex.P1SpinSnipe;

        public override IVaultState<NihilityStateContext> OnUpdate(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            NPC cell = ctx.Cell;
            Vector2 targetPos = ctx.Target.Center;

            ctx.KeepRotSpeed = true;

            cell.rotation = npc.rotation;
            npc.velocity *= NihilityDirector.SpinDrag;
            npc.velocity = (targetPos - npc.Center) * NihilityDirector.SpinFollow;
            npc.rotation += ctx.RotSpeed;
            ctx.RotSpeed += NihilityDirector.SpinRotAccel;
            ctx.RotSpeed *= NihilityDirector.SpinRotDamp;
            cell.velocity *= NihilityDirector.SpinCellDrag;
            cell.velocity += (npc.Center + npc.rotation.ToRotationVector2() * NihilityDirector.SpinCellOffset - cell.Center) * NihilityDirector.SpinCellLerp;

            if (ctx.Num1 > NihilityDirector.SpinWindup && IsServer
                && CEUtils.getDistance(cell.Center, npc.Center) < NihilityDirector.SpinFireRange) {
                Shoot<CellBullet>(cell.GetSource_FromThis(), cell.Center,
                    npc.rotation.ToRotationVector2() * NihilityDirector.SpinBulletSpeed,
                    BulletDamage(ctx), NihilityDirector.BulletKnockback);
            }

            ctx.Num1++;
            IVaultState<NihilityStateContext> next = null;
            if (ctx.Num1 > NihilityDirector.SpinDuration) {
                next = EndAttack(ctx);
            }
            //原代码在收招判定之后还额外推了两次绳索求解让绳子跟上自旋;绳已迁到 Rigs2D 的 VerletStrand
            //(15 次约束迭代、两端钉死),自旋期不再需要补推
            return next;
        }
    }
}
