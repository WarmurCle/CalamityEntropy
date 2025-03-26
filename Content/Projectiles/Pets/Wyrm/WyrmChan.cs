using CalamityEntropy.Content.Buffs.Pets;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Pets.Wyrm
{
    public class WyrmChan : ModProjectile
    {
        public int counter = 0;
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
            Projectile.width = 92;
            Projectile.height = 92;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.gameMenu)
            {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/WyrmChan").Value;
                Main.EntitySpriteDraw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            List<Texture2D> list = new List<Texture2D>();
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/WyrmChan").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/WyrmChan2").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/WyrmChan3").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/WyrmChan4").Value);
            List<Texture2D> list2 = new List<Texture2D>();
            list2.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/Eye").Value);
            list2.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/Eye2").Value);
            list2.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/Eye3").Value);
            list2.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Wyrm/Eye4").Value);
            Texture2D tx = list[(counter / 6) % list.Count];
            Texture2D tx2 = list2[(counter / 6) % list.Count];
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
            if (Projectile.direction == 1)
            {
                Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
                if (lightColor.R + lightColor.G + lightColor.B < 255)
                {
                    int gr = (255 - (lightColor.R + lightColor.G + lightColor.B));
                    Main.EntitySpriteDraw(tx2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255) * ((float)gr / 255), Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);
                }
            }
            else
            {
                Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
                if (lightColor.R + lightColor.G + lightColor.B < 255)
                {
                    int gr = (255 - (lightColor.R + lightColor.G + lightColor.B));
                    Main.EntitySpriteDraw(tx2, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 255) * ((float)gr / 255), Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
                }
            }



            return false;

        }
        void MoveToTarget(Vector2 targetPos)
        {
            if (Util.Util.getDistance(Projectile.Center, targetPos) > 1400)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
            }
            Projectile.rotation = MathHelper.ToRadians((Projectile.velocity.X * 1.4f));
            if (Util.Util.getDistance(Projectile.Center, targetPos) > 34)
            {
                Vector2 px = targetPos - Projectile.Center;
                px.Normalize();
                Projectile.velocity += px * 0.6f;

                Projectile.velocity *= 0.98f;

            }
            else
            {
                Projectile.velocity *= 0.8f;

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
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.zephyrfish = false;

            return true;
        }

        public override void AI()
        {
            counter++;
            Player player = Main.player[Projectile.owner];
            MoveToTarget(player.Center + new Vector2(0, -60) + new Vector2(-80 * player.direction, 0));
            if (!player.dead && player.HasBuff(ModContent.BuffType<WyrmChanBuff>()))
            {
                Projectile.timeLeft = 2;
            }
            if (Projectile.wet)
            {
                Projectile.extraUpdates = 1;
            }
            else
            {
                Projectile.extraUpdates = 0;
            }
        }


    }
}
