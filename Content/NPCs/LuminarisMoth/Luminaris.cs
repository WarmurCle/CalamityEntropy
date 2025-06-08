using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Accessories.SoulCards;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Prophet;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.LuminarisMoth
{
    [AutoloadBossHead]
    public class Luminaris : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 8;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.48f,
                PortraitScale = 0.56f,
                CustomTexturePath = "CalamityEntropy/Assets/BCL/LuminarisBossCheckList",
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 46
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }
        public List<Vector2> odp = new List<Vector2>();
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.LuminarisBestiary")
            });
        }

        public enum AIStyle
        {
            RoundShooting,
            AboveShooting,
            Subduction,
            Dashing,

            Shoot360,
            RoundAndDash,
            SmashDown,
            ShootTriangle,
            Starlit
        }
        public Vector2 vec1 = Vector2.Zero;
        public Vector2 vec2 = Vector2.Zero;
        public float num1 = 0;
        public float num2 = 0;
        public float num3 = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(vec1);
            writer.WriteVector2(vec2);
            writer.Write(num1);
            writer.Write(num2);
            writer.Write(num3);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            vec1 = reader.ReadVector2();
            vec2 = reader.ReadVector2();
            num1 = reader.ReadSingle();
            num2 = reader.ReadSingle();
            num3 = reader.ReadSingle();
        }
        public override void SetDefaults()
        {
            
            NPC.boss = true;
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 70;
            NPC.Calamity().DR = 0.1f;
            NPC.defense = 10;
            NPC.lifeMax = 54000;
            NPC.HitSound = SoundID.NPCHit32;
            NPC.DeathSound = SoundID.NPCDeath22;
            NPC.value = 1600f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontCountMe = true;
            NPC.timeLeft *= 20;
            if (!Main.dedServ)
            {
                Music = MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/LuminarisBoss");
            }
        }
        public int frameCounter = 0;
        public Vector2 oldPos = Vector2.Zero;
        public int AIRound = 0;
        public AIStyle ai = AIStyle.RoundShooting;

        public override void AI()
        {
            if(oldPos == Vector2.Zero)
            {
                oldPos = NPC.Center;
            }
            frameCounter++;
            
            if (tail1 == null || tail2 == null)
            {
                tail1 = new Rope(NPC.Center, 10, 11.6f, new Vector2(0, 0.14f), 0.054f, 30);
                tail2 = new Rope(NPC.Center, 10, 11.6f, new Vector2(0, 0.14f), 0.054f, 30);
            }
            if (NPC.life <= NPC.lifeMax / 2)
                phase = 2;
            if (!NPC.HasValidTarget)
            {
                NPC.TargetClosest();
            }
            if (NPC.HasValidTarget)
            {
                AttackPlayer(Main.player[NPC.target]);
            }
            else
            {
                NPC.velocity *= 0.96f;
                NPC.velocity.Y -= 0.2f;
            }
            for (float i = 0; i <= 1; i += 0.25f)
            {
                Vector2 tail1Pos = NPC.velocity + Vector2.Lerp(oldPos, NPC.Center, i) + new Vector2(-14, 32).RotatedBy(NPC.rotation) * NPC.scale;
                Vector2 tail2Pos = NPC.velocity + Vector2.Lerp(oldPos, NPC.Center, i) + new Vector2(14, 32).RotatedBy(NPC.rotation) * NPC.scale;
                tail1.Start = tail1Pos;
                tail2.Start = tail2Pos;
                tail1.gravity = new Vector2(0, 0.12f);
                tail2.gravity = new Vector2(0, 0.12f);
                tail1.Update();
                tail2.Update();
            }
            oldPos = NPC.Center;
            odp.Add(NPC.Center);
            if (odp.Count > 24)
            {
                odp.RemoveAt(0);
            }
        }

        public void AttackPlayer(Player player)
        {
            float enrange = 1;
            if(Main.expertMode)
            {
                enrange += 0.1f;
            }
            if (Main.masterMode)
            {
                enrange += 0.1f;
            }
            if (CalamityWorld.revenge)
            {
                enrange += 0.15f;
            }
            if (CalamityWorld.death)
            {
                enrange += 0.15f;
            }
            if (CalamityEntropy.EntropyMode)
            {
                enrange *= 1.4f;
            }
            if (Main.getGoodWorld)
            {
                enrange *= 1.1f;
            }
            if (Main.zenithWorld)
            {
                enrange *= 1.4f;
            }
            if(AIChangeCounter-- < 0)
            {
                vec1 = vec2 = Vector2.Zero;
                num1 = num2 = num3 = 0;
                SetAISyyle();
                AIRound++;
                AIChangeCounter = 20;
                if (ai == AIStyle.RoundShooting)
                {
                    AIChangeCounter = 260;
                }
                NPC.netUpdate = true;
            }
            if(ai == AIStyle.RoundShooting)
            {
                NPC.velocity *= 0;
                if(AIChangeCounter == 260)
                {
                    num3 = Main.rand.NextBool() ? -1 : 1;
                    vec1 = NPC.Center;
                }
                if(AIChangeCounter > 220)
                {
                    NPC.Center = Vector2.Lerp(vec1, player.Center + new Vector2(380 * Math.Sign(NPC.Center.X - player.Center.X), -380), CEUtils.GetRepeatedCosFromZeroToOne(1 - (AIChangeCounter - 220) / 40f, 1));
                }
                if(AIChangeCounter == 220)
                {
                    num1 = NPC.Center.Distance(player.Center);
                    num2 = (NPC.Center - player.Center).ToRotation();
                }
                if(AIChangeCounter < 220)
                {
                    for (int i = 0; i < odp.Count; i++)
                    {
                        odp[i] += player.velocity;
                    }
                    num2 += 0.08f * num3 * enrange;
                    NPC.Center = player.Center + num2.ToRotationVector2() * num1;
                }
            }


        }
        public int AIChangeCounter = 0;
        public void SetAISyyle()
        {
            if (phase == 1)
            {
                ai = (AIStyle)AIRound;
                if (AIRound >= 3)
                {
                    AIRound = -1;
                }
            }
            else
            {
                ai = (AIStyle)(AIRound + 4);
                if (AIRound >= 4)
                {
                    AIRound = -1;
                }
            }
            ai = AIStyle.RoundShooting;
        }

        public static Texture2D texture = null;
        public static Texture2D texTail1 = null;
        public static Texture2D texTail2 = null;
        public static Texture2D texStar = null;

        public Rope tail1 = null;
        public Rope tail2 = null;
        public override void Unload()
        {
            texture = null;
            texTail1 = null;
            texTail2 = null;
            texStar = null;
        }
        public int phase = 1;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if(texStar == null)
            {
                texStar = CEUtils.getExtraTex("StarTexture_White");
            }
            if(texture == null)
                texture = NPC.getTexture();
            if(texTail1 == null || texTail2 == null)
            {
                texTail1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LuminarisMoth/t1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                texTail2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LuminarisMoth/t2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
            
            DrawMyself(NPC.Center);
            return false;
        }
        public void DrawMyself(Vector2 pos)
        {
            DrawTails(pos - NPC.Center);
            Asset<Texture2D> textured = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Enchanted", AssetRequestMode.ImmediateLoad);
            Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/Transform3", AssetRequestMode.ImmediateLoad).Value;
            shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            shader.Parameters["color"].SetValue((phase == 1 ? new Color(0, 190, 250, 255) : new Color(160, 80, 255, 255)).ToVector4());
            shader.Parameters["strength"].SetValue(phase == 1 ? 0.2f : 1f);

            shader.CurrentTechnique.Passes["EnchantedPass"].Apply();
            Main.instance.GraphicsDevice.Textures[1] = textured.Value;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(0, Main.spriteBatch.GraphicsDevice.BlendState, Main.spriteBatch.GraphicsDevice.SamplerStates[0], Main.spriteBatch.GraphicsDevice.DepthStencilState, Main.spriteBatch.GraphicsDevice.RasterizerState, shader, Main.Transform);

            Rectangle frame = new Rectangle(0, (texture.Height / Main.npcFrameCount[Type]) * ((frameCounter / 4) % Main.npcFrameCount[Type]), texture.Width, (texture.Height / Main.npcFrameCount[Type]) - 2);
            Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, Color.White * NPC.Opacity, NPC.rotation, new Vector2(texture.Width / 2, 104), NPC.scale, SpriteEffects.None);
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            float starX = 1f + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 26) * 0.4f;
            Vector2 starScale = new Vector2(starX, starX);
            Main.spriteBatch.Draw(texStar, pos - Main.screenPosition, null, Color.LightBlue * NPC.Opacity, 0, texStar.Size() * 0.5f, new Vector2(1f, 0.8f * 0.7f) * starScale * NPC.scale * 0.74f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(texStar, pos - Main.screenPosition, null, Color.LightBlue * NPC.Opacity, 0, texStar.Size() * 0.5f, new Vector2(0.8f, 1f * 0.7f) * starScale * NPC.scale * 0.74f, SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();
            drawT();
        }
        public void drawT()
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            odp.Add(NPC.Center);
            if (odp.Count > 2)
            {
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    Color b = Color.SkyBlue;

                    float a = 0;
                    float lr = 0;
                    for (int i = 1; i < odp.Count; i++)
                    {
                        a += 1f / (float)odp.Count;

                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 20 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 20 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 0, 1),
                              b * a));
                        lr = (odp[i] - odp[i - 1]).ToRotation();
                    }
                    a = 1;

                    if (ve.Count >= 3)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/MegaStreakBacking2").Value;
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }
                {
                    List<ColoredVertex> ve = new List<ColoredVertex>();
                    Color b = Color.White;

                    float a = 0;
                    float lr = 0;
                    for (int i = 1; i < odp.Count; i++)
                    {
                        a += 1f / (float)odp.Count;

                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 16 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 1, 1),
                            b * a));
                        ve.Add(new ColoredVertex(odp[i] - Main.screenPosition + (odp[i] - odp[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 16 * ((i - 1f) / (odp.Count - 2f)),
                              new Vector3((float)(i + 1) / odp.Count + Main.GlobalTimeWrappedHourly, 0, 1),
                              b * a));
                        lr = (odp[i] - odp[i - 1]).ToRotation();
                    }
                    a = 1;

                    if (ve.Count >= 3)
                    {
                        Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Extra/Streak1").Value;
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }

            }
            odp.RemoveAt(odp.Count - 1);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        }
        #region drawTail
        public void DrawTails(Vector2 pos)
        {
            GraphicsDevice gd = Main.spriteBatch.GraphicsDevice;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (tail1 != null)
            {   
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = Color.White * NPC.Opacity;
                List<Vector2> tailPoints = tail1.GetPoints();
                for (int i = 1; i < tailPoints.Count; i++)
                {
                    ve.Add(new ColoredVertex(tailPoints[i] + pos - Main.screenPosition + (tailPoints[i] - tailPoints[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 18,
                          new Vector3((float)(i + 1) / tailPoints.Count, 1, 1),
                          b));
                    ve.Add(new ColoredVertex(tailPoints[i] + pos - Main.screenPosition + (tailPoints[i] - tailPoints[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 18,
                          new Vector3((float)(i + 1) / tailPoints.Count, 0, 1),
                          b));

                }
                if (ve.Count >= 3)
                {
                    Texture2D tx = texTail1;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            if (tail2 != null)
            {
                List<ColoredVertex> ve = new List<ColoredVertex>();
                Color b = Color.White * NPC.Opacity;
                List<Vector2> tailPoints = tail2.GetPoints();
                for (int i = 1; i < tailPoints.Count; i++)
                {
                    ve.Add(new ColoredVertex(tailPoints[i] + pos - Main.screenPosition + (tailPoints[i] - tailPoints[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 18,
                          new Vector3((float)(i + 1) / tailPoints.Count, 1, 1),
                          b));
                    ve.Add(new ColoredVertex(tailPoints[i] + pos - Main.screenPosition + (tailPoints[i] - tailPoints[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 18,
                          new Vector3((float)(i + 1) / tailPoints.Count, 0, 1),
                          b));

                }
                if (ve.Count >= 3)
                {
                    Texture2D tx = texTail2;
                    gd.Textures[0] = tx;
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                }
            }
            Main.spriteBatch.ExitShaderRegion();
        }
        #endregion

        public override void OnKill()
        {
            NPC.SetEventFlagCleared(ref EDownedBosses.downedLuminaris, -1);
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }
    }
}
