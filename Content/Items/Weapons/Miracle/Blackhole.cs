using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Miracle
{
    public class Blackhole : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 1000;
            Projectile.light = 1;
        }
        public void DrawRing(Vector2 position, Texture2D trail, Vector2 scaleOutside, Vector2 scaleInside, Color color, BlendState blend, bool? drawUpside = null)
        {
            List<ColoredVertex> ve = new List<ColoredVertex>();
            List<Vector2> points1 = new List<Vector2>();
            List<Vector2> points2 = new List<Vector2>();
            for(float i = 0; i <= 1; i += 0.01f)
            {
                Vector2 p = (i * MathHelper.TwoPi).ToRotationVector2() * scaleOutside;
                if (drawUpside.HasValue && ((drawUpside.Value && p.Y > 0.001) || (!drawUpside.Value && p.Y < 0)))
                    continue;
                if (drawUpside.HasValue && drawUpside.Value && i == 0)
                    continue;
                points1.Add(p);
                points2.Add((i * MathHelper.TwoPi).ToRotationVector2() * scaleInside);
            }
            for (int i = 0; i < points1.Count; i++)
            {
                ve.Add(new ColoredVertex(position + points1[i], color, new Vector3(i / 50f + Main.GlobalTimeWrappedHourly, 0, 1)));
                ve.Add(new ColoredVertex(position + points2[i], color, new Vector3(i / 50f + Main.GlobalTimeWrappedHourly, 1, 1)));
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, blend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            gd.Textures[0] = trail;
            gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            Main.spriteBatch.ExitShaderRegion();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D trail1 = CEUtils.getExtraTex("Streak2Trans");
            Texture2D trail2 = CEUtils.getExtraTex("MegaStreakBacking2b");
            float scale = 2f;
            float dist = 60;
            float width = 260;
            float hmul = 1.9f;
            float wm = 0.82f;
            Texture2D circle = CEUtils.getExtraTex("VoidMask");
            Vector2 sq = new Vector2(1, (float)Math.Pow(Math.Abs(Projectile.Center.Y - Main.LocalPlayer.Center.Y) * 0.00016f, 0.5f) * ((Projectile.Center.Y - Main.LocalPlayer.Center.Y) > 0 ? 1 : -1));
            CEUtils.DrawGlow(Projectile.Center, Color.White * 0.8f, 6 * scale);
            float num = 2.4f;
            DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(dist + width * wm, dist + width * wm) * scale * sq, new Vector2(dist, dist) * scale * sq, new Color(225, 225, 160, 255), BlendState.Additive, sq.Y > 0 ? true : false);
            DrawRing(Projectile.Center - Main.screenPosition, CEUtils.pixelTex, new Vector2(dist + width * 0.46f, dist + width * 0.46f) / hmul * scale, Vector2.Zero, Color.Black, BlendState.NonPremultiplied);
            DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(dist + width, dist + width) / hmul * scale, new Vector2(dist, dist) * scale / hmul, new Color(255, 255, 200), BlendState.Additive);
            DrawRing(Projectile.Center - Main.screenPosition, trail2, new Vector2(dist * num + width * 0.08f, dist * num + width * 0.08f) / hmul * scale, new Vector2(dist * num, dist * num) * scale / hmul, new Color(255, 255, 140) * 0.5f, BlendState.Additive);
            DrawRing(Projectile.Center - Main.screenPosition, trail1, new Vector2(dist + width * wm, dist + width * wm) * scale * sq, new Vector2(dist, dist) * scale * sq, new Color(225, 225, 160, 255), BlendState.Additive, sq.Y <= 0 ? true : false);
            return false;
        }
    }

}
