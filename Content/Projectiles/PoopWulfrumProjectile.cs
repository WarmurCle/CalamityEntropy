using System;
using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class PoopWulfrumProjectile : PoopProj
    {
        public int shield = 100;
        public static int shieldDistance = 200;
        public int shieldMax = 100;
        public int shieldCd = 0;
        public int lastTickShield = 100;
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(shield);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            shield = reader.ReadInt32();
        }
        public override void AI()
        {
            base.AI();
            if (shield > 0)
            {
                shieldCd = 10 * 60;
                if (Projectile.timeLeft % 16 == 0)
                {
                    shield += 1;
                }
                if(opc < 1)
                {
                    opc += 0.05f;
                }
            }
            else
            {
                shield = 0;
                shieldCd--;
                if (shieldCd <= 0)
                {
                    shield = shieldMax;
                }
                if(opc > 0)
                {
                    opc -= 0.05f;
                }
            }
            if(shield < lastTickShield)
            {
                SoundEngine.PlaySound(new("CalamityMod/Sounds/Custom/RoverDriveHit") { PitchVariance = 0.6f, Volume = 0.6f, MaxInstances = 0 }, Projectile.Center);
            }
            lastTickShield = shield;
        }
        public override bool BreakWhenHitNPC => false;
        float opc = 1;
        public override void PostDraw(Color lightColor)
        {
            if(shield <= 0)
            {
                return;
            }
            SpriteBatch spriteBatch = Main.spriteBatch;
            bool shieldExists = shield > 0;


            // The shield very gently grows and shrinks
            float scale = 0.8f * opc;


            // If in vanity, the shield is always projected as if it's at full strength.
            float shieldStrength = (0.1f + 0.5f * ((float)shield / 100f)) * opc;


            // Noise scale also grows and shrinks, although out of sync with the shield
            float noiseScale = MathHelper.Lerp(0.4f, 0.8f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 0.3f) * 0.5f + 0.5f);

            // Define shader parameters
            Effect shieldEffect = Filters.Scene["CalamityMod:RoverDriveShield"].GetShader().Shader;
            shieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 0.24f);
            shieldEffect.Parameters["blowUpPower"].SetValue(2.5f);
            shieldEffect.Parameters["blowUpSize"].SetValue(0.5f);
            shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

            // Shield opacity multiplier slightly changes, this is independent of current shield strength
            float baseShieldOpacity = (0.2f + 0.8f * ((float)shield / 100f)) * opc;
            shieldEffect.Parameters["shieldOpacity"].SetValue(baseShieldOpacity * (0.5f + 0.5f * shieldStrength));
            shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(2f);

            // Get the shield color.
            Color blueTint = new Color(51, 102, 255);
            Color cyanTint = new Color(71, 202, 255);
            Color wulfGreen = new Color(194, 255, 67) * 0.8f;
            Color edgeColor = CalamityUtils.MulticolorLerp(Main.GlobalTimeWrappedHourly * 0.2f, blueTint, cyanTint, wulfGreen);
            Color shieldColor = blueTint;


            // Define shader parameters for shield color
            shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
            shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

            // GOD I LOVE END BEGIN CAN THIS GAME PLEASE BE SWALLOWED BY THE FIRES OF HELL THANKS
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

            // Fetch shield noise overlay texture (this is the techy overlay fed to the shader)
            NoiseTex ??= ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/TechyNoise");
            Vector2 pos = Projectile.Center - Main.screenPosition;
            Texture2D tex = NoiseTex.Value;
            spriteBatch.Draw(tex, pos, null, Color.White, 0, tex.Size() / 2f, scale, 0, 0);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.Transform);
        
        }
        public static Asset<Texture2D> NoiseTex;
        public override int dustType => DustID.Poop;
    }

}