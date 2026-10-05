using CalamityEntropy.Core.AI;
using InnoVault.StateMachines;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Prophet.Core
{
    /// <summary>写入 ai[3],序号对齐迁移前的 AIStyle,CanHitPlayer 和 ModifyIncomingHit 还按这套编号读</summary>
    public enum ProphetStateIndex
    {
        /// <summary>四轮符文弹:侧向绕行,定期闪到远处再打一把扇形洪流</summary>
        RuneVolley = 0,
        /// <summary>冲刺:三次锁向反冲后突进,第三次是重击并沿途撒侧弹</summary>
        Dash = 1,
        /// <summary>符文晶簇:闪现后贴脸冲刺,半拍吐出高速晶体</summary>
        RuneCluster = 2,
        /// <summary>双层符文洪流:闪现后一次性铺满两层扇面</summary>
        RuneTorrentFan = 3,
        /// <summary>速射符文洪流:慢段 6 帧一发转快段 2 帧一发</summary>
        RapidTorrent = 4,
        /// <summary>环状符文:每三四帧闪一次并吐一发慢弹,收尾转普通追击</summary>
        RingBlink = 5,
        /// <summary>符文光球:在自身周围布一圈静止符文,二阶段连布三圈</summary>
        RuneOrb = 6,
        /// <summary>符文冲击:绕行与刹车放电交替,每 40 帧一次扇形闪电</summary>
        RuneImpact = 7,
        /// <summary>大激光:清场定位,把范围内玩家拖到炮口前,再放出眼球</summary>
        GrandLaser = 8,
        /// <summary>符文飞匕:闪到中距离后每 5 帧撒一把匕首</summary>
        RuneDagger = 9,
        /// <summary>虚空触手:慢速贴近,定期放出一整圈尖刺</summary>
        VoidSpike = 10,
        /// <summary>异形符文冲锋:先布一圈异形符文,再三次直线冲锋</summary>
        AltRuneCharge = 11,
    }

    /// <summary>
    /// 拍子用 Countdown 原样判定,不加会归零 Timer 的 beat 枚举
    /// 收招不由状态发起,选招在宿主里同帧跑新招,OnUpdate 恒返回 null
    /// </summary>
    public abstract class ProphetStateBase : CEBossStateBase<ProphetStateContext>
    {
        public override int StateId => (int)StateIndex;
        public abstract ProphetStateIndex StateIndex { get; }
        public override string StateName => StateIndex.ToString();

        /// <summary>原代码没有超时兜底,这里统一挂一个到不了的安全网,见 Director 的说明</summary>
        public override int TimeoutFrames => ProphetDirector.StateTimeoutFrames;

        /// <summary>超时的去处:当成正常收招,走轮换表选下一手(只有权威端真的选)</summary>
        protected override IVaultState<ProphetStateContext> OnTimeout(ProphetStateContext ctx)
            => IsServer ? ProphetRotation.Pick(ctx) : null;

        /// <summary>权威端 else 永远进不来,客户端在倒计时归零、包还没到的一两帧会走到,照搬</summary>
        public sealed override IVaultState<ProphetStateContext> OnUpdate(ProphetStateContext ctx) {
            if (ctx.Countdown > 0) {
                RunAttack(ctx);
            }
            else {
                IdleDrift(ctx);
            }
            return null;
        }

        /// <summary>招式本体。倒计时仍为正时每帧调用一次</summary>
        protected abstract void RunAttack(ProphetStateContext ctx);

        /// <summary>原 else 支:朝向跟速度走,轻微阻尼,再朝玩家加一个单位推力</summary>
        private static void IdleDrift(ProphetStateContext ctx) {
            NPC npc = ctx.Npc;
            npc.rotation = npc.velocity.ToRotation();
            npc.velocity *= ProphetDirector.IdleDrag;
            npc.velocity += (ctx.Target.Center - npc.Center).SafeNormalize(Vector2.Zero) * ProphetDirector.IdleThrust;
        }

        /// <summary>常规弹幕伤害:原代码一律 <c>NPC.damage / 6</c>(整数除法)</summary>
        protected static int ProjDamage(ProphetStateContext ctx)
            => ctx.Npc.damage / ProphetDirector.ProjDamageDivisor;

        /// <summary>符文结晶起手音。原代码在 0 / 1 / 3 / 6 号招里逐字重复了四遍,这里合成一处</summary>
        protected static void CrystalCue(NPC npc) {
            if (!Main.dedServ) {
                SoundEngine.PlaySound(SoundID.Item9 with { Pitch = 0.2f, Volume = 0.85f, MaxInstances = 8 }, npc.Center);
                CEUtils.PlaySound("crystedge_spawn_crystal", Main.rand.NextFloat(0.8f, 1.2f), npc.Center);
            }
        }

        /// <summary>
        /// 生成敌对弹幕,owner 固定 -1。客户端不生成
        /// (<c>IsServer</c> 就是原代码的 <c>Main.netMode != NetmodeID.MultiplayerClient</c>)
        /// </summary>
        protected static void Shoot<T>(ProphetStateContext ctx, Vector2 pos, Vector2 velocity, int damage,
            float knockback, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f) where T : ModProjectile {
            if (!IsServer) {
                return;
            }
            NPC npc = ctx.Npc;
            Projectile.NewProjectile(npc.GetSource_FromAI(), pos, velocity, ModContent.ProjectileType<T>(),
                damage, knockback, -1, ai0, ai1, ai2);
        }

        /// <summary>
        /// 只有权威端掷骰并落位,落点写 TeleportPos,流水号 +1,和位置速度同包
        /// 客户端看到流水号推进后补火花、清尾迹、丢预测
        /// </summary>
        protected static void Teleport(ProphetStateContext ctx, Vector2 pos) {
            if (!IsServer) {
                return;
            }
            ctx.TeleportSeq++;
            ctx.TeleportPos = pos;
            ctx.Owner.TeleportTo(pos);
            ctx.Npc.netUpdate = true;
        }
    }
}
