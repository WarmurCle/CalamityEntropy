using CalamityEntropy.Buffs;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Items.Accessories
{
	[AutoloadEquip(EquipType.Wings)]
	public class VoidElytra : ModItem
	{

		public override void SetStaticDefaults() {
			// Fly time: 
			// Fly speed:
			// Acceleration multiplier
			ArmorIDs.Wing.Sets.Stats[Item.wingSlot] = new WingStats(290, 10f, 3f, false, 20, 3f);
		}

		public override void SetDefaults() {
			Item.width = 22;
			Item.height = 20;
            Item.value = CalamityGlobalItem.RarityTurquoiseBuyPrice;
            Item.rare = ModContent.RarityType<Turquoise>();
            Item.accessory = true;
			
		}
		public override void VerticalWingSpeeds(Player player, ref float ascentWhenFalling, ref float ascentWhenRising,
			ref float maxCanAscendMultiplier, ref float maxAscentMultiplier, ref float constantAscend) {
            ascentWhenRising = 0.15f; // Rising speed
            maxCanAscendMultiplier = 1f;
            maxAscentMultiplier = 3f;
            constantAscend = 0.135f;
            if (!(player.GetModPlayer<EPlayerDash>().DashTimer > 0))
            {
                ascentWhenFalling = 0.97f; // Falling glide speed
                
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
            if (player.wingTime < 2 && !(player.mount.Active) && inUse)
            {
                player.wingTime = 2;
                player.AddBuff(ModContent.BuffType<VoidTouch>(), 5);
            }
            return base.WingUpdate(player, inUse);
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<EPlayerDash>().DashAccessoryEquipped = true;
            player.GetModPlayer<EPlayerDash>().velt = true;
            
        }

    }
}
