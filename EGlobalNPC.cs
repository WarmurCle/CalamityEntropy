using CalamityEntropy.Util;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.UI;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using ReLogic.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs;
using CalamityEntropy.Buffs;
using CalamityMod.NPCs.Abyss;
using CalamityEntropy.Items.Pets;
using CalamityMod.NPCs.TownNPCs;
using Terraria.Audio;
using Terraria.ModLoader.IO;
using System.IO;
using CalamityEntropy.NPCs.Cruiser;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.AstrumDeus;
using Terraria.DataStructures;
using System.Security.Cryptography.X509Certificates;
using CalamityEntropy.Projectiles.Pets.DoG;
using CalamityEntropy.Projectiles.Pets.Deus;
using CalamityMod.NPCs.DesertScourge;
using static System.Net.Mime.MediaTypeNames;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.StormWeaver;
using CalamityEntropy.Items;
using CalamityEntropy.Items.Accessories.Cards;
using CalamityMod.NPCs.SunkenSea;

namespace CalamityEntropy
{
    public class EGlobalNPC : GlobalNPC
    {
        public int VoidTouchTime = 0;
        public float VoidTouchLevel = 0;
        public float VoidTouchDR = 0;
        public int vtnoparticle = 0;
        public float damageMul = 1;
        public override bool InstancePerEntity => true;
        public int dscd = 0;
        public bool daTarget = false;
       
        public override bool PreAI(NPC npc)
        {
            if (npc.Entropy().daTarget && npc.realLife == -1) {
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
                if (Main.GameUpdateCount % 20 == 0)
                {
                    NPC.HitInfo hit = npc.CalculateHitInfo((int)(100 * npc.Entropy().VoidTouchLevel * (1 - npc.Entropy().VoidTouchDR)), 0, false, 0, DamageClass.Generic, false, 0);
                    hit.HideCombatText = true;
                    int damageDone = npc.StrikeNPC(hit, false, false);
                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendStrikeNPC(npc, hit);
                    CombatText.NewText(npc.getRect(), new Color(148, 148, 255), damageDone);
                }
                if (npc.boss)
                {
                    npc.velocity *= 0.96f;
                }
                else
                {
                    npc.velocity *= 0.88f;
                }
                var r = Main.rand;
                Dust.NewDust(npc.Center, npc.width, npc.height, DustID.CorruptSpray, (float)r.NextDouble() * 2 - 1, (float)r.NextDouble() * 2 - 1);
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
            modifiers.FinalDamage *= 1 + (npc.Entropy().VoidTouchLevel) * 0.05f * (1 - npc.Entropy().VoidTouchDR);
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
            modifiers.FinalDamage *= 1 + (npc.Entropy().VoidTouchLevel) * 0.05f * (1 - npc.Entropy().VoidTouchDR);
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
                nPC.Entropy().VoidTouchTime += time;
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
        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (npc.type != NPCID.BrainofCthulhu && (npc.type != NPCID.DukeFishron || npc.ai[0] <= 9f) && npc.active)
            {
                if (CalamityConfig.Instance.DebuffDisplay && (npc.boss || BossHealthBarManager.MinibossHPBarList.Contains(npc.type) || BossHealthBarManager.OneToMany.ContainsKey(npc.type) || CalamityEntropy.calDebuffIconDisplayList.Contains(npc.type)))
                {
                    IList<Texture2D> buffTextureList = new List<Texture2D>();
                    CalamityGlobalNPC cn = npc.Calamity();

                    // Damage over time debuffs
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

                    // Stat debuffs
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

                    // Visual debuff
                    if (cn.RancorBurnTime > 0)
                        buffTextureList.Add(Request("CalamityMod/Buffs/DamageOverTime/RancorBurn").Value);

                    // Vanilla damage over time debuffs
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
                    if (npc.onFire3) // Hellfire
                        buffTextureList.Add(TextureAssets.Buff[BuffID.OnFire3].Value);
                    if (npc.onFrostBurn2) // Frostbite
                        buffTextureList.Add(TextureAssets.Buff[BuffID.Frostburn2].Value);
                    if (npc.tentacleSpiked)
                        buffTextureList.Add(TextureAssets.Buff[BuffID.TentacleSpike].Value);

                    // Vanilla stat debuffs
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
                    if (npc.markedByScytheWhip) // Dark Harvest whip, the only Whip debuff that has an NPC bool
                        buffTextureList.Add(TextureAssets.Buff[BuffID.ScytheWhipEnemyDebuff].Value);
                    bool voidTouchDraw = false;
                    int voidTouchIndex = 0;
                    if (npc.HasBuff(ModContent.BuffType<VoidTouch>()) || npc.Entropy().VoidTouchTime > 0)
                    {
                        buffTextureList.Add(Request("CalamityEntropy/Buffs/VoidTouch").Value);
                        voidTouchDraw = true;
                        voidTouchIndex = buffTextureList.Count - 1;
                    }
                    // Total amount of elements in the buff list
                    int buffTextureListLength = buffTextureList.Count;

                    // Total length of a single row in the buff display
                    int totalLength = buffTextureListLength * 14;

                    // Max amount of buffs per row
                    int buffDisplayRowLimit = 5;

                    // The maximum length of a single row in the buff display
                    // Limited to 80 units, because every buff drawn here is half the size of a normal buff, 16 x 16, 16 * 5 = 80 units
                    float drawPosX = totalLength >= 80f ? 40f : (float)(totalLength / 2);

                    // The height of a single frame of the npc
                    float npcHeight = (float)(TextureAssets.Npc[npc.type].Value.Height / Main.npcFrameCount[npc.type] / 2) * npc.scale;

                    // Offset the debuff display based on the npc's graphical offset, and 16 units, to create some space between the sprite and the display
                    float drawPosY = npcHeight + npc.gfxOffY + 16f;

                    // Iterate through the buff texture list
                    for (int i = 0; i < buffTextureList.Count; i++)
                    {
                        // Reset the X position of the display every 5th and non-zero iteration, otherwise decrease the X draw position by 16 units
                        if (i != 0)
                        {
                            if (i % buffDisplayRowLimit == 0)
                                drawPosX = 40f;
                            else
                                drawPosX -= 14f;
                        }

                        // Offset the Y position every row after 5 iterations to limit each displayed row to 5 debuffs
                        float additionalYOffset = 14f * (float)Math.Floor(i * 0.2);

                        // Draw the display
                        var tex = buffTextureList.ElementAt(i);
                        spriteBatch.Draw(tex, npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset), null, Color.White, 0f, default, 0.5f, SpriteEffects.None, 0f);
                        if (voidTouchDraw && i == voidTouchIndex)
                        {
                            spriteBatch.DrawString(FontAssets.MouseText.Value, ((int)npc.Entropy().VoidTouchLevel).ToString(), npc.Center - screenPos - new Vector2(drawPosX, drawPosY + additionalYOffset), Color.White, 0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);

                        }
                        // TODO -- Show number of Shred stacks (how?)
                    }
                }
            }
            return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
        }

        public override void OnKill(NPC npc)
        {
            if (npc.type == NPCID.Harpy || npc.type == NPCID.WyvernHead)
            {
                if (Main.rand.NextDouble() < 0.04f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<LightningPendant>(), 1));
                }
            }
            if (npc.type == NPCID.Wraith || npc.type == NPCID.PossessedArmor)
            {
                if (Main.rand.NextDouble() < 0.04f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<SoulCandle>(), 1));
                }
            }
            if (npc.type == ModContent.NPCType<CeaselessVoid>())
            {
                if (Main.rand.NextDouble() < 0.16f)
                {
                    Item.NewItem(npc.GetSource_Death(), npc.getRect(), new Item(ModContent.ItemType<BottleDarkMatter>(), 1));
                }
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
                string modFolder = Path.Combine(Main.SavePath, "CalamityEntropy"); // 获取模组文件夹路径
                string myDataFilePath = Path.Combine(modFolder, "DeusKilled.txt"); // 定义文件路径

                if (!Directory.Exists(modFolder))
                {
                    Directory.CreateDirectory(modFolder); // 如果模组文件夹不存在，则创建
                }

                // 写入文件
                using (StreamWriter sw = new StreamWriter(myDataFilePath))
                {
                    sw.WriteLine("a");
                }
            }
            if (npc.type == ModContent.NPCType<DevourerofGodsHead>())
            {
                string modFolder = Path.Combine(Main.SavePath, "CalamityEntropy"); // 获取模组文件夹路径
                string myDataFilePath = Path.Combine(modFolder, "DoGKilled.txt"); // 定义文件路径

                if (!Directory.Exists(modFolder))
                {
                    Directory.CreateDirectory(modFolder); // 如果模组文件夹不存在，则创建
                }

                // 写入文件
                using (StreamWriter sw = new StreamWriter(myDataFilePath))
                {
                    sw.WriteLine("a");
                }
            }
           
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {

        }
        public override bool PreKill(NPC npc)
        {
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
            return base.PreKill(npc);
        }

        public override void ModifyShop(NPCShop shop)
        {
            if (shop.NpcType == 17)
            {
                shop.Add(ModContent.ItemType<SoyMilk>(), new Condition("Downed Boss2", () => NPC.downedBoss2));
                shop.Add(ModContent.ItemType<BrillianceCard>());
            }
            if (shop.NpcType == ModContent.NPCType<DILF>())
            {
                shop.Add(ModContent.ItemType<ThreadOfFate>());
            }
            if (shop.NpcType == 108)
            {
                shop.Add(ModContent.ItemType<AuraCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));
                shop.Add(ModContent.ItemType<BrillianceCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));
                shop.Add(ModContent.ItemType<EnduranceCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));
                shop.Add(ModContent.ItemType<EntityCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));
                shop.Add(ModContent.ItemType<InspirationCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));
                shop.Add(ModContent.ItemType<MetropolisCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));
                shop.Add(ModContent.ItemType<RadianceCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));
                shop.Add(ModContent.ItemType<TemperanceCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));
                shop.Add(ModContent.ItemType<WisdomCard>(), new Condition("Have Oracle Desk", () => Main.LocalPlayer.Entropy().oracleDeskInInv));

            }

        }
    }
}
