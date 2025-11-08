using CalamityEntropy.Assets.Register;
using CalamityEntropy.Utilities;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.NightmareHammer.ExtraProj
{
    public class DarkEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => CEUtils.InvisAsset;
        //完全重做这个玩意的AI
        private ref float AttackTimer => ref Projectile.ai[0];
        private int TargetIndex
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        private DoType AttackType
        {
            get =>(DoType)Projectile.ai[2];
            set => Projectile.ai[2] = (short)value;
        }
        private Player Owner => Projectile.GetOwner();
        private enum DoType
        {
            IsSpawned,
            IsHomingToOwner,
            IsChasingToTarget
        }
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 12;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 6;
            Projectile.penetrate = 3;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, TorchID.White);
            switch (AttackType)
            {
                case DoType.IsSpawned:
                    DoSpawned();
                    break;
                case DoType.IsHomingToOwner:
                    DoHomingToOwner();
                    break;
                case DoType.IsChasingToTarget:
                    DoChasingToTarget();
                    break;
            }
        }
        private void DoSpawned()
        {
            //生成逻辑。
            AttackTimer += 1;
            float progress = AttackTimer / 20f;
            Projectile.scale = MathHelper.Lerp(0f, 1f, progress).ToClamp();
            Projectile.Opacity = MathHelper.Lerp(0f, 1f, progress).ToClamp();
            Lighting.AddLight(Projectile.Center, TorchID.White);
            if(AttackTimer > 20f)
            {
                Projectile.scale = 1;
                Projectile.Opacity = 1;
                AttackType = DoType.IsHomingToOwner;
                Projectile.netUpdate = true;
                AttackTimer = 0f;
            }
        }
        private void DoHomingToOwner()
        {
            AttackTimer = AttackTimer + 0.05f * 20;
            Projectile.AccelerateToTarget(Owner.Center, 20f + AttackTimer, 1.2f);
            if(Projectile.Hitbox.Intersects(Owner.Hitbox))
            {
                AttackType = DoType.IsChasingToTarget;
                AttackTimer = 0f;
                Projectile.netUpdate = true;
            }

        }
        private void DoChasingToTarget()
        {
            if(Projectile.GetTargetSafe(out NPC target, TargetIndex, true))
                Projectile.HomingNPCBetter(target, 24f, 20f, 1);
        }
        private SpriteBatch SB { get => Main.spriteBatch; }
        private GraphicsDevice GD { get => Main.graphics.GraphicsDevice; }
        public override bool PreDraw(ref Color lightColor)
        {
            
            SB.End();
            SB.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //我踩到地雷了孩子们。
            DrawTrail(new Color(75,0,130), 10.5f);
            DrawTrail(Color.Black, 10f);
            DrawTrail(Color.Black, 9.8f);
            DrawTrail(Color.Black, 9.5f);
            SB.End();
            SB.BeginDefault();
            return false;
        }
        private void DrawTrail(Color color, float height, int width = 0)
        {
            //做掉可能存在的零向量。
            Projectile.ClearInvalidPoint(out List<Vector2> validPos, out List<float> validRot, Projectile.oldPos, Projectile.oldRot);
            BezierCurveHandler.GetValidBeizerCurvePow(validPos, validRot, out List<Vector2> smoothPos, out List<float> smoothRot, 4);
            List<VertexPosition2DColorTexture> list = [];
            //创建顶点列表
            for (int i = 0; i < smoothPos.Count; i++)
            {
                Vector2 worldCenter = smoothPos[i] + Projectile.Size / 2;
                Vector2 oldCenter = worldCenter - Main.screenPosition;
                float progress = (float)i / (smoothPos.Count - 1);
                Vector2 posOffset = new Vector2(0, height).RotatedBy(smoothRot[i]);
                VertexPosition2DColorTexture upClass = new(oldCenter + posOffset, color, new Vector2(progress, 1), 0);
                VertexPosition2DColorTexture downClass = new(oldCenter - posOffset, color, new Vector2(progress, 0), 0);
                list.Add(upClass);
                list.Add(downClass);
            }
            if (list.Count >= 3)
            {
                GD.Textures[0] = TextureRegister.Trail_RvSlash.Value;
                GD.DrawUserPrimitives(PrimitiveType.TriangleStrip, list.ToArray(), 0, list.Count - 2);
            }
        }

    }
}