using CalamityEntropy.Core.AI;
using InnoVault.StateMachines;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.Core
{
    /// <summary>写入 ai[3],原 aitype 的 0~6 两阶段是两套招,摊成 15 个序号</summary>
    public enum NihilityStateIndex
    {
        /// <summary>整备(原 <c>aitype == -1</c>):两端互相收拢并扑向玩家,81 帧后随机选下一手</summary>
        Regroup = 0,

        /// <summary>一阶段 0:突进环射。追出 1400 就重开追击窗,贴近时边加速边环射</summary>
        P1Rush = 1,
        /// <summary>一阶段 1:自旋狙击。本体原地自旋,细胞挂在后方 120 处朝背面连射</summary>
        P1SpinSnipe = 2,
        /// <summary>一阶段 2:细胞长矛。细胞外推 260 蓄力,再按锁存方向渐加速突刺并双侧散射</summary>
        P1CellLance = 3,
        /// <summary>一阶段 3:悬停爆发。抢占玩家上方 200,细胞外推 200 并每 30 帧一圈六发</summary>
        P1HoverBurst = 4,
        /// <summary>一阶段 4:广角自旋。细胞甩到 520 外,每 30 帧一次十一发扇形 + 一圈环射</summary>
        P1WideSpin = 5,
        /// <summary>一阶段 5:对拉旋转。两端分居玩家两侧 840,绕着玩家转 460 帧后一起上浮</summary>
        P1Orbit = 6,
        /// <summary>一阶段 6:绕细胞盘旋。本体定速 18 绕细胞飞,细胞持续吐环,160 帧后改为高速散射</summary>
        P1Circle = 7,

        /// <summary>二阶段 0:冲刺齐射。六次锁向冲刺,细胞同时环射,本体每 10 帧补侧刺</summary>
        P2DashVolley = 8,
        /// <summary>二阶段 1:细胞炮。细胞焊在本体后方蓄力自旋,100 帧锁向甩出并拖出弹幕走廊</summary>
        P2CellCannon = 9,
        /// <summary>二阶段 2:螺旋冲刺。三次冲刺,细胞吐慢速螺旋弹墙</summary>
        P2DashSpiral = 10,
        /// <summary>二阶段 3:对撞合体。两端反向拉开 40 帧后对撞,命中即爆散 36 发,1/2 概率原地再来</summary>
        P2Merge = 11,
        /// <summary>二阶段 4:分裂增殖。第 2 帧放出三只小细胞,随后持续环射</summary>
        P2Split = 12,
        /// <summary>二阶段 5:口部激光。抬头 40 帧后开一条 400 帧的扫射光束,细胞伴随散射</summary>
        P2Laser = 13,
        /// <summary>二阶段 6:能量球。第 2/62/122 帧各投九颗定向能量球</summary>
        P2EnergyBall = 14,
    }

    /// <summary>拍子用 Num1 区间判断,不加会归零 Timer 的 beat 枚举</summary>
    public abstract class NihilityStateBase : CEBossStateBase<NihilityStateContext>
    {
        public override int StateId => (int)StateIndex;
        public abstract NihilityStateIndex StateIndex { get; }
        public override string StateName => StateIndex.ToString();

        /// <summary>原代码没有超时兜底,这里统一挂一个到不了的安全网,见 Director 的说明</summary>
        public override int TimeoutFrames => NihilityDirector.StateTimeoutFrames;

        /// <summary>超时的去处:当成正常收招,退回整备</summary>
        protected override IVaultState<NihilityStateContext> OnTimeout(NihilityStateContext ctx)
            => EndAttack(ctx);

        /// <summary>
        /// 只有权威端真的换态,客户端返回 null 等包
        /// 原 netUpdate 由 AiSlotNetSync 在换态时补上
        /// </summary>
        protected static IVaultState<NihilityStateContext> EndAttack(NihilityStateContext ctx)
            => IsServer ? NihilityRotation.Regroup(ctx) : null;

        /// <summary>选下一手(原 <c>randomAI()</c>)。同样只有权威端真的掷点</summary>
        protected static IVaultState<NihilityStateContext> NextAttack(NihilityStateContext ctx)
            => IsServer ? NihilityRotation.Pick(ctx) : null;

        /// <summary>细胞弹 / 尖刺的基础伤害:<c>NPC.damage / 6</c>(整数除法,原代码如此)</summary>
        protected static int BulletDamage(NihilityStateContext ctx)
            => ctx.Npc.damage / NihilityDirector.ProjDamageDivisor;

        /// <summary>
        /// 沿本帧位移均匀撒十组尾迹尘。原代码在四个冲刺类状态里各抄了一遍同样的循环。
        /// 纯表现:<c>Dust.NewDust</c> 在服务端会直接返回,不吃随机数
        /// </summary>
        protected static void TrailBurst(NihilityStateContext ctx) {
            NPC npc = ctx.Npc;
            for (int i = 0; i < NihilityDirector.TrailSamples; i++) {
                ctx.Owner.SpawnParticle(npc.Center + npc.velocity * ((float)i / NihilityDirector.TrailSamples));
            }
        }

        /// <summary>
        /// 生成敌对弹幕。owner 固定 -1;客户端不生成。
        /// <paramref name="source"/> 用原代码的那一个(细胞出的弹用 <c>cell.GetSource_FromThis()</c>,
        /// 本体出的用 <c>NPC.GetSource_FromThis()</c>),继承链会被别的系统读到,不要统一
        /// </summary>
        protected static void Shoot<T>(IEntitySource source, Vector2 pos, Vector2 velocity,
            int damage, float knockback, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f) where T : ModProjectile {
            if (Main.netMode == NetmodeID.MultiplayerClient) {
                return;
            }
            Projectile.NewProjectile(source, pos, velocity, ModContent.ProjectileType<T>(), damage, knockback, -1, ai0, ai1, ai2);
        }
    }
}
