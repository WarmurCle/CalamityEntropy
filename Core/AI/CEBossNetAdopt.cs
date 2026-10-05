using System;

namespace CalamityEntropy.Core.AI
{
    /// <summary>
    /// 只收养帧计数,步长 ±1,整段跳度要远大于容差,否则直取
    /// 一个槽一种语义,清零残值小于等于容差会被吞
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
