﻿using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.PrimordialWyrm;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Sounds;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityEntropy.Content.NPCs
{
    [AutoloadHead]
    public class PrimordialWyrmNPC : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = Main.npcFrameCount[NPCID.Clothier];
            NPCID.Sets.ExtraFramesCount[Type] = NPCID.Sets.ExtraFramesCount[NPCID.Clothier];
            NPCID.Sets.AttackFrameCount[Type] = NPCID.Sets.AttackFrameCount[NPCID.Clothier];
            NPCID.Sets.DangerDetectRange[Type] = 1000;
            NPCID.Sets.AttackType[Type] = NPCID.Sets.AttackType[NPCID.Clothier];
            NPCID.Sets.AttackTime[Type] = 50;
            NPCID.Sets.AttackAverageChance[Type] = 1;
            NPCID.Sets.MagicAuraColor[base.NPC.type] = Color.Purple;
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 22;
            NPC.height = 32;
            NPC.aiStyle = 7;
            NPC.damage = 10;
            NPC.defense = 105;
            NPC.lifeMax = 7200000;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = PrimordialWyrmHead.DeathSound;
            NPC.knockBackResist = 0f;
            AnimationType = NPCID.Clothier;
        }
        public override bool PreAI()
        {
            dcd--;
            return base.PreAI();
        }
        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (DownedBossSystem.downedPrimordialWyrm)
            {
                return true;
            }
            return false;
        }

        public override string GetChat()
        {
            WeightedRandom<string> chat = new WeightedRandom<string>();
            {
                if(MaliciousCode.CALAMITY__OVERHAUL && Main.LocalPlayer.HeldItem.type == ModContent.ItemType<HalibutCannon>())
                {
                    chat.Add(Mod.GetLocalization("WyrmChatCOHalibut" + Main.rand.Next(1, 3).ToString()).Value);
                    return chat;
                }
                if (Main.rand.NextBool(6))
                {
                    string dns = "";
                    var lc = new List<string>();
                    foreach (string s in Donators.Donors)
                    {
                        lc.Add(s);
                    }
                    for (int i = 0; i < 10; i++)
                    {
                        int d = Main.rand.Next(lc.Count);
                        dns += lc[d];
                        if (i < 9)
                        {
                            dns += ", ";
                        }
                        lc.RemoveAt(d);
                    }
                    chat.Add(Mod.GetLocalization("WyrmChatDonors").Value.Replace("[0]", dns));
                    return chat;
                }
                if (!Main.bloodMoon && !Main.eclipse)
                {
                    if (NPC.homeless)
                    {
                        chat.Add(Mod.GetLocalization("WyrmChatNoHome").Value);
                    }
                    else
                    {
                        chat.Add(Mod.GetLocalization("WyrmChat" + Main.rand.Next(1, 12).ToString()).Value);
                        if(Main.raining)
                            chat.Add(Mod.GetLocalization("WyrmChatRain" + Main.rand.Next(1, 3).ToString()).Value);
                    }
                }
                else
                {
                    if (Main.eclipse)
                    {
                        chat.Add(Mod.GetLocalization("WyrmChatEclipse1").Value);
                        chat.Add(Mod.GetLocalization("WyrmChatEclipse2").Value);
                    }
                    if (Main.bloodMoon)
                    {
                        chat.Add(Mod.GetLocalization("WyrmChatBloodMoon1").Value);
                        chat.Add(Mod.GetLocalization("WyrmChatBloodMoon2").Value);
                    }
                }
                return chat;
            }
        }
        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }
        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {

            if (firstButton)
            {
                shopName = ShopName;
            }
        }
        public static string ShopName = "Shop";
        public override void AddShops()
        {
            var npcShop = new NPCShop(Type, ShopName)
                .Add<EidolicWail>()
                .Add<VoidEdge>()
                .Add<EidolonStaff>()
                .Add<HalibutCannon>()
                .Add<Lumenyl>()
                .Add<DepthCells>()
                .Add<PlantyMush>()
                .Add<AbyssalTreasure>()
                .Add<AbyssTorch>()
                .Add<AbyssShellFossil>()
                .Add<WyrmTooth>()
                .Add<Rock>()
                .Add(ModLoader.GetMod("CalamityModMusic").Find<ModItem>("PrimordialWyrmMusicBox").Type);
            npcShop.Register();
        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            foreach (Item item in items)
            {
                if (item == null || item.type == ItemID.None)
                {
                    continue;
                }

                int value = item.shopCustomPrice ?? item.value;
                item.shopCustomPrice = value / 8;
                if (item.type == ModContent.ItemType<Rock>())
                {
                    item.shopCustomPrice = 100000000;
                }
            }
        }
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = Main.zenithWorld ? 2000 : 700;

            knockback = 3f;

        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 15;
        }

        public int st = 0;
        public int dcd = 0;
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<EidolicWailSoundwave>();
            if (Main.zenithWorld)
            {
                projType = st % 2 == 1 ? ModContent.ProjectileType<HadopelagicLaser>() : ModContent.ProjectileType<HadopelagicWail>();
                if (dcd <= 0)
                {
                    st++;
                }
            }
            attackDelay = 4;
            if (dcd <= 0)
            {
                var sd = CommonCalamitySounds.WyrmScreamSound;
                if (Main.zenithWorld)
                {
                    sd = CEUtils.GetSound("he" + (Main.rand.NextBool() ? 1 : 3).ToString());
                }
                sd.MaxInstances = 6;
                SoundEngine.PlaySound(in sd, NPC.Center);
                dcd = 59;
            }
        }
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 14.5f;
            if (Main.zenithWorld)
            {
                multiplier = 32;
            }
            gravityCorrection = 0f;
            randomOffset = 0f;
        }
        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 2f;
        }
    }
}