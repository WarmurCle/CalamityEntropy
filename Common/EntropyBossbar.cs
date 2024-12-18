using CalamityEntropy.Content.NPCs.Cruiser;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.NPCs.SlimeGod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Events;
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
            EntropyBossbar.bossbarColor[636] = Main.DiscoColor;
            int type = npc.type;
            if (npc.type == ModContent.NPCType<CruiserHead>() && npc.ModNPC is CruiserHead cr)
            {
                if (cr.phaseTrans >= 120)
                {
                    return new Color(150, 60, 255);
                }
            }
            if (bossbarColor.ContainsKey(npc.type))
            {
                return bossbarColor[npc.type];
            }
            return new Color(255, 40, 40);
        }
        public int drawOfs = 0;
        public override bool PreventDraw => true;
        public override void Draw(SpriteBatch spriteBatch, IBigProgressBar currentBar, BigProgressBarInfo info)
        {
            
            if (info.npcIndexToAimAt >= 0 && info.npcIndexToAimAt.ToNPC().active && info.npcIndexToAimAt.ToNPC().IsABoss())
            {
                
                NPC npc = info.npcIndexToAimAt.ToNPC();
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

                if (npc.dontTakeDamage && !(npc.ModNPC is SlimeGodCore))
                {
                    barColor = Color.Lerp(barColor, new Color(200, 106, 205), 0.1f);
                }
                else
                {
                    barColor = Color.Lerp(barColor, getNpcBarColor(npc), 0.1f);
                    drawOfs -= 5;
                }

                Vector2 center = new Vector2(Main.screenWidth / 2, Main.screenHeight - 100);

                float prog = (float)info.npcIndexToAimAt.ToNPC().life / (float)info.npcIndexToAimAt.ToNPC().lifeMax;
                if (info.npcIndexToAimAt.ToNPC().type == NPCID.EaterofWorldsHead || info.npcIndexToAimAt.ToNPC().type == NPCID.EaterofWorldsBody || info.npcIndexToAimAt.ToNPC().type == NPCID.EaterofWorldsTail)
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
                if (info.npcIndexToAimAt.ToNPC().type == ModContent.NPCType<SlimeGodCore>())
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
                if (prog == 0)
                {
                    if (!npc.dontTakeDamage || npc.ModNPC is SlimeGodCore)
                    {
                        barColor = getNpcBarColor(npc);
                    }
                }
                comboTime--;
                if (comboTime < 0 || comboTarget - prog > 0.1)
                {
                    comboTime = 0;
                    comboTarget = prog;
                }
                if (prog < lastProg)
                {
                    if(prog < lastProg - 0.005f)
                    {
                        comboTime = 60;
                    }
                    lastProg = prog;
                    
                }
                comboProg = comboProg + (comboTarget - comboProg) * 0.1f;
                Texture2D bar1 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebar1").Value;
                Texture2D bar2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebar2").Value;
                Texture2D bar3 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebar3").Value;
                Texture2D barLocked = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/EbarLock").Value;
                Texture2D barWhite = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/EBarWhite").Value;
                Texture2D barWhite2 = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/EBarWhite2").Value;
                Texture2D barc = ModContent.Request<Texture2D>("CalamityEntropy/Assets/Bossbar/Ebarc").Value;


                spriteBatch.Draw(barWhite, center, new Rectangle(0, 0, 18 + (int)(500 * comboProg), bar1.Height), Color.White, 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);
                if (npc.dontTakeDamage && !(npc.ModNPC is SlimeGodCore))
                {
                    spriteBatch.Draw(barWhite2, center, new Rectangle(0, 0, 18 + (int)(500 * prog) + 2, bar1.Height), Color.Lerp(barColor, Color.White, 0.5f), 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);
                }

                spriteBatch.UseSampleState(SamplerState.LinearWrap);
                spriteBatch.Draw(bar2, center + new Vector2(0, 8), new Rectangle(drawOfs, 0, (int)(500 * prog), bar2.Height), barColor, 0, bar2.Size() / 2, 1, SpriteEffects.None, 0);
                spriteBatch.UseSampleState(SamplerState.AnisotropicClamp);
                
                spriteBatch.Draw(barc, center + new Vector2(0, 8), new Rectangle(0, 0, (int)(500 * prog), bar2.Height), barColor, 0, barc.Size() / 2, 1, SpriteEffects.None, 0);

                if (npc.dontTakeDamage && !(npc.ModNPC is SlimeGodCore))
                {
                    spriteBatch.Draw(barLocked, center, new Rectangle(0, 0, 18 + (int)(500 * prog), bar1.Height), Color.Lerp(barColor, Color.White, 0.36f), 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);

                }
                spriteBatch.Draw(bar3, center, null, buttomColor, 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);
                spriteBatch.Draw(bar1, center, null, Color.White, 0, bar1.Size() / 2, 1, SpriteEffects.None, 0);

                if(npc.GetBossHeadTextureIndex() >= 0)
                {
                    Texture2D headBoss = TextureAssets.NpcHeadBoss[npc.GetBossHeadTextureIndex()].Value;
                    spriteBatch.Draw(headBoss, center + new Vector2(0, -14), null, Color.White, 0, headBoss.Size() / 2, 1, SpriteEffects.None, 0);
                }
            }

        }
    }
}
