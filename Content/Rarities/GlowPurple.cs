namespace CalamityEntropy.Content.Rarities
{
    /// <summary>辉紫,主色跟旧提示框字色 (160,80,230) 相同,拾取飘字跟主色对齐</summary>
    public sealed class GlowPurple : CEGlowRarity
    {
        public static readonly Color Purple = new(160, 80, 230);

        protected override Color Glow => Purple;
    }
}
