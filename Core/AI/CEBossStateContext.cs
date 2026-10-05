using InnoVault.StateMachines;
using System;
using Terraria;

namespace CalamityEntropy.Core.AI
{
    /// <summary>运动声明不放这里,悬停、转向、贴地各 Boss 自己加</summary>
    public abstract class CEBossStateContext : INpcStateContext
    {
        public NPC Npc { get; set; }

        /// <summary>Target 可能已死,先看 TargetValid</summary>
        public Player Target { get; set; }

        /// <summary>TargetValid 表示目标存活且在感知距离内</summary>
        public bool TargetValid { get; set; }

        /// <summary>Phase 存在 ai[2],下限是 1</summary>
        public int Phase {
            get => Npc == null ? 1 : Math.Max(1, (int)Npc.ai[2]);
            set {
                if (Npc != null) {
                    Npc.ai[2] = value;
                }
            }
        }

        /// <summary>权威端写 AttackIndex,这个值要过线</summary>
        public int AttackIndex { get; set; }

        /// <summary>
        /// Countdown 是倒计时主钟,按帧计数并过线,拍点用递减版
        /// 状态的 OnUpdate 读到自减前的值时,初值和外部写入要减 1,否则这一拍不减
        /// 覆写必须用 override,同名 new 会让基类读到 0
        /// </summary>
        public virtual int Countdown { get; set; }

        /// <summary>子类覆写 BeginFrameDefaults 时先调 base,新通道再补默认值</summary>
        public virtual void BeginFrameDefaults() {
        }
    }
}
