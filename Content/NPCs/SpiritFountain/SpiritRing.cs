using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles.SpiritFountainShoots;
using CalamityMod;
using InnoVault.GameSystem;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.SpiritFountain
{
    [StaticImmunity(typeof(SpiritFountain))]
    public class SpiritRing : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            this.HideFromBestiary();
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.ImmuneToRegularBuffs[Type] = true;
            NPCID.Sets.MustAlwaysDraw[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 160;
            NPC.height = 46;
            NPC.damage = 200;
            NPC.dontCountMe = true;
            NPC.lifeMax = 80000;
            NPC.HitSound = SoundID.NPCHit11;
            NPC.DeathSound = SoundID.NPCDeath11;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.noGravity = true;
        }

        public NPC owner => ((int)(NPC.ai[0])).ToNPC();
        public SpiritFountain fountain => (SpiritFountain)(owner.ModNPC);
        public float Index => NPC.ai[1];
        public FountainColumn column => NPC.ai[2] == 0 ? fountain.column1 : fountain.column2;
        public float TrailLength = 60;
        private bool flag = true;
        public float columnOffset = Main.rand.NextFloat(-1200, 1200);
        public bool Lerping = false;
        public float LFrom = 0;
        public float LTo = 0;
        public float LProgress = 0;
        public void LerpTo(float offset)
        {
            Lerping = true;
            LFrom = columnOffset;
            LTo = offset;
            LProgress = 0;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(columnOffset);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            columnOffset = reader.ReadSingle();
        }
        public bool OnColumn = true;
        public bool BMRCd = false;
        public override void AI()
        {
            if (flag)
            {
                flag = false;
                NPC.Opacity = 0;
            }
            if (NPC.Opacity < 1)
            {
                NPC.Opacity += 0.05f;
            }


            if (!owner.active || owner.ModNPC is not SpiritFountain)
            {
                NPC.active = false;
                return;
            }

            NPC.scale = owner.scale;
            NPC.Entropy().VoidTouchDR = owner.Entropy().VoidTouchDR;
            NPC.defense = owner.defense;
            NPC.Calamity().DR = owner.Calamity().DR;
            NPC.dontTakeDamage = fountain.DontTakeDmg;
            NPC.realLife = (int)NPC.ai[0];
            NPC.lifeMax = owner.lifeMax;
            NPC.life = owner.life;
            NPC.damage = owner.damage;
            NPC.width = (int)float.Lerp(46, 160, CEUtils.GetRepeatedCosFromZeroToOne(Math.Abs(NPC.rotation.ToRotationVector2().X), 1));
            bool DontSetPos = false;
            bool DontSetRot = false;
            if (fountain.aiTimer == 1)
            {
                AlphaLaserWarning = 0;
            }
            if (fountain.ClearMyProjs > 0)
            {
                SRHandle = 5;
            }
            if (Lerping)
            {
                LProgress += 0.02f;
                columnOffset = float.Lerp(LFrom, LTo, CEUtils.GetRepeatedCosFromZeroToOne(LProgress, 1));
                if (LProgress >= 1)
                {
                    Lerping = false;
                    LProgress = 0;
                    columnOffset = LTo;
                }
            }
            NPC.height = (int)float.Lerp(46, 160, CEUtils.GetRepeatedCosFromZeroToOne(Math.Abs(NPC.rotation.ToRotationVector2().Y), 1));

            if (SRHandle-- < 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<SRDamageRect>(), NPC.damage / 6, 2, -1, NPC.whoAmI);
                }
            }
            drawColorLerp = Color.AliceBlue;

            #region Phase1
            if (fountain.ai == SpiritFountain.AIStyle.Moving)
            {
                float targetofs = Index * 1900 + (float)Math.Sin(fountain.Counter * 0.02f) * 400;
                columnOffset = float.Lerp(columnOffset, targetofs, 0.05f);
                TrailLength = float.Lerp(TrailLength, 68, 0.05f);
            }
            Player target = owner.HasValidTarget ? owner.target.ToPlayer() : Main.player[0];

            if (fountain.ai == SpiritFountain.AIStyle.Boomerang)
            {
                if (fountain.aiTimer == 10)
                {
                    LerpTo(Main.rand.NextFloat(-1800, 1800));
                    if (fountain.phase == 3)
                    {
                        LerpTo(Index * 1200);
                    }
                }
                if (fountain.num1 > (fountain.phase == 3 ? Math.Abs(Index) : (Index + 1) / 2f))
                {
                    if (OnColumn && !BMRCd)
                    {
                        CEUtils.PlaySound("scholarStaffAttack", Main.rand.NextFloat(0.6f, 1.4f), NPC.Center, -1);
                        OnColumn = false;
                        BMRCd = true;
                        NPC.rotation = 0;
                        NPC.velocity = (owner.target.ToPlayer().Center - NPC.Center).normalize() * 28f;
                        NPC.velocity.Y = float.Clamp(NPC.velocity.Y, -4 * fountain.phase, 4 * fountain.phase);
                        NPC.velocity.X = Math.Sign(NPC.velocity.X) * (fountain.phase == 3 ? 24 : 34);
                        if (fountain.phase == 3)
                        {
                            NPC.velocity.Y = 0;
                        }
                    }
                    if (!OnColumn)
                    {
                        TCounter += 0.2f * (NPC.whoAmI % 2 == 0 ? 1 : -1);
                        TrailLength = float.Lerp(TrailLength, 128, 0.1f);
                        if (NPC.localAI[1]++ > 35)
                        {
                            if (fountain.phase == 3)
                            {
                                NPC.damage = 0;
                            }
                            if (NPC.localAI[1] > 30 && NPC.localAI[1] <= 120 && fountain.phase == 3)
                            {

                                NPC.velocity *= 0.2f;
                                if (NPC.localAI[1] == 120)
                                {
                                    fountain.Shoot(ModContent.ProjectileType<SpiritLaser>(), NPC.Center, (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 5);
                                }
                                if (NPC.localAI[1] < 90)
                                {
                                    NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, (target.Center - NPC.Center).ToRotation() + MathHelper.PiOver2, 0.008f);
                                    NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, (target.Center - NPC.Center).ToRotation() + MathHelper.PiOver2, 0.044f, false);
                                    AlphaLaserWarning = float.Lerp(AlphaLaserWarning, 1, 0.04f);
                                }
                                else
                                {
                                    AlphaLaserWarning = float.Lerp(AlphaLaserWarning, 0, 0.04f);
                                }
                            }
                            else
                            {
                                AlphaLaserWarning = 0;
                                NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, 0, 0.12f, false);
                            }
                            if (NPC.localAI[1] == 66 && fountain.phase == 2)
                            {
                                fountain.Shoot(ModContent.ProjectileType<SpiritBullet>(), NPC.Center, CEUtils.randomRot().ToRotationVector2() * 4, 1, owner.whoAmI, 2, 1);
                            }
                            NPC.velocity.Y *= 0.97f;
                            NPC.velocity.X += (owner.Center.X + column.offset.X - NPC.Center.X) > 0 ? 1.6f : -1.6f;
                            if (NPC.localAI[1] > 40)
                            {
                                if (((owner.Center.X + column.offset.X - NPC.localAI[0]) > 0) != ((owner.Center.X + column.offset.X - (NPC.Center.X + NPC.velocity.X * 2)) > 0))
                                {
                                    OnColumn = true;
                                    NPC.velocity *= 0;
                                    columnOffset = -(NPC.Center.Y - (owner.Center.Y + column.offset.Y));
                                }
                            }
                            NPC.localAI[0] = NPC.Center.X;

                        }
                    }
                    else
                    {
                        NPC.velocity *= 0;
                    }
                }
                else
                {
                    if (fountain.num1 > (fountain.phase == 3 ? Math.Abs(Index) : (Index + 1) / 2f))
                    {
                        drawColorLerp = new Color(255, 40, 40);
                    }
                }
                if (fountain.aiTimer < 70)
                {
                    NPC.damage = 0;
                }
            }
            else
            {
                if (BMRCd)
                {
                    OnColumn = true;
                    NPC.velocity *= 0;

                    BMRCd = false;

                    NPC.localAI[1] = 0;
                }
            }
            if (fountain.ai == SpiritFountain.AIStyle.Lasers)
            {
                DontSetRot = true;
                int targetTime = (int)(fountain.phase == 3 ? 82 : (fountain.phase == 2 ? 98 : 120) / fountain.enrage);
                if (fountain.aiTimer < 416 || Lerping || AlphaLaserWarning > 0)
                {
                    if (fountain.aiTimer % (targetTime + 28) == 0)
                    {
                        LerpTo(Main.rand.NextFloat(-1200, 1200));
                    }
                    if (fountain.aiTimer % (targetTime + 28) <= targetTime)
                    {
                        NPC.rotation = CEUtils.RotateTowardsAngle(NPC.rotation, (target.Center.X > NPC.Center.X ? MathHelper.PiOver2 : -MathHelper.PiOver2) + ((fountain.phase - 1) * (NPC.whoAmI * 1.73523f).ToRotationVector2().ToRotation() * 0.08f), 0.24f, false);
                    }
                    else
                    {
                        NPC.rotation = (target.Center.X > NPC.Center.X ? MathHelper.PiOver2 : -MathHelper.PiOver2) + ((fountain.phase - 1) * (NPC.whoAmI * 1.73523f).ToRotationVector2().ToRotation() * 0.08f);
                    }
                    if (fountain.aiTimer % (targetTime + 28) <= targetTime - 12)
                    {
                        AlphaLaserWarning = float.Lerp(AlphaLaserWarning, 1, 0.04f);
                    }
                    else
                    {
                        AlphaLaserWarning = float.Lerp(AlphaLaserWarning, 0, 0.04f);
                    }
                    if (fountain.aiTimer % (targetTime + 28) == 1 + targetTime)
                    {
                        AlphaLaserWarning = 0;
                        fountain.Shoot(ModContent.ProjectileType<SpiritLaser>(), NPC.Center, (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 5);
                    }

                }
                else
                {
                    AlphaLaserWarning = 0;
                }
            }
            NPC.noTileCollide = true;

            if (fountain.ai == SpiritFountain.AIStyle.RingFountains)
            {
                DontSetPos = true;
                DontSetRot = true;
                if (fountain.aiTimer == 1)
                {
                    NPC.rotation = 0;
                    NPC.velocity = new Vector2(Main.rand.NextFloat(-30, 30), -12);
                }
                if (fountain.aiTimer > 1)
                {
                    if (NPC.velocity.Y == 0)
                    {
                        NPC.velocity *= 0;
                        NPC.rotation = 0;
                        if (fountain.aiTimer > 100)
                        {
                            if (fountain.aiTimer < 150)
                            {
                                AlphaWaveWarning = float.Lerp(AlphaWaveWarning, 0.5f, 0.06f);
                            }
                            else
                            {
                                if (fountain.aiTimer == 150)
                                {
                                    fountain.Shoot(ModContent.ProjectileType<SpiritWave>(), NPC.Center, Vector2.Zero);
                                }
                                if (fountain.aiTimer > 150)
                                {
                                    if (fountain.aiTimer > 170)
                                    {
                                        AlphaWaveWarning = float.Lerp(AlphaWaveWarning, 0, 0.1f);
                                    }
                                    else
                                    {
                                        AlphaWaveWarning = float.Lerp(AlphaWaveWarning, 1, 0.1f);
                                    }
                                }
                                if (fountain.aiTimer > 220)
                                {
                                    NPC.damage = 0;
                                    NPC.Center = Vector2.Lerp(NPC.Center, owner.Center + column.offset + column.rotation.ToRotationVector2() * columnOffset, 0.1f);
                                }
                            }
                        }
                    }
                    else
                    {
                        NPC.width = NPC.height = 20;
                        NPC.rotation += NPC.velocity.X * 0.004f;
                        NPC.velocity.X *= 0.98f;
                        NPC.velocity.Y += 0.6f;
                        NPC.noTileCollide = false;
                        NPC.damage = 0;
                    }
                }
            }
            else
            {
                AlphaWaveWarning = 0;
            }
            #endregion

            #region phase2
            if (fountain.ai == SpiritFountain.AIStyle.PhaseTranse1)
            {
                float targetofs = Index * 1900 + (float)Math.Sin(fountain.Counter * 0.02f) * 400;
                columnOffset = float.Lerp(columnOffset, targetofs, 0.05f);
                TrailLength = float.Lerp(TrailLength, 74, 0.05f);
                NPC.damage = 0;
            }
            if (fountain.ai == SpiritFountain.AIStyle.SpiritSlicing)
            {
                TrailLength = float.Lerp(TrailLength, 100, 0.05f);
                int counter = fountain.aiTimer;
                int t = 90;
                if (counter % (t + 42) < t)
                    NPC.damage = 0;
                if (counter % (t + 42) == 1)
                {
                    LerpTo(Main.rand.NextFloat(-1200, 1200));
                }
                if (counter % (t + 42) == 51)
                {
                    Vector2 offset = column.id == 0 ? new Vector2(column.Num < 0 ? 1 : -1, 0) : new Vector2(0, column.Num < 0 ? 1 : -1);
                    EParticle.spawnNew(new HadLine() { hm = 0.36f }, NPC.Center + offset * 80, Vector2.Zero, Color.LightBlue, 3f, 1, true, BlendState.Additive, offset.ToRotation(), 32);
                    EParticle.spawnNew(new HadLine() { hm = 0.36f }, NPC.Center + offset * 80, Vector2.Zero, Color.LightBlue, 3f, 1, true, BlendState.Additive, offset.ToRotation(), 32);

                }
            }


            #endregion
            if (OnColumn)
            {
                if (!DontSetPos)
                    NPC.Center = owner.Center + column.offset + column.rotation.ToRotationVector2() * columnOffset;
                if (!DontSetRot)
                    NPC.rotation = column.rotation + MathHelper.PiOver2;
            }
            color = Color.Lerp(color, drawColorLerp, 0.1f);
            TCounter += 0.16f * (NPC.whoAmI % 2 == 0 ? 1 : -1);

        }
        public Color drawColorLerp = Color.AliceBlue;
        public Color color = Color.AliceBlue;
        public float TCounter = Main.rand.NextFloat() * 3.14159f;
        public void DrawTrail(float r, float rot)
        {
            float rt = rot;
            Texture2D glow = CEUtils.getExtraTex("Glow2");
            var sb = Main.spriteBatch;
            sb.End();
            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

            for (float i = 0; i < (int)TrailLength; i++)
            {
                float s = (1 + i) / TrailLength;
                s = 0.32f + 0.68f * s;
                var ofs = rt.ToRotationVector2() * r;
                ofs.Y *= 0.25f;
                ofs = ofs.RotatedBy(NPC.rotation);
                sb.Draw(glow, NPC.Center + ofs - Main.screenPosition, null, color * NPC.Opacity * 0.7f * (NPC.damage > 0 ? 1 : 0.5f), 0, glow.Size() * 0.5f, s * 0.2f, SpriteEffects.None, 0);
                rt += MathHelper.ToRadians(4) * (NPC.whoAmI % 2 == 0 ? 1 : -1);
            }
            sb.End();
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.ZoomMatrix);

        }
        public float AlphaLaserWarning = 0;
        public float AlphaWaveWarning = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            yx += 0.036f;
            if (AlphaWaveWarning > 0.01f)
            {
                Texture2D glow = CEUtils.getExtraTex("a_circle");
                Texture2D tex = CEUtils.getExtraTex("LTLine");
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                Main.spriteBatch.Draw(glow, NPC.Center - Main.screenPosition, null, Color.White * AlphaWaveWarning, NPC.rotation, glow.Size() / 2f, new Vector2(3, 1.2f), SpriteEffects.None, 0);
                Main.spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, null, Color.White * AlphaWaveWarning * 0.8f, NPC.rotation - MathHelper.PiOver2, new Vector2(0, tex.Height / 2f), new Vector2(2f, 6.5f), SpriteEffects.None, 0);
            }
            if (AlphaLaserWarning > 0.01f)
            {
                List<Vector2> points = new();
                for (float i = 0; i <= 1; i += 0.005f)
                {
                    points.Add(Vector2.Lerp(NPC.Center, NPC.Center + (NPC.rotation - MathHelper.PiOver2).ToRotationVector2() * 3600, i));
                }
                Texture2D tx = CEUtils.pixelTex;

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();

                    float w = 12;
                    float p = -Main.GlobalTimeWrappedHourly * 2;
                    for (int i = 1; i < points.Count; i++)
                    {
                        float wd = (0.9f + 0.12f * (float)Math.Cos(i * 0.6f + Main.GlobalTimeWrappedHourly * 10)) * AlphaLaserWarning;
                        Color b = new Color(200, 200, 255) * wd;
                        ve.Add(new ColoredVertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * NPC.scale * w * wd,
                              new Vector3((float)i / points.Count, 1, 1),
                              b));
                        ve.Add(new ColoredVertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * NPC.scale * w * wd,
                              new Vector3((float)i / points.Count, 0, 1),
                              b));
                    }

                    SpriteBatch sb = Main.spriteBatch;
                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    if (ve.Count >= 3)
                    {
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }
            }
            DrawTrail(90, TCounter);

            return false;
        }
        public float yx = 0;
        public override bool CheckActive()
        {
            if (owner.active)
            {
                return false;
            }
            return true;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        public override bool? CanFallThroughPlatforms()
        {
            return true;
        }
        public int SRHandle = 3;
    }

    public class SRDamageRect : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            Projectile.timeLeft = 4;
        }
        public override void AI()
        {
            var o = ((int)(Projectile.ai[0])).ToNPC();
            Projectile.damage = o.damage / 6;
            Projectile.Center = (o.Center);
            if (o.active && o.ModNPC is SpiritRing sr)
            {
                Projectile.timeLeft = 5;
                Projectile.rotation = o.rotation;
                sr.SRHandle = 3;
                if (sr.fountain.ClearMyProjs > 0)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center - Projectile.rotation.ToRotationVector2() * 80 * Projectile.scale, Projectile.Center + Projectile.rotation.ToRotationVector2() * 80 * Projectile.scale, targetHitbox, 36);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
