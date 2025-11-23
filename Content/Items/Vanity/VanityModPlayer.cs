using CalamityEntropy.Content.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Vanity
{
    public class VanityModPlayer : ModPlayer
    {
        //一个ModPlayer，用于将时装效果作用于玩家

        //当前装备的时装，留空为未装备

        public string vanityEquippedLast = "";
        public string vanityEquipped = "";
        public int TheocrazyDye = -1;
        public int TheocrazyDyeItemID = -1;
        public bool TheocracyMark = false;
        public override void PostUpdate()
        {
            if (TheocracyMark)
            {
                if (Player.ownedProjectileCounts[ModContent.ProjectileType<Theostring>()] < 1)
                {
                    if (Main.myPlayer == Player.whoAmI)
                    {
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Theostring>(), 0, 0, Player.whoAmI, -1);
                        Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero, ModContent.ProjectileType<Theostring>(), 0, 0, Player.whoAmI, 1);
                    }
                }
            }
        }
        public override void ResetEffects()
        {
            //每帧重置效果
            vanityEquippedLast = vanityEquipped;
            TheocracyMark = false;
            vanityEquipped = "";
        }

        public override void FrameEffects()
        {
            //玩家绘制前，如果装备了时装（vanityEquipped不为空）则让玩家贴图改为该时装
            if (TheocracyMark)
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, "TheocracyMark", EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, "TheocracyMark", EquipType.Body);
            }
            if (vanityEquipped != "")
            {

                Player.legs = EquipLoader.GetEquipSlot(Mod, vanityEquipped, EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, vanityEquipped, EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, vanityEquipped, EquipType.Head);

            }
        }
        public override void DrawEffects(PlayerDrawSet drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
        {
            if (TheocracyMark)
                drawInfo.colorHair = Color.Transparent;
        }
    }
}