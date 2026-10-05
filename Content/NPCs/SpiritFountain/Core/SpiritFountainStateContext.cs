using CalamityEntropy.Core.AI;

namespace CalamityEntropy.Content.NPCs.SpiritFountain.Core
{
    /// <summary>事实过线,声明每帧回落,柱子四量由宿主写进 ExtraAI</summary>
    public class SpiritFountainStateContext : CEBossStateContext
    {
        #region 核心引用
        /// <summary>宿主。状态要读柱子、要调 <c>Shoot</c>,都从这里走</summary>
        public SpiritFountain Owner { get; set; }
        #endregion

        #region 事实:过线
        /// <summary>原 Counter,永不归零,射速节拍和驻点摆动拿它取模</summary>
        public float GlobalCounter { get; set; }

        /// <summary>原 num1,魂环按 Index 和它比大小决定脱柱,必须过线</summary>
        public float Num1 { get; set; }

        /// <summary>横扫段的摇摆相位,对应原 <c>mCounter</c>。逐帧积分出来又直接决定柱子横坐标,典型的持久累加量</summary>
        public float MCounter { get; set; }

        /// <summary>横扫段的摇摆幅度,对应原 <c>mAmp</c>。同上,从 0 慢慢涨到 1</summary>
        public float MAmp { get; set; }

        /// <summary>原 GatheringAnimation,中途加入靠它对上演出进度</summary>
        public int GatheringAnimation { get; set; } = SpiritFountainDirector.GatheringFrames;

        /// <summary>一号柱魂环的一次性生成闸(出场演出结束时用掉)</summary>
        public bool SpawnSpirits { get; set; } = true;

        /// <summary>二号柱魂环的一次性生成闸(血量跌破 66% 时用掉)</summary>
        public bool SpawnSpirits2 { get; set; } = true;
        #endregion

        #region 事实:每帧重算(各端同算,不过线)
        /// <summary>宿主每帧重算,来源必须已同步,否则射速和摇摆分叉</summary>
        public float Enrage { get; set; } = 1f;
        #endregion

        #region 事实:跨帧闸(由已过线的状态号确定性推导)
        /// <summary>只有转阶段打开,等价于状态号,不必单独过线</summary>
        public bool DontTakeDmg { get; set; }

        /// <summary>落点只写一次的闸,对应原 <c>SetPos</c>。各端各自落一次,随后由原版位置同步对账</summary>
        public bool SetPos { get; set; }
        #endregion

        #region 声明:每帧回落
        /// <summary>
        /// 本帧眼睛透明度的目标值,对应原 <c>EyeAlphaT</c>。默认 0.6,
        /// 出场演出改成跟随自身、三阶段横扫与十字斩顶到 1
        /// </summary>
        public float EyeAlphaTarget { get; set; } = SpiritFountainDirector.EyeAlphaIdle;

        /// <summary>
        /// 本帧瞳孔是否直接钉在本地玩家身上。对应原代码挂在「当前不是出场演出」上的那条 else,
        /// 所以只有出场演出会关掉它(它自己用带插值的追踪)
        /// </summary>
        public bool StareAtLocalPlayer { get; set; } = true;

        /// <summary>只有 Moving 打开,其余状态把摇摆清零</summary>
        public bool KeepMovingSway { get; set; }

        /// <summary>聚魂期间的 return,同时吃掉脱战、眼睛插值和摇摆清零</summary>
        public bool HaltFrame { get; set; }
        #endregion

        #region 表现:纯本地
        /// <summary>眼睛当前透明度,对应原 <c>EyeAlpha</c>。只读于绘制,收敛型插值,不过线</summary>
        public float EyeAlpha { get; set; }

        /// <summary>喷流贴图滚动速度,对应原 <c>FountainSpeed</c>。只喂 <c>trailDrawOffset</c>,不过线</summary>
        public float FountainSpeed { get; set; } = 20f;

        /// <summary>瞳孔注视点,对应原 <c>starePoint</c>。读的是 <c>Main.LocalPlayer</c>,天然是本地量</summary>
        public Vector2 StarePoint { get; set; }

        /// <summary>上一帧的一号柱横坐标,对应原 <c>c1LastPos</c>。同帧内写完即读,用来求柱子的倾斜角</summary>
        public float C1LastPos { get; set; }

        /// <summary>原 <c>CenterRing</c>。原代码写它但从不读,照搬保留</summary>
        public int CenterRing { get; set; }
        #endregion

        /// <summary>四个声明回落,跨帧闸和表现累加量不在这里动</summary>
        public override void BeginFrameDefaults() {
            base.BeginFrameDefaults();
            EyeAlphaTarget = SpiritFountainDirector.EyeAlphaIdle;
            StareAtLocalPlayer = true;
            KeepMovingSway = false;
            HaltFrame = false;
        }
    }
}
