using CalamityEntropy.Core.Construction;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.PunishmentHammer.Beam
{
    public class HolyJudgementMounted : BaseScarletProj
    {
        public override string Texture => CEUtils.InvisAsset;
        
        private enum DoType
        {
            Begin,
            End,
            Extra
        }
        public int TargetIndex
        {
            get => (int)Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[2];
            set => Projectile.ai[2] = (short)value;
        }
        private ref float GeneralProgress => ref Projectile.localAI[0];
        private float BeginAniTime = 15f;
        private float EndAniTime = 30f;
        private List<int> BeamList = [];
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionShot[Type] = true;
        }
        public override void ExSD()
        {
            Projectile.height = Projectile.width = 2;
            Projectile.friendly = true;
            Projectile.extraUpdates = 0;
            Projectile.ignoreWater = true;
            Projectile.noEnchantments = true;
            Projectile.noEnchantmentVisuals = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            //让这个东西全局跟随转角动画
            Projectile.rotation += MathHelper.ToRadians(1);
            //全局获取敌对单位的位置并时刻更新。
            if (Projectile.GetTargetSafe(out NPC target, TargetIndex, false))
                Projectile.Center = target.Center;

            switch(AttackType)
            {
                case DoType.Begin:
                    DoBeginAI();
                    break;
                case DoType.End:
                    DoEndAI();
                    break;
                case DoType.Extra:
                    DoExta();
                    break;
            }
        }

        

        private void DoBeginAI()
        {
            //这里的generalprogress会被动画受限并不断累积至1.
            GeneralProgress += EasingHandler.EaseInOutExpo(AttackTimer / BeginAniTime);
            GeneralProgress = GeneralProgress.ToClamp();
            AttackTimer++;
            if(GeneralProgress >= 1f)
            {
                AttackType = DoType.End;
                AttackTimer = 0f;
                Projectile.netUpdate = true;

            }
        }
        private void DoEndAI()
        {
            //此处需要逆转进程，方便predraw内的放缩绘制
            GeneralProgress -= EasingHandler.EaseInCubic(AttackTimer / EndAniTime);
            GeneralProgress = GeneralProgress.ToClamp();
            AttackTimer++;
            if (GeneralProgress <= 0f)
            {
                //处死时释放
                for (int i = 0; i < 4; i++)
                {
                    //666我还要存他们的数组信息来着
                    float curRadians = MathHelper.ToRadians(MathHelper.ToDegrees(Projectile.rotation) + 90f * i);
                    Vector2 velocity = curRadians.ToRotationVector2() * 16;
                    Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<HolyJudgement>(), (int)(Projectile.damage * 0.35f), 0f, Projectile.owner);
                    proj.DamageType = CEUtils.RogueDC;
                    proj.ai[1] = 16f;
                    proj.Calamity().stealthStrike = true;
                    proj.rotation = curRadians;
                    proj.localAI[0] = Projectile.Center.X;
                    proj.localAI[1] = Projectile.Center.Y;
                    BeamList.Add(proj.whoAmI);
                }
                //延后更新
                AttackType = DoType.Extra;
                AttackTimer = 0f;
                Projectile.netUpdate = true;
            }
        }
        private void DoExta()
        {
            //等待一段时间后自然死亡。
            //遍历所有可能存在的射弹
            int[] array = [.. BeamList];
            for (int i =0; i < array.Length;i++)
            {
                //这里写的一堆shit本质上是为了更新四个射线的中心点。
                Main.projectile[array[i]].localAI[0] = Projectile.Center.X;
                Main.projectile[array[i]].localAI[1] = Projectile.Center.Y;
            }
            AttackTimer += 1f;
            if (AttackTimer > 10f && Owner.ownedProjectileCounts[ModContent.ProjectileType<HolyJudgement>()] < 1)
                Projectile.Kill();
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 backgroundScale = new Vector2(1.4f, 1.4f);
            Vector2 bloomScale = new Vector2(0.8f, 0.8f);
            //这里的放缩会被lerp进行一次总控。
            Vector2 dynamicBackgroundScale = Vector2.Lerp(Vector2.Zero, backgroundScale, GeneralProgress) *Projectile.scale * 1.1f;
            Vector2 dynamicBloomScale = Vector2.Lerp(Vector2.Zero, bloomScale, GeneralProgress) *Projectile.scale * 1.1f;
            //最后我们实际绘制他。
            //前提是我们需要重置他的绘制批次。
            Texture2D projTex =  ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/StarTexture").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(projTex, drawPos, null, Color.Yellow, Projectile.rotation, projTex.Size() / 2, dynamicBackgroundScale, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(projTex, drawPos, null, Color.White, Projectile.rotation, projTex.Size() / 2, dynamicBloomScale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}
