using InnoVault.StateMachines;
using System;
using Terraria;

namespace CalamityEntropy.Core.AI
{
    /// <summary>运动声明不放这里,悬停、转向、贴地各 Boss 自己加</summary>
    public abstract class CEBossStateContext : INpcStateContext
    {
        public NPC Npc { get; set; }

        /// <summary>可能已死,先看 TargetValid</summary>
        public Player Target { get; set; }

        /// <summary>存活且在感知距离内</summary>
        public bool TargetValid { get; set; }

        /// <summary>ai[2],下限 1</summary>
        public int Phase {
            get => Npc == null ? 1 : Math.Max(1, (int)Npc.ai[2]);
            set {
                if (Npc != null) {
                    Npc.ai[2] = value;
                }
            }
        }

        /// <summary>权威端写,要过线</summary>
        public int AttackIndex { get; set; }

        /// <summary>
        /// 倒计时主钟,按帧计数过线,拍点用递减版
        /// 体读到自减前的值时,初值和外部写入减 1,否则不减
        /// 覆写用 override,同名 new 会让基类读到 0
        /// </summary>
        public virtual int Countdown { get; set; }

        /// <summary>子类先调 base,新通道补默认</summary>
        public virtual void BeginFrameDefaults() {
        }
    }
}
