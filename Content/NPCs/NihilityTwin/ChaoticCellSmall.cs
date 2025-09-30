using CalamityEntropy.Content.Biomes;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.NihilityTwin
{
    public class ChaoticCellSmall : ModNPC
    {
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(ModContent.BuffType<VoidVirus>(), 160);
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityEntropy.CCellSmallBestiary")
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.damage = 1;
            if (Main.expertMode)
            {
                NPC.damage += 1;
            }
            if (Main.masterMode)
            {
                NPC.damage += 1;
            }
            NPC.lifeMax = 2200;
            if (CalamityWorld.death)
            {
                NPC.damage += 1;
            }
            else if (CalamityWorld.revenge)
            {
                NPC.damage += 1;
            }
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCHit1;
            NPC.value = Item.buyPrice(0, 0, 40, 0);
            NPC.knockBackResist = 0.7f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.Entropy().VoidTouchDR = 0.5f;
            NPC.dontCountMe = true;
            NPC.netAlways = true;
            SpawnModBiomes = new int[] { ModContent.GetInstance<VoidDummyBoime>().Type };
        }
        public bool init = true;
        Rope rope = null;
        public override void AI()
        {
            NPC.velocity *= 0.98f;
            if (!owner.active)
            {
                NPC.active = false;
            }
            NPC.rotation += NPC.velocity.X * 0.002f;
            if (owner.HasValidTarget)
            {
                NPC.velocity += (owner.target.ToPlayer().Center - NPC.Center).SafeNormalize(Vector2.Zero) * 0.36f;
            }
            NPC.velocity += (owner.Center - NPC.Center) * 0.0022f;
            if (Main.netMode != NetmodeID.MultiplayerClient && (Main.GameUpdateCount % 10 == 0 && Main.rand.NextBool(16)))
            {
                float rot = (owner.target.ToPlayer().Center - NPC.Center).ToRotation();
                for (int i = 0; i < 2; i++)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, rot.ToRotationVector2().RotatedBy(0.03f * i) * 22, ModContent.ProjectileType<CellBullet>(), NPC.damage / 7, 4);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, rot.ToRotationVector2().RotatedBy(-0.03f * i) * 22, ModContent.ProjectileType<CellBullet>(), NPC.damage / 7, 4);

                }
            }
            if (rope == null)
            {
                rope = new Rope(NPC.Center, owner.Center, 30, 0, new Vector2(0, 0f), 0.006f, 15, false);
            }
            Vector2 rend = owner.Center;
            rope.segmentLength = CEUtils.getDistance(NPC.Center, rend) / 35f;
            rope.Start = NPC.Center;
            rope.End = rend;
            rope.Update();
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == NPC.type && n.whoAmI != NPC.whoAmI)
                {
                    if (n.getRect().Intersects(NPC.getRect()))
                    {
                        NPC.velocity += (NPC.Center - n.Center).SafeNormalize(Vector2.UnitX) * 1f;
                    }
                }
            }
        }
        public NPC owner { get { return ((int)NPC.ai[0]).ToNPC(); } }

        public override bool CheckActive()
        {
            return !owner.active;
        }
        public void drawRope()
        {
            List<ColoredVertex> ve = new List<ColoredVertex>();
            List<Vector2> points = new List<Vector2>();

            // Addresses a crash when viewing the NPC in a browser.
            if (rope == null)
                return;

            points = rope.GetPoints();

            points.Insert(0, NPC.Center);
            points.Add(owner.Center);
            points.Add(owner.Center);
            float lc = 1;
            float jn = 0;

            for (int i = 1; i < points.Count - 1; i++)
            {
                jn += CEUtils.getDistance(points[i - 1], points[i]) / (float)28 * lc;

                ve.Add(new ColoredVertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(90)) * 7 * lc,
                      new Vector3(jn, 1, 1),
                      Color.White));
                ve.Add(new ColoredVertex(points[i] - Main.screenPosition + (points[i] - points[i - 1]).ToRotation().ToRotationVector2().RotatedBy(MathHelper.ToRadians(-90)) * 7 * lc,
                      new Vector3(jn, 0, 1),
                      Color.White));

            }

            SpriteBatch sb = Main.spriteBatch;
            GraphicsDevice gd = Main.graphics.GraphicsDevice;
            if (ve.Count >= 3)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                gd.Textures[0] = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/NihilityTwin/NihRope").Value;
                gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;
            drawRope();
            Texture2D tex = TextureAssets.Npc[NPC.type].Value;
            Color color = Color.White;
            Main.EntitySpriteDraw(tex, NPC.Center - Main.screenPosition, null, color, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None);
            return false;
        }

    }
}
