using CalamityEntropy.Common;
using CalamityEntropy.Content.NPCs.AbyssalWraith;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class SnowgraveScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => Main.LocalPlayer.Entropy().snowgrave > 0;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityEntropy:Snowgrave", isActive);
        }
    }
    public class SnowgraveSky : CustomSky
    {
        private bool skyActive;
        private float opacity;
        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Entropy().snowgrave > 0;
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
            return Color.Lerp(inColor, new Color(230, 230, 255, inColor.A), opacity);
        }
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            spriteBatch.End();
            Texture2D tex = CEUtils.getExtraTex("WhiteFade");
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null);

            Color c1 = new Color(180, 200, 255, (int)(255 * opacity * 0.9f));
            Color c2 = new Color(50, 50, 255, (int)(255 * opacity * 0.5f));

            spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), null, c2, 0, Vector2.Zero, SpriteEffects.FlipVertically, 0);

            spriteBatch.Draw(tex, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), c1);
            
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null);


        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.Entropy().snowgrave <= 0 || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.025f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.025f;

            Opacity = opacity;
        }

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.5f + 0.5f;
        }
    }
}
