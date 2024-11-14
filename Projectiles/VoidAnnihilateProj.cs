using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Projectiles
{
    public class VoidAnnihilateProj : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 68;
            Projectile.height = 68;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2600;
        }
        public bool sInit = true;
        public Vector2 getTargetPos()
        {
            return new Vector2(Projectile.ai[1], Projectile.ai[2]);
        }
        public bool mprdLast = true;
        public bool playerComeimg = false;
        public override void AI()
        {
            bool MouseRight = Mouse.GetState().RightButton == ButtonState.Pressed;
            if (!playerComeimg)
            {
                if (Projectile.ai[0] > 0)
                {
                    if (MouseRight && !mprdLast && new Rectangle((int)Main.MouseWorld.X, (int)Main.MouseWorld.Y, 1, 1).Intersects(Projectile.getRect()))
                    {
                        playerComeimg = true;
                        SoundEngine.PlaySound(new("CalamityEntropy/Sounds/teleport"), Projectile.Center);
                    }
                }
            }
            else
            {
                Player player = Projectile.owner.ToPlayer();
                player.Center += (Projectile.Center - player.Center).SafeNormalize(Vector2.Zero) * 80;

                player.eocDash = 6;
                player.Entropy().VaMoving = 5;
                if (Util.Util.getDistance(player.Center, Projectile.Center) < 90)
                {
                    player.velocity = (Projectile.Center - player.Center).SafeNormalize(Vector2.Zero) * 14;
                    player.Center = Projectile.Center;
                    playerComeimg = false;
                    Projectile.Kill();
                }
            }
            mprdLast = MouseRight;
            if (sInit)
            {
                sInit = false;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.netUpdate = true;
                    Projectile.ai[1] = Main.MouseWorld.X;
                    Projectile.ai[2] = Main.MouseWorld.Y;

                }
                
            }

            if (Projectile.ai[0] == 0)
            {
                Projectile.velocity = (getTargetPos() - Projectile.Center).SafeNormalize(Vector2.Zero) * 54;
                Projectile.rotation = Projectile.velocity.ToRotation();
                if (Util.Util.getDistance(Projectile.Center, getTargetPos()) < 56)
                {
                    Projectile.Center = getTargetPos();
                    Projectile.velocity *= 0;
                    Projectile.ai[0]++;
                }
                
            }
            else
            {
                Projectile.ai[0] += 1;
                if (Projectile.ai[0] < 300)
                {
                    Projectile.rotation += 0.6f;
                    if (Projectile.ai[0] % 25 == 0)
                    {
                        if (Projectile.owner == Main.myPlayer)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(10, 0).RotatedBy(Main.rand.NextDouble() * Math.PI * 2), ModContent.ProjectileType<VaProj>(), Projectile.damage / 2, 6, Projectile.owner);
                        }
                    }
                }
                else
                {
                    backing = true;
                }
            }
            if (Main.myPlayer == Projectile.owner)
            {
                if (CEKeybinds.RetrieveVoidAnnihilateHotKey.JustPressed)
                {
                    if (!backing)
                    {
                        Projectile.damage *= 2;

                        Projectile.ai[0] = 400;
                        backing = true;
                        Projectile.netUpdate = true;
                    }
                }
            }
            if (backing)
            {
                Projectile.rotation = Projectile.velocity.ToRotation();
                Projectile.velocity += (Projectile.owner.ToPlayer().Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 1.4f;
                Projectile.velocity = new Vector2(Projectile.velocity.Length(), 0).RotatedBy((Projectile.owner.ToPlayer().Center - Projectile.Center).ToRotation());
                if (Util.Util.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center) < Projectile.velocity.Length() * 1.2f)
                {
                    Projectile.Kill();
                }
            }
            
        }
        public bool backing = false;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(backing);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            backing = reader.ReadBoolean();
        }
        public int counter = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            counter -= 2;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D chaintx = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak").Value;
            Main.spriteBatch.Draw(chaintx, Projectile.Center - Main.screenPosition, new Rectangle(counter, 0, (int)Util.Util.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center), chaintx.Height), Color.Purple, (Projectile.owner.ToPlayer().Center - Projectile.Center).ToRotation(), new Vector2(0, chaintx.Height) / 2, new Vector2(1, 0.4f), SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
            return false;
        }
    }
}
