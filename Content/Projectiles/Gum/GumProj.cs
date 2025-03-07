using CalamityEntropy.Common;
using CalamityEntropy.Util;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Gum
{
    public class GumProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Default;
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = 2;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }
        public override bool? CanHitNPC(NPC target)
        {
            if (hited)
            {
                return false;
            }
            return null;
        }
        public override bool CanHitPlayer(Player target)
        {
            return !hited;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            hited = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hited = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            hited = true;
            return false;
        }
        public int frameType = 0;
        public Color color;
        public override void OnSpawn(IEntitySource source)
        {
            frameType = Main.rand.Next(0, 4);
            List<int> c = new List<int>() { 200, 200, 200 };
            for(int i = 0; i < Main.rand.Next(1, 3); i++)
            {
                c[Main.rand.Next(0, 3)] = 0;
            }
            Projectile.scale *= 1 + (float)Main.rand.Next(-20, 30) * 0.01f;
            color = new Color(c[0], c[1], c[2]);
            Projectile.velocity = Projectile.velocity.RotatedByRandom(0.16f);
        }
        float grv = 0;
        public override void AI(){
            if (hited)
            {
                Projectile.velocity *= 0;
                frameCounter--;
                if (frameCounter == 0)
                {
                    frameCounter = 3;
                    hitTex++;
                    if (hitTex > 3)
                    {
                        Projectile.Kill();
                    }
                }
            }
            else
            {
                if(grv < 0.8f)
                {
                    grv += 0.1f;
                }
                Projectile.rotation += 0.1f * (Projectile.velocity.X > 0 ? 1 : -1);
                Projectile.velocity.Y += grv;
            }
        }
       
        public bool hited = false;
        public int hitTex = 0;
        public int frameCounter = 3;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Gum/Gum" + frameType.ToString()).Value;
            if (hited)
            {
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Gum/E" + hitTex.ToString()).Value;

            }
            Color tileC = Lighting.GetColor((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16));
            Color cr = new Color(color.R * tileC.R / 255, color.G * tileC.G / 255, color.B * tileC.B / 255, 255); ;
             
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D pt = tx;
            Vector2 lu = new Vector2(-1, -1) * pt.Width / 2 * Projectile.scale * 1f;
            Vector2 ru = new Vector2(1, -1) * pt.Width / 2 * Projectile.scale * 1f;
            Vector2 ld = new Vector2(-1, 1) * pt.Width / 2 * Projectile.scale * 1f;
            Vector2 rd = new Vector2(1, 1) * pt.Width / 2 * Projectile.scale * 1f;
            lu = lu.RotatedBy(Projectile.rotation - Projectile.velocity.ToRotation());
            ru = ru.RotatedBy(Projectile.rotation - Projectile.velocity.ToRotation());
            ld = ld.RotatedBy(Projectile.rotation - Projectile.velocity.ToRotation());
            rd = rd.RotatedBy(Projectile.rotation - Projectile.velocity.ToRotation());

            float decayFactor = 0.028f;

            float yo = 1.0f / (1.0f + decayFactor * Projectile.velocity.Length());
            lu.Y *= yo;
            ru.Y *= yo;
            ld.Y *= yo;
            rd.Y *= yo;
            lu.X /= yo;
            ru.X /= yo;
            ld.X /= yo;
            rd.X /= yo;
            lu = lu.RotatedBy(Projectile.velocity.ToRotation());
            ru = ru.RotatedBy(Projectile.velocity.ToRotation());
            ld = ld.RotatedBy(Projectile.velocity.ToRotation());
            rd = rd.RotatedBy(Projectile.velocity.ToRotation());
            Util.Util.drawTextureToPoint(Main.spriteBatch, pt, cr, Projectile.Center + lu - Main.screenPosition, Projectile.Center + ru - Main.screenPosition, Projectile.Center + ld - Main.screenPosition, Projectile.Center + rd - Main.screenPosition);

            return false;
        }
    }
    

}