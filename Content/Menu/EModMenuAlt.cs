using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Menu
{
    public class EModMenuAlt : ModMenu
    {
        public int counter = 0;
        public override Asset<Texture2D> SunTexture => ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white");
        public override Asset<Texture2D> MoonTexture => ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white");
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/startmenu");
        public override Asset<Texture2D> Logo => ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Logo");
        public override string DisplayName => "Calamity Entropy - The Church";
        public override bool PreDrawLogo(SpriteBatch spriteBatch, ref Vector2 logoDrawCenter, ref float logoRotation, ref float logoScale, ref Color drawColor)
        {
            Main.time = 27000;
            Main.dayTime = true;
            Texture2D l1 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/menu/alt").Value;
            Texture2D pixel = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
            drawColor = Color.White;
            counter++;
            logoScale = 1;
            logoRotation = 0;
            logoDrawCenter += new Vector2(36, (float)Math.Cos(counter * 0.008f) * 4 + 30);
            spriteBatch.Draw(pixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(1, 2, 32));

            spriteBatch.Draw(l1, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White, 0, l1.Size() / 2, Main.screenWidth / 2560f, SpriteEffects.None, 0);

            Texture2D logo = Logo.Value;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            for (int i = 1; i < 10; i++)
            {
                float rot = 0;
                for (int j = 0; j < 8; j++)
                {
                    spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Logool").Value, logoDrawCenter + rot.ToRotationVector2() * ((float)i * 0.5f), null, Color.LightBlue * 0.15f, logoRotation, logo.Size() / 2, logoScale, SpriteEffects.None, 0);
                    rot += MathHelper.ToRadians(45);
                }
            }


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            spriteBatch.Draw(logo, logoDrawCenter, null, Color.White, logoRotation, logo.Size() / 2, logoScale, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            for (int i = 1; i < 10; i++)
            {
                float rot = 0;
                for (int j = 0; j < 8; j++)
                {
                    spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Logool").Value, logoDrawCenter + rot.ToRotationVector2() * ((float)i * 0.5f), null, Color.LightBlue * 0.15f, logoRotation, logo.Size() / 2, logoScale, SpriteEffects.None, 0);
                    rot += MathHelper.ToRadians(45);
                }
            }


            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);

            spriteBatch.Draw(logo, logoDrawCenter, null, Color.White, logoRotation, logo.Size() / 2, logoScale, SpriteEffects.None, 0);

            return false;
        }

        public override bool IsAvailable => true;
        public override ModSurfaceBackgroundStyle MenuBackgroundStyle => ModContent.GetInstance<MenuBack>();

    }
}
