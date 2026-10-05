using CalamityEntropy.Common;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Core.CalamityRef;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public abstract class BookMark : ModItem
    {
        public override void SetDefaults() {
            Item.width = Item.height = 40;
            Item.rare = ItemRarityID.Orange;
        }
        public virtual Texture2D UITexture => null;
        public virtual EBookProjectileEffect getEffect() {
            return null;
        }
        public static Texture2D GetUITexture(string name) {
            return ModContent.Request<Texture2D>("CalamityEntropy/Assets/UI/BookMarks/" + name).Value;
        }
        public virtual void ModifyStat(EBookStatModifer modifer) {

        }
        public virtual int modifyProjectile(int pNow) {
            return -1;
        }
        public virtual int modifyBaseProjectile() {
            return -1;
        }
        public virtual void modifyShootCooldown(ref int shootCooldown) {

        }

        public virtual bool CanBeEquipWith(Item item) {
            return item.type != Item.type;
        }

        /// <summary>返回 null 走 BookMarkLoader 的默认实现</summary>
        public virtual BookmarkAttackResult PerformAttack(BookmarkAttackContext context) {
            return null;
        }

        public virtual Color tooltipColor => Color.Green;
        public override void ModifyTooltips(List<TooltipLine> tooltips) {
            foreach (TooltipLine tooltipLine in tooltips) {
                if (!tooltipLine.Name.StartsWith("Tooltip")) {
                    continue;
                }
                tooltipLine.OverrideColor = tooltipColor;
                //获取途径按灾厄在否分发:只有写了 [OBT] 的书签才查键,免得给其余书签注册空键
                if (tooltipLine.Text.Contains("[OBT]")) {
                    tooltipLine.Text = tooltipLine.Text.Replace("[OBT]", Mod.GetLocalization(Name + (CERef.Has ? "ObtCal" : "Obt")).Value);
                }
            }
            tooltips.Add(new TooltipLine(CalamityEntropy.Instance, "BookMarkTooltip", CalamityEntropy.Instance.GetLocalization("TooltipBookMark").Value) { OverrideColor = Color.Yellow });
            if (EBookUI.active) {
                tooltips.Add(new TooltipLine(CalamityEntropy.Instance, "BookMarkTooltip2", CalamityEntropy.Instance.GetLocalization("TooltipBookMark2").Value) { OverrideColor = Color.Lerp(new Color(124, 124, 0), Color.LightGoldenrodYellow, 0.5f + 0.25f * (float)(Math.Cos(Main.GlobalTimeWrappedHourly * 15))) });
            }
        }
    }
}