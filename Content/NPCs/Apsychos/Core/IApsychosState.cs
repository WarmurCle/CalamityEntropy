using CalamityEntropy.Core.AI;
using InnoVault.StateMachines;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Apsychos.Core
{
    /// <summary>写入 ai[3],序号对齐重构前的 AIStyle</summary>
    public enum ApsychosStateIndex
    {
        /// <summary>接近:转向玩家并按距离推进,轮换表里每隔一手就垫一次</summary>
        MoveToTarget = 0,
        /// <summary>冲刺:蓄力刹速涨描边,点火后 20 帧推进,再 20 帧收尾</summary>
        Dash = 1,
        /// <summary>三连火球:尾巴前伸当炮口,五轮三发扇形</summary>
        FireballShooting = 2,
        /// <summary>喷火:尾巴摆动瞄人,60 到 140 帧持续喷射</summary>
        FlameThrow = 3,
        /// <summary>巨型火球:蓄满即发,共四发</summary>
        FireballBig = 4,
        /// <summary>转阶段:白化涨满换配色,80 帧置阶段,120 帧收</summary>
        PhaseTrans = 5,
        /// <summary>甩尾:尾巴内收蓄力,60 帧定向突进,尾巴弹出打尾刺,按血量循环 3 或 6 次</summary>
        TailDash = 6,
        /// <summary>激光:尾巴指向本体正前方,开局即发,350 帧慢速扫场</summary>
        Laser = 7,
    }

    /// <summary>拍子用 Timer 区间判断,不加会归零 Timer 的 beat 枚举</summary>
    public abstract class ApsychosStateBase : CEBossStateBase<ApsychosStateContext>
    {
        public override int StateId => (int)StateIndex;
        public abstract ApsychosStateIndex StateIndex { get; }
        public override string StateName => StateIndex.ToString();

        /// <summary>原代码没有超时兜底,这里统一挂一个到不了的安全网,见 Director 的说明</summary>
        public override int TimeoutFrames => ApsychosDirector.StateTimeoutFrames;

        /// <summary>超时的去处:当成正常收招,走轮换表选下一手</summary>
        protected override IVaultState<ApsychosStateContext> OnTimeout(ApsychosStateContext ctx)
            => NextAttack(ctx);

        /// <summary>
        /// 只有权威端真的选招,客户端返回 null 等包
        /// 不清 TailDashReps,原 SetAIStyle 也不清
        /// </summary>
        protected static IVaultState<ApsychosStateContext> NextAttack(ApsychosStateContext ctx)
            => IsServer ? ApsychosRotation.Pick(ctx) : null;

        /// <summary>一阶段 damage/6.5,二阶段 damage/5.4,击退 4,owner -1,客户端不生成</summary>
        protected static void Shoot<T>(ApsychosStateContext ctx, Vector2 pos, Vector2 velocity,
            float damageMult = 1f, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f) where T : ModProjectile {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                return;
            }
            NPC npc = ctx.Npc;
            float divisor = ctx.Phase == 1 ? ApsychosDirector.ProjDamageDivisorPhase1 : ApsychosDirector.ProjDamageDivisorPhase2;
            int baseDamage = (int)(npc.damage / divisor);
            Projectile.NewProjectile(npc.GetSource_FromAI(), pos, velocity, ModContent.ProjectileType<T>(),
                (int)(baseDamage * damageMult), ApsychosDirector.ProjKnockback, -1, ai0, ai1, ai2);
        }
    }
}
