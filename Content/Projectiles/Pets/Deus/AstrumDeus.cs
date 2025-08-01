﻿using CalamityEntropy.Content.Buffs.Pets;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Pets.Deus
{
    public class AstrumDeus : ModProjectile
    {
        public int counter = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
            base.SetStaticDefaults();

        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 24;
            Projectile.height = 58;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            bool hat = Projectile.owner.ToPlayer().Entropy().PetsHat;
            if (Main.gameMenu)
            {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/AstrumDeus").Value;
                Main.EntitySpriteDraw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            List<Texture2D> list = new List<Texture2D>();
            if (hat)
            {
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/s/AstrumDeus").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/s/AstrumDeus2").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/s/AstrumDeus3").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/s/AstrumDeus4").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/s/AstrumDeus5").Value);
            }
            else
            {
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/AstrumDeus").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/AstrumDeus2").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/AstrumDeus3").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/AstrumDeus4").Value);
                list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/AstrumDeus5").Value);
            }
            Texture2D tx = list[(counter / 6) % list.Count];
            if (Projectile.velocity.X > -2 && Projectile.velocity.X < 2f)
            {
                if (Main.player[Projectile.owner].Center.X > Projectile.Center.X)
                {
                    Projectile.direction = 1;
                }
                else
                {
                    Projectile.direction = -1;
                }
            }
            if (Projectile.direction == -1)
            {
                Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

            }
            else
            {
                Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
            }


            return false;

        }
        void MoveToTarget(Vector2 targetPos)
        {
            if (CEUtils.getDistance(Projectile.Center, targetPos) > 1400)
            {
                Projectile.Center = Main.player[Projectile.owner].Center - new Vector2(0, 50);
            }
            if (Projectile.ai[1] == 1 || true)
            {
                Projectile.tileCollide = false;
                Projectile.rotation = MathHelper.ToRadians((Projectile.velocity.X * 1.4f));
                if (CEUtils.getDistance(Projectile.Center, targetPos) > 100)
                {
                    Vector2 px = targetPos - Projectile.Center;
                    px.Normalize();
                    Projectile.velocity += px * 0.8f;

                    Projectile.velocity *= 0.94f;

                }
                if (CEUtils.getDistance(Projectile.Center, targetPos) < 100)
                {
                    Projectile.ai[1] = 0;
                }
                if (Projectile.velocity.X > 0)
                {
                    Projectile.direction = 1;
                }
                else
                {
                    Projectile.direction = -1;
                }
            }
            else
            {
                Projectile.tileCollide = true;
                Projectile.rotation = 0;
                Projectile.velocity.Y += 0.5f;
                if (CEUtils.getDistance(targetPos, Projectile.Center) > 600)
                {
                    Projectile.ai[1] = 1;
                }
                else if (CEUtils.getDistance(targetPos * new Vector2(1, 0), Projectile.Center * new Vector2(1, 0)) > 200)
                {
                    if (targetPos.X > Projectile.Center.X)
                    {
                        Projectile.velocity.X += 0.6f;
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.6f;
                    }
                }
                if (targetPos.X > Projectile.Center.X)
                {
                    Projectile.direction = 1;
                }
                else
                {
                    Projectile.direction = -1;
                }
                Projectile.velocity.X *= 0.96f;
            }

        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.zephyrfish = false;
            if (DownedBossSystem.downedAstrumDeus)
            {
                Lighting.AddLight(Projectile.Center, 1.2f, 1f, 1.2f);
            }
            else
            {
                if (Main.hardMode)
                {
                    Lighting.AddLight(Projectile.Center, 0.6f, 0.5f, 0.6f);
                }
                else
                {
                    Lighting.AddLight(Projectile.Center, 0.4f, 0.4f, 0.4f);
                }
            }
            return true;
        }
        public int shotCd = 0;

        public override void AI()
        {
            counter++;
            Player player = Main.player[Projectile.owner];
            MoveToTarget(player.Center + new Vector2(0, -60));
            if (!player.dead && player.HasBuff(ModContent.BuffType<AstrumDeusBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            NPC n = Projectile.FindTargetWithinRange(1000, false);
            shotCd--;
            if (n != null && shotCd < 0)
            {
                if (Projectile.owner == Main.myPlayer && DownedBossSystem.downedAstrumDeus)
                {
                    shotCd = 400;

                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, (n.Center - Projectile.Center).SafeNormalize(Vector2.Zero), ModContent.ProjectileType<AstralShot>(), 460, 2, Projectile.owner);
                }
            }
        }


    }
}
