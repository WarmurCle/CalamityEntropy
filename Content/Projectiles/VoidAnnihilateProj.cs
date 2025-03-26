using CalamityEntropy.Common;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class VoidAnnihilateProj : ModProjectile
    {

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
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
        int select = -1;



        public override void AI()
        {
            bool MouseRight = Mouse.GetState().RightButton == ButtonState.Pressed;
            if (!playerComeimg)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    select = -1;
                    float dist = 5000;
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.active && p.type == Projectile.type)
                        {
                            if (Util.Util.getDistance(Main.MouseWorld, p.Center) < dist)
                            {
                                select = p.whoAmI;
                                dist = Util.Util.getDistance(Main.MouseWorld, p.Center);
                            }
                        }
                    }
                    if (Projectile.ai[0] > 0 && select == Projectile.whoAmI && !Main.LocalPlayer.HasBuff(BuffID.ChaosState))
                    {
                        if (MouseRight && !mprdLast)
                        {
                            Main.LocalPlayer.AddBuff(BuffID.ChaosState, 300);
                            playerComeimg = true;
                            SoundEngine.PlaySound(new("CalamityEntropy/Assets/Sounds/teleport"), Projectile.Center);
                        }
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
                            NPC target = null;
                            target = Projectile.FindTargetWithinRange(800, false);
                            if (target != null)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(10, 0).RotatedBy((target.Center - Projectile.Center).ToRotation()), ModContent.ProjectileType<VaProj>(), Projectile.damage / 2, 6, Projectile.owner);

                            }
                            else
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(10, 0).RotatedBy(Main.rand.NextDouble() * Math.PI * 2), ModContent.ProjectileType<VaProj>(), Projectile.damage / 2, 6, Projectile.owner);

                            }
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
            lightColor = Color.White;
            counter -= 2;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D chaintx = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/FabstaffStreak").Value;
            Main.spriteBatch.Draw(chaintx, Projectile.Center - Main.screenPosition, new Rectangle(counter, 0, (int)Util.Util.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center), chaintx.Height), Color.Purple, (Projectile.owner.ToPlayer().Center - Projectile.Center).ToRotation(), new Vector2(0, chaintx.Height) / 2, new Vector2(1, 0.4f), SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            if (select == Projectile.whoAmI)
            {
                List<Vector2> adv = new List<Vector2>() { new Vector2(-2, -2), new Vector2(2, -2), new Vector2(-2, 2), new Vector2(2, 2) };
                foreach (Vector2 v in adv)
                {
                    Main.spriteBatch.Draw(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Vaoutline").Value, Projectile.Center - Main.screenPosition + v, null, Color.Yellow, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);
                }
            }
            Main.spriteBatch.Draw(TextureAssets.Projectile[Projectile.type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, TextureAssets.Projectile[Projectile.type].Value.Size() / 2, Projectile.scale, Microsoft.Xna.Framework.Graphics.SpriteEffects.None, 0);

            return false;
        }
    }
}
