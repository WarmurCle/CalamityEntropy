using CalamityEntropy.Core.AI;
using InnoVault.StateMachines;
using Terraria;

namespace CalamityEntropy.Content.NPCs.NihilityTwin.Core
{
    /// <summary>
    /// 事实字段要过线,声明每帧回落
    /// Num1 是原 aicounter,自增位置各段不同,不能折进 Timer++
    /// </summary>
    public class NihilityStateContext : CEBossStateContext
    {
        #region 核心引用
        public NihilityActeriophage Owner { get; set; }

        /// <summary>混沌细胞实体。可能为 null(尚未生成或已死),状态里读之前宿主已保证非空且存活</summary>
        public NPC Cell => Owner?.cell;
        #endregion

        #region 事实:过线
        /// <summary>多数是帧计时,二阶段 0/2 是冲刺次数,二阶段 1 蓄力期每两帧多跳一格,选招时清零</summary>
        public int Num1 { get; set; }

        /// <summary>
        /// 原 <c>NPC.ai[2]</c>。一阶段 6 号拿它存环射基准角,二阶段 0/2 号拿它当冲刺子计时。
        /// 两处共用一个槽是原代码的写法,按原写法保留,换招时不清零,残值会原样带进下一手
        /// </summary>
        public float Num2 { get; set; }

        /// <summary>
        /// 原 <c>NPC.ai[3]</c>。二阶段 1 号在发射帧锁存的服向,之后 30 帧细胞按它反向定速飞行。
        /// 新骨架里 <c>ai[3]</c> 归状态号占用,所以它挪成上下文字段并随包过线
        /// </summary>
        public float Num3 { get; set; }

        /// <summary>
        /// 原 <c>nz</c>。一阶段 2 号在蓄力期逐帧重算、突刺期只读的锁存突进向量。
        /// 它会反过来扰动本体速度(<c>NPC.velocity -= j * 0.01f</c>),所以必须过线
        /// </summary>
        public Vector2 Nz { get; set; }

        /// <summary>
        /// 原 <c>rotSpeed</c>。逐帧积分的自旋角速度,会累进写进 <c>NPC.rotation</c>,
        /// 而朝向又反过来决定细胞挂点与本体推进方向,属于典型的持久累加量,必须过线
        /// </summary>
        public float RotSpeed { get; set; }

        /// <summary>
        /// 原 <c>NPC.ai[0]</c>:一阶段 0 号的追击窗倒计时。<b>仍然住在 <c>ai[0]</c></b>,
        /// 原版同步槽白送一次同步,不必再进 ExtraAI
        /// </summary>
        public float ChaseTimer {
            get => Npc == null ? 0f : Npc.ai[0];
            set {
                if (Npc != null) {
                    Npc.ai[0] = value;
                }
            }
        }

        /// <summary>
        /// 原 <c>counter</c>(<c>NPC.ai[1]</c>):永不归零的全局帧计数,一堆 <c>counter % N</c> 射速门都读它。
        /// 走原版同步槽,两端天然一致
        /// </summary>
        public int FrameCounter {
            get => Npc == null ? 0 : (int)Npc.ai[1];
            set {
                if (Npc != null) {
                    Npc.ai[1] = value;
                }
            }
        }
        #endregion

        #region 事实:每帧重算(各端同算,不过线)
        /// <summary>目标距离,每帧由宿主重算</summary>
        public float TargetDistance { get; set; }
        #endregion

        #region 声明:每帧回落
        /// <summary>除自旋类外每帧清零,只有一阶段 1/4 号和二阶段 1 号声明它</summary>
        public bool KeepRotSpeed { get; set; }
        #endregion

        /// <summary>只回落 KeepRotSpeed,其余是持久事实,prepareAiChange 只动 aicounter 和 aitype</summary>
        public override void BeginFrameDefaults() {
            base.BeginFrameDefaults();
            KeepRotSpeed = false;
        }
    }
}
