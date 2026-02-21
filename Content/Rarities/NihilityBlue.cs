using CalamityMod;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace CalamityEntropy.Content.Rarities
{
    public class NihilityBlue : ModRarity
    {
        public override Color RarityColor => Color.Blue;

        public override int GetPrefixedRarity(int offset, float valueMult) => Type;
        public static void Draw(Item Item, SpriteBatch spriteBatch, string text, int X, int Y, float rotation,
            Vector2 baseScale, float time, DynamicSpriteFont font)
        {
            Texture2D particle = CEUtils.getExtraTex("NBParticle");
            for(float r = 0; r < 360; r += 120)
            {
                Vector2 addVec = MathHelper.ToRadians(r).ToRotationVector2() * ((float)(0.5f + Math.Sin(time * 8) * 0.5f) * 3 + 1);
                spriteBatch.DrawString(font, text, new Vector2(X, Y) + addVec, new Color(200, 160, 255));
            }
            spriteBatch.DrawString(font, text, new Vector2(X, Y), new Color(120, 30, 235));
            spriteBatch.UseBlendState_UI(BlendState.Additive);
            UnifiedRandom rand = new UnifiedRandom((X + Y).GetHashCode());
            
            int particleCount = 9;
            for (int i = 0; i < particleCount; i++)
            {
                Vector2 vec = new Vector2(rand.NextFloat(), rand.NextFloat());
                vec.X = CEUtils.Frac(vec.X - time * 1.2f);
                float alpha = 1;
                if (vec.X < 0.2f)
                    alpha = vec.X / 0.2f;
                if(vec.X > 0.8f)
                    alpha = (1 - vec.X) / 0.2f;
                Color clr = Color.Lerp(TextColor1, TextColor2, CEUtils.Parabola(vec.X, 1));
                Vector2 adjPos = vec * new Vector2(font.MeasureString(text).X, MaxY);
                spriteBatch.Draw(particle, adjPos, null, clr, 0, Vector2.Zero, baseScale, SpriteEffects.None, 0);
            }
            spriteBatch.UseBlendState_UI(BlendState.AlphaBlend);
        }
        public static float MaxY = 4.5f;

        public static Color TextColor1 = new Color(60, 5, 255);
        public static Color TextColor2 = new Color(245, 200, 255);
        public static void Draw(Item Item, DrawableTooltipLine line)
        {
            Draw(Item, Main.spriteBatch, line.Text, line.X, line.Y, 0, line.BaseScale, Main.GlobalTimeWrappedHourly, FontAssets.MouseText.Value)
        }
    }
}
