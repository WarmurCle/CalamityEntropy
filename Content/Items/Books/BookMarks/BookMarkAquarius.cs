
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Content.UI.EntropyBookUI;
using CalamityEntropy.Util;
using CalamityMod.Items;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Turret;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books.BookMarks
{
    public class BookMarkAquarius : BookMark
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.rare = ItemRarityID.Orange;
            Item.Entropy().stroke = true;
            Item.Entropy().NameColor = Color.LightBlue;
            Item.Entropy().strokeColor = Color.DarkBlue;
            Item.Entropy().tooltipStyle = 4;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
        }
        public override Texture2D UITexture => BookMark.GetUITexture("Aquarius");
        public override Color tooltipColor => Color.LightBlue;
        public override EBookProjectileEffect getEffect()
        {
            return new AquariusBMEffect();
        }
    }

    public class AquariusBMEffect : EBookProjectileEffect
    {
        public override void onHitNPC(Projectile projectile, NPC target, int damageDone)
        {
            int shootCount = 5;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                float rot = 0;
                if (npc != target && npc.Distance(target.Center) < 800 && !npc.friendly && !npc.dontTakeDamage)
                {
                    rot = (target.Center - npc.Center).ToRotation();
                    Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, rot.ToRotationVector2() * 8, ModContent.ProjectileType<WaterShot>(), projectile.damage / 15, projectile.knockBack + 0.5f, projectile.owner);
                    shootCount--;
                }
            }
            for (; shootCount > 0; shootCount--)
            {
                float rot = Util.Util.randomRot();
                Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center, rot.ToRotationVector2() * 8, ModContent.ProjectileType<WaterShot>(), projectile.damage / 15, projectile.knockBack + 0.5f, projectile.owner);
            }
        }
    }
}