using CalamityEntropy.Core.AI;

namespace CalamityEntropy.Content.NPCs.Cruiser.Core
{
    /// <summary>事实字段要过线,声明每帧回落,表现不过线</summary>
    public class CruiserStateContext : CEBossStateContext
    {
        #region 核心引用
        public CruiserHead Owner { get; set; }

        /// <summary>当前状态号。读的是已同步的 <c>ai[3]</c>,所以两端同值(尾鞭新星要按当前招削弱)</summary>
        public CruiserStateIndex StateIndex => Npc == null ? CruiserStateIndex.TryToClosePlayer : (CruiserStateIndex)(int)Npc.ai[3];
        #endregion

        #region 事实:过线
        /// <summary>
        /// 这是原 changeCounter,跨状态保持,只在选招时清零
        /// 转阶段、等咬中、激光瞄准窗都不推进。转阶段收尾不调 changeAi,尖刺带着残值起跑
        /// </summary>
        public int ChangeCounter { get; set; }

        /// <summary>原 localAI[2],原版不过线,这里随包走,只激光读写,收招时清零</summary>
        public int LaserAim { get; set; }
        #endregion

        #region 事实:每帧重算(各端同算,不过线)
        /// <summary>目标距离,每帧由宿主重算</summary>
        public float TargetDistance { get; set; }
        #endregion

        #region 声明:每帧回落
        /// <summary>
        /// 本帧请求一次尾鞭(原 <c>tjv = 1</c>)。原代码里它在同一帧就被尾鞭段消费掉,
        /// 所以这里做成每帧回落的声明通道,不是跨帧闩锁
        /// </summary>
        public bool TailWhipCue { get; set; }
        #endregion

        #region 表现:纯本地
        /// <summary>
        /// 嘴部张角(原 <c>mouthRot</c>)。纯绘制量:判定盒与伤害都不看它。
        /// 下限钳在 <see cref="CruiserDirector.MouthMin"/>
        /// </summary>
        public float MouthRot { get; set; }

        /// <summary>咬合闩锁(原 <c>bite</c>)。同样只影响嘴的绘制</summary>
        public bool Biting { get; set; }
        #endregion

        /// <summary>
        /// 每帧默认值。只回落尾鞭声明——<see cref="ChangeCounter"/> / <see cref="LaserAim"/>
        /// 是跨状态持久量,嘴部两项由宿主在状态机之后统一结算,都不在这里动
        /// </summary>
        public override void BeginFrameDefaults() {
            base.BeginFrameDefaults();
            TailWhipCue = false;
        }
    }
}
