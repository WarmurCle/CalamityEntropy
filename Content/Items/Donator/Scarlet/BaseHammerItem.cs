using System;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.Scarlet
{
    public abstract class BaseHammerItem : RogueWeapon, ILocalizedModType, IDevItem
    {
        public new string LocalizationCategory => "Weapons.Rogue";
        public virtual int ShootProjID { get; }
        //锤类武器初始提供的潜伏值
        public const float BaseMaxStealth = 0.1f;
        public string DevName => "TrueScarlet";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ExSSD();
        }
        public virtual void ExSSD() {}
        public override void SetDefaults()
        {
            Item.DamageType = CEUtils.RogueDC;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.shoot = ShootProjID;
            Item.knockBack = 18f;
            ExSD();
        }
        //全体锤子采用0.7f伤害倍率
        public override float StealthDamageMultiplier => 0.5f;
        //复写modifyshoot，我们只让其生效潜伏伤害倍率
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
                damage = (int)(damage * StealthDamageMultiplier);
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            ExModifyTooltips(tooltips);
            string path = $"{CEUtils.LocalPrefix}.Weapons.Rogue.HammerGeneral";
            tooltips.QuickAddTooltip(path, Color.Yellow);
        }
        public virtual void ExModifyTooltips(List<TooltipLine> tooltips) {}
        public virtual void ExSD() { }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile st = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
            st.Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
