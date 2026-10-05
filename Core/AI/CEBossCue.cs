using System;

namespace CalamityEntropy.Core.AI
{
    /// <summary>
    /// CEBossCue 是过线计数上的一次性拍,窗内触发,越窗只记账,不许第二次开火
    /// 弹幕、骰点和世界写入仍放在 IsServer,Cue 必须是 class,属性上的 struct 改的是副本
    /// </summary>
    public sealed class CEBossCue
    {
        /// <summary>CatchUpGrace 是 20 帧,容差 ±2,包间隔约 4 到 5 帧,再小会误伤慢客户端</summary>
        public const int CatchUpGrace = 20;

        private bool fired;
        private int periodicFired;

        /// <summary>Fired 表示已经记过账,越窗静默也算记过</summary>
        public bool Fired => fired;

        /// <summary>进状态时调 Reset,漏掉的话下一次整拍会哑</summary>
        public void Reset() {
            fired = false;
            periodicFired = 0;
        }

        /// <summary>TryFire 给递增计数用</summary>
        public bool TryFire(float counter, int beat, int grace = CatchUpGrace) {
            if (fired || counter < beat) {
                return false;
            }
            fired = true;
            return counter <= beat + grace;
        }

        /// <summary>TryFireDescending 给递减计数用,窗是 [beat - grace, beat]</summary>
        public bool TryFireDescending(float countdown, int beat, int grace = CatchUpGrace) {
            if (fired || countdown > beat) {
                return false;
            }
            fired = true;
            return countdown >= beat - grace;
        }

        /// <summary>
        /// TryFirePeriodic 不取模,取模前推会跳拍,回退会重放
        /// 它没有递减版,倒计时先折成已过时间,或确认副作用全在 IsServer
        /// </summary>
        public bool TryFirePeriodic(float counter, int interval) {
            if (interval <= 0) {
                return false;
            }
            int due = (int)Math.Floor(counter / interval) + 1;
            if (due <= periodicFired) {
                return false;
            }
            periodicFired = due;
            return true;
        }

        /// <summary>Passed 留给手写闩锁,新代码用 TryFire</summary>
        public static bool Passed(float counter, int beat, int grace = CatchUpGrace)
            => counter > beat + grace;

        /// <summary>PassedDescending 是倒计时的手写闩锁</summary>
        public static bool PassedDescending(float countdown, int beat, int grace = CatchUpGrace)
            => countdown < beat - grace;
    }
}
