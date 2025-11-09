using CalamityEntropy.Core.Construction;
using CalamityMod;
using Microsoft.Build.Construction;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities.Terraria.Utilities;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.PunishmentHammer.ExtraProj
{
    public class PunishmentStarMounted : ModProjectile,ILocalizedModType
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/StarTexture";
        public new string LocalizationCategory => "Projectiles.Rogue";
        private ref float MountedX => ref Projectile.localAI[0];
        private ref float MountedY => ref Projectile.localAI[1];
        public ref float AttackTimer => ref Projectile.Entropy().ExtraProjAI[0];
        private ref float AniProgress => ref Projectile.ai[0];
        private ref float MountedIndex => ref Projectile.ai[2];
        private static int PureStarID => ModContent.ProjectileType<PunishmentStar>();
        private bool ShouldGrowUp = true;
        private Player Owner => Projectile.GetOwner();
        public override bool ShouldUpdatePosition() => false;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.knockBack = 0;
        }
        public override bool? CanDamage() => false;
        public override void AI()
        {
            Projectile mountedProj = Main.projectile[(int)MountedIndex];
            if (!Owner.dead && mountedProj.active)
                Projectile.timeLeft = 2;
            //时刻更新射弹位置为原本挂载锤子的中心点
            //没了。
            Projectile.Center = new Vector2(MountedX, MountedY);
            Projectile.rotation += MathHelper.ToRadians(1.2f);
            //总控圣光新星挂载射弹的放缩
            // wtf.
            if(ShouldGrowUp)
            {
                float t = AttackTimer / 30f;
                float progress = EasingHandler.EaseInOutExpo(t);
                AttackTimer += 1f;
                AniProgress = progress.ToClamp();
                if (t >= 1f)
                {
                    AttackTimer = 10f;
                    AniProgress = 1f;
                    ShouldGrowUp = false;
                    CanSpawnHolyStar();
                }
            }
            else
            {
                float t = AttackTimer / 10f;
                float progress = EasingHandler.EaseInOutExpo(t);
                AttackTimer -= 1f;
                AniProgress = progress.ToClamp();
                if (t <= 0f)
                {
                    AttackTimer = 0f;
                    AniProgress = 0f;
                    ShouldGrowUp = true;
                }
            }
            
        }
        private void CanSpawnHolyStar()
        {
            Projectile.Center.CirclrDust(36, Main.rand.NextFloat(1.2f, 1.8f), DustID.HallowedWeapons, 4, 16f);
            SoundEngine.PlaySound(SoundID.Item82 with { Pitch = 0.8f }, Projectile.Center);
            const int TotalProjCounts = 4;
            float angleStep = MathHelper.TwoPi / TotalProjCounts;
            float beginAngle = Projectile.rotation;
            for (int i = 0; i < TotalProjCounts; i++)
            {
                float totalOffset = i * angleStep;
                Vector2 dir = Vector2.UnitX.RotatedBy(beginAngle + totalOffset);
                Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, dir * 8f, PureStarID, (int)(Projectile.damage * 0.80f), Projectile.knockBack, Projectile.owner);
                //标记ai2为1f，让这个射弹在发起追踪前进行一段圆弧运动
                proj.ai[2] = 1f;
            }
            Projectile.netUpdate = true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 backgroundScale = new Vector2(1.0f, 1.0f);
            Vector2 bloomScale = new Vector2(0.5f, 0.5f);
            //这里的放缩会被lerp进行一次总控。
            Vector2 dynamicBackgroundScale = Vector2.Lerp(Vector2.Zero, backgroundScale, AniProgress) *Projectile.scale * 1.1f;
            Vector2 dynamicBloomScale = Vector2.Lerp(Vector2.Zero, bloomScale, AniProgress) *Projectile.scale * 1.1f;
            //最后我们实际绘制他。
            //前提是我们需要重置他的绘制批次。
            Texture2D projTex =  ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/StarTexture").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(projTex, drawPos, null, Color.Gold, Projectile.rotation, projTex.Size() / 2, dynamicBackgroundScale, SpriteEffects.None, 0.1f);
            Main.spriteBatch.Draw(projTex, drawPos, null, Color.White, Projectile.rotation, projTex.Size() / 2, dynamicBloomScale, SpriteEffects.None, 0.1f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}