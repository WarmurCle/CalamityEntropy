using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using CalamityEntropy.Projectiles;
using Terraria.GameContent;
using CalamityEntropy.Util;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Systems;
using CalamityMod.Projectiles.Melee;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace CalamityEntropy.Projectiles
{
    public class PoTPhantom : ModProjectile
    {
        float sr = 0;
        float j = 0.01f;
        List<float> odr = new List<float>();
        List<float> ods = new List<float>();
        public int noSlowTime = 0;
        public float timej = 1f;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.ArmorPenetration = 80;
            Projectile.timeLeft = 3;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(rotSpeed);
            writer.Write(live);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            rotSpeed = reader.ReadSingle();
            live = reader.ReadBoolean();
        }
        public float scaleD = 1f;
        public float rotSpeed = 0f;
        public float scale2 = 1.2f;
        public bool live = true;
        public override void AI(){
            soundCd--;
            Player owner = Projectile.owner.ToPlayer();
            Projectile.netUpdate = true;
            if (Projectile.owner == Main.myPlayer)
            {
                if (!owner.channel || !(owner.Entropy().pot_amp >= 10))
                {
                    live = false;
                    return;
                }
                else
                {
                    live = true;
                    Projectile.timeLeft = 3;
                }
            }
            else
            {
                if (live)
                {
                    Projectile.timeLeft = 3;
                }
                else
                {
                    return;
                }
            }
            
            scaleD = ((PoTProj)((int)Projectile.ai[0]).ToProj().ModProjectile).scaleD;
            rotSpeed = ((PoTProj)((int)Projectile.ai[0]).ToProj().ModProjectile).rotSpeed;
            Projectile.rotation = ((int)Projectile.ai[0]).ToProj().rotation;
            Projectile.velocity = ((int)Projectile.ai[0]).ToProj().velocity;
            Projectile.Center = owner.MountedCenter - new Vector2(owner.direction * 40, 90);
            /*if (((PoTProj)((int)Projectile.ai[0]).ToProj().ModProjectile).ods.Count <= 1)
            {
                ods.Clear();
                odr.Clear();
            }

            if (rotSpeed != 0)
            {
                float ds;
                if (ods.Count > 0)
                {
                    ds = ods[ods.Count - 1];
                }
                else
                {
                    ds = scaleD * scale2;
                }
                odr.Add(Projectile.rotation - rotSpeed * 0.8f);
                ods.Add(MathHelper.Lerp(ds, scaleD * scale2, 0.2f));
                if (odr.Count > 17)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
                odr.Add(Projectile.rotation - rotSpeed * 0.6f);
                ods.Add(MathHelper.Lerp(ds, scaleD * scale2, 0.4f));
                if (odr.Count > 17)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
                odr.Add(Projectile.rotation - rotSpeed * 0.4f);
                ods.Add(MathHelper.Lerp(ds, scaleD * scale2, 0.6f));
                if (odr.Count > 17)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
                odr.Add(Projectile.rotation - rotSpeed * 0.2f);
                ods.Add(MathHelper.Lerp(ds, scaleD * scale2, 0.8f));
                if (odr.Count > 17)
                {
                    odr.RemoveAt(0);
                    ods.RemoveAt(0);
                }
            }
            odr.Add(Projectile.rotation);
            ods.Add(scaleD * scale2);
            if (odr.Count > 17)
            {
                odr.RemoveAt(0);
                ods.RemoveAt(0);
            }*/
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            
            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/PoTYharimPhantom").Value;

            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, 0, tx.Size() / 2, Projectile.scale * 3, (Projectile.velocity.X >= 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally), 0);


            /*SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            Player player = Main.player[Projectile.owner];
            
            Texture2D tail = ModContent.Request<Texture2D>("CalamityEntropy/Extra/Extra_201").Value;  //SwordSlashTexture
            Effect se = GameShaders.Misc["CalamityEntropy:Dogma"].Shader;
            var r = Main.rand;
            se.Parameters["r1"].SetValue((float)r.Next(1, 100000 / 4));
            se.Parameters["r2"].SetValue((float)r.Next(1, 100000 / 4));
            se.Parameters["SpriteTexture"].SetValue(tail);
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            List<Vertex> ve = new List<Vertex>();
            for (int i = 0; i < odr.Count; i++)
            {
                Color b = Color.Lerp(new Color(24, 24, 20), new Color(180, 50, 60), (float)i / (float)odr.Count) * 0.8f;
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(280 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3(i / (float)odr.Count, 1, 1),
                      b));
                ve.Add(new Vertex(Projectile.Center - Main.screenPosition + (new Vector2(0 * ods[i] * Projectile.scale, 0).RotatedBy(odr[i])),
                      new Vector3(i / (float)odr.Count, 0, 1),
                      b));
            }
            
            if (ve.Count >= 3)//因为顶点需要围成一个三角形才能画出来 所以需要判顶点数>=3 否则报错
            {
                gd.Textures[0] = tail;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
            }

            //结束顶点绘制
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            */
            Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, Color.White * 0.8f, Projectile.rotation + (float)Math.PI * 0.25f, new Vector2(0, TextureAssets.Projectile[Projectile.type].Value.Height), Projectile.scale * 1.5f * scaleD * scale2, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }
        public int soundCd = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (soundCd <= 0)
            {
                SoundStyle s = new SoundStyle("CalamityEntropy/Sounds/swing4"); s.Pitch = 1 - timej;
                SoundEngine.PlaySound(s, Projectile.Center); odr.Clear();
                soundCd = 2;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (scaleD < 0.6f || rotSpeed == 0)
            {
                return false;
            }
            return Util.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 170 * Projectile.scale * scaleD * scale2, targetHitbox, 140) || Util.Util.LineThroughRect(Projectile.Center, Projectile.Center + (Projectile.rotation - rotSpeed * 0.5f).ToRotationVector2() * 300 * Projectile.scale * scaleD, targetHitbox, 100);
        }
    }

}