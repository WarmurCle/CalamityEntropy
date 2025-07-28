using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Vanity
{
    public class VanityModPlayer : ModPlayer
    {
        //一个ModPlayer，用于将时装效果作用于玩家

        //当前装备的时装，留空为未装备
        public string vanityEquipped = "";

        public override void ResetEffects()
        {
            //每帧重置效果
            vanityEquipped = "";
        }

        public override void FrameEffects()
        {
            //玩家绘制前，如果装备了时装（vanityEquipped不为空）则让玩家贴图改为该时装
            if (vanityEquipped != "")
            {
                Player.legs = EquipLoader.GetEquipSlot(Mod, vanityEquipped, EquipType.Legs);
                Player.body = EquipLoader.GetEquipSlot(Mod, vanityEquipped, EquipType.Body);
                Player.head = EquipLoader.GetEquipSlot(Mod, vanityEquipped, EquipType.Head);
            }
        }
    }
}