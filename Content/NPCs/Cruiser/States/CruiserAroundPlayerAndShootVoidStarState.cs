using CalamityEntropy.Content.NPCs.Cruiser.Core;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.Cruiser.States
{
    /// <summary>锚点是玩家看向本体再转 0.6、外推 600,尾鞭削弱在链条那一侧</summary>
    [VaultState((int)CruiserStateIndex.AroundPlayerAndShootVoidStar, typeof(CruiserStateContext))]
    public class CruiserAroundPlayerAndShootVoidStarState : CruiserStateBase
    {
        public override CruiserStateIndex StateIndex => CruiserStateIndex.AroundPlayerAndShootVoidStar;

        /// <summary>原 % 40 == 0 会被 ±2 收养跨过,改成应发次数 = ChangeCounter / 40,只涨不重发</summary>
        private int whipsFired;

        public override void OnEnter(CruiserStateContext ctx) {
            base.OnEnter(ctx);
            whipsFired = 0;
        }

        public override IVaultState<CruiserStateContext> OnUpdate(CruiserStateContext ctx) {
            NPC npc = ctx.Npc;
            Player player = ctx.Target;

            Vector2 targetPos = player.Center
                + (npc.Center - player.Center).normalize().RotatedBy(CruiserDirector.AroundOrbitAngle) * CruiserDirector.AroundOrbitRadius;
            npc.velocity += (targetPos - npc.Center).normalize() * CruiserDirector.AroundThrust;
            npc.velocity *= CruiserDirector.AroundDrag;

            ctx.ChangeCounter++;
            //权威端每帧 +1,所以「应发次数」恰在 40 的倍数那一帧涨 1,与原等值判定逐帧等价
            int due = ctx.ChangeCounter / CruiserDirector.AroundWhipInterval;
            if (due > whipsFired) {
                int beat = due * CruiserDirector.AroundWhipInterval;
                whipsFired = due;
                //中途加入且已越过这一拍很久就静默记账,不补演出
                if (!CuePassed(ctx.ChangeCounter, beat)) {
                    ctx.TailWhipCue = true;
                    MarkNetUpdate(ctx);
                }
            }
            if (ctx.ChangeCounter > CruiserDirector.AroundDuration) {
                return NextAttack(ctx);
            }
            return null;
        }
    }
}
