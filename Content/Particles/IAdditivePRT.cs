using Microsoft.Xna.Framework.Graphics;

namespace CalamityEntropy.Content.Particles
{
    /// <summary>
    /// 不走 PRT 常规分桶的 Additive 绘制契约；CEPixelScreen 攒批后单独开 Additive 批次调 Draw
    /// 给 PreDraw 里自己管 SpriteBatch、跟 PRT 桶状态对不上的粒子留的钩子,目前无实现类
    /// </summary>
    internal interface IAdditivePRT
    {
        public void Draw(SpriteBatch spriteBatch);
    }

    /// <summary>
    /// PixelPassActive 为假时不画,别加回退绘制
    /// 画在 CEPixelScreen 的 Screen2 上,之后才过 Pixel shader
    /// </summary>
    internal interface IPixelPassPRT
    {
        bool PixelPass { get; set; }
        void DrawPixelPass(SpriteBatch sb);
    }
}

