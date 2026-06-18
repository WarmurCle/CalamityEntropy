using CalamityMod.DataStructures;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using InfernumMode.Content.Items.Weapons.Summoner;
using InfernumMode.Content.Buffs;
using Terraria.ModLoader;
using System;
using Terraria;
using CalamityEntropy.Content.Items.Weapons.Whips;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items.Weapons;

namespace CalamityEntropy.Core.Systems
{
    public class CalamityEntropySummonTagSystem : ModSystem
    {
        private struct SummonTagEntry
        {
            public Func<int> ItemType;

            public Func<int> BuffType;

            public Action<SummonTag> Setup;
        }

        public override void PostSetupContent()
        {
            List<SummonTagEntry> entries = new();

            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<JailerWhip>(),
                BuffType = () => ModContent.BuffType<JailerWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/JailerWhip", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<Crystedge>(),
                BuffType = () => ModContent.BuffType<CrystedgeWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Crystedge", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<Daylight>(),
                BuffType = () => ModContent.BuffType<DaylightWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/Daylight", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<ForeseeWhip>(),
                BuffType = () => ModContent.BuffType<ForeseeWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/ForeseeWhip", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<LashingBramblerod>(),
                BuffType = () => ModContent.BuffType<LashingBramblerodWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/LashingBramblerod", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<MindCorruptor>(),
                BuffType = () => ModContent.BuffType<MindCorruptorWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/MindCorruptor", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<SinewLash>(),
                BuffType = () => ModContent.BuffType<SinewLashWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/SinewLash", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<Vitalfeather>(),
                BuffType = () => ModContent.BuffType<DragonWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/Vitalfeather", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<WhipOfEvilKing>(),
                BuffType = () => ModContent.BuffType<WhipOfEvilKingWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/WhipOfEvilKing", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<WindOfUndertaker>(),
                BuffType = () => ModContent.BuffType<CruiserWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/WindOfUndertaker", (AssetRequestMode)1);
                }
            });
            entries.Add(new SummonTagEntry
            {
                ItemType = () => ModContent.ItemType<Ystralyn>(),
                BuffType = () => ModContent.BuffType<WyrmWhipDebuff>(),
                Setup = delegate (SummonTag summonTag)
                {
                    summonTag.AutoDrawTooltip = false;
                    summonTag.TagTexture = ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Weapons/Whips/Ystralyn", (AssetRequestMode)1);
                }
            });

            foreach (SummonTagEntry summonTagEntry in entries)
            {
                SummonTag tag =
                    new SummonTag(summonTagEntry.ItemType());

                summonTagEntry.Setup?.Invoke(tag);

                int debuff = summonTagEntry.BuffType();

                if (CalamityBuffSets.SummonTagDebuff[debuff] == null)
                {
                    CalamityBuffSets.SummonTagDebuff[debuff] = tag;
                }
            }
        }
    }
}
