using CalamityEntropy.Assets.Register;
using InfernumMode.Content.BehaviorOverrides.BossAIs.WallOfFlesh;
using InfernumMode.Content.Dusts;
using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.FallenHammer
{
    public class FallenVolcano :BaseScarletProj
    {
        //写在全局里防止局部多次调用。
        private int InitPhase = 10;
        private int EarlyPhase = 20;
        private int MiddlePhase = 40;
        public override string Texture => CEUtils.InvisAsset;
        private ref float FirePillerTimer => ref Projectile.ai[0];
        public override void ExSD()
        {
            Projectile.timeLeft = 80;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.friendly = true;
            Projectile.height = Projectile.width = 2;
        }
        public override bool ShouldUpdatePosition() => false;
        public override void AI()
        {
            PickTagDust(out short HigherDust, out short BottemDust);
            FirePillerTimer++;
            Vector2 scale = GetScaleFromAI();
            if (FirePillerTimer < InitPhase)
            {

            }
            else if (FirePillerTimer < EarlyPhase)
            {
                for (int i = 0; i < 5; i++)
                {
                    float rnd = Main.rand.NextFloat(-1, 1);
                    Dust side = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? HigherDust : BottemDust);
                    side.noGravity = true;
                    side.position.X += rnd * 5 * scale.X;
                    side.velocity = new Vector2(rnd * 15, -Main.rand.NextFloat(3));
                    side.scale *= 2f;
                }
            }
            else if (FirePillerTimer < MiddlePhase)
            {
                if (FirePillerTimer == 20)
                {
                    SoundEngine.PlaySound(SoundID.Item103, Projectile.Center);
                    SlotId slotId = SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);
                    if (SoundEngine.TryGetActiveSound(slotId, out var sound)) 
                        sound.Volume /= 2;
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 vel = new Vector2(0f, -Main.rand.NextFloat(14f, 18f)).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4 / 3, MathHelper.PiOver4 / 3));
                        Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), new Vector2(Projectile.Center.X, Projectile.Center.Y - Main.rand.NextFloat(160f, 320f)), vel, ModContent.ProjectileType<FallenEruption>(), Projectile.damage, Projectile.knockBack);
                        proj.timeLeft = 300;
                        proj.ai[0] = 20f;
                    }
                }
                if (FirePillerTimer == 30)
                {
                    SlotId slotId = SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
                    if (SoundEngine.TryGetActiveSound(slotId, out var sound)) 
                        sound.Volume /= 2;
                }
                for (int i = 0; i < 30; i++)
                {
                    float rnd = Main.rand.NextFloat(-1, 1);
                    Dust side = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? HigherDust : BottemDust);
                    side.noGravity = true;
                    side.position.X += rnd * 5 * scale.X;
                    side.velocity = new Vector2(rnd * 15, -Main.rand.NextFloat(2));
                    side.scale *= 2f;
                }
                for (int i = 0; i < 60; i++)
                {
                    float height = Main.rand.NextFloat();
                    Dust flame = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextFloat() >= height ? HigherDust: BottemDust);
                    flame.noGravity = true;
                    flame.position += new Vector2(Main.rand.NextFloat(height - 1, 1 - height) * 5 * scale.X, -height * 21 * scale.Y);
                    flame.velocity.Y -= 10f;
                    if (Main.rand.NextBool(4))
                    {
                        flame.position.X -= 12;
                        flame.velocity.X += 0.02f;
                        flame.scale *= 2f;
                    }
                    else 
                        flame.velocity.Y -= Main.rand.NextFloat(5f, 12f);
                }
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    float rnd = Main.rand.NextFloat(-1, 1);
                    Dust side = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? HigherDust : BottemDust);
                    side.noGravity = true;
                    side.position.X += rnd * 5 * scale.X;
                    side.velocity = new Vector2(rnd * 5, -Main.rand.NextFloat());
                    side.scale *= 2f;
                }
            }
        }
        private void PickTagDust(out short HigherDust, out short BottemDust)
        {
            string name = Owner.name.ToLower();
            if (name.Contains("polaris"))
                name = "polaris";
            switch (name)
            {
                case "scarletshelf":
                case "truescarlet":
                case "fakeaqua":
                    HigherDust = DustID.CrimsonTorch;
                    BottemDust = DustID.RedTorch;
                    break;
                case "chalost":
                case "查诗络":
                    BottemDust = DustID.GoldCoin;
                    HigherDust = DustID.YellowTorch;
                    break;
                case "yinjiu":
                case "银九":
                    HigherDust = DustID.PinkTorch;
                    BottemDust = DustID.WhiteTorch;
                    break;
                case "kino":
                    HigherDust = DustID.BlueTorch;
                    BottemDust = DustID.GemSapphire;
                    break;
                case "锯角":
                    HigherDust = DustID.PurpleTorch;
                    BottemDust = DustID.WhiteTorch;
                    break;
                case "polaris":
                    HigherDust = DustID.PurpleTorch;
                    BottemDust = DustID.CrimsonTorch;
                    break;
                case "fr9ezes":
                    HigherDust = DustID.JungleTorch;
                    BottemDust = DustID.GreenTorch;
                    break;
                case "kikastorm":
                case "kika":
                    HigherDust = DustID.WhiteTorch;
                    BottemDust = DustID.ShimmerTorch;
                    break;
                default:
                    HigherDust = DustID.OrangeTorch;
                    BottemDust = DustID.Torch;
                    break;
            }
        }
        private void PickTagColor(out Color baseColor, out Color targetColor)
        {
            string name = Owner.name.ToLower();
            if (name.Contains("polaris"))
                name = "polaris";
            switch (Owner.name.ToLower())
            {
                //绯红书架 - 红
                case "scarletshelf":
                case "truescarlet":
                case "polaris":
                case "fakeaqua":
                    baseColor = Color.LightCoral;
                    targetColor = Color.Red;
                    break;
                //查 -- 金
                case "chalost":
                case "查诗络":
                    baseColor = Color.Goldenrod;
                    targetColor = Color.White;
                    break;
                //银九 - 粉
                case "yinjiu":
                case "银九":

                    baseColor = Color.HotPink;
                    targetColor = Color.Pink;
                    break;
                //Kino - 蓝
                case "kino":
                    baseColor = Color.RoyalBlue;
                    targetColor = Color.LightBlue;
                    break;
                //聚胶 - 紫
                case "锯角":
                    baseColor = Color.DarkViolet;
                    targetColor = Color.White;
                    break;
                //绿
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
                    baseColor = Color.OrangeRed;
                    targetColor = Color.Orange;
                    break;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) =>
           targetHitbox.Intersects(Utils.CenteredRectangle(Projectile.Center - Vector2.UnitY * 10.5f * GetScaleFromAI().X, new Vector2(10, 21) * GetScaleFromAI()));
        public override bool? CanDamage()
        {
            return base.CanDamage();
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust burst = Dust.NewDustPerfect(target.Center, GetDust);
                if (FirePillerTimer < 40) 
                    burst.velocity -= Vector2.UnitY * 5f;
            }
        }
        private int GetDust
        {
            get
            {
                PickTagDust(out short HigherDust, out short BottemDust);
                return (Main.rand.NextFloat() < FirePillerTimer / 80f) ? BottemDust : HigherDust;
            }
        }
        //痴情不是罪过，忘情不是洒脱，为你想的撕心裂肺有什么结果！
        //你说到底为什么，都是我的错，都把爱情想得太美现实太诱惑，到底为什么，让你更难过，这样爱你除了安慰还能怎么做
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Extra[ExtrasID.SharpTears].Value;
            Rectangle cutSource = tex.Bounds;
            //切边。
            cutSource.Height /= 2;
            //重新设定原点
            Vector2 ori = new Vector2(cutSource.Width / 2, cutSource.Height);
            Vector2 scale = GetScaleFromAI();
            //最底层绘制纯黑射弹模拟黑烟
            DrawSmoke(tex, scale, cutSource, ori);
            CEUtils.ReSetToBeginShader();
            //实际绘制火柱，套用shader
            DrawPillar(tex, scale, cutSource, ori);
            CEUtils.ReSetToEndShader();
            return false;
        }
        private void DrawSmoke(Texture2D tex, Vector2 scale, Rectangle cutSource, Vector2 ori)
        {
            //叠图5次以增强层次感 
            for (int j = 0; j < 5; j++)
            {
                //以1起步进行多次绘制缩放
                for (float k = 1; k >= 0; k -= 0.1f)
                {
                    Color smoke = Color.Lerp(Color.Black, Color.Lerp(Color.Black, new Color(5, 5, 5), FirePillerTimer / 40f), k) * 0.4f;
                    smoke.A /= 2;
                    Vector2 scale3 = scale * new Vector2((1 - k) * 0.55f, k * 0.4f);
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, cutSource, smoke, 0f, ori, scale3, SpriteEffects.None, 0);
                }
            }
        }
        private void DrawPillar(Texture2D tex, Vector2 scale, Rectangle cutSource, Vector2 ori)
        {
            PickTagColor(out Color baseColor, out Color tarColor);
            Effect shader = EntropyShaderHandler.DisplacemenShader;
            if (Owner.name.ToLower().Contains("polaris"))
            {
                shader = EntropyShaderHandler.DisplacemenShaderP;
                shader.Parameters["uBaseColor1"].SetValue(new Color(70, 8, 255).ToVector4() * 0.3f);
                shader.Parameters["uTargetColor1"].SetValue(new Color(120, 60, 255).ToVector4() * 0.9f);
                shader.Parameters["uBaseColor2"].SetValue(Color.Crimson.ToVector4() * 0.3f);
                shader.Parameters["uTargetColor2"].SetValue(new Color(255, 150, 150).ToVector4() * 0.9f);
            }
            else
            {
                shader.Parameters["uBaseColor"].SetValue(baseColor.ToVector4() * 0.3f);
                shader.Parameters["uTargetColor"].SetValue(tarColor.ToVector4() * 0.9f);
            }
            shader.Parameters["uTime"].SetValue(-Main.GlobalTimeWrappedHourly * 45);
            shader.Parameters["uIntensity"].SetValue(0.20f);
            
            shader.Parameters["uNoiseScale"].SetValue(new Vector2(8f, 5f));
            shader.Parameters["UseColor"].SetValue(true);
            shader.Parameters["uFadeRange"].SetValue(0.6f);
            shader.CurrentTechnique.Passes[0].Apply();
            Main.graphics.GraphicsDevice.Textures[1] = TextureRegister.Noise_Misc2.Value;

            //叠图4次以增强层次感 
            for (int j = 0; j < 4; j++)
            {
                //以1起步进行多次绘制缩放
                for (float i = 1; i >= 0; i -= 0.1f)
                {
                    EntropyShaderHandler.DisplacemenShader.Parameters["uColorFactor"].SetValue(i);
                    Color drawColor = Color.White;
                    drawColor.A /= 2;
                    Vector2 scale2 = scale * new Vector2(1 - i, i);
                    Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, cutSource, drawColor, 0f, ori, scale2, SpriteEffects.None, 0);
                }
            }

        }
        //封装一个缩放进程方法
        private Vector2 GetScaleFromAI()
        {
            Vector2 scale;
            Vector2 init = new(0, 6);
            Vector2 early = new(2, 10);
            Vector2 middle = new(30, 5);
            Vector2 late = new(15, 80);
            Vector2 end = new(0, 20);
            if (FirePillerTimer < InitPhase)
            {
                scale = Vector2.SmoothStep(init, early, FirePillerTimer / InitPhase);
            }
            else if (FirePillerTimer < EarlyPhase)
            {
                scale = Vector2.SmoothStep(early, middle, (FirePillerTimer - InitPhase)/ InitPhase);
            }
            else if (FirePillerTimer < MiddlePhase)
            {
                scale = Vector2.SmoothStep(middle, late, (FirePillerTimer - EarlyPhase) / EarlyPhase);
            }
            else
            {
                scale = Vector2.SmoothStep(late, end, (FirePillerTimer - MiddlePhase) / MiddlePhase);
            }
            return scale;
        }
    }
}
