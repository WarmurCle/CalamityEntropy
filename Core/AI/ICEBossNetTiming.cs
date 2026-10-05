namespace CalamityEntropy.Core.AI
{
    /// <summary>StateId/Timer/Counter 用 VaultState 自带的</summary>
    public interface ICEBossNetTiming
    {
        int StateId { get; }
        int Timer { get; }
        int Counter { get; }
        /// <summary>客户端收养权威计时,带容差</summary>
        void AdoptNetTiming(int timer, int counter);
    }
}
