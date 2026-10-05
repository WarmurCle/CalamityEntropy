namespace CalamityEntropy.Content.Rarities
{
    /// <summary>天蓝,主色跟旧提示框字色 (84,84,255) 相同,拾取飘字跟主色对齐</summary>
    public sealed class SkyBlue : CEGlowRarity
    {
        public static readonly Color Blue = new(84, 84, 255);

        protected override Color Glow => Blue;
    }
}
