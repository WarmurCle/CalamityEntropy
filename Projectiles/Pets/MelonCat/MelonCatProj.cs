using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using System.IO;
using Terraria;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using ReLogic.Content;
using CalamityEntropy.Buffs.Pets;
using CalamityEntropy.Projectiles;
using CalamityMod;
using CalamityEntropy.Util;
namespace CalamityEntropy.Projectiles.Pets.MelonCat
{
    public class MelonCatProj : ModProjectile
    {
        public float counter = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            base.SetStaticDefaults();

        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 24;
            Projectile.height = 44;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            
            if (Main.gameMenu) {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/Pets/MelonCat/MelonCatProj").Value;
                Main.spriteBatch.Draw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            Player player = Main.player[Projectile.owner];
            int tc = 4;
            Texture2D tx;
            if (Projectile.ai[1] == 1)
            {
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/Pets/MelonCat/flying").Value;
            }
            else
            {
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/Pets/MelonCat/walk").Value;
                tc = 6;
            }
            if (Projectile.velocity.X > -2 && Projectile.velocity.X < 2f)
            {
                if (player.Center.X > Projectile.Center.X)
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
                Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, Util.Util.GetCutTexRect(tx, tc, (int)(counter / 2f % tc)), lightColor, Projectile.rotation, new Vector2(tx.Width / tc, tx.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

            }
            else
            {
                Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, Util.Util.GetCutTexRect(tx, tc, (int)(counter / 2f % tc)), lightColor, Projectile.rotation, new Vector2(tx.Width / tc, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
            }


            return false;

        }
        void MoveToTarget(Vector2 targetPos)
        {
            if (Util.Util.getDistance(Projectile.Center, targetPos) > 1400)
            {
                Projectile.Center = Main.player[Projectile.owner].Center - new Vector2(0, 50);
            }
            if (Projectile.ai[1] == 1)
            {
                counter++;
                Projectile.tileCollide = false;
                Projectile.rotation = MathHelper.ToRadians((Projectile.velocity.X * 1.4f));
                if (Util.Util.getDistance(Projectile.Center, targetPos) > 90)
                {
                    Vector2 px = targetPos - Projectile.Center;
                    px.Normalize();
                    Projectile.velocity += px * 0.68f;

                    Projectile.velocity *= 0.986f;

                }
                if (Util.Util.getDistance(Projectile.Center, targetPos) < 100 && !(Util.Util.isAir(Projectile.owner.ToPlayer().Center + new Vector2(0, Projectile.owner.ToPlayer().height / 2 + 2), true)))
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
                counter += Math.Abs(Projectile.velocity.X / 7f);
                Projectile.tileCollide = true;
                Projectile.rotation = 0;
                Projectile.velocity.Y += 0.5f;
                if (Util.Util.getDistance(targetPos, Projectile.Center) > 500) {
                    Projectile.ai[1] = 1;
                }
                else if (Util.Util.getDistance(targetPos * new Vector2(1, 0), Projectile.Center * new Vector2(1, 0)) > 120)
                {
                    if (targetPos.X > Projectile.Center.X)
                    {
                        Projectile.velocity.X += 0.7f;
                    }
                    else
                    {
                        Projectile.velocity.X -= 0.7f;
                    }
                    Projectile.velocity.X *= 0.98f;
                }
                else
                {
                    Projectile.velocity.X *= 0.93f;
                }
                if (targetPos.X > Projectile.Center.X)
                {
                    Projectile.direction = 1;
                }
                else
                {
                    Projectile.direction = -1;
                }
                
                if (!Util.Util.isAir(Projectile.Center + Projectile.velocity * new Vector2(1, 0).SafeNormalize(Vector2.Zero) * 13 + new Vector2(0, 18)) && Util.Util.isAir(Projectile.Center))
                {
                    Projectile.velocity.Y -= 1;
                }
            }
            
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.zephyrfish = false;
            return true;
        }

        public override void AI()
        {
            
            Player player = Main.player[Projectile.owner];
            MoveToTarget(player.Center + new Vector2(0, 0));
            if (!player.dead && player.HasBuff(ModContent.BuffType<MelonCatBuff>()))
            {
                Projectile.timeLeft = 2;
            }

        }


    }
}
