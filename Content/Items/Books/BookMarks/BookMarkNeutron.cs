using CalamityMod.Items;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkNeutron : BookMark
    {
        //这样可以修复联动配方没有加载的问题
        private readonly static string[] fullItems =
                ["0", "0", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityOverhaul/NeutronStarIngot", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityOverhaul/NeutronStarIngot", "CalamityOverhaul/NeutronStarIngot", "CalamityOverhaul/NeutronStarIngot", "CalamityEntropy/VoidBar", "0", "0",
                    "0", "0", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "CalamityEntropy/VoidBar", "0", "0",
                    "CalamityEntropy/BookMarkNeutron"
                ];
        //配方加载需要在Load阶段才能保证时机正确
        public override void Load()
        {
            if (ModLoader.TryGetMod("CalamityOverhaul", out var co))
            {
                co.Call(0, fullItems);
            }
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Red;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            //设置物品实例的终焉配方用于自动分配合成内容
            if (ModLoader.TryGetMod("CalamityOverhaul", out var co))
            {
                co.Call(1, Item, fullItems);
            }
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Neutron");
        public override EBookProjectileEffect getEffect()
        {
            return new NeutronBMEffect();
        }
        public override Color tooltipColor => Color.Lerp(Color.White, Color.Black, (0.5f + (float)Math.Cos(Main.GlobalTimeWrappedHourly * 12) * 0.5f));
    }

    public class NeutronBMEffect : EBookProjectileEffect
    {
        public override void OnProjectileSpawn(Projectile projectile, bool ownerClient)
        {
            (projectile.ModProjectile as EBookBaseProjectile).color = Color.DarkGray;
        }
        public override void OnHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            if (Main.rand.NextBool(projectile.HasEBookEffect<APlusBMEffect>() ? 6 : 10))
            {
                CEUtils.PlaySound("blackholeEnd", 1.25f, projectile.Center, 1, 1.2f);
                if (ModLoader.TryGetMod("CalamityOverhaul", out var co))
                {
                    if (co.TryFind<ModProjectile>("EXNeutronExplode", out var mp))
                    {
                        Projectile.NewProjectile(projectile.GetSource_FromThis(), target.Center, Vector2.Zero, mp.Type, projectile.damage * 16, projectile.knockBack, projectile.owner).ToProj().DamageType = projectile.DamageType;
                    }
                }
            }
        }
    }
}