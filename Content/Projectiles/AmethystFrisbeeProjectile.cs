using System.Collections.Generic;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class AmethystFrisbeeProjectile : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = CUtil.rogueDC;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.light = 0.1f;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.ArmorPenetration = 10;
        }
        public float counter { get { return Projectile.localAI[0]; } set { Projectile.localAI[0] = value; } }
        public float grav { get { return Projectile.localAI[2]; } set { Projectile.localAI[2] = value; } }
        public bool hited { 
            get { return Projectile.ai[1] > 0; }
            set { Projectile.ai[1] = 1; }
        }
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/AmethystFrisbee";
        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X * 0.1f;
            counter++;
            if (Projectile.ai[0] == 0)
            {
                Projectile.localNPCHitCooldown = 20;
                if(hited && Projectile.ai[2] == 0) {
                    Projectile.ai[2]++;
                    Projectile.velocity = new Vector2(0, -14);
                    Projectile.netUpdate = true;
                }
                if (hited)
                {
                    if (Projectile.localAI[1] < 0.06f)
                    {
                        Projectile.localAI[1] += 0.001f;
                    }
                    Projectile.velocity *= 1f - Projectile.localAI[1];
                    Projectile.velocity += (Projectile.getOwner().Center - Projectile.Center).normalize() * (Projectile.localAI[1] * 28);
                    if (Projectile.Distance(Projectile.getOwner().Center) < Projectile.velocity.Length() + 40)
                    {
                        if(Projectile.getOwner().HeldItem.ModItem is AmethystFrisbee af)
                        {
                            af.altShotCount = 16;
                        }
                        Projectile.Kill();
                    }
                }
                else
                {
                    Projectile.velocity.Y += grav;
                    if(grav < 1f)
                    {
                        grav += 0.02f;
                    }
                    if (counter % 6 == 0)
                    {
                        Util.Util.PlaySound("spin" + Main.rand.Next(1, 3).ToString(), 1, Projectile.Center);
                    }
                }
            }
            else
            {
                Projectile.localNPCHitCooldown = 5;
                if (counter > 46)
                {
                    Projectile.velocity *= 0.974f;
                    Projectile.velocity += (Projectile.getOwner().Center - Projectile.Center).normalize() * 0.8f;
                    if (Projectile.Distance(Projectile.getOwner().Center) < Projectile.velocity.Length() + 40)
                    {
                        Projectile.Kill();
                    }
                }
                else
                {
                    if(counter % 6 == 0)
                    {
                        Util.Util.PlaySound("spin" + Main.rand.Next(1, 3).ToString(), 1, Projectile.Center);
                    }
                }
            }
            odp.Add(Projectile.Center);
            if(odp.Count > 18)
            {
                odp.RemoveAt(0);
            }
        }
        
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hited)
            {
                Util.Util.PlaySound("shield", pos: Projectile.Center);
                Util.Util.PlaySound("SarosDiskThrow1", pos: Projectile.Center);
            }
            Util.Util.PlaySound("bne_hit2", pos: Projectile.Center);
            if (!hited && Projectile.Calamity().stealthStrike)
            {
                Util.Util.PlaySound("crystalShieldBreak", pos: Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<AmethystExplosion>(), Projectile.damage, 1, Projectile.owner);
            }
            hited = true;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (!hited)
            {
                Util.Util.PlaySound("shield", pos: Projectile.Center);
            }
            hited = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!hited)
            {
                Util.Util.PlaySound("shield", pos: Projectile.Center);
            }
            hited = true;
            if (Projectile.ai[0] == 1)
            {
                counter = 41;
            }
            Projectile.tileCollide = false;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            for(int i = 1; i < odp.Count; i++)
            {
                Util.Util.drawLine(odp[i - 1], odp[i], Color.Purple * 0.36f, (float)i / odp.Count * 9);
            }
            Texture2D tx = Projectile.getTexture();
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }

    }

}