using CalamityEntropy.Assets.Register;
using CalamityEntropy.Core.Construction;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Placeables.Walls;
using CalamityMod.Particles;
using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.PunishmentHammer.ExtraProj
{
    public class PunishmentStar : BaseScarletProj
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/StarTexture";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        private enum DoType
        {
            IsShooted,
            IsHomingToTarget,
            IsFading
        }
        private DoType AttackType
        {
            get => (DoType)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        private ref float AttackTimer => ref Projectile.ai[1];
        private int UseArcRotating
        {
            get => (int)Projectile.ai[2];
            set => Projectile.ai[2] = value;
        }
        private ref float InitRot => ref Projectile.localAI[0];
        private bool CanHitTarget = false;
        private bool CanArcRotate => UseArcRotating == 1f;
        private float DrawScale = 1f;
        public override void ExSD()
        {
            Projectile.extraUpdates = 1;
            Projectile.height = Projectile.width = 6;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            //这里的判定也是为了演出效果+dps，重点是演出效果
            //所以做平衡的也别tm动这！！
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 150;
        }
        public override bool? CanDamage() => CanHitTarget;
        public override void AI()
        {
            //下方会进行手动处死，这里需要全程保证射弹存活维持演出效果
            //所以做平衡的别他妈动这里！！！
            Projectile.timeLeft = 2;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if ((Projectile.Center - Owner.Center).Length() > 1800f)
                Projectile.Kill();
            switch (AttackType)
            {
                case DoType.IsShooted:
                    DoShooted();
                    break;
                case DoType.IsHomingToTarget:
                    DoHomingToTarget();
                    break;
                case DoType.IsFading:
                    DoFading();
                    break;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType == DoType.IsHomingToTarget)
            {
                Projectile.netUpdate = true;
                if (CanArcRotate && Projectile.numHits > 1)
                    AttackType = DoType.IsHomingToTarget;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;
            return false;
        }
        private SpriteBatch SB { get => Main.spriteBatch; }
        private GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projTex = Projectile.GetTexture();
            Vector2 ori = projTex.Size() / 2;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Vector2 backscale = 0.36f * Projectile.scale * new Vector2(0.8f, 0.4f) * DrawScale;
            Vector2 bloomScale = 0.36f * Projectile.scale * new Vector2(0.4f, 0.2f) * DrawScale;

            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            SB.Draw(projTex, drawPos, null, Color.Gold, Projectile.rotation, ori, backscale, SpriteEffects.None, 0);
            SB.Draw(projTex, drawPos, null, Color.White, Projectile.rotation, ori, bloomScale, SpriteEffects.None, 0);

            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            DrawTrail(Color.Gold, 6f);
            DrawTrail(Color.White, 3f); 

            SB.End();
            SB.BeginDefault();

            return false;
        }
        private void DrawTrail(Color drawColor, float height)
        {
            Projectile.ClearInvalidPoint(out List<Vector2> validPos, out List<float> validRot);
            BezierCurveHandler.GetValidBeizerCurvePow(validPos, validRot, out List<Vector2> smoothPosList, out List<float> smootRotList, 4);
            List<VertexPosition2DColorTexture> list = [];
            for (int i = 0; i < smoothPosList.Count; i++)
            {
                Vector2 worldCenter = smoothPosList[i] + Projectile.Size / 2;
                Vector2 oldCenter = worldCenter - Main.screenPosition;
                float progress = (float)i / (smoothPosList.Count - 1);
                //处理一下起始顶点，宽度小一点
                float handleHeight = i < 6 ? height / 6 * i : height;
                handleHeight *= DrawScale;
                Vector2 posOffset = new Vector2(0, handleHeight).RotatedBy(smootRotList[i]);
                Color color = drawColor * (1f - progress * 0.5f);
                VertexPosition2DColorTexture upClass = new(oldCenter + posOffset, color, new Vector2(progress, 0), 0f);
                VertexPosition2DColorTexture downClass = new(oldCenter - posOffset, color, new Vector2(progress, 1), 0f);
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
            {
                //这里这个轨迹大部分都被预存了。
                GD.Textures[0] = TextureRegister.Trail_Streak1w.Value;
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
                GD.Textures[0] = null;
            }

        }

        #region AI方法合集
        private void DoShooted()
        {
            AttackTimer += 1f;
            if (AttackTimer < 30f * Projectile.extraUpdates)
                return;

            Projectile.netUpdate = true;
            AttackTimer = 0f;
            AttackType = DoType.IsHomingToTarget;
            //临时注册一次
            InitRot = Projectile.rotation;
            CanHitTarget = false;
        }
        private void HandleHoming()
        {
            Projectile.tileCollide = false;
            AttackTimer = 15f;
            //允许造成伤害
            CanHitTarget = true;
            //正式执行追踪AI
            if (Projectile.GetTargetSafe(out NPC target, 0, true))
            {
                Projectile.HomingNPCBetter(target, 20f, 20f, 1);
            }
            else
                Projectile.Kill();
            

        }
        private void DoHomingToTarget()
        {
            AttackTimer += 1;
            //除非整个过程完成，否则不执行追踪AI
            float progress = AttackTimer / 8f;
            //progress没进去
            //潜伏状态下，神圣新星才会转圈
            bool canHome = !CanArcRotate || progress > 1f && CanArcRotate;
            if(canHome)
                HandleHoming();
            else
            { 
                progress = progress.ToClamp();
                float finalRotation = InitRot + MathHelper.Pi * progress;
                //将转角实际转化为需要的速度
                Projectile.velocity = finalRotation.ToRotationVector2() * 12f;
            }
        }
        //这个Fading什么都不会做，因为射弹的处死本身由timeleft手动控制
        //好吧还是会做点。
        private void DoFading()
        {
            float progress = AttackTimer / 15f;
            DrawScale = EasingHandler.EaseOutCubic(progress).ToClamp();
            AttackTimer--;
        }
        #endregion
    }
}
