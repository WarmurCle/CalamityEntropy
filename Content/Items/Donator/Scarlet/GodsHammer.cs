using CalamityEntropy.Content.Projectiles.Donator.ScarletHammers.GodsHammer.MainHammer;
using CalamityMod;
using CalamityMod.Rarities;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.Scarlet
{
    public class GodsHammer: BaseHammerItem
    {
        public override int ShootProjID => ModContent.ProjectileType<GodsHammerProj>();
        public override void ExSSD()
        {
        }
        public override void ExSD()
        {
            Item.width = Item.height = 86;
            Item.damage = 457;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 20f;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.value = Item.buyPrice(gold: 12);
        }
    }
    public class GodsHammerShimmerIL : ModSystem
    {
        public override void OnModLoad()
        {
            On_ShimmerTransforms.IsItemTransformLocked += ShimmerRequirementHandler;
        }
        public static bool ShimmerRequirementHandler(On_ShimmerTransforms.orig_IsItemTransformLocked orig, int type)
        {
            if (type == ModContent.ItemType<NightmareHammer>())
                return !DownedBossSystem.downedDoG;
            return orig(type);
        }
    }
}