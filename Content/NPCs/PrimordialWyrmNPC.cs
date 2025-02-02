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
            //NPC总共帧图数，一般为16+下面两种帧的帧数
            NPCID.Sets.ExtraFramesCount[Type] = NPCID.Sets.ExtraFramesCount[NPCID.Clothier];
            //额外活动帧，一般为5
            //关于这个，一般来说是除了攻击帧以外的那几帧（包括坐下、谈话等动作），但实际上填写包含攻击帧在内的帧数也不影响（比如你写9），如果有知道的可以解释一下。
            NPCID.Sets.AttackFrameCount[Type] = NPCID.Sets.AttackFrameCount[NPCID.Clothier];
            //攻击帧，这个帧数取决于你的NPC攻击类型，射手填5，战士和投掷填4，法师填2，当然，也可以多填，就是不知效果如何（这里直接引用商人的）
            NPCID.Sets.DangerDetectRange[Type] = 1000;
            //巡敌范围，以像素为单位，这个似乎是半径
            NPCID.Sets.AttackType[Type] = NPCID.Sets.AttackType[NPCID.Clothier];
            //攻击类型，默认为0，想要模仿其他NPC就填他们的AttackType
            NPCID.Sets.AttackTime[Type] = 50;
            //单次攻击持续时间，越短，则该NPC攻击越快（可以用来模拟长时间施法的NPC）
            NPCID.Sets.AttackAverageChance[Type] = 1;
            //NPC遇敌的攻击优先度，该数值越大则NPC遇到敌怪时越会优先选择逃跑，反之则该NPC越好斗。
            //最小一般为1，你可以试试0或负数LOL~
            NPCID.Sets.MagicAuraColor[base.NPC.type] = Color.Purple;
        }
        public override void SetDefaults()
        {
            NPC.townNPC = true;
            //必带项，没有为什么
            //加上这个后NPC就不会在退出地图后消失了，你可以灵活运用一下
            NPC.friendly = true;
            //如果你想写敌对NPC也行
            NPC.width = 22;
            //碰撞箱宽
            NPC.height = 32;
            //碰撞箱高            
            NPC.aiStyle = 7;
            //必带项，如果你能自己写出城镇NPC的AI可以不带
            //PS:这个决定了NPC是否能入住房屋
            NPC.damage = 10;
            //碰撞伤害，由于城镇NPC没有碰撞伤害所以可以忽略
            NPC.defense = 105;
            //防御力
            NPC.lifeMax = 7200000;
            //生命值
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = PrimordialWyrmHead.DeathSound;
            NPC.knockBackResist = 0f;
            //抗击退性，数字越小抗性越高，0就是完全抗击退
            AnimationType = NPCID.Clothier;
            //如果你的NPC属于除投掷类NPC以外的其他攻击类型，请带上，值可以填对应NPC的ID
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
                    //无家可归时
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
			npcShop.Register(); // Name of this shop tab
		}

		public override void ModifyActiveShop(string shopName, Item[] items) {
            foreach (Item item in items)
            {
                // Skip 'air' items and null items.
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
            //弹幕射速
            gravityCorrection = 0f;
            randomOffset = 0f;
        }
        public override void TownNPCAttackMagic(ref float auraLightMultiplier)
        {
            auraLightMultiplier = 2f;
        }
    }
}