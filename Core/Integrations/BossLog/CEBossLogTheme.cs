using Microsoft.Xna.Framework.Graphics;

namespace CalamityEntropy.Core.Integrations.BossLog
{
    /// <summary>书外不铺整屏背景,2026-09-19</summary>
    internal abstract class CEBossLogTheme
    {
        public abstract Color Cover { get; }
        /// <summary>CoverEdge 画封边亮线和书脊高光</summary>
        public abstract Color CoverEdge { get; }
        public abstract Color Spine { get; }
        /// <summary>Paper 要暗,右页白字、金标题和物品格才读得清</summary>
        public abstract Color Paper { get; }
        /// <summary>PaperLight 画纸面纤维亮部和页缘叠层线</summary>
        public abstract Color PaperLight { get; }
        /// <summary>Rule 画页内细线</summary>
        public abstract Color Rule { get; }
        /// <summary>Accent 是强调光,用在标题和场景框角</summary>
        public abstract Color Accent { get; }
        /// <summary>Muted 是次要文字,模组名用它</summary>
        public abstract Color Muted { get; }

        /// <summary>TitleBand 是最小高度,单位是 UI 像素,TitleBandOf 再按字号撑开</summary>
        public virtual int TitleBand => 58;

        /// <summary>
        /// DrawOrnament 只许落在书脊和底封边,书签在左右外突,左右封边不能用
        /// </summary>
        public virtual void DrawOrnament(SpriteBatch sb, Rectangle book, Rectangle spine, Rectangle bottomMargin, float time, float blend) { }
    }
}
