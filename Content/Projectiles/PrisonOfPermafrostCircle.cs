using System;
using System.Collections.Generic;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class PrisonOfPermafrostCircle : ModProjectile
    {
        public int usingTime = 0;
        public int counter = 0;
        public float a1 = 0;
        public float a2 = 0;
        public float a3 = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 200;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 60;
            usingTime = 0;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void AI(){
            
            Player player = Main.player[Projectile.owner];
            Projectile.netImportant = true;
            player.manaRegenDelay = 80;
            Projectile.Center = player.Center;
            if (Projectile.Entropy().OnProj != -1)
            {
                Projectile.Center = Projectile.Entropy().OnProj.ToProj().Center;
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (player.channel && !player.HeldItem.IsAir && player.HeldItem.type == ModContent.ItemType<PrisonOfPermafrost>())
            {
                if (Projectile.owner == Main.myPlayer){
                    Projectile.rotation = Util.Util.rotatedToAngle(Projectile.rotation, (Main.MouseScreen + Main.screenPosition  - player.Center).ToRotation(), 0.1f, false);
                    Projectile.velocity = Projectile.rotation.ToRotationVector2();
                    Projectile.netUpdate = true;
                }
                
                usingTime++;
                Projectile.timeLeft = 60;
                if (usingTime > 60)
                {
                    if (Projectile.owner == Main.myPlayer)
                    {
                        
                        if (usingTime < 100){
                            if (usingTime % 5 == 0)
                            {
                                int p = Projectile.NewProjectile(player.GetSource_FromAI(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 80 + new Vector2(12 + Main.rand.Next(-6, 7), 12 + Main.rand.Next(-60, 61)).RotatedBy(Projectile.rotation), Projectile.rotation.ToRotationVector2() * 40, ModContent.ProjectileType<IceSpike>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack * 0.3f, Projectile.owner);
                            }
                        }
                        else{
                            if (usingTime < 160){
                                if (usingTime % 4 == 0)
                                {
                                int p = Projectile.NewProjectile(player.GetSource_FromAI(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 80 + new Vector2(12 + Main.rand.Next(-6, 7), 12 + Main.rand.Next(-60, 61)).RotatedBy(Projectile.rotation), Projectile.rotation.ToRotationVector2() * 40, ModContent.ProjectileType<IceSpike>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack * 0.3f, Projectile.owner);
                                }
                            }
                            else{
                                if (usingTime < 250){
                                    if (usingTime % 3 == 0)
                                    {
                                        int p = Projectile.NewProjectile(player.GetSource_FromAI(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 80 + new Vector2(12 + Main.rand.Next(-6, 7), 12 + Main.rand.Next(-60, 61)).RotatedBy(Projectile.rotation), Projectile.rotation.ToRotationVector2() * 40, ModContent.ProjectileType<IceSpike>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack * 0.3f, Projectile.owner);
                                    }
                                }
                                else{
                                    if (usingTime % 2 == 0)
                                    {
                                        int p = Projectile.NewProjectile(player.GetSource_FromAI(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 80 + new Vector2(12 + Main.rand.Next(-6, 7), 12 + Main.rand.Next(-60, 61)).RotatedBy(Projectile.rotation), Projectile.rotation.ToRotationVector2() * 40, ModContent.ProjectileType<IceSpike>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack * 0.3f, Projectile.owner);
                                    }
                                }
                            }
                        }
                        if (usingTime % Math.Max(1, 65 - player.Entropy().WeaponBoost * 20) == 0 && usingTime > 120)
                        {
                            Vector2 ofs;
                            float ag = (float)(Main.rand.NextDouble() * Math.PI * 2);
                            int projCount = 1 + (int)Math.Sqrt((usingTime + 500) / 110);
                            for (int i = 0; i < projCount; i++)
                            {
                                ofs = Main.screenPosition + Main.MouseScreen + ag.ToRotationVector2() * 450 + new Vector2(0, -40);
                                int p = Projectile.NewProjectile(player.GetSource_FromAI(), Projectile.Center + Projectile.rotation.ToRotationVector2() * 80 + new Vector2(Main.rand.Next(-17, 18), Main.rand.Next(-17, 18)), Vector2.Zero, ModContent.ProjectileType<Icicle>(), Projectile.damage, Projectile.knockBack * 3f, Projectile.owner, ofs.X, ofs.Y);
                                if (i == 0)
                                {
                                    Main.projectile[p].ai[2] = 1;
                                }
                                ag += MathHelper.ToRadians(360f / (float)projCount);
                            }
                        }
                        if (usingTime % Math.Max(20, 180 - player.Entropy().WeaponBoost * 50) == 0)
                        {
                            float anglep = MathHelper.ToRadians(5);
                            for (int i = 0; i < 6; i++)
                            {
                                int p = Projectile.NewProjectile(player.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IceEdge>(), Projectile.damage, 6);
                                Main.projectile[p].rotation = Projectile.rotation + anglep;
                                p = Projectile.NewProjectile(player.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<IceEdge>(), Projectile.damage, 6);
                                Main.projectile[p].rotation = Projectile.rotation - anglep;

                                anglep += MathHelper.ToRadians(10);
                            }
                        }
                        }
                }
            }
            else
            {
                if (usingTime > 60)
                {
                    usingTime = 58;
                    Projectile.timeLeft = 60;
                }
                else
                {
                    usingTime -= 1;
                    Projectile.timeLeft = usingTime;
                }
            }
            if (counter % 12 == 0)
            {
                if(Projectile.damage < 1000){
                    if(counter % 32 == 0){
                        Projectile.damage += 1;
                    
                    }
                }
                int cost = 2 + usingTime / 300;
                if (player.CheckMana(player.ActiveItem(), cost, true, false)) {
                    player.manaRegenDelay = 80;
                }
                else
                {
                    Projectile.Kill();
                }
            }
            counter++;
            if (usingTime > 60)
            {
                a1 += 0.02f;
                if (usingTime > 100)
                {
                    a2 += 0.02f;
                }
                else
                {
                    a2 -= 0.02f;
                }
                if (usingTime > 140)
                {
                    a3 += 0.02f;
                }
                else
                {
                    a3 -= 0.02f;
                }
                if (a1 > 1)
                {
                    a1 = 1;
                }
                if (a2 > 1)
                {
                    a2 = 1;
                }
                if (a3 > 1)
                {
                    a3 = 1;
                }
                if (a1 < 0)
                {
                    a1 = 0;
                }
                if (a2 < 0)
                {
                    a2 = 0;
                }
                if (a3 < 0)
                {
                    a3 = 0;
                }
            }
            else {
                a1 -= 0.02f;
                a2 -= 0.02f;
                a3 -= 0.02f;
                if (a1 < 0)
                {
                    a1 = 0;
                }
                if (a2 < 0)
                {
                    a2 = 0;
                }
                if (a3 < 0)
                {
                    a3 = 0;
                }
            }
            player.itemTime = 2;
            player.itemAnimation = 2;
            Projectile.netImportant = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.Entropy().OnProj != -1)
            {
                Projectile.Center = Projectile.Entropy().OnProj.ToProj().Center;
            }
            List<Texture2D> flames = new List<Texture2D>();
            flames.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/flame1").Value);
            flames.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/flame2").Value);
            flames.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/flame3").Value);
            flames.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/flame4").Value);
            Texture2D triangle = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/triangle").Value;
            float alpha = (float)usingTime / 60f;
            if (alpha > 1)
            {
                alpha = 1;
            }
            Texture2D flameDraw = flames[(counter / 3) % 4];
            Main.spriteBatch.Draw(flameDraw, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * alpha * 0.7f, 0, new Vector2(flameDraw.Width, flameDraw.Height) / 2, 1.3f, SpriteEffects.None, 0);
            int rg = (int)(220 + 35 * Math.Cos((float) counter / 10));
            Color triC = new Color(rg, rg, 255);
            
            Main.spriteBatch.Draw(triangle, Projectile.Center - Main.screenPosition + new Vector2(0, 12), null, triC * alpha, 0, new Vector2(triangle.Width, triangle.Height) / 2, 1, SpriteEffects.None, 0);

            Texture2D ice1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/ice1").Value;
            Texture2D ice2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/ice2").Value;
            Texture2D ice3 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/ice3").Value;
            List<Texture2D> ices = new List<Texture2D>();
            ices.Add(ice1);
            ices.Add(ice2);
            ices.Add(ice3);
            float angle = ((float)counter) / 180f * (float)Math.PI * 2f;
            int size = (int)((alpha * 8.3f) * (alpha * 8.3f));
            Vector2 ofs = Vector2.Zero;
            
            for (int i = 0; i < 6; i++) {
                Texture2D itx = ices[i % 3];
                Main.spriteBatch.Draw(itx, Projectile.Center - Main.screenPosition + ofs + (new Vector2(size, 0).RotatedBy(angle + MathHelper.ToRadians(angle + i * 60))) * new Vector2(1, 0.8f), null, Color.White * alpha, 0, new Vector2(itx.Width, itx.Height) / 2, 1, SpriteEffects.None, 0);
            }

            Texture2D c1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/c1").Value;
            Texture2D c2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/c2").Value;
            Texture2D c3 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/c3").Value;
            Texture2D c4 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/c4").Value;
            Texture2D c5 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/c5").Value;
            Texture2D c6 = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/c6").Value;
            List<Texture2D> cs = new List<Texture2D>();
            cs.Add(c1);
            cs.Add(c2);
            cs.Add(c3);
            cs.Add(c4);
            cs.Add(c5);
            cs.Add(c6);
            angle = -((float)counter) / 180f * (float)Math.PI * 2f;
            size = (int)((alpha * 12f) * (alpha * 12f)) + (int)(Math.Cos((float)counter / 10) * 30);
            int size2 = (int)((alpha * 12f) * (alpha * 12f)) - (int)(Math.Cos((float)counter / 10) * 30); ;
            ofs = Vector2.Zero;

            for (int i = 0; i < 6; i++)
            {
                Texture2D itx = cs[i];
                float alpha2 = alpha;
                if (i == 0 || i == 3)
                {
                    alpha2 = a1;
                }
                if (i == 1 || i == 4)
                {
                    alpha2 = a2;
                }
                if (i == 2 || i == 5)
                {
                    alpha2 = a3;
                }


                Main.spriteBatch.Draw(itx, Projectile.Center - Main.screenPosition + ofs + (new Vector2(size, 0).RotatedBy(angle + MathHelper.ToRadians(angle + i * 60))) * new Vector2(1, 0.8f), null, new Color(160, 180, 215) * alpha2 * 0.6f, 0, new Vector2(itx.Width, itx.Height) / 2, new Vector2(1.4f + 0.2f * (float)Math.Cos((float)counter / 20), 1.4f + 0.2f * (float)Math.Cos((float)counter / 20)), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(itx, Projectile.Center - Main.screenPosition + ofs + (new Vector2(size, 0).RotatedBy(angle + MathHelper.ToRadians(angle + i * 60))) * new Vector2(1, 0.8f), null, triC * alpha2, 0, new Vector2(itx.Width, itx.Height) / 2, 1, SpriteEffects.None, 0);


            }

            Texture2D circle = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/POP/circle").Value;
            angle = MathHelper.ToDegrees(counter);
            size = (int)(alpha * 100);
            Vector2 lu = new Vector2(size, 0).RotatedBy(angle - MathHelper.ToRadians(angle - 135));
            Vector2 ru = new Vector2(size, 0).RotatedBy(angle - MathHelper.ToRadians(angle - 45));
            Vector2 ld = new Vector2(size, 0).RotatedBy(angle - MathHelper.ToRadians(angle + 135));
            Vector2 rd = new Vector2(size, 0).RotatedBy(angle - MathHelper.ToRadians(angle + 45));

            lu.X *= 0.3f;
            ru.X *= 0.3f;
            ld.X *= 0.3f;
            rd.X *= 0.3f;

            Vector2 dp = Projectile.Center - Main.screenPosition;
            Vector2 offset = new Vector2(100, 0);   

            lu += offset;
            ru += offset;
            ld += offset;
            rd += offset;

            lu = lu.RotatedBy(Projectile.rotation);
            ru = ru.RotatedBy(Projectile.rotation);
            ld = ld.RotatedBy(Projectile.rotation);
            rd = rd.RotatedBy(Projectile.rotation);

            Util.Util.drawTextureToPoint(Main.spriteBatch, circle, Color.White * alpha, dp + lu, dp + ru, dp + ld, dp + rd);

            return false;

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return false;
        }

        public override bool? CanCutTiles()
        {
            return false;
        }
    }

}