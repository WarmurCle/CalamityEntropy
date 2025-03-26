using CalamityEntropy.Content.Items.Weapons;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.SamsaraCasket
{
    public class ZeratosBullet0 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = ModContent.GetInstance<AverageDamageClass>();
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.ai[0] > 5;
        }
        int framecounter = 4;
        int frame = 0;
        public List<Vector2> odp = new List<Vector2>();
        public List<float> odr = new List<float>();
        public bool pld = true;
        public override void AI()
        {
            if (pld)
            {
                pld = false;
                SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            }
            Projectile.ArmorPenetration = HorizonssKey.getArmorPen();
            framecounter--;
            Projectile.ai[0]++;
            if (framecounter == 0)
            {
                frame++;
                framecounter = 4;
                if (frame > 4)
                {
                    frame = 0;
                }
            }
            Projectile.velocity.Y += 0.6f;
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            Util.Util.recordOldPosAndRots(Projectile, ref odp, ref odr, 6);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<ZeratosFireball0>(), Projectile.damage, Projectile.knockBack * 2, Projectile.owner);

            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor = Color.White;
            Util.Util.DrawAfterimage(TextureAssets.Projectile[Projectile.type].Value, odp, odr);
            Texture2D tex = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/SamsaraCasket/ZeratosBullet" + frame.ToString()).Value;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

    }

}