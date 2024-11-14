using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using CalamityEntropy.Projectiles;
using Terraria.Audio;
using CalamityEntropy.Util;
using Terraria.DataStructures;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod;
using CalamityMod.NPCs.TownNPCs;
namespace CalamityEntropy.Projectiles
{
    public class Brimstone : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 6000;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }
        float dLength = 0;
        public bool ssd = true;
        public NPC target = null;
        public override void AI()
        {
            if (Projectile.ai[0] == 0)
            {
                Projectile.scale = Projectile.ai[1];
                
            }
            
            if (Projectile.timeLeft < 30)
            {
                Projectile.scale -= 1f / 30f;
                
            }
            if (Projectile.scale < 0f)
            {
                Projectile.scale = 0;
            }
            Projectile.Center = ((int)Projectile.ai[2]).ToProj().Center + Projectile.rotation.ToRotationVector2() * 16;
            Projectile.rotation = Projectile.velocity.ToRotation();
            target = null;
            if (((int)Projectile.ai[2]).ToProj().type == ModContent.ProjectileType<LilBrimstone>())
            {
                Projectile opj = ((int)Projectile.ai[2]).ToProj();
                
                if (Projectile.owner.ToPlayer().HasMinionAttackTargetNPC)
                {
                    target = Main.npc[Projectile.owner.ToPlayer().MinionAttackTargetNPC];
                    float betw = Vector2.Distance(target.Center, Projectile.Center);
                    if (betw > 2000f)
                    {
                        target = null;
                    }

                }
                if (target == null || !target.active)
                {
                    int t = opj.FindTargetWithLineOfSight(2000);
                    if (t > -1)
                    {
                        target = Main.npc[t];
                    }
                }
                if (target != null)
                {
                    Projectile.velocity = (Util.Util.rotatedToAngle(Projectile.velocity.ToRotation(), (target.Center - Projectile.Center).ToRotation(), 2, true)).ToRotationVector2();
                }
            }
            if (ssd)
            {
                for (int s = 0; s < 60; s++)
                {
                    points.Insert(0, Projectile.Center);
                    prs.Insert(0, Projectile.rotation);
                    updatePoints();
                }
                ssd = false;
            }
            Projectile.ai[0]++;
            points.Insert(0, Projectile.Center);
            prs.Insert(0, Projectile.rotation);

            updatePoints();
            points.RemoveAt(points.Count - 1);
            prs.RemoveAt(points.Count - 1);


        }
        public void updatePoints()
        {
            float speed = 42;
            
            for (int i = points.Count - 1; i >= 0; i--) {

                points[i] += speed * prs[i].ToRotationVector2();
            }
            
        }
        public override bool ShouldUpdatePosition()
        {
            return true;//false;
        }
        public float counter = 0;
        public List<Vector2> points = new List<Vector2>();
        public List<float> prs = new List<float>();
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!Util.Util.isAir(Projectile.Center))
            {
                return false;
            }
            if (points.Count < 4)
            {
                return false;
            }
            for (int i = 1; i < points.Count; i++)
            {
                if (Util.Util.isAir(points[i]))
                {
                    if (Util.Util.LineThroughRect(points[i - 1], points[i], targetHitbox, 56))
                    {
                        return true;
                    }
                }
                else
                {
                    break;
                }
            }
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lend = Util.Util.getExtraTex("vlend");
            counter++;
            if (!Util.Util.isAir(Projectile.Center))
            {
                Main.spriteBatch.Draw(lend, Projectile.Center - Main.screenPosition, Util.Util.GetCutTexRect(lend, 4, (int)(counter * 0.5f) % 4), Color.Red, Projectile.rotation + (float)Math.PI / 2f, new Vector2(64, 104) / 2, new Vector2(Projectile.scale * 2, 2), SpriteEffects.None, 0);
                return false;
            }
            if (Projectile.ai[0] < 1)
            {
                return false;
            }
            
            Texture2D tx1 = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/brimstone1").Value;
            Texture2D tx1b = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/brimstone1b").Value;
            Texture2D tx2 = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/brimstone2").Value;
            Texture2D tx2b = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/brimstone2b").Value;
            Texture2D tx3 = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/brimstone3").Value;
            Texture2D tx3b = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/brimstone3b").Value;
            Texture2D tx4 = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/brimstone4").Value;
            Texture2D tx4b = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/brimstone4b").Value;
            List<Texture2D> txl = new List<Texture2D>();
            
            txl.Add(tx4b);
            txl.Add(tx4);
            txl.Add(tx3b);
            txl.Add(tx3);
            txl.Add(tx2b);
            txl.Add(tx2);
            txl.Add(tx1b);
            txl.Add(tx1);

            Texture2D start = ModContent.Request<Texture2D>("CalamityEntropy/Extra/brimstone/start").Value;

            

            List<Vector2> point = new List<Vector2>();
            dLength = 0;

            point.Add(Projectile.Center + Projectile.rotation.ToRotationVector2() * 24);
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (Util.Util.isAir(points[i]))
                {
                    point.Add(points[i]);
                }
                else
                {
                    point.Add(points[i]);
                    break;
                }
            }
            
            
            if (point.Count > 2)
            {
                Main.spriteBatch.Draw(start, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 5, null, Color.Red, Projectile.rotation, start.Size() / 2, new Vector2(2, Projectile.scale * 2), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(lend, point[point.Count - 2] - Main.screenPosition + (point[point.Count - 1] - point[point.Count - 2]).ToRotation().ToRotationVector2() * -16, Util.Util.GetCutTexRect(lend, 4, (int)(counter * 0.5f) % 4), Color.Red, (point[point.Count - 1] - point[point.Count - 2]).ToRotation() + (float)Math.PI / 2f, new Vector2(64, 104) / 2, new Vector2(Projectile.scale * 2, 2), SpriteEffects.None, 0);

                Util.Util.drawLaser(Main.spriteBatch, txl, point, 64, Color.Red, (int)(128 * Projectile.scale), (int)(counter / 3f), Projectile.rotation);
                
            }
            else
            {
                Main.spriteBatch.Draw(lend, Projectile.Center - Main.screenPosition, Util.Util.GetCutTexRect(lend, 4, (int)(counter * 0.5f) % 4), Color.Red, Projectile.rotation + (float)Math.PI / 2f, new Vector2(64, 104) / 2, new Vector2(Projectile.scale * 2, 2), SpriteEffects.None, 0);
            }

            return false;
        }


    }
    

}