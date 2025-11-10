using CalamityEntropy.Content.Menu;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Skies
{
    public class VoidVortexScene : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

        public override bool IsSceneEffectActive(Player player) => Main.LocalPlayer.Entropy().VortexSky > 0;

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityEntropy:VoidVortex", isActive);
        }
    }
    public class VoidVortexSky : CustomSky
    {
        private bool skyActive;
        private float opacity;
        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Entropy().VortexSky > 0;
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
        public int counter;
        public static List<MenuParticle> particles = new List<MenuParticle>();
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Texture2D l1 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/menu/VoidVortex").Value;
            Texture2D pixel = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
            var drawColor = Color.White;
            
            spriteBatch.Draw(pixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), new Color(1, 2, 32) * opacity);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
            spriteBatch.Draw(l1, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * opacity * 0.88f, MathHelper.ToRadians(counter * 0.1f), l1.Size() / 2, 1f, SpriteEffects.None, 0);
            spriteBatch.Draw(l1, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * opacity * 0.9f, MathHelper.ToRadians(counter * 0.2f), l1.Size() / 2, 0.8f, SpriteEffects.None, 0);
            spriteBatch.Draw(l1, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * opacity * 0.92f, MathHelper.ToRadians(counter * 0.3f), l1.Size() / 2, 0.5f, SpriteEffects.None, 0);
            spriteBatch.Draw(l1, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * opacity * 0.94f, MathHelper.ToRadians(counter * 0.5f), l1.Size() / 2, 0.3f, SpriteEffects.None, 0);
            spriteBatch.Draw(l1, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * opacity * 0.96f, MathHelper.ToRadians(counter * 0.7f), l1.Size() / 2, 0.2f, SpriteEffects.None, 0);
            spriteBatch.Draw(l1, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * opacity * 0.98f, MathHelper.ToRadians(counter * 1f), l1.Size() / 2, 0.15f, SpriteEffects.None, 0);
            spriteBatch.Draw(l1, new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), null, Color.White * opacity * 1f, MathHelper.ToRadians(counter * 2f), l1.Size() / 2, 0.1f, SpriteEffects.None, 0);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
            Texture2D mask = CEUtils.getExtraTex("menumask");
            //spriteBatch.Draw(mask, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * 0.6f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);

            foreach (MenuParticle p in particles)
            {
                p.draw(opacity);
            }
            
            spriteBatch.End();spriteBatch.begin_();
        }
        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.Entropy().VortexSky <= 0 || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.005f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.01f;
            counter++;
            foreach (MenuParticle p in particles)
            {
                p.update();
            }
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].timeleft <= 0)
                {
                    particles.RemoveAt(i);
                }
            }
            if (counter % 15 == 0)
            {
                MenuParticle particle = new MenuParticle(new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), new Vector2(Main.screenWidth / 2, Main.screenHeight / 2), CEUtils.randomRot().ToRotationVector2() * 1, new Vector2(1.5f, 1), 660);
                particles.Add(particle);
                particle.pos += particle.velocity * 2;
            }
            Opacity = opacity;
        }
        public override float GetCloudAlpha()
        {
            return (1f - opacity);
        }
    }
}
