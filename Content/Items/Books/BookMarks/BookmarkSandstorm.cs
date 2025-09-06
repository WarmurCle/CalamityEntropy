using CalamityMod;
using CalamityMod.Dusts;
using CalamityMod.Items;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookmarkSandstorm : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Sandstorm");
        public override EBookProjectileEffect getEffect()
        {
            return new RoyalBMEffect();
        }

        public override Color tooltipColor => Color.Yellow;
        public static void ShootProjectile(int count, Player player, EntropyBookHeldProjectile book)
        {
            for(int i = 0; i < count; i++)
            {
                book.ShootSingleProjectile(ModContent.ProjectileType<SandBullet>(), player.MountedCenter, (Main.MouseWorld - player.MountedCenter).normalize() * 16 + CEUtils.randomPointInCircle(8), 0.12f);
            }
        }
    }
    public class SandBullet : EBookBaseProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.alpha = 255;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            base.AI();
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 4f)
            {
                Projectile.alpha -= 50;
                if (Projectile.alpha < 0)
                    Projectile.alpha = 0;

                int sandyDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (int)CalamityDusts.SulphurousSeaAcid, 0f, 0f, 100, default, 1f);
                Main.dust[sandyDust].noGravity = true;
                Main.dust[sandyDust].velocity *= 0f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = (int)(32 * Projectile.scale);
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            int dustAmt = 36;
            for (int i = 0; i < dustAmt; i++)
            {
                Vector2 dustRotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.75f;
                dustRotate = dustRotate.RotatedBy((double)((float)(i - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + Projectile.Center;
                Vector2 dustDirection = dustRotate - Projectile.Center;
                int killSand = Dust.NewDust(dustRotate + dustDirection, 0, 0, (int)CalamityDusts.SulphurousSeaAcid, dustDirection.X, dustDirection.Y, 100, default, 1.2f);
                Main.dust[killSand].noGravity = true;
                Main.dust[killSand].noLight = true;
                Main.dust[killSand].velocity = dustDirection * 0.5f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = (byte)(255 * Projectile.Opacity);
            lightColor.G = (byte)(255 * Projectile.Opacity);
            lightColor.B = (byte)(255 * Projectile.Opacity);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
    public class SandstormBMEffect : EBookProjectileEffect
    {

    }
}