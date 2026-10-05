using Microsoft.Xna.Framework.Graphics;

namespace CalamityEntropy.Core.Integrations.BossLog
{
    /// <summary>书外不铺整屏背景,2026-09-19</summary>
    internal abstract class CEBossLogTheme
    {
        public abstract Color Cover { get; }
        /// <summary>封边亮线和书脊高光</summary>
        public abstract Color CoverEdge { get; }
        public abstract Color Spine { get; }
        /// <summary>要暗,右页白字、金标题和物品格才读得清</summary>
        public abstract Color Paper { get; }
        /// <summary>纸面纤维亮部 / 页缘叠层线</summary>
        public abstract Color PaperLight { get; }
        /// <summary>页内细线</summary>
        public abstract Color Rule { get; }
        /// <summary>强调光:标题、场景框角</summary>
        public abstract Color Accent { get; }
        /// <summary>次要文字(模组名)</summary>
        public abstract Color Muted { get; }

        /// <summary>最小高度,UI px,TitleBandOf 随字号再撑</summary>
        public virtual int TitleBand => 58;

        /// <summary>
        /// 只许落在书脊和底封边,书签在左右外突,左右封边不能用
        /// </summary>
        public virtual void DrawOrnament(SpriteBatch sb, Rectangle book, Rectangle spine, Rectangle bottomMargin, float time, float blend) { }
    }
}
