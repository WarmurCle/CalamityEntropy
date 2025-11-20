using CalamityEntropy.Content.ILEditing;
using CalamityEntropy.Content.NPCs;
using CalamityEntropy.Content.NPCs.AbyssalWraith;
using CalamityEntropy.Content.NPCs.SpiritFountain;
using CalamityMod;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SlimeGod;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class EntropyBossbar : ModBossBarStyle
    {
        public override string DisplayName => "Calamity Entropy";
        public Color barColor = Color.White;
        public Color buttomColor = Color.Yellow;
        public float comboProg = 1;
        public int comboTime = 0;
        public float lastProg = 1;
        public float comboTarget = 1;
        public static Dictionary<int, Color> bossbarColor;
        public float whiteLerp = 0;
        public int comboTimeCount = 0;
        public override void Load()
        {
            bossbarColor = new Dictionary<int, Color>();
        }
        public override void Unload()
        {
            bossbarColor = null;
        }
        public static Color getNpcBarColor(NPC npc)
        {
            if (npc.GetGlobalNPC<DeliriumGlobalNPC>().delirium)
            {
                return Color.White;
            }
            EntropyBossbar.bossbarColor[636] = Main.DiscoColor;
            int type = npc.type;
            /*if (npc.type == ModContent.NPCType<CruiserHead>() && npc.ModNPC is CruiserHead cr)
            {
                if (cr.phaseTrans >= 120)
                {
                    return new Color(150, 60, 255);
                }
            }*/
            if (npc.ModNPC is Providence || npc.ModNPC is ProfanedGuardianCommander || npc.ModNPC is ProfanedGuardianHealer || npc.ModNPC is ProfanedGuardianDefender)
            {
                if (!Main.dayTime)
                {
                    return new Color(102, 255, 255);
                }
            }
            if (bossbarColor.ContainsKey(npc.type))
            {
                return bossbarColor[npc.type];
            }
            return new Color(255, 50, 50);
        }
        public int drawOfs = 0;
        public override bool PreventDraw => true;
        public override void Draw(SpriteBatch spriteBatch, IBigProgressBar currentBar, BigProgressBarInfo info)
        {
            bool flag = false;
            NPC npc = null;
            if (info.npcIndexToAimAt >= 0 && info.npcIndexToAimAt.ToNPC().active && info.npcIndexToAimAt.ToNPC().IsABoss())
            {
                flag = true;
                npc = info.npcIndexToAimAt.ToNPC();
            }
            else
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (CalamityLists.minibossList.Contains(n.type) || n.IsABoss())
                    {
                        if (n.realLife < 0 && CEUtils.getDistance(n.Center, Main.LocalPlayer.Center) < 9000)
                        {
                            flag = true;
                            npc = n;
                            break;
                        }
                    }
                }
            }
            if (!flag)
            {
                return;
            }

            Color turnColorBtm = Color.Yellow;
            if (npc.Calamity().CurrentlyIncreasingDefenseOrDR)
            {
                turnColorBtm = Color.Gray;
            }
            if (npc.Calamity().CurrentlyEnraged)
            {
                turnColorBtm = Color.Red;
            }
            buttomColor = Color.Lerp(buttomColor, turnColorBtm, 0.1f);
            bool immune = npc.dontTakeDamage && !(npc.ModNPC is SlimeGodCore) && !(npc.ModNPC is SpiritFountain);
            if (immune)
            {
                barColor = Color.Lerp(barColor, new Color(200, 106, 205), 0.1f);
            }
            else
            {
                barColor = Color.Lerp(barColor, getNpcBarColor(npc), 0.1f);
                drawOfs -= 9;
            }

            Vector2 center = new Vector2(Main.screenWidth / 2, Main.screenHeight - 70);

            float prog = (float)npc.life / (float)npc.lifeMax;
            if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
            {
                int eowLifes = 0;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == NPCID.EaterofWorldsHead || n.type == NPCID.EaterofWorldsBody || n.type == NPCID.EaterofWorldsTail)
                    {
                        eowLifes += n.life;
                    }

                }
                prog = (float)eowLifes / (float)ModContent.GetInstance<EModSys>().eowMaxLife;
            }
            if (npc.type == ModContent.NPCType<SlimeGodCore>())
            {
                int sgLifes = 0;
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (ModContent.NPCType<CrimulanPaladin>() == n.type || ModContent.NPCType<SplitCrimulanPaladin>() == n.type || ModContent.NPCType<EbonianPaladin>() == n.type || ModContent.NPCType<SplitEbonianPaladin>() == n.type)
                    {
                        sgLifes += n.life;
                    }

                }
                prog = (float)sgLifes / (float)ModContent.GetInstance<EModSys>().slimeGodMaxLife;

            }
            if (prog < 0)
                prog = 0;
            if (prog == 0)
            {
                if (!immune)
                {
                    barColor = getNpcBarColor(npc);
                }
            }
            if (immune)
            {
                comboTime = 0;
            }
            comboTime--;
            if (comboTime < 0 || comboTarget - prog > 0.2f || comboTimeCount > 240)
            {
                comboTime = 0;
                comboTarget = prog;
                comboTimeCount = 0;
            }
            if (comboTime > 0)
            {
                comboTimeCount++;
            }
            if (prog < lastProg)
            {
                comboTime = 60;
            }
            lastProg = prog;
            comboProg = comboProg + (comboTarget - comboProg) * 0.1f;
            Texture2D bar1Norm = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebar1").Value;
            Texture2D bar1_ = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebar1Alt").Value;
            Texture2D bar2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebar2").Value;
            Texture2D bar3 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebar3").Value;
            Texture2D barLocked = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/EbarLock").Value;
            Texture2D barWhite = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/EBarWhite").Value;
            Texture2D barWhite2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/EBarWhite2").Value;
            Texture2D barc = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebarc").Value;
            Texture2D bar1 = bar1Norm;
            Texture2D gzmBar = CEUtils.getExtraTex("ColorMapGoozma");
            Texture2D noise = CEUtils.getExtraTex("noise");
            Texture2D awBar = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/awraithbar").Value;
            bool goozma = false;
            bool namelessDeity = false;
            bool abyssalWraith = false;
            if (npc.GetGlobalNPC<DeliriumGlobalNPC>().delirium)
            {
                namelessDeity = true;
            }
            if (npc.ModNPC is AbyssalWraith)
            {
                abyssalWraith = true;
            }
            if (ModLoader.TryGetMod("CalamityHunt", out Mod calHunt))
            {
                if (npc.type == calHunt.Find<ModNPC>("Goozma").Type)
                {
                    goozma = true;
                }
            }
            if (ModLoader.TryGetMod("NoxusBoss", out Mod nxb))
            {
                if (npc.type == nxb.Find<ModNPC>("NamelessDeityBoss").Type)
                {
                    namelessDeity = true;
                }
            }
            if (npc.GetBossHeadTextureIndex() < 0)
            {
                bar1 = bar1_;
            }

            spriteBatch.Draw(barWhite, center, new Rectangle(0, 0, 18 + (int)(500 * comboProg), bar1.Height), Color.White, 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);
            if (npc.dontTakeDamage && !(npc.ModNPC is SlimeGodCore))
            {
                spriteBatch.Draw(barWhite2, center, new Rectangle(0, 0, 18 + (int)(500 * prog) + 2, bar1.Height), Color.Lerp(barColor, Color.White, 0.5f), 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);
            }

            spriteBatch.UseSampleState_UI(SamplerState.LinearWrap);
            if (abyssalWraith)
            {
                spriteBatch.Draw(awBar, center + new Vector2(0, 8), new Rectangle(0, (int)(1.4f * -drawOfs), (int)(500 * prog), bar2.Height), barColor, 0, bar2.Size() / 2, 1, SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(bar2, center + new Vector2(0, 8), new Rectangle(drawOfs, 0, (int)(500 * prog), bar2.Height), barColor, 0, bar2.Size() / 2, 1, SpriteEffects.None, 0);
            }
            if (goozma)
            {
                CEUtils.UseState_UI(spriteBatch, BlendState.Additive, SamplerState.LinearWrap);
                spriteBatch.Draw(gzmBar, center + new Vector2(0, 8), new Rectangle((int)(drawOfs * 5.6f), 0, (int)(500 * prog), bar2.Height), Color.White, 0, bar2.Size() / 2, 1, SpriteEffects.None, 0);
            }
            if (namelessDeity)
            {
                spriteBatch.Draw(noise, center + new Vector2(0, 8), new Rectangle((int)(drawOfs * 1.6f), (int)(drawOfs * 0.4f), (int)(500 * prog), bar2.Height), Color.White, 0, bar2.Size() / 2, 1, SpriteEffects.None, 0);
            }
            spriteBatch.UseSampleState_UI(SamplerState.AnisotropicClamp);


            if (immune && !namelessDeity)
            {
                spriteBatch.Draw(barLocked, center, new Rectangle(0, 0, 18 + (int)(500 * prog), bar1.Height), Color.Lerp(barColor, Color.White, 0.36f), 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);

            }
            spriteBatch.Draw(bar3, center, null, buttomColor, 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(bar1, center, null, Color.White, 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);

            if (npc.GetBossHeadTextureIndex() >= 0)
            {
                Texture2D headBoss = TextureAssets.NpcHeadBoss[npc.GetBossHeadTextureIndex()].Value;
                spriteBatch.Draw(headBoss, center + new Vector2(0, -14), null, Color.White, 0, headBoss.Size() / 2, 1, SpriteEffects.None, 0);
            }
            spriteBatch.UseSampleState_UI(SamplerState.PointClamp);
            Texture2D hl = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/hl").Value;
            Texture2D df = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/df").Value;
            Texture2D atk = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/atk").Value;
            Vector2 statDrawPos = center + new Vector2(-170, -30);
            Main.spriteBatch.Draw(hl, statDrawPos, null, Color.White, 0, hl.Size() / 2, 1, SpriteEffects.None, 0);
            string dstring = npc.life.ToString() + "/" + npc.lifeMax.ToString() + "(" + ((int)(((float)npc.life / (float)npc.lifeMax) * 100)).ToString() + "%)";
            Main.spriteBatch.DrawString(CalamityEntropy.efont2, dstring, statDrawPos + new Vector2(6, 0), Color.Yellow, 0, CalamityEntropy.efont2.MeasureString(dstring) / 2, 0.42f, SpriteEffects.None, 0);
            statDrawPos.X += 105 + 45 + 4 + 146;

            Main.spriteBatch.Draw(df, statDrawPos, null, Color.White, 0, df.Size() / 2, 1, SpriteEffects.None, 0);
            dstring = npc.defense.ToString() + "(-" + (int)(npc.Calamity().DR * EModILEdit.GetNPCDRMultiply(npc) * 100f) + "%)";
            Main.spriteBatch.DrawString(CalamityEntropy.efont2, dstring, statDrawPos + new Vector2(6, 0), Color.Yellow, 0, CalamityEntropy.efont2.MeasureString(dstring) / 2, 0.44f, SpriteEffects.None, 0);

            statDrawPos.X += 70 + 33;

            Main.spriteBatch.Draw(atk, statDrawPos, null, Color.White, 0, atk.Size() / 2, 1, SpriteEffects.None, 0);
            dstring = npc.damage.ToString();
            Main.spriteBatch.DrawString(CalamityEntropy.efont2, dstring, statDrawPos + new Vector2(6, 0), Color.Yellow, 0, CalamityEntropy.efont2.MeasureString(dstring) / 2, 0.44f, SpriteEffects.None, 0);

            string name = npc.FullName;
            Color tColor = getNpcBarColor(npc);
            if (!bossbarColor.ContainsKey(npc.type))
            {
                tColor = new Color(126, 135, 255);
            }
            for (int i = 0; i < 36; i++)
            {
                Main.spriteBatch.DrawString(CalamityEntropy.efont1, name, center + new Vector2(0, 28) + new Vector2(2, 0).RotatedBy(MathHelper.ToRadians(i * 10)), new Color(tColor.R / 2, tColor.G / 2, tColor.B / 2), 0, CalamityEntropy.efont1.MeasureString(name) / 2 * new Vector2(1, 0), 1.4f, SpriteEffects.None, 0);
            }

            if ((Math.Abs(tColor.R - buttomColor.R / 2) + Math.Abs(tColor.G - buttomColor.G / 2) + Math.Abs(tColor.B - buttomColor.B / 2)) / 3 < 90)
            {
                if (whiteLerp < 1)
                {
                    whiteLerp += 0.05f;
                }
            }
            else
            {
                if (whiteLerp > 0)
                {
                    whiteLerp -= 0.05f;
                }
            }
            Main.spriteBatch.DrawString(CalamityEntropy.efont1, name, center + new Vector2(0, 28), tColor * 1.1f, 0, CalamityEntropy.efont1.MeasureString(name) / 2 * new Vector2(1, 0), 1.4f, SpriteEffects.None, 0);
            spriteBatch.UseSampleState_UI(SamplerState.AnisotropicClamp);



        }
    }
}
