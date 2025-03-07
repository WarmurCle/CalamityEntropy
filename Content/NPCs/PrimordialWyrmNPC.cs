using System.Collections.Generic;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.NPCs.PrimordialWyrm;
using Terraria.Utilities;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Sounds;
using Terraria.Localization;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityEntropy.Content.Items;

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
                if (!Main.bloodMoon && !Main.eclipse)
                {
                                         if (NPC.homeless)
                    {
                        chat.Add(Mod.GetLocalization("WyrmChatNoHome").Value);
                    }
                    else
                    {
                        chat.Add(Mod.GetLocalization("WyrmChat" + Main.rand.Next(1, 12).ToString()).Value);
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
        public override void AddShops() {
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
                .Add(ModLoader.GetMod("CalamityModMusic").Find<ModItem>("PrimordialWyrmMusicBox").Type);
			npcShop.Register();  		}

		public override void ModifyActiveShop(string shopName, Item[] items) {
            foreach (Item item in items)
            {
                                 if (item == null || item.type == ItemID.None)
                {
                    continue;
                }

                int value = item.shopCustomPrice ?? item.value;
                item.shopCustomPrice = value / 8;

            }
		}
        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 700;
            knockback = 3f;
            var sd = CommonCalamitySounds.WyrmScreamSound;
            sd.MaxInstances = 6;
            SoundEngine.PlaySound(in sd, NPC.Center);
        }
        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 15;
        }
        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<EidolicWailSoundwave>();
            attackDelay = 4;
        }
        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 14.5f;
                         gravityCorrection = 0f;
            randomOffset = 0f;
        }
        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 2f;
        }
    }
}