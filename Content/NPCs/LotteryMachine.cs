using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Util;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs
{
    public class RewardPoolItem
    {
        public int item = 0;
        public int stack = 1;
        public RewardPoolItem(int n, int s) {
            this.item = n;
            this.stack = s;
        }
    }
    public class RewardPool
    {
        public List<RewardPoolItem> items = new List<RewardPoolItem>();
        public void addPool(RewardPool pool)
        {
            foreach (RewardPoolItem item in pool.items)
            {
                this.items.Add(item);
            }
        }
        public void Add(RewardPoolItem item)
        {
            this.items.Add(item);
            
        }
        public RewardPoolItem RandomItem()
        {
            return this.items[Main.rand.Next(0, this.items.Count)];
        }
    }
    public class LotteryMachine : ModNPC
    {
        private Texture2D closed = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/off").Value;
        private Texture2D openf1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/open1").Value;
        private Texture2D openf2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/open2").Value;
        private Texture2D openf3 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/open3").Value;
        private Texture2D opened = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/on").Value;
        private Texture2D warning = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/warn").Value;
        private Texture2D warning2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/warn2").Value;
        private Texture2D unhappy = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/unhappy").Value;
        private Texture2D serious = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/serious").Value;
        private Texture2D toMad1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/tmad1").Value;
        private Texture2D toMad2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/tmad2").Value;
        private Texture2D mad = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/mad").Value;
        private Texture2D madangry = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/angry").Value;
        private Texture2D madtalk = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/mad_talk").Value;
        private Texture2D smile = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/smile").Value;
        private Texture2D flowey = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/flowey").Value;
        private Texture2D think = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/think").Value;
        private Texture2D prepare = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/prepare").Value;
        private Texture2D specialReward = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/sreward").Value;
        private Texture2D what = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/LM/what").Value;
        public bool open = false;
        public int openCouter = 0;
        public int openFrame = 0;
        public int sameItemCount = 0;
        public int lastCItem = -2;
        public int textureSpecial = 0;
        public int specialTime = 0;
        public int warnCounter = 0;
        public int SpawnTimer = 0;
        public int nucTime = 0;
        public int useCd = 0;
        public bool flag1 = false;
        private bool mouseRightClicked = false;
        public RewardPool s1;
        public RewardPool g1;
        public RewardPool p1;
        public RewardPool g2;
        public RewardPool p2;
        public RewardPool p3;
        public RewardPool p4;
        public bool sd = true;
        public bool say = false;
        public Color sayColor = Color.White;
        public string sayStr = "";

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(open);
            writer.Write(sameItemCount);
            writer.Write(lastCItem);
            writer.Write(textureSpecial);
            writer.Write(specialTime);
            writer.Write(nucTime);
            writer.Write(useCd);
            writer.Write(flag1);
            writer.Write(say);
            writer.WriteRGB(sayColor);
            writer.Write(sayStr);
                
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            open = reader.ReadBoolean();
            sameItemCount = reader.ReadInt32();
            lastCItem = reader.ReadInt32();
            textureSpecial = reader.ReadInt32();
            specialTime = reader.ReadInt32();
            nucTime = reader.ReadInt32();
            useCd = reader.ReadInt32();
            flag1 = reader.ReadBoolean();
            say = reader.ReadBoolean();
            sayColor = reader.ReadRGB();
            sayStr = reader.ReadString();
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            this.HideFromBestiary();
        }

        public override void SetDefaults()
        {
            NPC.width = 176;
            NPC.height = 176; 
            NPC.damage = 0;
            NPC.defense = 16;
            NPC.lifeMax = 200;

            NPC.value = 0f; 
            NPC.knockBackResist = 1f; 
            NPC.noTileCollide = false;
            NPC.noGravity = false;
            NPC.friendly = true;

            NPC.netAlways = true;
            
        }
        public override void OnSpawn(IEntitySource source)
        {
            
        }
        public override bool CanBeHitByNPC(NPC attacker)
        {
            return false;
        }
        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            return false;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 0;
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D tx;
            tx = closed;
            if (open)
            {
                tx = opened;
                if (openFrame < 3)
                {
                    if (openFrame == 0)
                    {
                        tx = openf1;
                    }
                    if (openFrame == 1)
                    {
                        tx = openf2;
                    }
                    if (openFrame == 2)
                    {
                        tx = openf3;
                    }
                }
                else
                {
                    if (textureSpecial == 1)
                    {
                        tx = warning;
                    }
                    if (textureSpecial == 2)
                    {
                        tx = unhappy;
                    }
                    if (textureSpecial == 3)
                    {
                        tx = serious;
                    }
                    if (textureSpecial == 4 || textureSpecial == 5 || textureSpecial == 6 || textureSpecial == 7)
                    {
                        if (warnCounter < 5)
                        {
                            tx = toMad1;
                        }
                        if (warnCounter < 10)
                        {
                            tx = toMad2;
                        }
                        if (warnCounter >= 10)
                        {
                            if (textureSpecial == 4)
                            {
                                tx = mad;
                            }
                            if (textureSpecial == 5)
                            {
                                tx = madangry;
                            }
                            if (textureSpecial == 6)
                            {
                                tx = madtalk;
                            }
                            if (textureSpecial == 7)
                            {
                                tx = warning2;
                            }

                            
                        }
                    }
                    if (textureSpecial == 8)
                    {
                        tx = smile;
                    }

                    if (textureSpecial == 10)
                    {
                        tx = think;
                    }
                    if (textureSpecial == 11)
                    {
                        tx = what;
                    }
                    if (textureSpecial == 12)
                    {
                        tx = prepare;
                    }
                    if (textureSpecial == 13)
                    {
                        tx = specialReward;
                    }
                    if (textureSpecial == 9)
                    {
                        tx = flowey;
                    }

                }
            }
            spriteBatch.Draw(tx, NPC.Center - Main.screenPosition, null, Color.White, 0, new Vector2(NPC.width, NPC.height) / 2, 1, SpriteEffects.None, 0);
            return false;
        }

        public override void AI()
        {
            if (sd)
            {
                sd = false;
                #region pools

                s1 = new RewardPool();
                s1.Add(new RewardPoolItem(ModContent.ItemType<WulfrumMetalScrap>(), 10)); s1.Add(new RewardPoolItem(ItemID.Feather, 10));
                s1.Add(new RewardPoolItem(ModContent.ItemType<EnergyCore>(), 1)); s1.Add(new RewardPoolItem(ItemID.LifeCrystal, 1));
                s1.Add(new RewardPoolItem(ModContent.ItemType<DubiousPlating>(), 2)); s1.Add(new RewardPoolItem(ModContent.ItemType<MysteriousCircuitry>(), 4));
                s1.Add(new RewardPoolItem(ModContent.ItemType<StormlionMandible>(), 1)); s1.Add(new RewardPoolItem(ModContent.ItemType<StormjawStaff>(), 1));
                s1.Add(new RewardPoolItem(ModContent.ItemType<BloodOrb>(), 20)); s1.Add(new RewardPoolItem(ItemID.Diamond, 5));
                s1.Add(new RewardPoolItem(68, 8)); s1.Add(new RewardPoolItem(ModContent.ItemType<MurkyPaste>(), 10));
                s1.Add(new RewardPoolItem(ModContent.ItemType<AncientBoneDust>(), 2)); s1.Add(new RewardPoolItem(ItemID.PoopBlock, 50));
                s1.Add(new RewardPoolItem(296, 1));
                s1.Add(new RewardPoolItem(0, 1));

                g1 = new RewardPool();
                g1.Add(new RewardPoolItem(ModContent.ItemType<SeaPrism>(), 10)); g1.Add(new RewardPoolItem(ModContent.ItemType<PearlShard>(), 3));
                g1.Add(new RewardPoolItem(ModContent.ItemType<SulphuricScale>(), 3)); g1.Add(new RewardPoolItem(1320, 1));
                g1.Add(new RewardPoolItem(ModContent.ItemType<DemonicBoneAsh>(), 2)); g1.Add(new RewardPoolItem(ModContent.ItemType<SuspiciousScrap>(), 1));
                g1.Add(new RewardPoolItem(ModContent.ItemType<BloodSample>(), 3)); g1.Add(new RewardPoolItem(ModContent.ItemType<RottenMatter>(), 3));
                g1.Add(new RewardPoolItem(ItemID.LifeCrystal, 4)); g1.Add(new RewardPoolItem(ModContent.ItemType<AerialiteBar>(), 1));
                g1.Add(new RewardPoolItem(ModContent.ItemType<AshenStalactite>(), 1)); g1.Add(new RewardPoolItem(ModContent.ItemType<VoltaicJelly>(), 1));
                g1.Add(new RewardPoolItem(ModContent.ItemType<RottenDogtooth>(), 1));
                g1.Add(new RewardPoolItem(ModContent.ItemType<AncientBoneDust>(), 2)); s1.Add(new RewardPoolItem(ItemID.PoopBlock, 50));
                g1.Add(new RewardPoolItem(0, 1));

                p1 = new RewardPool();
                p1.Add(new RewardPoolItem(2341, 1));
                p1.Add(new RewardPoolItem(906, 1)); p1.Add(new RewardPoolItem(ModContent.ItemType<AbyssalAmulet>(), 1));
                p1.Add(new RewardPoolItem(ItemID.GoldenKey, 3)); p1.Add(new RewardPoolItem(ItemID.LifeFruit, 1));
                p1.Add(new RewardPoolItem(ModContent.ItemType<OldLordClaymore>(), 1)); p1.Add(new RewardPoolItem(2296, 1));
                p1.Add(new RewardPoolItem(ItemID.FallenStar, 300)); p1.Add(new RewardPoolItem(ModContent.ItemType<AeroStone>(), 1));
                p1.Add(new RewardPoolItem(2430, 1));
                p1.Add(new RewardPoolItem(ModContent.ItemType<AncientBoneDust>(), 2));
                p1.Add(new RewardPoolItem(ItemID.PoopBlock, 50));

                g2 = new RewardPool();
                g2.Add(new RewardPoolItem(ModContent.ItemType<TitanHeart>(), 3));
                g2.Add(new RewardPoolItem(ModContent.ItemType<UrsaSergeant>(), 1));
                g2.Add(new RewardPoolItem(ModContent.ItemType<TrapperBulb>(), 5));
                g2.Add(new RewardPoolItem(ModContent.ItemType<Lumenyl>(), 30));
                g2.Add(new RewardPoolItem(ModContent.ItemType<LivingShard>(), 5));
                g2.Add(new RewardPoolItem(ModContent.ItemType<SolarVeil>(), 10));
                g2.Add(new RewardPoolItem(ModContent.ItemType<DepthCells>(), 9));
                g2.Add(new RewardPoolItem(ModContent.ItemType<AshesofCalamity>(), 1));
                g2.Add(new RewardPoolItem(1508, 3));
                g2.Add(new RewardPoolItem(391, 15));
                g2.Add(new RewardPoolItem(1198, 15));
                g2.Add(new RewardPoolItem(ModContent.ItemType<CryonicBar>(), 10));
                g2.Add(new RewardPoolItem(1612, 1));
                g2.Add(new RewardPoolItem(ModContent.ItemType<GrandScale>(), 1));
                g2.Add(new RewardPoolItem(ModContent.ItemType<MomentumCapacitor>(), 1));
                g2.Add(new RewardPoolItem(ModContent.ItemType<Abaddon>(), 1));
                g2.Add(new RewardPoolItem(ModContent.ItemType<PerennialBar>(), 5));
                g2.Add(new RewardPoolItem(ModContent.ItemType<AncientBoneDust>(), 2)); s1.Add(new RewardPoolItem(ItemID.PoopBlock, 50));

                p2 = new RewardPool();
                p2.Add(new RewardPoolItem(3456, 10));
                p2.Add(new RewardPoolItem(3457, 10));
                p2.Add(new RewardPoolItem(3458, 10));
                p2.Add(new RewardPoolItem(3459, 10));
                p2.Add(new RewardPoolItem(ModContent.ItemType<MeldBlob>(), 30));
                p2.Add(new RewardPoolItem(ModContent.ItemType<PlagueCellCanister>(), 22));
                p2.Add(new RewardPoolItem(ModContent.ItemType<UnholyEssence>(), 2));
                p2.Add(new RewardPoolItem(ModContent.ItemType<EffulgentFeather>(), 3));
                p2.Add(new RewardPoolItem(ModContent.ItemType<StarbusterCore>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<TheFirstShadowflame>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<EtherealExtorter>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<Baroclaw>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<InfectedJewel>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<AstralBar>(), 2));
                p2.Add(new RewardPoolItem(ModContent.ItemType<YharimsStimulants>(), 16));
                p2.Add(new RewardPoolItem(ModContent.ItemType<TheCamper>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<HadalMantle>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<OccultSkullCrown>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<FabsolsVodka>(), 2));
                p2.Add(new RewardPoolItem(ModContent.ItemType<Fabsoup>(), 1));
                p2.Add(new RewardPoolItem(ModContent.ItemType<AncientBoneDust>(), 2)); s1.Add(new RewardPoolItem(ItemID.PoopBlock, 50));
                p3 = new RewardPool();

                p3.Add(new RewardPoolItem(ModContent.ItemType<Necroplasm>(), 30));
                p3.Add(new RewardPoolItem(ModContent.ItemType<DarkPlasma>(), 3));
                p3.Add(new RewardPoolItem(ModContent.ItemType<UelibloomBar>(), 5));
                p3.Add(new RewardPoolItem(ModContent.ItemType<Bloodstone>(), 10));
                p3.Add(new RewardPoolItem(ModContent.ItemType<AuricOre>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<DivineGeode>(), 8));
                p3.Add(new RewardPoolItem(ModContent.ItemType<RuinousSoul>(), 3));
                p3.Add(new RewardPoolItem(ModContent.ItemType<ReaperTooth>(), 5));
                p3.Add(new RewardPoolItem(ModContent.ItemType<ReaperToothNecklace>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<TwistingNether>(), 3));
                p3.Add(new RewardPoolItem(ModContent.ItemType<ArmoredShell>(), 3));
                p3.Add(new RewardPoolItem(ModContent.ItemType<CosmicWorm>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<SupremeHealingPotion>(), 50));
                p3.Add(new RewardPoolItem(ModContent.ItemType<EidolicWail>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<PhosphorescentGauntlet>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<Murasama>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<EmpyreanKnives>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<AscendantSpiritEssence>(), 5));
                p3.Add(new RewardPoolItem(ItemID.LastPrism, 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<YharimsCrystal>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<CodebreakerBase>(), 1));

                p3.Add(new RewardPoolItem(ModContent.ItemType<PristineFury>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<Norfleet>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<PrisonOfPermafrost>(), 1));
                p3.Add(new RewardPoolItem(ModContent.ItemType<AncientBoneDust>(), 2)); s1.Add(new RewardPoolItem(ItemID.PoopBlock, 50));

                p4 = new RewardPool();
                p4.Add(new RewardPoolItem(ModContent.ItemType<AuricBar>(), 100));
                p4.Add(new RewardPoolItem(ModContent.ItemType<ShadowspecBar>(), 3));
                p4.Add(new RewardPoolItem(ModContent.ItemType<ExoPrism>(), 55));
                p4.Add(new RewardPoolItem(ModContent.ItemType<DraedonsHeart>(), 1));
                p4.Add(new RewardPoolItem(ItemID.Zenith, 1));
                p4.Add(new RewardPoolItem(ModContent.ItemType<AncientBoneDust>(), 2)); s1.Add(new RewardPoolItem(ItemID.PoopBlock, 50));

                #endregion
            }
            if (NPC.ai[0] == 1 && !Main.dedServ)
            {
                NPC.ai[0] = 0;
                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    ModPacket packet = Mod.GetPacket();
                    packet.Write((byte)CalamityEntropy.NetPackages.LotteryMachineRightClicked);
                    packet.Write(Main.LocalPlayer.whoAmI);
                    packet.Write(NPC.whoAmI);
                    packet.Write(Main.myPlayer);
                    packet.Send();
                }
                else { RightClicked(Main.LocalPlayer); }
            }
            var r = Main.rand;
            NPC.onFire = false;
            if (useCd > 0)
            {
                useCd--;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                if (!mouseRightClicked && Mouse.GetState().RightButton == ButtonState.Pressed)
                {
                    if (new Rectangle((int)Main.MouseWorld.X - 1, (int)Main.MouseWorld.Y - 1, 2, 2).Intersects(NPC.getRect()))
                    {
                        if (Util.Util.getDistance(NPC.Center, Main.LocalPlayer.Center) < 250)
                        {
                            if ((SpawnTimer <= 0 && nucTime == 0) || (Main.LocalPlayer.HeldItem.type == ItemID.CopperCoin || Main.LocalPlayer.HeldItem.type == ItemID.SilverCoin || Main.LocalPlayer.HeldItem.type == ItemID.GoldCoin || Main.LocalPlayer.HeldItem.type == ItemID.PlatinumCoin))
                            {
                                if (useCd <= 0)
                                {
                                    useCd = 60;

                                    NPC.ai[0] = 1;
                                    NPC.ai[1] = Main.myPlayer;
                                    if (Main.LocalPlayer.HeldItem.type == ItemID.CopperCoin || Main.LocalPlayer.HeldItem.type == ItemID.SilverCoin || Main.LocalPlayer.HeldItem.type == ItemID.GoldCoin || Main.LocalPlayer.HeldItem.type == ItemID.PlatinumCoin)
                                    {
                                        Main.LocalPlayer.itemAnimation = 14;
                                        Main.LocalPlayer.itemAnimationMax = 14;
                                        Main.LocalPlayer.itemTime = 14;
                                        Main.LocalPlayer.itemTimeMax = 14;
                                        Main.LocalPlayer.ApplyItemAnimation(Main.LocalPlayer.HeldItem);

                                    }

                                }
                            }
                        }
                    }
                }
                mouseRightClicked = Mouse.GetState().RightButton == ButtonState.Pressed;
            }
            NPC.life = NPC.lifeMax;
            if (nucTime > 0)
            {
                nucTime = 0;
                Vector2 spawnPos = Main.LocalPlayer.position + new Vector2(0, -600);
                int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, new Vector2(0, 10), ModContent.ProjectileType<AtlasNuc>(), 0, 0, Main.myPlayer);
                NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, p);
                if (sameItemCount > 6)
                {
                    for (int i = 0; i < sameItemCount - 6; i++)
                    {
                        Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromAI(), spawnPos + new Vector2(r.Next(-120, 120), r.Next(-100, 100)), new Vector2(0, 10), ModContent.ProjectileType<AtlasNuc>(), 0, 0, Main.myPlayer);

                    }
                }
            }
            if (open)
            {
                if (openFrame < 3)
                {
                    openCouter += 1;
                    if (openCouter == 5)
                    {
                        openCouter = 0;
                        openFrame++;
                        if(openFrame == 3)
                        {
                            Say("你好，如你所见，我是一台抽奖机", Color.Green);
                        }
                    }
                }
                else
                {
                    if (textureSpecial == 0 || textureSpecial == -1)
                    {
                        warnCounter = 0;
                        specialTime = 0;
                    }
                    else
                    {
                        
                        specialTime--;
                        if (specialTime <= 0)
                        {
                            textureSpecial = 0;
                        }
                    }
                    if (textureSpecial == 4 || textureSpecial == 5 || textureSpecial == 6 || textureSpecial == 7)
                    {
                        warnCounter++;
                        specialTime = 60;
                    }
                    else
                    {
                        warnCounter = 0;
                    }
                    if(SpawnTimer > 0)
                    {
                        SpawnTimer--;
                        if (SpawnTimer == 0)
                        {
                            if (textureSpecial == 7)
                            {
                                nucTime = 120;
                            }
                        }
                    }
                }
            }
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }
        
        public void RightClicked(Player player)
        {
            if (Main.dedServ)
            {
                if(NPC.netSpam >= 10)
                {
                    NPC.netSpam = 9;
                }
            }
            var r = Main.rand;
            if (!open)
            {
                open = true;
                SoundEngine.PlaySound(new("CalamityEntropy/Assets/Sounds/system_open"), NPC.Center);
            }
            else
            {
                if (openFrame >= 3)
                {
                    Mod nxb = null;
                    ModLoader.TryGetMod("NoxusBoss", out nxb);
                    bool hasBoss = false;
                    string bossName = "";
                    foreach(NPC n in Main.npc)
                    {
                        if (n.boss && n.active)
                        {
                            hasBoss = true;
                            bossName = n.FullName;
                            if (n.realLife >= 0)
                            {
                                bossName = Main.npc[n.realLife].FullName;
                            }
                        }
                    }
                    int itemType = -1;
                    itemType = player.HeldItem.type;
                    if (itemType == lastCItem)
                    {
                        sameItemCount++;
                    }
                    else
                    {
                        lastCItem = itemType;
                        sameItemCount = 0;
                    }
                    if (itemType == 0)
                    {
                        if (sameItemCount == 0)
                        {
                            Say("嘿，伙计，你的钱呢", Color.Green);
                            textureSpecial = 8;
                            specialTime = 120;
                        }
                        else if (sameItemCount == 1)
                        {
                            Say("额，我是说，要钱才能启动", Color.Orange);
                            textureSpecial = 3;
                            specialTime = 160;
                        }
                        else if (sameItemCount == 2)
                        {
                            Say("你个白痴！", Color.Orange);
                            textureSpecial = 1;
                            specialTime = 160;
                        }
                        else if (sameItemCount == 3)
                        {
                            Say("你在耍我吗？", Color.Orange);
                            textureSpecial = 2;
                            specialTime = 160;
                        }
                        else if (sameItemCount == 4)
                        {
                            Say("我知道了，你就是想看我笑话", Color.OrangeRed);
                            textureSpecial = 4;
                            specialTime = 160;
                        }
                        else if (sameItemCount == 5)
                        {
                            Say("你还有一次机会！", Color.Red);
                            textureSpecial = 5;
                            specialTime = 160;
                        }
                        else if (sameItemCount >= 6)
                        {
                            Say("你完了！", Color.Red);
                            textureSpecial = 7;
                            specialTime = 160;
                            SpawnTimer = 100;
                            useCd = 10;
                        }
                    }
                    else if (itemType == ItemID.PoopBlock || itemType == ItemID.PoopWall)
                    {
                        Say("你完了！", Color.Red);
                        textureSpecial = 7;
                        specialTime = 160;
                        SpawnTimer = 100;
                        useCd = 400;
                        sameItemCount = 60;
                    }
                    else if (itemType == ModContent.ItemType<AuricOre>())
                    {
                        Say("把他拿走，会唐！", Color.Red);
                        textureSpecial = 9;
                        specialTime = 90;
                    }
                    else if (itemType == ItemID.CopperCoin)
                    {
                        if (SpawnTimer > 0)
                        {
                            flag1 = true;
                            textureSpecial = 9;
                            specialTime = 100;
                            SpawnTimer = 0;
                            Say("嘿，为什么不早点拿出来......你在逗我吗？", Color.Yellow, 0.7f);

                        }
                        else
                        {
                            textureSpecial = -1;
                            Say("给，这是你的奖励;)", Color.Yellow, 0.86f);
                            useCd = 160;
                            SoundEngine.PlaySound(new("CalamityMod/Sounds/Item/SevensStrikerDoubles"), NPC.Center);
                            if (Main.myPlayer == player.whoAmI)
                            {
                                int pj = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center - new Vector2(0, 650), new Vector2(0, 16), ModContent.ProjectileType<AtlasItem>(), 0, 0, Main.myPlayer);
                                Main.projectile[pj].Entropy().AtlasItemStack = 0;
                                Main.projectile[pj].Entropy().AtlasItemType = 0;
                                Main.projectile[pj].netUpdate = true;
                            }
                        }
                    }
                    else if (itemType == ItemID.SilverCoin)
                    {
                        if (SpawnTimer > 0)
                        {
                            flag1 = true;
                            textureSpecial = 9;
                            specialTime = 100;
                            SpawnTimer = 0;
                            Say("嘿，为什么不早点拿出来......", Color.Yellow, 0.7f);

                        }
                        else
                        {
                            textureSpecial = -1;
                            player.HeldItem.stack--;
                            int rtype = 0;
                            int stack = 1;
                            RewardPool pool = new RewardPool();

                            pool.addPool(s1);


                            RewardPoolItem ri = pool.RandomItem();

                            rtype = ri.item;
                            stack = ri.stack;
                            useCd = 160;
                            SoundEngine.PlaySound(new("CalamityMod/Sounds/Item/SevensStrikerDoubles"), NPC.Center);
                            if (Main.myPlayer == player.whoAmI)
                            {
                                int pj = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center - new Vector2(0, 650), new Vector2(0, 16), ModContent.ProjectileType<AtlasItem>(), 0, 0, Main.myPlayer);
                                Main.projectile[pj].Entropy().AtlasItemStack = stack;
                                Main.projectile[pj].Entropy().AtlasItemType = rtype;
                                Main.projectile[pj].netUpdate = true;
                            }
                        }
                    }
                    else if (itemType == ItemID.GoldCoin)
                    {
                        if (SpawnTimer > 0)
                        {
                            flag1 = true;
                            textureSpecial = 9;
                            specialTime = 100;
                            SpawnTimer = 0;
                            Say("嘿，为什么不早点拿出来......", Color.Yellow, 0.7f);

                        }
                        else
                        {
                            textureSpecial = -1;
                            player.HeldItem.stack--;
                            int rtype = 0;
                            int stack = 1;
                            RewardPool pool = new RewardPool();

                            pool.addPool(s1);
                            pool.addPool(g1);
                            if (Main.hardMode)
                            {
                                pool.addPool(g2);
                            }

                            RewardPoolItem ri = pool.RandomItem();


                            rtype = ri.item;
                            stack = ri.stack;
                            useCd = 160;
                            SoundEngine.PlaySound(new("CalamityMod/Sounds/Item/SevensStrikerDoubles"), NPC.Center);
                            if (Main.myPlayer == player.whoAmI)
                            {
                                int pj = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center - new Vector2(0, 650), new Vector2(0, 16), ModContent.ProjectileType<AtlasItem>(), 0, 0, Main.myPlayer);
                                Main.projectile[pj].Entropy().AtlasItemStack = stack;
                                Main.projectile[pj].Entropy().AtlasItemType = rtype;
                                Main.projectile[pj].netUpdate = true;
                            }
                        }
                    }
                    else if (itemType == ItemID.PlatinumCoin)
                    {
                        if (SpawnTimer > 0)
                        {
                            flag1 = true;
                            textureSpecial = 9;
                            specialTime = 100;
                            SpawnTimer = 0;
                            if (Main.hardMode)
                            {
                                Say("嘿，为什么不早点拿出来.....", Color.Yellow, 0.7f);
                            }
                            else
                            {
                                Say("嘿，为什么不早点拿出来.....你可真是富有", Color.Yellow, 0.7f);
                            }

                        }
                        else
                        {
                            textureSpecial = -1;
                            player.HeldItem.stack--;
                            int rtype = 0;
                            int stack = 1;
                            RewardPool pool = new RewardPool();

                            pool.addPool(s1);
                            pool.addPool(g1);
                            pool.addPool(p1);
                            if (Main.hardMode)
                            {
                                pool.addPool(g2);
                                pool.addPool(p2);
                            }
                            if (NPC.downedMoonlord)
                            {
                                pool.addPool(p3);
                            }
                            if (DownedBossSystem.downedYharon)
                            {
                                pool.addPool(p4);
                            }

                            RewardPoolItem ri = pool.RandomItem();


                            rtype = ri.item;
                            stack = ri.stack;
                            useCd = 160;
                            SoundEngine.PlaySound(new("CalamityMod/Sounds/Item/SevensStrikerDoubles"), NPC.Center);
                            if (Main.myPlayer == player.whoAmI)
                            {
                                int pj = Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center - new Vector2(0, 650), new Vector2(0, 16), ModContent.ProjectileType<AtlasItem>(), 0, 0, Main.myPlayer);
                                Main.projectile[pj].Entropy().AtlasItemStack = stack;
                                Main.projectile[pj].Entropy().AtlasItemType = rtype;
                                Main.projectile[pj].netUpdate = true;
                            }

                        }
                    }
                    else if (itemType == ModContent.ItemType<LotteryBox>())
                    {
                        Say("你知道这是什么的", Color.Blue);
                        textureSpecial = 8;
                        specialTime = 90;
                    }
                    else if (itemType == 5335)
                    {
                        if (nxb != null && Main.zenithWorld)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, nxb.Find<ModNPC>("NamelessDeityBoss").Type);
                        }
                        else
                        {
                            Say("这个物品的设计是失败的...你不应该使用它", Color.Red);
                        }
                    }
                    else if (hasBoss) {
                        Say("你难道没看到那个" + bossName + "吗？可真是不知死活", Color.Green, 0.7f);
                    }
                    else if (itemType == ItemID.StarCannon)
                    {
                        if (sameItemCount == 0)
                        {
                            Say("这是一把可以发射星星的武器，很适合拿来对付一些难的过分的boss", Color.Yellow, 0.7f);
                        }
                        else if (sameItemCount == 1)
                        {
                            Say("对了，记得准备多一点的星星和粘鞍", Color.Yellow);
                        }
                        else if(sameItemCount == 2)
                        {
                            Say("不过真的有人会用这把武器来打boss吗", Color.Yellow);
                        }
                        else
                        {
                            Say("你玩够没有", Color.Yellow);
                        }
                    }
                    else if (itemType == ItemID.SuperStarCannon)
                    {
                        if (sameItemCount == 0)
                        {
                            Say("这是星星炮的加强版", Color.Yellow, 0.7f);
                        }
                        else if (sameItemCount == 1)
                        {
                            Say("或许有人能用来击败超越神明的造物？我不知道", Color.Yellow);
                        }
                    }
                    else if (nxb != null && itemType == nxb.Find<ModItem>("NoxusSprayer").Type) {
                        Say("把它拿开", Color.Red);
                        Say("把它拿开", Color.Red);
                        Say("把它拿开", Color.Red);
                        Say("把它拿开", Color.Red);
                        Say("把它拿开", Color.Red);
                        Say("把它拿开", Color.Red);
                    }
                    else if (nxb != null && itemType == nxb.Find<ModItem>("Genesis").Type)
                    {
                        Say("是的，快使用它", Color.Red);
                    }
                    
                    else if (itemType == ItemID.DirtBlock || itemType == ItemID.StoneBlock || itemType == ItemID.Wood || itemType == ItemID.Mushroom || itemType == ItemID.Gel || itemType == 52)
                    {
                        Say("我不收垃圾，谢谢", Color.Red);
                        textureSpecial = 10;
                        specialTime = 90;
                    }
                    else if (itemType == ModContent.ItemType<Fabsoup>())
                    {
                        if (player.ItemAnimationActive == false)
                        {
                            player.controlUseItem = true;
                            player.ItemCheck();
                            Say("它很好喝，是吧？", Color.Green);
                        }
                    }
                    else
                    {
                        Say("你塞的是什么东西？", Color.Green, 0.4f);

                    }
                   
                }
            }
        }


        public void Say(string text, Color color, float pitch = 1)
        {
            if (Main.dedServ)
            {
                return;
            }
            int t = CombatText.NewText(NPC.getRect(), color, text);
            Main.combatText[t].lifeTime = 16 * text.Length;
            SoundStyle s1 = new("CalamityMod/Sounds/Custom/WulfrumDroidChirp1");
            SoundStyle s2 = new("CalamityMod/Sounds/Custom/WulfrumDroidChirp2");
            SoundStyle s3 = new("CalamityMod/Sounds/Custom/WulfrumDroidChirp3");
            SoundStyle s4 = new("CalamityMod/Sounds/Custom/WulfrumDroidChirp4");
            s1.Pitch = pitch;
            s2.Pitch = pitch;
            s3.Pitch = pitch;
            s4.Pitch = pitch;
            SoundStyle toPlay = s1;
            int tpl = Main.rand.Next(0, 4);
            if (tpl == 1)
            {
                toPlay = s2;
            }
            else if (tpl == 2)
            {
                toPlay = s3;
            }
            else if (tpl == 3)
            {
                toPlay = s4;
            }

            SoundEngine.PlaySound(toPlay, NPC.Center);
        }
    }
}