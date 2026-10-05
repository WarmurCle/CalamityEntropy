using System;

namespace CalamityEntropy.Core.AI
{
    /// <summary>
    /// 过线计数的一次性拍,窗内触发,越窗只记账,不许第二次
    /// 弹幕、骰点、世界写入仍放 IsServer,必须是 class,属性上的 struct 改的是副本
    /// </summary>
    public sealed class CEBossCue
    {
        /// <summary>20 帧,容差 ±2,包间隔约 4~5,再小会误伤慢客户端</summary>
        public const int CatchUpGrace = 20;

        private bool fired;
        private int periodicFired;

        /// <summary>记过账,含越窗静默</summary>
        public bool Fired => fired;

        /// <summary>进状态复位,漏了第二次整拍哑</summary>
        public void Reset() {
            fired = false;
            periodicFired = 0;
        }

        /// <summary>递增</summary>
        public bool TryFire(float counter, int beat, int grace = CatchUpGrace) {
            if (fired || counter < beat) {
                return false;
            }
            fired = true;
            return counter <= beat + grace;
        }

        /// <summary>递减,窗是 [beat - grace, beat]</summary>
        public bool TryFireDescending(float countdown, int beat, int grace = CatchUpGrace) {
            if (fired || countdown > beat) {
                return false;
            }
            fired = true;
            return countdown >= beat - grace;
        }

        /// <summary>
        /// 不取模,取模前推会跳拍、回退会重放
        /// 没有递减版,倒计时先折成已过时间,或确认副作用全在 IsServer
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

        /// <summary>手写闩锁用,新代码用 TryFire</summary>
        public static bool Passed(float counter, int beat, int grace = CatchUpGrace)
            => counter > beat + grace;

        /// <summary>倒计时的手写闩锁</summary>
        public static bool PassedDescending(float countdown, int beat, int grace = CatchUpGrace)
            => countdown < beat - grace;
    }
}
