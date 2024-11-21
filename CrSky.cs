using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using System.Reflection;
using CalamityEntropy.Util;
using System.Xml.Linq;
using System;
using CalamityEntropy.NPCs.AbyssalWraith;

namespace CalamityEntropy
{
    public class CrSky : CustomSky
    {
        private bool skyActive;
        private float opacity;
        
        

        public override void Deactivate(params object[] args)
        {
            skyActive = Main.LocalPlayer.Entropy().crSky > 0;
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
            return Color.Lerp(inColor, new Color(255, 255, 255, inColor.A), opacity);
        }
        public int counter = 0;
        public int awtime = 0;
        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            
            counter++;
            Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Extra/CrSky").Value;
            //float pc = 1 + ((float)(Math.Cos(counter * 0.01f))) * 0.12f;
            float pc = 1f;
            Color ocolor = new Color((int)(12 * pc), (int)(65 * pc), (int)(100 * pc));
            bool drawAWMask = false;
            int AWIndex = -1;
            if (NPC.AnyNPCs(ModContent.NPCType<AbyssalWraith>()))
            {
                awtime = 180;
                ocolor = new Color((int)(220 * pc), (int)(65 * pc), (int)(255 * pc));
                foreach (NPC n in Main.npc)
                {
                    if (n.active && n.type == ModContent.NPCType<AbyssalWraith>())
                    {
                        drawAWMask = true;
                        AWIndex = n.whoAmI;
                        break;

                    }
                }
            }
            awtime--;
            if (awtime > 0)
            {
                ocolor = new Color((int)(220 * pc), (int)(65 * pc), (int)(255 * pc));
            }

            Vector2 dp = new Vector2((Main.screenPosition.X * -0.5f + counter * 0.3f) % txd.Width, (Main.screenPosition.Y * -0.5f + counter * -0.1f) % txd.Height);
            spriteBatch.Draw(txd, dp + new Vector2(0, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp - txd.Size(), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(0, -txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(txd.Width, -txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(-txd.Width, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(txd.Width, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(-txd.Width, txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(0, txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + txd.Size(), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            float c = 1f;
            ocolor = new Color((int)(12 * pc), (int)(65 * pc), (int)(100 * pc));
            dp = new Vector2((Main.screenPosition.X * -0.5f * c + counter * -0.3f * c) % txd.Width, (Main.screenPosition.Y * -0.5f * c + counter * 0.1f * c) % txd.Height);
            spriteBatch.Draw(txd, dp + new Vector2(0, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp - txd.Size(), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(0, -txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(txd.Width, -txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(-txd.Width, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(txd.Width, 0), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(-txd.Width, txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + new Vector2(0, txd.Height), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(txd, dp + txd.Size(), null, ocolor * opacity, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            if (drawAWMask)
            {
                Texture2D m = ModContent.Request<Texture2D>("CalamityEntropy/Extra/lightball").Value;
                float size = ((AbyssalWraith)AWIndex.ToNPC().ModNPC).anmlerp;

                spriteBatch.Draw(m, AWIndex.ToNPC().Center - Main.screenPosition, null, Color.Purple, 0, m.Size() / 2, 16 * size, SpriteEffects.None, 0);
            }
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            
        }

        public override void Update(GameTime gameTime)
        {
            if (Main.LocalPlayer.Entropy().crSky <= 0 || Main.gameMenu)
                skyActive = false;

            if (skyActive && opacity < 1f)
                opacity += 0.02f;
            else if (!skyActive && opacity > 0f)
                opacity -= 0.02f;

            Opacity = opacity;
        }

        public override float GetCloudAlpha()
        {
            return (1f - opacity) * 0.97f + 0.03f;
        }
    }
}
