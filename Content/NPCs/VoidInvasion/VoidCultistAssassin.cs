using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.VoidInvasion
{
    public class VoidCultistAssassin : VoidCultist
    {
        public List<Vector2> oldPos = new List<Vector2>();
        public override void SetDefaults()
        {
            base.SetDefaults();
            this.walking.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/VoidInvasion/Assassin/walk1").Value);
            this.walking.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/VoidInvasion/Assassin/walk2").Value);
        }

        public override Texture2D BodyTex => ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/VoidInvasion/Assassin/body").Value;
        public override Texture2D LeftHandTex => ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/VoidInvasion/Assassin/handLeft").Value;
        public override Texture2D RightHandTex => ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/VoidInvasion/Assassin/handRight").Value;
        public int attackAnmStyle = 0;
        public int attackFrameCounter = 0;
        public int attackFrame = 0;
        public Vector2 dashVec = Vector2.Zero;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (NPC.ai[1] > 7)
            {
                NPC.ai[1] = 7;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(dashVec);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            dashVec = reader.ReadVector2();
        }
        public override void attackAI()
        {
            if (attackAnmStyle == 0 && attackFrame == 0)
            {
                SoundEngine.PlaySound(new SoundStyle("CalamityEntropy/Assets/Sounds/vbuse"), NPC.Center);
            }
            attackFrameCounter++;
            if (attackFrameCounter >= 6)
            {
                attackFrame += 1;
                attackFrameCounter = 0;
            }
            if (attackAnmStyle == 0)
            {
                NPC.noGravity = true;
                NPC.velocity *= 0;
                if (Target.Center.X > NPC.Center.X)
                {
                    NPC.direction = 1;
                }
                else
                {
                    NPC.direction = -1;
                }
                NPC.velocity.X = 0.01f * NPC.direction;
                if (attackFrame >= 9)
                {
                    attackFrame = 0;
                    attackAnmStyle = 1;
                    dashVec = (Target.Center - (NPC.Center + new Vector2(0, NPC.height / 2 - Target.height / 2))).SafeNormalize(Vector2.Zero) * 40;
                    NPC.ai[1] = 24;
                    NPC.netUpdate = true;
                    attackFrameCounter = 0;
                    if (!Main.dedServ)
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/YanmeiKnifeHit"), NPC.Center);
                    }
                }
            }
            if (attackAnmStyle == 1)
            {
                NPC.noGravity = true;
                drawAlpha = 0.7f;
                if (NPC.ai[1] > 0)
                {
                    NPC.ai[1]--;

                    if (NPC.velocity.X == 0)
                    {
                        NPC.ai[1] = 0;
                    }
                    NPC.velocity = dashVec;
                }
                else
                {
                    attackAnmStyle = 2;
                    drawAlpha = 1;
                    attackFrame = 0;
                    attackFrameCounter = 0;
                    NPC.Center = NPC.Center - new Vector2(0, 2);
                }
            }
            if (attackAnmStyle == 2)
            {
                NPC.noGravity = true;
                NPC.velocity *= 0.1f;
                if (attackFrame >= 8)
                {
                    aiStyle = AIStyle.Avoid;
                    AvoidTime = 80;
                    if (Main.masterMode)
                    {
                        AvoidTime -= 23;
                    }
                    if (CalamityWorld.revenge)
                    {
                        AvoidTime -= 23;
                    }
                    if (Main.masterMode && CalamityWorld.death)
                    {
                        if (Main.rand.NextBool(3))
                        {
                            AvoidTime = 9;
                        }
                    }
                    attackAnmStyle = 0;
                    drawAlpha = 1;
                    attackFrame = 0;
                    attackFrameCounter = 0;
                    NPC.noGravity = false;
                }
            }
        }
        public override int CloseTime => (Main.masterMode && CalamityWorld.death ? 20 : base.CloseTime);
        public override int maxAtkDist => (Main.masterMode && CalamityWorld.death ? 800 : base.maxAtkDist);

        public override void PostAI()
        {
            base.PostAI();
            if (NPC.ai[1] > 0)
            {
                oldPos.Add(NPC.Center);
            }
            if (oldPos.Count > 12 || (attackAnmStyle != 1 && oldPos.Count > 0))
            {
                oldPos.RemoveAt(0);
            }
        }

        public override bool? CanFallThroughPlatforms()
        {
            if (aiStyle == AIStyle.Attack && NPC.ai[1] > 0)
            {
                return true;
            }
            return base.CanFallThroughPlatforms();
        }

        public override Texture2D getTex()
        {
            if (aiStyle == AIStyle.Attack)
            {
                if (attackAnmStyle == 0)
                {
                    return ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/VoidInvasion/Assassin/preatk" + (attackFrame + 1).ToString()).Value;
                }
                if (attackAnmStyle == 1)
                {
                    return ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/VoidInvasion/Assassin/atk").Value;

                }
                if (attackAnmStyle == 2)
                {
                    return ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/VoidInvasion/Assassin/postatk" + (attackFrame + 1).ToString()).Value;
                }
            }
            return base.getTex();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (aiStyle == AIStyle.Attack && NPC.ai[1] > 0)
            {
                float ap = 1f / (float)this.oldPos.Count;
                for (int i = 0; i < this.oldPos.Count; i++)
                {
                    Main.spriteBatch.Draw(getTex(), oldPos[i] + drawOffset * NPC.scale - screenPos, null, drawColor * ap * 0.71f * drawAlpha, NPC.rotation, getTex().Size() / 2, NPC.scale, (NPC.direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None), 0);
                    ap += 1f / (float)oldPos.Count;
                }
            }
            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }
    }

}