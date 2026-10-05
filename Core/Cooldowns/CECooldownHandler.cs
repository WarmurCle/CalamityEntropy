using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Core.Cooldowns
{
    /// <summary>形状对齐原 CooldownHandler</summary>
    public abstract class CECooldownHandler : ModType
    {
        /// <summary>子类 static new 覆盖,null 时用类型全名</summary>
        public static string ID => null;

        public CECooldownInstance instance;

        protected sealed override void Register() {
            ModTypeLookup<CECooldownHandler>.Register(this);
        }

        #region 玩法行为
        /// <summary>计时不减也调</summary>
        public virtual void Tick() { }

        /// <summary>自然走完才调,死亡清除不调</summary>
        public virtual void OnCompleted() { }

        public virtual bool CanTickDown => true;

        public virtual bool PersistsThroughDeath => false;

        public virtual bool SavedWithPlayer => true;

        /// <summary>null 无声</summary>
        public virtual SoundStyle? EndSound => null;

        public virtual bool ShouldPlayEndSound => true;
        #endregion

        #region 显示
        public virtual LocalizedText DisplayName => LocalizedText.Empty;

        public virtual bool ShouldDisplay => true;

        /// <summary>20x20</summary>
        public virtual string Texture => "";

        /// <summary>没有专门贴图时 DrawCompact 用图标本体</summary>
        public virtual string OverlayTexture => $"{Texture}Overlay";

        public virtual string OutlineTexture => $"{Texture}Outline";

        public virtual Color OutlineColor => Color.White;

        public virtual Color CooldownStartColor => Color.Gray;

        public virtual Color CooldownEndColor => Color.White;

        public virtual void DrawExpanded(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale) {
            Texture2D sprite = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D outline = ModContent.Request<Texture2D>(OutlineTexture).Value;

            DrawProgressRing(spriteBatch, position, opacity, scale);
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }

        public virtual void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale) {
            Texture2D sprite = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D outline = ModContent.Request<Texture2D>(OutlineTexture).Value;

            //没有 Overlay 就用图标本体裁
            Texture2D overlay = ModContent.RequestIfExists<Texture2D>(OverlayTexture, out var overlayAsset) ? overlayAsset.Value : sprite;

            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            int lostHeight = (int)Math.Ceiling(overlay.Height * (1 - instance.Completion));
            Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
            spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }

        /// <summary>已恢复部分从顶端顺时针点亮,替代原 CircularBarShader</summary>
        public virtual void DrawProgressRing(SpriteBatch spriteBatch, Vector2 center, float opacity, float scale) {
            Texture2D px = TextureAssets.MagicPixel.Value;
            Rectangle src = new Rectangle(0, 0, 1, 1);
            const int segments = 48;
            float radius = 19f * scale;
            float thickness = 5f * scale;
            float segLength = MathHelper.TwoPi * radius / segments + 1f;
            float recovered = 1f - instance.Completion;

            for (int i = 0; i < segments; i++) {
                float f = (i + 0.5f) / segments;
                float angle = MathHelper.TwoPi * f - MathHelper.PiOver2;
                Color color = f <= recovered
                    ? Color.Lerp(CooldownStartColor, CooldownEndColor, f)
                    : new Color(24, 24, 24);
                spriteBatch.Draw(px, center + angle.ToRotationVector2() * radius, src, color * opacity,
                    angle + MathHelper.PiOver2, new Vector2(0.5f), new Vector2(segLength, thickness), SpriteEffects.None, 0f);
            }
        }

        /// <summary>替代原 CalamityUtils.DrawBorderStringEightWay</summary>
        public static void DrawBorderStringEightWay(SpriteBatch spriteBatch, DynamicSpriteFont font, string text, Vector2 baseDrawPosition, Color main, Color border, float scale = 1f) {
            for (int x = -1; x <= 1; x++) {
                for (int y = -1; y <= 1; y++) {
                    if (x == 0 && y == 0)
                        continue;
                    spriteBatch.DrawString(font, text, baseDrawPosition + new Vector2(x, y) * scale, border, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
                }
            }
            spriteBatch.DrawString(font, text, baseDrawPosition, main, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        }
        #endregion
    }
}
