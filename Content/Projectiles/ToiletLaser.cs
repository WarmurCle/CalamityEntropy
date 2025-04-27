using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Utilities;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{

    public class ToiletLaser : ModProjectile
    {
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<VoidTouch>(), 160);
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 8000;
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
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 0f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 12;
            Projectile.ArmorPenetration = 100;
        }
        public bool st = true;
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);
        }
        public override void AI()
        {
            if (st)
            {
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
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Utils.Remap(Main.LocalPlayer.Distance(Projectile.Center), 1800f, 1000f, 0f, 4.5f) * 8;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (Projectile.timeLeft < 6)
            {
                width -= 1f / 7f;
            }
            else
            {
                width += 1f / 7f;

            }
            aicounter++;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return width >= 0.1f && Utilities.Util.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * length, targetHitbox, 30, 24);
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
            Texture2D tb = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/clback").Value;
            Texture2D px = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/white").Value;
            Texture2D tl = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cllight").Value;
            Texture2D th = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/clinghth").Value;
            Texture2D tl2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/cllight2").Value;
            Main.spriteBatch.Draw(tb, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(0, tb.Height / 2), new Vector2(length, width), SpriteEffects.None, 0);
            foreach (Vector2 ps in p)
            {
                Utilities.Util.drawLine(Main.spriteBatch, px, Projectile.Center + (ps * new Vector2(1, width)).RotatedBy(Projectile.rotation), Projectile.Center + ((ps * new Vector2(1, width)) + new Vector2(40, 0)).RotatedBy(Projectile.rotation), Color.White, 2 * width);
            }
            SpriteBatch sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(tl2, Projectile.Center - Main.screenPosition, null, new Color(160, 160, 255) * 0.8f, Projectile.rotation, new Vector2(0, tl2.Height / 2), new Vector2(length, width * 1.2f), SpriteEffects.None, 0);

            foreach (Vector2 ps in l)
            {
                Main.spriteBatch.Draw(tl, Projectile.Center + (ps * new Vector2(1, width)).RotatedBy(Projectile.rotation) - Main.screenPosition, null, new Color(160, 160, 255), Projectile.rotation, tl.Size() / 2, new Vector2(1.5f, 1.5f * width), SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(th, Projectile.Center - Main.screenPosition, null, new Color(160, 160, 255) * 0.5f, Projectile.rotation, new Vector2(0, th.Height / 2), new Vector2(1, width), SpriteEffects.None, 0);

            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

}