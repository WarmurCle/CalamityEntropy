using CalamityEntropy.Content.Particles;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class DarkScripture : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 39;
            Item.useAnimation = Item.useTime = 22;
            Item.mana = 7;
            Item.rare = ItemRarityID.Red;
        }
        public override Texture2D BookMarkTexture => ModContent.Request<Texture2D>("CalamityEntropy/Content/UI/EntropyBookUI/DS").Value;
        public override int HeldProjectileType => ModContent.ProjectileType<DarkScriptureHeld>();
        public override int SlotCount => 3;

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<UpdraftTome>()
                .AddIngredient(ItemID.SoulofNight, 8)
                .AddIngredient(ItemID.Book)
                .AddIngredient(ItemID.FallenStar, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class DarkScriptureHeld : EntropyBookDrawingAlt
    {
        public override string OpenAnimationPath => $"{EntropyBook.BaseFolder}/Textures/DarkScripture/Open";
        public override string PageAnimationPath => $"{EntropyBook.BaseFolder}/Textures/DarkScripture/Page";
        public override string UIOpenAnimationPath => $"{EntropyBook.BaseFolder}/Textures/DarkScripture/UI";
        public override int frameChange => 3;
        public override EBookStatModifer getBaseModifer()
        {
            var m = base.getBaseModifer();
            m.Homing = 1f;
            m.HomingRange = 1.25f;
            m.armorPenetration += 16;
            return m;
        }
        public override float randomShootRotMax => 0.16f;
        public override int baseProjectileType => ModContent.ProjectileType<DarkBullet>();
    }
    public class DarkBullet : EBookBaseProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public List<Vector2> oldPos = new List<Vector2>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = true;
            Projectile.light = 0.2f;
            Projectile.timeLeft = 800;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 2;
        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            CEUtils.PlaySound("blackholeEnd", Main.rand.NextFloat(2f, 2.4f), Projectile.Center, volume: 0.8f);
            GeneralParticleHandler.SpawnParticle(new PulseRing(Projectile.Center, Vector2.Zero, Color.Red, 0.1f, 0.8f, 8));
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Red * 0.8f, 1.5f, 1, true, BlendState.Additive, 0, 16);
            EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 0.2f, 1, true, BlendState.Additive, 0, 16);
            if (Projectile.owner == Main.myPlayer)
            {
                CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), Projectile.owner.ToPlayer(), Projectile.Center, Projectile.damage, 120, Projectile.DamageType);
            }
            CEUtils.SetShake(Projectile.Center, 6);
        }
        public TrailParticle t1;
        public TrailParticle t2;
        public override void AI()
        {
            base.AI();
            if (t1 == null || t2 == null)
            {
                t1 = new TrailParticle() { maxLength = 16, TimeLeftMax = 10 };
                t2 = new TrailParticle() { maxLength = 16, TimeLeftMax = 10 };
                EParticle.spawnNew(t1, Projectile.Center, Vector2.Zero, new Color(255, 60, 60), 0.8f, 1, true, BlendState.Additive);
                EParticle.spawnNew(t2, Projectile.Center, Vector2.Zero, new Color(255, 60, 60), 0.8f, 1, true, BlendState.Additive);

            }
            t1.AddPoint(Projectile.Center + Projectile.velocity.normalize().RotatedBy(MathHelper.PiOver2) * 12 * Projectile.scale);
            t2.AddPoint(Projectile.Center + Projectile.velocity.normalize().RotatedBy(-MathHelper.PiOver2) * 12 * Projectile.scale);
            t1.Lifetime = 12;
            t2.Lifetime = 12;
            oldPos.Add(Projectile.Center);
            if (oldPos.Count > 9)
            {
                oldPos.RemoveAt(0);
            }
            if (Projectile.timeLeft % 4 == 0)
            {
                CalamityMod.Particles.Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.2f, new Color(255, 42, 42), new Vector2(0.38f, 1f), Projectile.velocity.ToRotation(), 0.42f * Projectile.scale, 0.05f, 38);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            float scale = 2 * Projectile.scale;
            List<Vector2> points = new();
            for (int i = 1; i < oldPos.Count; i++)
            {
                for (float j = 0.1f; j <= 1f; j += 0.1f)
                {
                    points.Add(Vector2.Lerp(oldPos[i - 1], oldPos[i], j));
                }
            }
            for (int i = 0; i < points.Count; i++)
            {
                float c = (i + 1f) / points.Count;
                DrawDark(points[i], scale * c * 0.6f, Projectile.Opacity * c);
            }
            for (int i = 0; i < points.Count; i++)
            {
                float c = (i + 1f) / points.Count;
                DrawEnergyBall(points[i], scale * c, Projectile.Opacity * c);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public void DrawEnergyBall(Vector2 pos, float size, float alpha)
        {
            Texture2D tex = CEUtils.getExtraTex("a_circle");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(80, 50, 50) * alpha, Projectile.rotation, tex.Size() * 0.5f, new Vector2(1 + (Projectile.velocity.Length() * 0.01f), 1) * size * 0.25f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, new Color(36, 2, 2) * alpha, Projectile.rotation, tex.Size() * 0.5f, new Vector2(1 + (Projectile.velocity.Length() * 0.01f), 1) * size * 0.4f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.begin_();
        }
        public void DrawDark(Vector2 pos, float size, float alpha)
        {
            CEUtils.DrawGlow(pos, Color.Black * alpha * Projectile.Opacity, size, false);
        }
    }
}
