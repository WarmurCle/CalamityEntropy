using System;

namespace CalamityEntropy.Core.AI
{
    /// <summary>
    /// AdoptFrameCounter 只收养步长 ±1 的帧计数,整段跳变必须远大于容差,否则调用方直接取值
    /// 同一个槽只能一种语义,清零残值小于等于容差时会被吞掉
    /// 权威端不收养,客户端的等值拍点才靠容差
    /// </summary>
    public static class CEBossNetAdopt
    {
        public static int AdoptFrameCounter(int local, int synced, int tolerance = CEBossNetMotion.TimerTolerance)
            => Math.Abs(synced - local) > tolerance ? synced : local;

        /// <summary>num 槽常被复用成角度或比例,那些不是帧计数</summary>
        public static float AdoptFrameCounter(float local, float synced, float tolerance = CEBossNetMotion.TimerTolerance)
            => Math.Abs(synced - local) > tolerance ? synced : local;
    }
}
