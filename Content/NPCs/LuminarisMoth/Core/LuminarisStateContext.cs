using CalamityEntropy.Core.AI;
using System.Collections.Generic;

namespace CalamityEntropy.Content.NPCs.LuminarisMoth.Core
{
    /// <summary>
    /// 事实字段要过线,每帧重算的值和表现不过线
    /// BeginFrameDefaults 故意空着,AfterImage 和 MegaTrail 靠宿主衰减
    /// </summary>
    public class LuminarisStateContext : CEBossStateContext
    {
        #region 核心引用
        public Luminaris Owner { get; set; }
        #endregion

        #region 事实:过线
        /// <summary>
        /// 这是原 AIChangeCounter,进状态时不赋值,生成首帧靠初值 -1
        /// 从原字段搬要减 1,天顶分身 210~270 同样
        /// </summary>
        public override int Countdown { get; set; } = -1;

        /// <summary>这是原 vec1,当作 Lerp 起点,要过线</summary>
        public Vector2 Vec1 { get; set; }

        /// <summary>这是原 vec2,当作落点,ShootTriangle 把它当上一帧位置</summary>
        public Vector2 Vec2 { get; set; }

        /// <summary>原 num1,半径或锁向起始角</summary>
        public float Num1 { get; set; }

        /// <summary>原 num2,绕转角或锁向目标角</summary>
        public float Num2 { get; set; }

        /// <summary>这是原 num3,绕转方向是 ±1,AboveMoving 和 ShootTriangle 骰了它但不读</summary>
        public float Num3 { get; set; }
        #endregion

        #region 事实:每帧重算(各端同算,不过线)
        /// <summary>宿主每帧重算,来源必须已同步,否则速度误差无界</summary>
        public float Enrange { get; set; } = 1f;
        #endregion

        #region 表现:本地推导
        /// <summary>这是原 oldPos,Dashing 不读它,朝向不回头改航向</summary>
        public Vector2 OldPos { get; set; }

        /// <summary>这是原 odp,RoundShooting 整条平移,起冲那一拍清空</summary>
        public List<Vector2> Trail { get; } = new List<Vector2>();

        /// <summary>这是原 MegaTrail,SmashDown 时小于等于 0 就没有接触伤,它自身不过线</summary>
        public float MegaTrail { get; set; }

        /// <summary>这是原 AfterImageTime,状态把它写满,宿主每帧减 1</summary>
        public int AfterImageTime { get; set; }
        #endregion

        /// <summary>故意空着</summary>
        public override void BeginFrameDefaults() {
            base.BeginFrameDefaults();
        }
    }
}
