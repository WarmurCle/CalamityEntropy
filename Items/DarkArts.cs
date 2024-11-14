using CalamityEntropy.Buffs;
using CalamityEntropy.Cooldowns;
using CalamityEntropy.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items
{
	public class DarkArts : ModItem
	{
        public override void SetStaticDefaults() {
		}

		public override void SetDefaults() {
			Item.width = 26;
			Item.height = 26;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = -1;
            Item.damage = 460;
            Item.DamageType = Util.CUtil.rougeDC;
            Item.noMelee = true;
            
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
			
		}
        public override bool CanUseItem(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<StealthState>());
        }

        public override bool? UseItem(Player player)
        {
            SoundEffect se = ModContent.Request<SoundEffect>("CalamityEntropy/Sounds/da1").Value;
            if (se != null) { se.Play(Main.soundVolume, 0, 0); }
            player.AddBuff(ModContent.BuffType<StealthState>(), 120);
            return true;
        }

    }
}
