using CalamityEntropy.Assets.Register;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.FallenHammer
{
    public class FallenEruption : BaseScarletProj
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 8;
            Projectile.height = 4;
            Projectile.friendly = true;
            //?
            Projectile.alpha = 0;
            Projectile.penetrate = 1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public bool CanHome = false;
        public override bool? CanDamage() => CanHome;
        public override void AI()
        {

            PickTagDust(out short Pick);
            float minScale = 1.9f;
            float maxScale = 2.5f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                int dust = Dust.NewDust(Projectile.Center, 4, 4, Pick, 0f, -2f, 0, default, Main.rand.NextFloat(minScale, maxScale));
                Main.dust[dust].noGravity = true;
            }

            ref float Timer = ref Projectile.ai[0];
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (!CanHome)
            {
                Projectile.velocity.Y += 0.22f;
                if (Timer > 30)
                {
                    CanHome = true;
                    Projectile.timeLeft = 120;
                    Timer = 0;
                }
            }
            if(CanHome)
            {
                if (Timer < 50)
                    Timer++;
                if (Projectile.GetTargetSafe(out NPC target, 0, true))
                    Projectile.HomingNPCBetter(target, 18f + Timer / 5f, 20f);
                else
                    Projectile.velocity.Y += 0.18f;
            }
        }
        private void PickTagDust(out short Pick)
        {
            Pick = Owner.name.ToLower() switch
            {
                "scarletshelf" or "truescarlet" or "fakeaqua" => DustID.CrimsonTorch,
                "chalost" or "查诗络" => DustID.YellowTorch,
                "yinjiu" or "银九" => DustID.PinkTorch,
                "kino" => DustID.BlueTorch,
                "锯角" => DustID.PurpleTorch,
                "fr9ezes" => DustID.JungleTorch,
                "kikastorm" or "kika" => DustID.WhiteTorch,
                _ => DustID.OrangeTorch,
            };
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            switch (Owner.name.ToLower())
            {
                case "scarletshelf":
                case "truescarlet":
                case "fakeaqua":
                    baseColor = Color.LightCoral;
                    targetColor = Color.Crimson;
                    break;
                 //查 -- 金
                case "chalost":
                case "查诗络":
                    baseColor = Color.LightGoldenrodYellow;
                    targetColor = Color.Yellow;
                    break;
                //银九 - 粉
                case "yinjiu":
                case "银九":
                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                case "kino":
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                //聚胶 - 紫
                case "锯角":
                    targetColor = Color.White;
                    baseColor = Color.DarkViolet;
                    break;
                case "fr9ezes":
                    baseColor = Color.LimeGreen;
                    targetColor = Color.White;
                    break;
                //银 - 未指定
                case "kikastorm":
                case "kika":
                    baseColor = Color.DarkGray;
                    targetColor = Color.GhostWhite;
                    break;
                default:
                    targetColor = Color.Orange;
                    baseColor = new Color(255, 215, 100);
                    break;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            PickTagDust(out short Pick);
            float minScale = 1.6f;
            float maxScale = 2.2f;
            int numDust = 2;
            for (int i = 0; i < numDust; i++)
            {
                Dust.NewDust(Projectile.position, 4, 4, Pick, Projectile.velocity.X, Projectile.velocity.Y, 0, default, Main.rand.NextFloat(minScale, maxScale));
            }

            Projectile.Kill();
            return true;
        }
        //抄的地狱飞剑
        //todo:换一个更好的火球贴图，这个现在是个占位符
        public override bool PreDraw(ref Color lightColor)
        {
            PickTagColor(out Color baseColor, out Color targetColor);
            Texture2D tex = TextureRegister.General_WhiteOrb.Value;
            float scale = 1f;
            //绘制残影
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                Vector2 projCenter = (Projectile.Center - Main.screenPosition) - Projectile.velocity * i * 0.45f;
                scale *= 0.94f;
                float radius = (float)i / (Projectile.oldPos.Length);
                Color color = ((Color.Lerp(baseColor, targetColor, radius)) * 0.6f * (1 - radius));
                Main.spriteBatch.Draw(tex, projCenter , null, color * Projectile.Opacity, Projectile.oldRot[i], tex.Size() / 2f, Projectile.scale * new Vector2(1.0f, scale), SpriteEffects.None, 0f);
            }
            //绘制火球本身
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, baseColor with { A = 100}, Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
            
            return false;
        }
    }
}
