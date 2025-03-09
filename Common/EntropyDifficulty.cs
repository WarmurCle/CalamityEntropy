using CalamityMod.Systems;
using CalamityMod.World;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria;
using static CalamityMod.Systems.DifficultyModeSystem;
using Microsoft.Xna.Framework;

namespace CalamityEntropy.Common
{

    public class EntropyDifficulty : DifficultyMode
    {
        public override bool Enabled
        {
            get => CalamityEntropy.EntropyMode;
            set
            {
                CalamityEntropy.EntropyMode = value;
                if (value)
                {
                    CalamityWorld.revenge = true;
                    CalamityWorld.death = true;
                    if(Main.netMode != NetmodeID.SinglePlayer)
                    {
                        ModPacket packet = CalamityEntropy.Instance.GetPacket();
                        packet.Write((byte)CalamityEntropy.NetPackages.SyncEntropyMode);
                        packet.Write(value);
                        packet.Send();
                    }
                }
            }
        }

        private Asset<Texture2D> _texture;
        public override Asset<Texture2D> Texture
        {
            get
            {
                _texture ??= ModContent.Request<Texture2D>("CalamityEntropy/Assets/UI/EntropyMode");

                return _texture;
            }
        }

        public override LocalizedText ExpandedDescription => CalamityEntropy.Instance.GetLocalization("EntropyModeDesc");

        public EntropyDifficulty()
        {
            DifficultyScale = 0.8f;
            Name = CalamityEntropy.Instance.GetLocalization("EntropyModeName");
            ShortDescription = CalamityEntropy.Instance.GetLocalization("EntropyModeDescShort");

            ActivationTextKey = "Mods.CalamityEntropy.EntropyModeActive";
            DeactivationTextKey = "Mods.CalamityEntropy.EntropyModeDeactive";

            ActivationSound = Util.Util.GetSound("soul");
            ChatTextColor = new Color(170, 18, 225);
        }

        public override int FavoredDifficultyAtTier(int tier)
        {
            DifficultyMode[] tierList = DifficultyTiers[tier];

            for (int i = 0; i < tierList.Length; i++)
            {
                if (tierList[i].Name.Value == "Death")
                    return i;
            }

            return 0;
        }
    }
}
