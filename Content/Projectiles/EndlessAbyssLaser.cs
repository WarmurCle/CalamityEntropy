
using System.Collections.Generic;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    
    public class EndlessAbyssLaser : ModProjectile
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 20, 1, 600, 20);
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 6000;
        }
        public int counter = 0;
        List<Vector2> p = new List<Vector2>();
        List<Vector2> l = new List<Vector2>();
        public int length = 6000;
        NPC ownern = null;
        public float width = 0;
        public int aicounter = 0;
        public override void SetDefaults()
        {
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 12;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            
        }
        public bool st = true;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public LoopSound sound = null;
        public LoopSound sound2 = null;
        public override void AI(){
            if (st)
            {
                SoundEffect sf = CalamityEntropy.ealaserSound;
                SoundEffect sf2 = CalamityEntropy.ealaserSound2;
                sound = new LoopSound(sf);
                sound.play();
                sound2 = new LoopSound(sf2);
                sound2.play();
                st = false;
                for (int ii = 0; ii < 100; ii++)
                {
                    counter++;
                    var rand = Main.rand;
                    int tspeed = 46;
                    if (counter % 1 == 0)
                    {
                        p.Add(new Vector2(0, rand.Next(0, 41) - 20));
                    }
                    if (counter % 6 == 0)
                    {
                        l.Add(new Vector2(0, rand.Next(0, 17) - 8));
                    }
                    for (int i = 0; i < p.Count; i++)
                    {
                        p[i] = p[i] + new Vector2(tspeed, 0);
                    }
                    for (int i = 0; i < l.Count; i++)
                    {
                        l[i] = l[i] + new Vector2(tspeed, 0);
                    }
                    for (int i = 0; i < p.Count; i++)
                    {
                        if (p[i].X > length)
                        {
                            p.RemoveAt(i);
                            break;
                        }
                    }
                    for (int i = 0; i < l.Count; i++)
                    {
                        if (l[i].X > length)
                        {
                            l.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
            sound.timeleft = 2;
            sound2.timeleft = 2;
            if(Util.Util.getDistance(Projectile.Center, Main.LocalPlayer.Center) > 600)
            {
                if(Util.Util.getDistance(Projectile.Center, Main.LocalPlayer.Center) > 2000){
                    sound.setVolume(0);
                    sound2.setVolume(0);
                }
                else{
                    sound.setVolume(1 - (float)(Util.Util.getDistance(Projectile.Center, Main.LocalPlayer.Center) - 600) / 1400f);
                    sound2.setVolume(1 - (float)(Util.Util.getDistance(Projectile.Center, Main.LocalPlayer.Center) - 600) / 1400f);
                }
            }
            else
            {
                sound.setVolume(1);
                sound2.setVolume(1);

            }
            
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(Projectile.Center), 1800f, 1000f, 0f, 4.5f) * 1;
            if (Main.myPlayer == Projectile.owner)
            {
                Player owner = Main.LocalPlayer;
                Vector2 nv = (Main.MouseWorld - owner.MountedCenter).SafeNormalize(Vector2.One) * 16;
                if (nv != Projectile.velocity)
                {
                    Projectile.netUpdate = true;
                }
                Projectile.velocity = nv;

            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = Projectile.owner.ToPlayer().MountedCenter + Projectile.owner.ToPlayer().gfxOffY * Vector2.UnitY + Projectile.rotation.ToRotationVector2() * 14;
            if (Projectile.timeLeft < 6)
            {
                width -= 1f / 7f;
            }
            else if(Projectile.timeLeft > 6)
            {
                width += 1f / 7f;
                
            }
            if (Projectile.timeLeft == 6 && Projectile.owner.ToPlayer().channel)
            {
                Projectile.timeLeft++;
            }
            aicounter++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return width >= 0.3f && Util.Util.LineThroughRect(Projectile.Center,  Projectile.Center + Projectile.rotation.ToRotationVector2() * length, targetHitbox, 30, 24);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            counter++;
            var rand = Main.rand;
            int tspeed = 34;
            if (counter % 1 == 0)
            {
                p.Add(new Vector2(0, rand.Next(0, 41) - 20));
            }
            if (counter % 6 == 0)
            {
                l.Add(new Vector2(0, rand.Next(0, 17) - 8));
            }
            for (int i = 0; i < p.Count; i++)
            {
                p[i] = p[i] + new Vector2(tspeed, 0);
            }
            for (int i = 0; i < l.Count; i++)
            {
                l[i] = l[i] + new Vector2(tspeed, 0);
            }
            for (int i = 0; i < p.Count; i++)
            {
                if (p[i].X > length)
                {
                    p.RemoveAt(i);
                    break;
                }
            }
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].X > length)
                {
                    l.RemoveAt(i);
                    break;
                }   
            }
            Texture2D tb = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/clback2").Value;
            Texture2D px = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
            Texture2D tl = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cllight").Value;
            Texture2D th = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/clinghth").Value;
            Texture2D tl2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cllight2").Value;
            Main.spriteBatch.Draw(tb, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, tb.Height / 2), new Vector2(length, width), SpriteEffects.None, 0);
            foreach (Vector2 ps in p) {
                Util.Util.drawLine(Main.spriteBatch, px, Projectile.Center + (ps * new Vector2(1, width)).RotatedBy(Projectile.rotation), Projectile.Center + ((ps * new Vector2(1, width)) + new Vector2(26, 0)).RotatedBy(Projectile.rotation), Color.White * 0.6f, 4 * width);
            }
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(tl2, Projectile.Center - Main.screenPosition, null, new Color(255, 160, 160) * 0.5f, Projectile.rotation, new Vector2(0, tl2.Height / 2), new Vector2(length, width * 1.2f), SpriteEffects.None, 0);

            foreach (Vector2 ps in l)
            {
                Main.spriteBatch.Draw(tl, Projectile.Center + (ps * new Vector2(1, width)).RotatedBy(Projectile.rotation) - Main.screenPosition, null, new Color(160, 160, 255) * 0.7f, Projectile.rotation, tl.Size() / 2, new Vector2(1.5f, 1.5f * width), SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(th, Projectile.Center - Main.screenPosition, null, new Color(255, 160, 160) * 0.3f, Projectile.rotation, new Vector2(0, th.Height / 2), new Vector2(1, width), SpriteEffects.None, 0);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            
            return false;
        }
    }

}