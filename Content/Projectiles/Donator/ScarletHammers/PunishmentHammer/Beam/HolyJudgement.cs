using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.PunishmentHammer.Beam
{
    public class HolyJudgement : BaseScarletProj
    {
        public override string Texture => CEUtils.InvisAsset;
        private int DrawCount = 0;
        private ref float Counter => ref Projectile.ai[0];
        private float OriginalSpeed => Projectile.ai[1];
        private ref float Rotation => ref Projectile.ai[2];
        private float MountedX => Projectile.localAI[0];
        private float MountedY => Projectile.localAI[1];
        private float InitVec = 0;
        public override void ExSD()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            //这玩意是转起来的，所以实际dps会更少的，给他多点判定吧！！
            Projectile.localNPCHitCooldown = 10;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.scale = 1f;
            Projectile.timeLeft = 80;

        }
        float opc = 1;
        public override void AI()
        {
            //刚加入时的初始化。
            if (Counter == 0)
            {
                SoundEngine.PlaySound(new("CalamityEntropy/Assets/Sounds/angel_blast1"), Projectile.Center);
                InitVec = Projectile.velocity.ToRotation();
                Projectile.rotation = InitVec;
            }
            Lighting.AddLight(Projectile.Center, TorchID.White);
            Counter++;
            //让这个东西绕着转一会……
            Rotation += MathHelper.ToRadians(2);
            //增加这个……转角。
            float curRot = InitVec + Rotation;
            //最后算速度。和一些别的。
            Projectile.velocity = curRot.ToRotationVector2() * OriginalSpeed;
            //转角处理。
            Projectile.rotation = Projectile.velocity.ToRotation();
            //维持悬挂让他跟随敌对单位
            Projectile.Center = new Vector2(MountedX, MountedY);
            if (Counter > 60)
            {
                opc -= 1f / 20f;
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 900, targetHitbox, 24);
        }
        SpriteBatch SB { get => Main.spriteBatch; }
        public override bool PreDraw(ref Color lightColor)
        {

            DrawCount++;
            SB.End();
            SB.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            //贴图。
            Texture2D warn = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/vlbw").Value;
            Vector2 ori = warn.Size() / 2 * new Vector2(0, 1);
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //底部颜色
            Color backGroundColor = Color.Yellow * opc;
            //辉光颜色
            Color bloomColor = (DrawCount / 2 % 2 is 0 ? Color.White : Color.Yellow) * opc;
            //基础大小设定
            Vector2 baseScale = Projectile.scale * 1.46f * new Vector2(1, opc);
            //底部大小
            Vector2 backScale = baseScale * new Vector2(10, 1.2f);
            //辉光大小
            Vector2 bloomScale = baseScale * new Vector2(10, 1f);
            //实际绘制。
            SB.Draw(warn, drawPos, null, backGroundColor, Projectile.rotation, ori, backScale, SpriteEffects.None, 0);
            SB.Draw(warn, drawPos, null, bloomColor, Projectile.rotation, ori, bloomScale, SpriteEffects.None, 0);

            SB.End();
            SB.BeginDefault();

            return false;
        }
    }
}
