using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.DimDungeon;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Accessories.Cards;
using CalamityEntropy.Content.Items.Accessories.EvilCards;
using CalamityEntropy.Content.Items.Books.BookMarks;
using CalamityEntropy.Content.Items.Pets;
using CalamityEntropy.Content.Items.Vanity;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.NPCs.FriendFinderNPC;
using CalamityEntropy.Content.NPCs.VoidInvasion;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Pets;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.NPCs.Yharon;
using CalamityMod.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using SubworldLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace CalamityEntropy.Common
{
    public class EGlobalNPC : GlobalNPC
    {
        public int VoidTouchTime = 0;
        public float VoidTouchLevel = 0;
        public float VoidTouchDR = 0;
        public int vtnoparticle = 0;
        public float damageMul = 1;
        public int AnimaTrapped = 0;
        public int[] tfriendlyNPCHitCooldown = new int[201];
        public override bool InstancePerEntity => true;
        public int dscd = 0;
        public bool daTarget = false;
        public bool ToFriendly = false;
        public int hitCd = 30;
        public int f_target = -1;
        public Vector2? plrOldPos = null;
        public Vector2? plrOldVel = null;
        public Vector2? plrOldPos2 = null;
        public Vector2? plrOldVel2 = null;
        public Vector2? plrOldPos3 = null;
        public Vector2? plrOldVel3 = null;
        public int applyMarkedOfDeath = 0;
        public int StareOfAbyssLevel = 0;
        public int EclipsedImprintLevel = 0;
        public int StareOfAbyssTime = 0;
        public int EclipsedImprintTime = 0;
        public int friendFinderOwner = 0;
        public int TDRCounter = 5 * 60 * 60;
        public List<Vector2> getAbyssalCirclePointsRelative(NPC npc, float distAdd = 0, float c = 1)
        {
            float dist = (npc.width + npc.height) / 2f + 30 - (float)Math.Cos(Main.GlobalTimeWrappedHourly) * 12 + distAdd;
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i <= 60; i++)
            {
                points.Add(new Vector2(dist, 0).RotatedBy(MathHelper.ToRadians(i * 6 - 80 * c * Main.GlobalTimeWrappedHourly)));
            }
            return points;
        }
        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.Entropy().StareOfAbyssLevel > 0)
            {

                float alpha = npc.Entropy().StareOfAbyssLevel / 12f;
                Main.spriteBatch.End();

                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                {
                    List<Vertex> ve = new List<Vertex>();
                    List<Vector2> points = npc.Entropy().getAbyssalCirclePointsRelative(npc, -50);
                    List<Vector2> pointsOutside = npc.Entropy().getAbyssalCirclePointsRelative(npc, 50);
                    int i;
                    for (i = 0; i < points.Count; i++)
                    {
                        ve.Add(new Vertex(npc.Center - Main.screenPosition + points[i],
                              new Vector3((float)i / points.Count, 1, 1),
                              Color.SkyBlue * 0.66f * alpha));
                        ve.Add(new Vertex(npc.Center - Main.screenPosition + pointsOutside[i],
                              new Vector3((float)i / points.Count, 0, 1),
                              Color.SkyBlue * 0.66f * alpha));

                    }
                    SpriteBatch sb = Main.spriteBatch;
                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    if (ve.Count >= 3)
                    {
                        Texture2D tx = Util.Util.getExtraTex("AbyssalCircle");
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }
                {
                    List<Vertex> ve = new List<Vertex>();
                    List<Vector2> points = npc.Entropy().getAbyssalCirclePointsRelative(npc, -50, -1);
                    List<Vector2> pointsOutside = npc.Entropy().getAbyssalCirclePointsRelative(npc, 50, -1);
                    int i;
                    for (i = 0; i < points.Count; i++)
                    {
                        ve.Add(new Vertex(npc.Center - Main.screenPosition + points[i],
                              new Vector3((float)i / points.Count, 1, 1),
                              Color.SkyBlue * 0.66f * alpha));
                        ve.Add(new Vertex(npc.Center - Main.screenPosition + pointsOutside[i],
                              new Vector3((float)i / points.Count, 0, 1),
                              Color.SkyBlue * 0.66f * alpha));

                    }
                    SpriteBatch sb = Main.spriteBatch;
                    GraphicsDevice gd = Main.graphics.GraphicsDevice;
                    if (ve.Count >= 3)
                    {
                        Texture2D tx = Util.Util.getExtraTex("AbyssalCircle");
                        gd.Textures[0] = tx;
                        gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    }
                }
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            }
        }
        public override void PostAI(NPC npc)
        {
            if (TDRCounter > 0)
            {
                TDRCounter--;
            }
            noelctime--;
            if (deusBloodOut > 0 && !npc.dontTakeDamage)
            {
                int dmgApply = (int)(deusBloodOut * 0.02f + 1);
                if (dmgApply > deusBloodOut)
                {
                    dmgApply = deusBloodOut;
                }
                deusBloodOut -= dmgApply;
                dmgApply *= 800;
                (npc.realLife >= 0 ? npc.realLife.ToNPC() : npc).life -= dmgApply;
                if ((npc.realLife >= 0 ? npc.realLife.ToNPC() : npc).life < 1)
                {
                    (npc.realLife >= 0 ? npc.realLife.ToNPC() : npc).life = 1;
                }
                if ((npc.realLife >= 0 ? npc.realLife.ToNPC() : npc).life <= 10)
                {
                    deusBloodOut = 0;
                }
            }
            for (int i = 0; i < tfriendlyNPCHitCooldown.Length; i++)
            {
                if (tfriendlyNPCHitCooldown[i] > 0)
                {
                    tfriendlyNPCHitCooldown[i]--;
                }
            }
            if (StareOfAbyssTime > 0)
            {
                StareOfAbyssTime--;
            }
            if (StareOfAbyssTime <= 0)
            {
                StareOfAbyssLevel = 0;
            }
            if (EclipsedImprintTime > 0)
            {
                EclipsedImprintTime--;
            }
            if (EclipsedImprintTime <= 0)
            {
                EclipsedImprintLevel = 0;
            }

            if (applyMarkedOfDeath > 0)
            {
                npc.AddBuff(ModContent.BuffType<MarkedforDeath>(), applyMarkedOfDeath);
                applyMarkedOfDeath = 0;
            }
            if (plrOldPos.HasValue)
            {
                Main.player[0].position = plrOldPos.Value;
                plrOldPos = null;
            }
            if (plrOldVel.HasValue)
            {
                Main.player[0].velocity = plrOldVel.Value;
                plrOldVel = null;
            }
            if (plrOldPos2.HasValue)
            {
                Main.player[0].position = plrOldPos2.Value;
                plrOldPos2 = null;
            }
            if (plrOldVel2.HasValue)
            {
                Main.player[0].velocity = plrOldVel2.Value;
                plrOldVel2 = null;
            }
            if (!ToFriendly)
            {
                ModContent.GetInstance<EModSys>().LastPlayerVel = Main.player[0].velocity;
                ModContent.GetInstance<EModSys>().LastPlayerPos = Main.player[0].Center;

            }
        }
        public static int TamedDmgMul = 16;
        public override bool CheckActive(NPC npc)
        {
            return !ToFriendly && !SubworldSystem.IsActive<DimDungeonSubworld>();
        }
        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            binaryWriter.Write(ToFriendly);
            binaryWriter.Write(f_owner);
            /*binaryWriter.Write(StareOfAbyssLevel);
            binaryWriter.Write(StareOfAbyssTime);
            binaryWriter.Write(EclipsedImprintLevel);
            binaryWriter.Write(EclipsedImprintTime);*/
            /*
            binaryWriter.Write(VoidTouchLevel);
            binaryWriter.Write(VoidTouchTime);
            */
        }
        public override bool CanHitPlayer(NPC npc, Player target, ref int cooldownSlot)
        {
            if (AnimaTrapped > 0)
            {
                return false;
            }
            return base.CanHitPlayer(npc, target, ref cooldownSlot);
        }
        public override bool CanHitNPC(NPC npc, NPC target)
        {
            if (AnimaTrapped > 0)
            {
                return false;
            }
            return base.CanHitNPC(npc, target);
        }
        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            ToFriendly = binaryReader.ReadBoolean();
            f_owner = binaryReader.ReadInt32();
            /*StareOfAbyssLevel = binaryReader.ReadInt32();
            StareOfAbyssTime = binaryReader.ReadInt32();
            EclipsedImprintLevel = binaryReader.ReadInt32();
            EclipsedImprintTime = binaryReader.ReadInt32();*/
            /* VoidTouchLevel = binaryReader.ReadSingle();
             VoidTouchTime = binaryReader.ReadInt32();*/
        }
        public static void setFriendly(int id, int owner = 0)
        {
            if (id.ToNPC().Entropy().ToFriendly)
            {
                return;
            }
            id.ToNPC().Entropy().ToFriendly = true;
            id.ToNPC().Entropy().f_owner = owner;
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                ModPacket p = CalamityEntropy.Instance.GetPacket();
                p.Write((byte)CEMessageType.TurnFriendly);
                p.Write(id);
                p.Write(owner);
                p.Send();
            }

        }
        public bool friendlyDecLife = true;
        public int counter = 0;
        public override bool PreAI(NPC npc)
        {
            if (npc.ModNPC is FriendFindNPC && npc.localAI[3] > 0)
            {
                friendFinderOwner = (int)npc.localAI[3] - 1;
                npc.localAI[3] = 0;
            }
            counter++;
            if (npc.Entropy().EclipsedImprintLevel > 0)
            {
                int c = 16 - npc.Entropy().EclipsedImprintLevel;
                if (counter % c == 0)
                {
                    AbyssalLine p = new AbyssalLine() { lx = 1.2f, xadd = 0.32f };
                    p.spawnColor = Color.Gold;
                    p.endColor = Color.DarkGoldenrod;
                    EParticle.spawnNew(p, npc.Center, Vector2.Zero, Color.White, 1, 1, true, BlendState.Additive, Util.Util.randomRot());

                }
            }
            /*if (ToFriendly)
            {
                if (friendlyDecLife)
                {
                    friendlyDecLife = false;
                    npc.life = 1 + (int)(npc.life / damageMul);
                }
                hitCd--;
                npc.boss = false;
                
                npc.friendly = true;

                bool h = false;
                foreach (NPC npcc in Main.npc)
                {
                    if (npcc.active && !npcc.friendly && !npcc.dontTakeDamage && npc.Hitbox.Intersects(npcc.Hitbox))
                    {
                        if (hitCd <= 0 && !(Main.netMode == NetmodeID.MultiplayerClient))
                        {
                            h = true;
                            npcc.StrikeNPC(npcc.CalculateHitInfo(npc.damage * TamedDmgMul, npc.velocity.X > 0 ? 1 : -1, false, 6, DamageClass.Generic));
                        }
                    }
                }
                if (h)
                {
                    hitCd = 20;
                }
                NPC t = null;
                float dist = 4600;
                foreach (NPC n in Main.npc)
                {
                    if (n.active && !n.friendly && !n.dontTakeDamage)
                    {
                        if (Util.Util.getDistance(n.Center,npc.Center) < dist)
                        {
                            t = n;
                            dist = Util.Util.getDistance(n.Center, npc.Center);
                        }
                    }
                }
                if (t == null)
                {
                    f_target = -1;
                    plrOldPos = Main.player[0].position;
                    plrOldVel = Main.player[0].velocity;
                    Main.player[0].Center = f_owner.ToPlayer().Center;
                    Main.player[0].velocity = f_owner.ToPlayer().velocity;
                }
                else
                {
                    f_target = t.whoAmI;
                    plrOldPos = Main.player[0].position;
                    plrOldVel = Main.player[0].velocity;
                    Main.player[0].Center = t.Center;
                    Main.player[0].velocity = t.velocity;
                }
                if (npc.aiStyle == NPCAIStyleID.Slime)
                {
                    Main.LocalPlayer.npcTypeNoAggro[npc.type] = false;
                    npc.TargetClosest();
                }
                if (npc.realLife < 0) {
                    foreach (NPC nPC in Main.npc)
                    {
                        if (nPC.realLife == npc.whoAmI)
                        {
                            nPC.Entropy().ToFriendly = true;
                            nPC.Entropy().f_owner = f_owner;
                        }
                    } 
                }
            }*/
            if (npc.Entropy().daTarget && npc.realLife == -1)
            {
                npc.velocity *= 0;
                return false;
            }
            dscd--;
            vtnoparticle--;
            if (npc.Entropy().VoidTouchTime > 0)
            {
                if (vtnoparticle <= 0 && false)
                {
                    for (int i = 0; i < 1; i++)
                    {
                        Particle p = new Particle();
                        p.position = npc.Center;
                        p.alpha = 0.5f;

                        var rd = Main.rand;
                        p.velocity = new Vector2((float)((rd.NextDouble() - 0.5) * 6), (float)((rd.NextDouble() - 0.5) * 6));
                        VoidParticles.particles.Add(p);
                    }
                }
                if (Main.GameUpdateCount % 20 == 0 && !npc.dontTakeDamage)
                {
                    NPC.HitInfo hit = npc.CalculateHitInfo((int)(50 * npc.Entropy().VoidTouchLevel * (1 - npc.Entropy().VoidTouchDR)), 0, false, 0, DamageClass.Generic, false, 0);
                    hit.HideCombatText = true;
                    int damageDone = npc.StrikeNPC(hit, false, false);
                    CombatText.NewText(npc.getRect(), new Color(148, 148, 255), damageDone);


                }
                if (!(npc.ModNPC is VoidCultist))
                {
                    if (npc.boss)
                    {
                        if (VoidTouchDR < 0.2f)
                        {
                            npc.velocity *= 0.98f;
                        }
                    }
                    else
                    {
                        npc.velocity *= 0.96f;
                    }
                }
                var r = Main.rand;
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.CorruptSpray, (float)r.NextDouble() * 2 - 1, (float)r.NextDouble() * 2 - 1);
                npc.Entropy().VoidTouchTime = VoidTouchTime - 1;
            }
            if (npc.Entropy().VoidTouchTime > 0)
            {
                npc.AddBuff(ModContent.BuffType<VoidTouch>(), npc.Entropy().VoidTouchTime);
            }
            else
            {
                npc.Entropy().VoidTouchLevel = 0;
            }

            return base.PreAI(npc);
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage += (npc.Entropy().VoidTouchLevel) * 0.01f * (1 - npc.Entropy().VoidTouchDR);
            if (projectile.owner >= 0)
            {
                if (projectile.owner.ToPlayer().Entropy().nihShell)
                {
                    modifiers.CritDamage += 0.10f;
                }
            }
            if (projectile.owner >= 0)
            {
                if (projectile.owner.ToPlayer().Entropy().VFSet)
                {

                    if (projectile.owner.ToPlayer().Entropy().VFHelmMelee)
                    {
                        projectile.owner.ToPlayer().Entropy().VoidCharge += 0.005f;
                    }
                    if (projectile.Calamity().stealthStrike)
                    {
                        projectile.owner.ToPlayer().Entropy().VoidCharge += 0.06f;
                    }
                    else
                    {
                        projectile.owner.ToPlayer().Entropy().VoidCharge += 0.008f;
                    }
                    if (projectile.owner.ToPlayer().Entropy().VoidCharge > 1)
                    {
                        projectile.owner.ToPlayer().Entropy().VoidCharge = 1;
                    }
                }
            }

        }
        public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            if (player.Entropy().nihShell)
            {
                modifiers.CritDamage += 0.10f;
            }
            modifiers.FinalDamage += (npc.Entropy().VoidTouchLevel) * 0.05f * (1 - npc.Entropy().VoidTouchDR);
            if (player.Entropy().VFSet)
            {
                player.Entropy().VoidCharge += 0.008f;
                if (player.Entropy().VFHelmMelee)
                {
                    player.Entropy().VoidCharge += 0.005f;
                }

                if (player.Entropy().VoidCharge > 1)
                {
                    player.Entropy().VoidCharge = 1;
                }
            }

        }

        public static bool AddVoidTouch(NPC nPC, int time, float level, int maxTime = 600, int maxLevel = 10)
        {
            if (nPC.Entropy().VoidTouchTime < maxTime)
            {
                nPC.Entropy().VoidTouchTime += (int)(time * 1.4f);
                if (nPC.Entropy().VoidTouchTime > maxTime)
                {
                    nPC.Entropy().VoidTouchTime = maxTime;
                }
            }
            if (nPC.Entropy().VoidTouchLevel < maxLevel)
            {
                nPC.Entropy().VoidTouchLevel += level / 10;
                if (nPC.Entropy().VoidTouchLevel > maxLevel)
                {
                    nPC.Entropy().VoidTouchLevel = maxLevel;
                }
            }
            return true;
        }
        public static bool AddVoidTouch(Player nPC, int time, int level, int maxTime = 600, int maxLevel = 10)
        {
            nPC.AddBuff(ModContent.BuffType<VoidTouch>(), maxTime);
            return true;
        }
        public static ReLogic.Content.Asset<Texture2D> Request(string p)
        {
            return ModContent.Request<Texture2D>(p);
        }
        public override void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (npc.Entropy().daTarget)
            {
                drawColor = Color.Black;
            }
        }
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<Yharon>())
            {
                npcLoot.Add(ItemDropRule.ByCondition(new IsNormal(), ModContent.ItemType<Vitalfeather>(),10 ));
            }
            if (npc.type == ModContent.NPCType<PrimordialWyrmHead>())
            {
                npcLoot.Add(ModContent.ItemType<WyrmTooth>(), 1, 28, 32);
            }
            if (npc.type == ModContent.NPCType<EidolonWyrmHead>())
            {
                npcLoot.Add(ModContent.ItemType<Nothing>(), 4, 1, 1);
                npcLoot.Add(ModContent.ItemType<BookMarkAbyss>(), 8, 1, 1);
            }
            if (npc.type == ModContent.NPCType<GiantClam>())
            {
                npcLoot.Add(ModContent.ItemType<BookMarkSunkenSea>(), 1, 1, 1);
            }
        }
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type != NPCID.BrainofCthulhu && (npc.type != NPCID.DukeFishron || npc.ai[0] <= 9f) && npc.active)
            {
                if (CalamityConfig.Instance.DebuffDisplay && (npc.boss || BossHealthBarManager.MinibossHPBarList.Contains(npc.type) || BossHealthBarManager.OneToMany.ContainsKey(npc.type) || CalamityEntropy.calDebuffIconDisplayList.Contains(npc.type)))
                {
                    IList<Texture2D> buffTextureList = new List<Texture2D>();
                    CalamityGlobalNPC cn = npc.Calamity();

                    if (cn.astralInfection > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/AstralInfectionDebuff").Value);
                    if (cn.banishingFire > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/BanishingFire").Value);
                    if (cn.bFlames > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/BrimstoneFlames").Value);
                    if (cn.bBlood > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/BurningBlood").Value);
                    if (cn.brainRot > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/BrainRot").Value);
                    if (cn.cDepth > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/CrushDepth").Value);
                    if (cn.rTide > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/RiptideDebuff").Value);
                    if (cn.dragonFire > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/Dragonfire").Value);
                    if (cn.miracleBlight > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/MiracleBlight").Value);
                    if (cn.gsInferno > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/GodSlayerInferno").Value);
                    if (cn.hFlames > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/HolyFlames").Value);
                    if (cn.nightwither > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/Nightwither").Value);
                    if (cn.pFlames > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/Plague").Value);
                    if (cn.sagePoisonTime > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/SagePoison").Value);
                    if (cn.shellfishVore > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/ShellfishClaps").Value);
                    if (cn.somaShredStacks > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/Shred").Value);
                    if (cn.clamDebuff > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/SnapClamDebuff").Value);
                    if (cn.sulphurPoison > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/SulphuricPoisoning").Value);
                    if (cn.vaporfied > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/Vaporfied").Value);
                    if (cn.vulnerabilityHex > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/VulnerabilityHex").Value);

                    if (cn.aCrunch > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/ArmorCrunch").Value);
                    if (cn.crumble > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/Crumbling").Value);
                    if (cn.eutrophication > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/Eutrophication").Value);
                    if (cn.gState > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/GlacialState").Value);
                    if (cn.irradiated > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/Irradiated").Value);
                    if (cn.kamiFlu > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/KamiFlu").Value);
                    if (cn.marked > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/MarkedforDeath").Value);
                    if (cn.absorberAffliction > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/AbsorberAffliction").Value);
                    if (cn.pearlAura > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/PearlAura").Value);
                    if (cn.relicOfResilienceWeakness > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/ProfanedWeakness").Value);
                    if (cn.tSad > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/TemporalSadness").Value);
                    if (cn.tesla > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/GalvanicCorrosion").Value);
                    if (cn.timeSlow > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/TimeDistortion").Value);
                    if (cn.wDeath > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/WhisperingDeath").Value);
                    if (cn.wither > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/StatDebuffs/WitherDebuff").Value);

                    if (cn.RancorBurnTime > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/RancorBurn").Value);

                    if (cn.electrified > 0)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Electrified].Value);
                    if (npc.onFire)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.OnFire].Value);
                    if (npc.poisoned)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Poisoned].Value);
                    if (npc.onFire2)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.CursedInferno].Value);
                    if (npc.onFrostBurn)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Frostburn].Value);
                    if (npc.venom)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Venom].Value);
                    if (npc.shadowFlame)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.ShadowFlame].Value);
                    if (npc.oiled)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Oiled].Value);
                    if (npc.javelined)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.BoneJavelin].Value);
                    if (npc.daybreak)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Daybreak].Value);
                    if (npc.celled)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.StardustMinionBleed].Value);
                    if (npc.dryadBane)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.DryadsWardDebuff].Value);
                    if (npc.dryadWard)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.DryadsWard].Value);
                    if (npc.soulDrain && npc.realLife == -1)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.SoulDrain].Value);
                    if (npc.onFire3) buffTextureList.Add(TextureAssets.Buff[BuffID.OnFire3].Value);
                    if (npc.onFrostBurn2) buffTextureList.Add(TextureAssets.Buff[BuffID.Frostburn2].Value);
                    if (npc.tentacleSpiked)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.TentacleSpike].Value);

                    if (npc.confused)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Confused].Value);
                    if (npc.ichor)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Ichor].Value);
                    if (cn.slowed > 0)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Slow].Value);
                    if (cn.webbed > 0)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Webbed].Value);
                    if (npc.midas)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Midas].Value);
                    if (npc.loveStruck)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Lovestruck].Value);
                    if (npc.stinky)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Stinky].Value);
                    if (npc.betsysCurse)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.BetsysCurse].Value);
                    if (npc.dripping)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Wet].Value);
                    if (npc.drippingSlime)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Slimed].Value);
                    if (npc.drippingSparkleSlime)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.GelBalloonBuff].Value);
                    if (npc.markedByScytheWhip)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.ScytheWhipEnemyDebuff].Value);
                    if (npc.HasBuff<VoidVirus>())
                    {
                        buffTextureList.Add(TextureAssets.Buff[ModContent.BuffType<VoidVirus>()].Value);
                    }
                    if (npc.HasBuff<Deceive>())
                    {
                        buffTextureList.Add(TextureAssets.Buff[ModContent.BuffType<Deceive>()].Value);
                    }
                    if (npc.GetGlobalNPC<ScorpioEffectNPC>().effectLevel > 0)
                    {
                        buffTextureList.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Buffs/AstralScorpionPoisonous").Value);
                    }
                    bool voidTouchDraw = false;
                    int voidTouchIndex = 0;
                    if (npc.Entropy().VoidTouchTime > 0)
                    {
                        buffTextureList.Add(Request("CalamityEntropy/Content/Buffs/VoidTouch").Value);
                        voidTouchDraw = true;
                        voidTouchIndex = buffTextureList.Count - 1;
                    }
                    bool abyssMarkDraw = false;
                    int abyssMarkIndex = 0;
                    if (npc.Entropy().StareOfAbyssLevel > 0)
                    {
                        buffTextureList.Add(Request("CalamityEntropy/Content/Buffs/Wyrm/StareOfTheAbyss").Value);
                        abyssMarkDraw = true;
                        abyssMarkIndex = buffTextureList.Count - 1;
                    }
                    bool eclipseMarkDraw = false;
                    int eclipseMarkIndex = 0;
                    if (npc.Entropy().EclipsedImprintLevel > 0)
                    {
                        buffTextureList.Add(Request("CalamityEntropy/Content/Buffs/Wyrm/EclipsedImprint").Value);
                        eclipseMarkDraw = true;
                        eclipseMarkIndex = buffTextureList.Count - 1;
                    }
                    int buffTextureListLength = buffTextureList.Count;

                    int totalLength = buffTextureListLength * 14;

                    int buffDisplayRowLimit = 5;

                    float drawPosX = totalLength >= 80f ? 40f : totalLength / 2;

                    float npcHeight = TextureAssets.Npc[npc.type].Value.Height / Main.npcFrameCount[npc.type] / 2 * npc.scale;

                    float drawPosY = npcHeight + npc.gfxOffY + 16f;

                    for (int i = 0; i < buffTextureList.Count; i++)
                    {
                        if (i != 0)
                        {
                            if (i % buffDisplayRowLimit == 0)
                                drawPosX = 40f;
                            else
                                drawPosX -= 14f;
                        }

                        float additionalYOffset = 14f * (float)Math.Floor(i * 0.2);

                        var tex = buffTextureList.ElementAt(i);
                        spriteBatch.Draw(tex, npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset), null, Color.White, 0f, default, 0.5f, SpriteEffects.None, 0f);
                        if (voidTouchDraw && i == voidTouchIndex)
                        {
                            spriteBatch.DrawString(FontAssets.MouseText.Value, ((int)npc.Entropy().VoidTouchLevel).ToString(), npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        }
                        if (abyssMarkDraw && i == abyssMarkIndex)
                        {
                            spriteBatch.DrawString(FontAssets.MouseText.Value, npc.Entropy().StareOfAbyssLevel.ToString(), npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        }
                        if (eclipseMarkDraw && i == eclipseMarkIndex)
                        {
                            spriteBatch.DrawString(FontAssets.MouseText.Value, npc.Entropy().EclipsedImprintLevel.ToString(), npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                        }
                    }
                }
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void OnKill(NPC npc)
        {
            if (npc.type == ModContent.NPCType<PrimordialWyrmHead>())
            {
                DownedBossSystem.downedPrimordialWyrm = true;
            }
            if (DownedBossSystem.downedAquaticScourge)
            {
                if (npc.ModNPC is Viperfish)
                {
                    if (Main.rand.NextBool(5))
                    {
                        Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<AbyssalPiercer>()));
                    }
                }
                if (npc.ModNPC is GiantSquid)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<AbyssalPiercer>()));
                    }
                }
            }
            if (npc.type == -3 || npc.type == NPCID.BlueSlime || npc.type == -8 || npc.type == -7 || npc.type == -9 || npc.type == -6 || npc.type == NPCID.IceSlime || npc.type == -10)
            {
                if (Main.rand.NextBool(420))
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<CarlosIceCream>()));
                }
            }
            if (!npc.friendly && npc.lifeMax > 20)
            {
                if (Main.bloodMoon)
                {
                    if (Main.rand.NextBool(500))
                    {
                        Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<CrimsonNight>()));
                    }
                }
                Player n = null;
                Player h = null;
                foreach (Player plr in Main.player)
                {
                    if (plr.active && Util.Util.getDistance(plr.Center, npc.Center) < 4000)
                    {
                        if (plr.ZoneHallow)
                        {
                            n = plr;
                        }
                        if (plr.Center.Y / 16 > Main.UnderworldLayer)
                        {
                            h = plr;
                        }
                    }
                }
                if (n != null)
                {
                    if (Main.rand.NextBool(70))
                    {
                        Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<HolyMantle>()));
                    }
                    if (Main.rand.NextBool(80) && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    {
                        Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<TheRevelation>()));
                    }
                }
                if (h != null)
                {
                    if (Main.rand.NextBool(60) && NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
                    {
                        Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<MawOfTheVoid>()));
                    }
                }
            }
            if (ToFriendly)
            {
                Main.player[Main.player.Length - 1].active = false;
            }
            if (npc.boss)
            {
                if (Main.dedServ)
                {
                    ModPacket pack = Mod.GetPacket();
                    pack.Write((byte)CEMessageType.BossKilled);
                    pack.Write(npc.ModNPC == null || npc.ModNPC.Mod is not CalamityMod.CalamityMod);
                    pack.Send();
                }
                else
                {
                    if (ModContent.GetInstance<Config>().BindingOfIsaac_Rep_BossMusic && !Main.dedServ && CalamityEntropy.noMusTime <= 0 && !BossRushEvent.BossRushActive && (ModContent.GetInstance<Config>().RepBossMusicReplaceCalamityMusic || npc.ModNPC == null || npc.ModNPC.Mod is not CalamityMod.CalamityMod))
                    {
                        CalamityEntropy.noMusTime = 300;
                        SoundEngine.PlaySound(new("CalamityEntropy/Assets/Sounds/Music/RepTrackJingle"));
                    }
                }
                if (lostSoulDrop)
                {
                    foreach (Projectile p in Main.projectile)
                    {
                        if (p.active && p.ModProjectile is LostSoulProj ls)
                        {
                            if (ls.hideVisualTime <= 0 && npc.realLife < 0)
                            {
                                ls.bosses.Add(npc.whoAmI);
                            }
                        }
                    }
                }
            }
            if (npc.type == ModContent.NPCType<GiantClam>())
            {
                if (!(Main.netMode == NetmodeID.MultiplayerClient))
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<EntityCard>()));
                }
            }
            if (npc.type == NPCID.SkeletronHead)
            {
                Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<WisdomCard>()));

            }
            if (npc.type == NPCID.SkeletronPrime)
            {
                Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<TemperanceCard>()));
            }

            if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
            {
                Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<Perplexed>()));
            }
            if (npc.type == NPCID.GoblinSorcerer)
            {
                if (Main.rand.NextBool(4))
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<Tarnish>()));
                }
            }
            if (npc.type == ModContent.NPCType<Eidolist>())
            {
                if (Main.rand.NextBool(3))
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<Fool>()));
                }
            }
            if (npc.type == ModContent.NPCType<SlimeGodCore>())
            {
                Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<Frail>()));

            }
            if (npc.type == NPCID.GiantWormHead)
            {
                if (Main.rand.NextDouble() < 0.1f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<CannedCarrion>(), 10));
                }
            }
            if (npc.type == NPCID.WyvernHead)
            {
                if (Main.rand.NextDouble() < 0.06f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<DreamCatcher>(), 10));
                }
            }
            if (npc.type == NPCID.Harpy || npc.type == NPCID.WyvernHead)
            {
                if (Main.rand.NextDouble() < 0.04f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<LightningPendant>(), 10));
                }
            }
            if (npc.type == NPCID.Wraith || npc.type == NPCID.PossessedArmor)
            {
                if (Main.rand.NextDouble() < 0.03f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<SoulCandle>(), 10));
                }
                if (Main.rand.NextDouble() < 0.03f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<LostSoul>(), 10));
                }
            }
            if (npc.type == ModContent.NPCType<CeaselessVoid>())
            {
                if (Main.rand.NextDouble() < 0.16f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<BottleDarkMatter>(), 1));
                }
            }
            if (npc.type == ModContent.NPCType<RavagerBody>())
            {
                Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<SacrificalMask>(), 1));
            }

            if (npc.type == ModContent.NPCType<DevilFish>() || npc.type == ModContent.NPCType<Laserfish>() || npc.type == ModContent.NPCType<ToxicMinnow>() || npc.type == ModContent.NPCType<LuminousCorvina>() || npc.type == ModContent.NPCType<Viperfish>() || npc.type == ModContent.NPCType<OarfishHead>())
            {
                if (Main.rand.NextDouble() < 0.03f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<ToyRock>(), 1));
                }
            }
            if (npc.type == ModContent.NPCType<AstrumDeusHead>())
            {
                string modFolder = Path.Combine(Main.SavePath, "CalamityEntropy"); string myDataFilePath = Path.Combine(modFolder, "DeusKilled.txt");
                if (!Directory.Exists(modFolder))
                {
                    Directory.CreateDirectory(modFolder);
                }

                using (StreamWriter sw = new StreamWriter(myDataFilePath))
                {
                    sw.WriteLine("a");
                }
            }
            if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
            {
                string modFolder = Path.Combine(Main.SavePath, "CalamityEntropy"); string myDataFilePath = Path.Combine(modFolder, "DoGKilled.txt");
                if (!Directory.Exists(modFolder))
                {
                    Directory.CreateDirectory(modFolder);
                }

                using (StreamWriter sw = new StreamWriter(myDataFilePath))
                {
                    sw.WriteLine("a");
                }
            }

        }
        public class IsNormal : IItemDropRuleCondition, IProvideItemConditionDescription
        {
            public bool CanDrop(DropAttemptInfo info) => !Main.expertMode;
            public bool CanShowItemDropInUI() => !Main.expertMode;
            public string GetConditionDescription() => "Normal Only";
        }
        public int f_owner = -1;
        public bool lostSoulDrop = true;
        public int deusBloodOut = 0;
        public int noelctime = 0;
        public void onHurt(NPC npc, int damage, Player player, Entity source, NPC.HitInfo hit)
        {
            if (player != null)
            {
                if (player.Entropy().heartOfStorm)
                {
                    int lasertype = ModContent.ProjectileType<ElectricLaser>();
                    if (source == null || ((source is Projectile pj) && ((!(pj.type == lasertype)) || pj.ai[2] < Main.rand.Next(1, 6))))
                    {
                        if ((source is Projectile j && j.type == lasertype) || Main.rand.NextBool(3))
                        {
                            NPC target = null;
                            float dist = 500;
                            foreach (NPC npcf in Main.ActiveNPCs)
                            {
                                float d = Util.Util.getDistance(npcf.Center, npc.Center);
                                if (npcf.whoAmI != npc.whoAmI && !npcf.friendly && d < dist && npcf.Entropy().noelctime <= 0)
                                {
                                    target = npcf;
                                    dist = d;
                                }
                            }
                            if (target != null)
                            {
                                noelctime = 4;
                                Projectile.NewProjectile(player.GetSource_FromThis(), npc.Center, Vector2.Zero, lasertype, damage / 4, 0, player.whoAmI, target.Center.X, target.Center.Y, (source is Projectile p && p.type == lasertype) ? p.ai[2] + 1 : 0);
                            }
                        }
                    }
                }
            }
        }
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            onHurt(npc, damageDone, player, null, hit);
            if (player.Entropy().deusCoreBloodOut > 0 && player.Entropy().bloodTrCD <= 0)
            {
                int btransfer = (int)MathHelper.Min(player.Entropy().deusCoreBloodOut, damageDone / 80 + 1);
                if (btransfer > 60)
                {
                    btransfer = 60;
                }
                player.Entropy().bloodTrCD = 4;
                player.Entropy().deusCoreBloodOut -= btransfer;
                deusBloodOut += btransfer;
            }
            if (player.Entropy().nihShell)
            {
                NihilityShell.checkDamage(player, hit);
            }
            if (player.Entropy().ConfuseCard)
            {
                npc.AddBuff(ModContent.BuffType<Deceive>(), 420);
            }
            if (player.Entropy().AttackVoidTouch > 0)
            {
                float vt = player.Entropy().AttackVoidTouch * 10;
                AddVoidTouch(npc, (int)(vt * 120), vt, 600, (int)Math.Round(vt * 8));
            }
            player.Entropy().damageRecord += damageDone;
            if (player.Entropy().brokenAnkh && player.Entropy().damageRecord > 420)
            {
                player.Entropy().damageRecord = 0;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int i = Item.NewItem(player.GetSource_FromThis(), player.getRect(), new Item(ModContent.ItemType<PoopPickup>()), false, true);
                    Main.item[i].noGrabDelay = 100;
                    if (!Main.dedServ)
                    {
                        Util.Util.PlaySound("fart", 1, player.Center);
                    }
                }
                else
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)CEMessageType.SpawnItem);
                    packet.Write(player.whoAmI);
                    packet.Write(ModContent.ItemType<PoopPickup>());
                    packet.Write(1);
                    packet.Send();
                }
            }
        }

        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            Player sourcePlr = null;
            if (projectile.friendly)
            {
                Player player = projectile.owner.ToPlayer();
                sourcePlr = player;
                if (player.Entropy().deusCoreBloodOut > 0 && player.Entropy().bloodTrCD <= 0)
                {
                    int btransfer = (int)MathHelper.Min(player.Entropy().deusCoreBloodOut, damageDone / 80 + 1);
                    if (btransfer > 60)
                    {
                        btransfer = 60;
                    }
                    player.Entropy().bloodTrCD = 4;
                    player.Entropy().deusCoreBloodOut -= btransfer;
                    deusBloodOut += btransfer;
                }
                if (player.Entropy().ConfuseCard)
                {
                    npc.AddBuff(ModContent.BuffType<Deceive>(), 420);
                }
                if (projectile.owner != -1)
                {
                    if (projectile.owner.ToPlayer().active)
                    {
                        if (projectile.owner.ToPlayer().Entropy().AttackVoidTouch > 0)
                        {
                            float vt = projectile.owner.ToPlayer().Entropy().AttackVoidTouch * 10;
                            AddVoidTouch(npc, (int)(vt * 120), vt, 600, (int)Math.Round(vt * 8));
                        }
                    }
                }
                if (player.Entropy().nihShell)
                {
                    NihilityShell.checkDamage(player, hit);
                }
                player.Entropy().damageRecord += damageDone;
                if (player.Entropy().brokenAnkh && player.Entropy().damageRecord > 420)
                {
                    player.Entropy().damageRecord = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int i = Item.NewItem(player.GetSource_FromThis(), player.getRect(), new Item(ModContent.ItemType<PoopPickup>()), false, true);
                        Main.item[i].noGrabDelay = 100;
                        if (!Main.dedServ)
                        {
                            Util.Util.PlaySound("fart", 1, player.Center);
                        }
                    }
                    else
                    {
                        ModPacket packet = Mod.GetPacket();
                        packet.Write((byte)CEMessageType.SpawnItem);
                        packet.Write(player.whoAmI);
                        packet.Write(ModContent.ItemType<PoopPickup>());
                        packet.Write(1);
                        packet.Send();
                    }
                }
            }
            onHurt(npc, damageDone, sourcePlr, projectile, hit);
        }
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            /*if(source is EntitySource_Parent esource)
            {
                if(esource.Entity is NPC np)
                {
                    ToFriendly = np.Entropy().ToFriendly;
                    if (ToFriendly)
                    {
                        f_owner = np.Entropy().f_owner;
                    }
                }
                if (esource.Entity is Projectile pj)
                {
                    ToFriendly = pj.Entropy().ToFriendly;
                }
                if (ToFriendly)
                {
                    npc.friendly = true;
                }
            }*/
        }



        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == 17)
            {
                shop.Add(ModContent.ItemType<SoyMilk>(), new Condition(Mod.GetLocalization("DownedBoss2").Value, () => NPC.downedBoss2));
                shop.Add(ModContent.ItemType<BrillianceCard>());
            }
            if (shop.NpcType == ModContent.NPCType<DILF>())
            {
                shop.Add(ModContent.ItemType<ThreadOfFate>());
                shop.Add(ModContent.ItemType<ArchmagesHandmirror>());
            }
            if (shop.NpcType == 108)
            {
                shop.Add(ModContent.ItemType<AuraCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));
                shop.Add(ModContent.ItemType<BrillianceCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));
                shop.Add(ModContent.ItemType<EnduranceCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));
                shop.Add(ModContent.ItemType<EntityCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));
                shop.Add(ModContent.ItemType<InspirationCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));
                shop.Add(ModContent.ItemType<MetropolisCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));
                shop.Add(ModContent.ItemType<RadianceCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));
                shop.Add(ModContent.ItemType<TemperanceCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));
                shop.Add(ModContent.ItemType<WisdomCard>(), new Condition(Mod.GetLocalization("HaveOracleDeck"), () => Main.LocalPlayer.Entropy().oracleDeckInInv));

                shop.Add(ModContent.ItemType<Barren>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));
                shop.Add(ModContent.ItemType<Confuse>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));
                shop.Add(ModContent.ItemType<Fool>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));
                shop.Add(ModContent.ItemType<Frail>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));
                shop.Add(ModContent.ItemType<GreedCard>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));
                shop.Add(ModContent.ItemType<Nothing>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));
                shop.Add(ModContent.ItemType<Perplexed>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));
                shop.Add(ModContent.ItemType<Sacrifice>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));
                shop.Add(ModContent.ItemType<Tarnish>(), new Condition(Mod.GetLocalization("HaveTaintedDeck"), () => Main.LocalPlayer.Entropy().taintedDeckInInv));

            }
            if (shop.NpcType == 663)
            {
                shop.Add(ModContent.ItemType<CarlosIceCream>());
            }
            if (shop.NpcType == 20)
            {
                shop.Add(ModContent.ItemType<Confuse>());
            }
            if (shop.NpcType == ModContent.NPCType<THIEF>())
            {
                shop.Add(ModContent.ItemType<Barren>());
            }
        }
    }
}
