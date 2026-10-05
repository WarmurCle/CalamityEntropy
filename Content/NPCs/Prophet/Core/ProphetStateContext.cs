using CalamityEntropy.Core.AI;

namespace CalamityEntropy.Content.NPCs.Prophet.Core
{
    /// <summary>事实过线,没有声明通道,招式直接写 velocity 和 rotation</summary>
    public class ProphetStateContext : CEBossStateContext
    {
        public ProphetStateContext() {
            //原 AIC 字段初值是 -1,所以第一次选招自增后正好落在 0 号槽(第一手必定是四轮符文弹)。
            //基类的 AttackIndex 默认 0,不在这里扳回来的话开场第一手会变成 3 号招
            AttackIndex = ProphetDirector.AttackIndexStart;
        }

        #region 核心引用
        public TheProphet Owner { get; set; }
        #endregion

        #region 事实:过线
        /// <summary>
        /// 原 AIChangeDelay,倒计时,选招当帧以满值跑,帧末自减
        /// 节拍原样判定,不减 1,原代码在状态体之后才自减,中途压到 30 也会过线
        /// </summary>
        public override int Countdown { get; set; }

        /// <summary>归零后继续走负数,大于 0 不出招、不更新尾迹、强制朝上且免疫,过线免得中途加入从 120 重跑</summary>
        public int SpawnAnim { get; set; } = ProphetDirector.SpawnAnimFrames;

        /// <summary>逐帧积分,不过线则中途加入一直按 0.26 算</summary>
        public float DrRamp { get; set; } = ProphetDirector.DrRampInitial;

        /// <summary>权威端每次瞬移 +1,客户端据此认成瞬移而不是失步</summary>
        public int TeleportSeq { get; set; }

        /// <summary>权威端掷骰算出的瞬移落点。与流水号一起过线,客户端补演出时用它定位</summary>
        public Vector2 TeleportPos { get; set; }
        #endregion

        #region 事实:每帧重算(各端同算,不过线)
        /// <summary>宿主每帧重算,来源必须已同步,否则位置误差无界</summary>
        public float Difficult { get; set; } = 1f;
        #endregion

        /// <summary>先知没有需要回落的声明通道,这里只保持契约形状</summary>
        public override void BeginFrameDefaults() {
            base.BeginFrameDefaults();
        }
    }
}
