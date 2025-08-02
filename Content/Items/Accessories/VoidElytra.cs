using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Particles;
using CalamityMod.Items;
using CalamityMod.Rarities;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Accessories
{
    [AutoloadEquip(EquipType.Wings)]
    public class VoidElytra : ModItem
    {
        public static float HorSpeed = 10;
        public static float AccMul = 3;
        public static int wTime = 290;
        public override void SetStaticDefaults()
        {
            ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(wTime, HorSpeed, AccMul, false, 20, 3f);
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 20;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;

        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", HorSpeed);
            tooltips.Replace("[B]", AccMul);
            tooltips.Replace("[C]", wTime);
        }
        public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
            ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend)
        {
            ascentWhenRising = 0.15f; maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
            if (!(player.GetModPlayer<EPlayerDash>().DashTimer > 0))
            {
                ascentWhenFalling = 0.97f;
            }
            else
            {
                ascentWhenFalling = 1f;
            }
        }
        public override bool WingUpdate(Player player, bool inUse)
        {
            if (inUse && player.GetModPlayer<EPlayerDash>().DashTimer <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Particle p = new Particle();
                    p.position = (player.Center - new Vector2(14f, 0f) * (float)player.direction) - player.velocity * ((float)i * 0.1f);
                    p.velocity = new Vector2(-8f, 10f) * new Vector2((float)player.direction, 1f);
                    p.alpha = 0.2f;
                    p.position += p.velocity * ((float)i * 0.1f);
                    VoidParticles.particles.Add(p);
                }

            }

            return base.WingUpdate(player, inUse);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EPlayerDash>().DashAccessoryEquipped = true;
            player.GetModPlayer<EPlayerDash>().velt = true;
            if (player.wingTime < 2 && !(player.mount.Active))
            {
                player.wingTime = 2;
                player.AddBuff(ModContent.BuffType<VoidTouch>(), 5);
            }
        }

    }
}
