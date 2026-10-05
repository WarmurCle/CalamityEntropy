namespace CalamityEntropy.Content.Rarities
{
    /// <summary>辉绿,主色跟旧提示框字色 (80,255,80) 相同,拾取飘字跟主色对齐</summary>
    public sealed class GlowGreen : CEGlowRarity
    {
        public static readonly Color Green = new(80, 255, 80);

        protected override Color Glow => Green;
    }
}
