using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class SunriseSkyScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;

        public override bool IsSceneEffectActive(Player player) => Main.LocalPlayer.Entropy().SunriseScene > 0;
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/Cliff");

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityEntropy:SunriseSky", isActive);
        }
    }
    public class SunriseSky : CustomSky
    {
        private bool skyActive;
        private float opacity;
        public override void Deactivate(params object[] args)
        {
            skyActive = ModContent.GetInstance<SunriseSkyScene>().IsSceneEffectActive(Main.LocalPlayer);
        }

        public override void Reset()
        {
            skyActive = false;
        }

        public override bool IsActive()
        {
            return skyActive || opacity > 0f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            skyActive = true;
        }

        public override Color OnTileColor(Color inColor)
        {
            return Color.Lerp(inColor, Color.Lerp(new Color(25, 50, 50, 255), fColor, sunPos * 0.5f + 0.5f), opacity);
        }
        public static Color fColor = new Color(255, 240, 80);
        public static Texture2D Background;
        public static Texture2D Cliffs;
        public static Texture2D CloudsBack1;
        public static Texture2D CloudsBack2;
        public static Texture2D CloudsFore;
        public static Texture2D CloudsMid;
        public static List<Texture2D> Fields;
        public static Texture2D Sun;
        public override void OnLoad()
        {
            if(!Main.dedServ)
            {
                string path = "CalamityEntropy/Assets/Sunrise/";
                Background = CEUtils.RequestTex(path + "Background");
                Cliffs = CEUtils.RequestTex(path + "Cliffs");
                CloudsBack1 = CEUtils.RequestTex(path + "CloudsBack1");
                CloudsBack2 = CEUtils.RequestTex(path + "CloudsBack2");
                CloudsFore = CEUtils.RequestTex(path + "CloudsFore");
                CloudsMid = CEUtils.RequestTex(path + "CloudsMid");
                Sun = CEUtils.RequestTex(path + "Sun");
                Fields = new List<Texture2D>();
                for (int i = 0; i < 8; i++)
                    Fields.Add(CEUtils.RequestTex(path + "Field" + i));
            }
        }
        public float sunPos = 0;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            float time = Main.GameUpdateCount;
            sunPos = 0;
            if(Main.dayTime)
            {
                sunPos = (float)(Math.Cos(Main.time / 54000.0 * MathHelper.TwoPi - MathHelper.Pi) * 0.5 + 0.5);
            }
            else
            {
                sunPos = -(float)(Math.Cos(Main.time / 32400.0 * MathHelper.TwoPi - MathHelper.Pi) * 0.5 + 0.5);
            }
            Color lColor = Color.Lerp(new Color(54, 50, 50, 255), Color.White, sunPos * 0.5f + 0.5f);
            Vector2 screenSize = Main.ScreenSize.ToVector2();
            Rectangle fullscreen = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            float worldHeight = Main.maxTilesY * 16;
            float bgOffset = Utils.Remap(Main.screenPosition.Y / 10000f, worldHeight * 0.3f, worldHeight * 0.42f, 0, 280);
            float xOffset = Main.screenPosition.X;
            Main.spriteBatch.UseSampleState(SamplerState.PointWrap);
            float offsetn = xOffset * 0.005f + time * 0.05f;
            float scale = Main.screenWidth / Background.Width * 1.35f;
            Main.spriteBatch.Draw(CEUtils.pixelTex, fullscreen, Color.Lerp(new Color(52, 20, 12, 255), new Color(254, 224, 79), sunPos * 0.5f + 0.5f) * opacity);

            Main.spriteBatch.Draw(CloudsBack2, Vector2.UnitY * -60 * scale, new Rectangle((int)offsetn, 0, CloudsBack2.Width, CloudsBack2.Height), lColor * opacity, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            offsetn = xOffset * 0.006f + time * 0.1f;
            Main.spriteBatch.Draw(CloudsBack1, Vector2.Zero * -40 * scale, new Rectangle((int)offsetn, 0, CloudsBack1.Width, CloudsBack1.Height), lColor * opacity, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            offsetn = xOffset * 0.007f + time * 0.25f;
            Main.spriteBatch.Draw(CloudsMid, new Vector2(0, 15 * scale), new Rectangle((int)offsetn, 0, CloudsMid.Width, CloudsMid.Height), lColor * opacity, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            offsetn = xOffset * 0.008f + time * 0.4f;
            Main.spriteBatch.Draw(CloudsFore, Vector2.Zero, new Rectangle((int)offsetn, 0, CloudsFore.Width, CloudsFore.Height), lColor * opacity, 0, Vector2.Zero, scale, SpriteEffects.None, 0);

            offsetn = 6;
            Main.spriteBatch.Draw(Cliffs, new Vector2(0, 60 * scale), new Rectangle((int)offsetn, 0, (int)(Main.screenWidth / scale) + 1, Cliffs.Height), lColor * opacity, 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(Sun, new Vector2(Main.screenWidth / 2, (100 - sunPos * 20) * scale), null, Color.White * opacity, 0, Sun.Size().Half(), scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(CEUtils.pixelTex, new Vector2(0, 97 * scale), fullscreen, Color.Lerp(new Color(40, 32, 16, 255), new Color(247, 210, 43), sunPos * 0.5f + 0.5f) * opacity);
            for(int i = 0; i < 8; i++)
            {
                offsetn = xOffset * ((i / 7f) * (i / 7f) * (i / 7f) * (i / 7f) * (i / 7f) * (i / 7f) * 0.04f);
                float yset = (float)Math.Pow(float.Max(0, worldHeight * 0.205f - Main.screenPosition.Y) * ((i / 7f) * (i / 7f) * (i / 7f) * (i / 7f) * (i / 7f) * (i / 7f) * 0.6f), 0.52f);
                Main.spriteBatch.Draw(Fields[i], Vector2.UnitY * (98 + yset) * scale, new Rectangle((int)offsetn, 0, fullscreen.Width, Fields[i].Height), lColor * opacity * (0.3f + 0.7f * (i / 7f)), 0, Vector2.Zero, scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.ExitShaderRegion();
        }

        public override float GetCloudAlpha()
        {
            return 1f - opacity;
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;

            Opacity = opacity;
        }
    }
}
