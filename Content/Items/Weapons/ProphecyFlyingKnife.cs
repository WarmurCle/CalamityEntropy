using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class ProphecyFlyingKnife : RogueWeapon
    {
        public int atkType = 1;
        public override void SetStaticDefaults()
        {
            Item.staff[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 20;
            Item.ArmorPenetration = 5;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 1.6f;
            Item.UseSound = SoundID.Item1;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<FutureKnife>();
            Item.shootSpeed = 26f;
            Item.DamageType = CEUtils.RogueDC;
        }

        public override float StealthDamageMultiplier => 4.4f;
        public override float StealthVelocityMultiplier => 3f;
        public override float StealthKnockbackMultiplier => 2f;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PFKnifeHeld>(), 0, 0, player.whoAmI, atkType);

            if (player.Calamity().StealthStrikeAvailable())
            {
                int p = Projectile.NewProjectile(source, position, velocity, type, damage * 2, knockback, player.whoAmI);
                if (p.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[p].Calamity().stealthStrike = true;
                    p.ToProj().netUpdate = true;
                }
                CEUtils.PlaySound("bne0", 1, position);
                return false;
            }
            else
            {
                float r = 0.6f;
                for (int i = 0; i < 9; i++)
                {
                    float c = (atkType < 0 ? (Math.Abs(r - ((i / 9f) * r - r * 0.5f))) : (Math.Abs(r - (((8 - i) / 9f) * r - r * 0.5f))));
                    c = 1f / c;
                    Projectile.NewProjectile(source, position, velocity.RotatedBy((i / 9f) * r - r * 0.5f) * c, type, (int)(damage * 0.8f), knockback, player.whoAmI);
                }
            }

            atkType *= -1;
            return false;
        }
    }
    public class PFKnifeHeld : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/ProphecyFlyingKnife";
        List<float> odr = new List<float>();
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;

        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 100000;
            Projectile.MaxUpdates = 14;
        }
        public float counter = 0;
        public float scale = 1;
        public float alpha = 0;
        public bool init = true;
        public bool shoot = true;

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override void AI()
        {
            Player owner = Projectile.GetOwner();
            float MaxUpdateTimes = owner.itemTimeMax * Projectile.MaxUpdates;
            float progress = (counter / MaxUpdateTimes);
            counter++;

            odr.Add(Projectile.rotation);
            Projectile.timeLeft = 3;
            float RotF = 3.2f;
            alpha = 1;
            scale = 1f;
            float cr = MathHelper.ToRadians(90);
            if (progress <= 0.5f)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + (RotF * -0.5f + CEUtils.Parabola(progress, RotF + cr)) * Projectile.ai[0];
            }
            else
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + (RotF * 0.5f + cr - CEUtils.GetRepeatedCosFromZeroToOne(2 * (progress - 0.5f), 1) * cr) * Projectile.ai[0];
            }
            Projectile.Center = Projectile.GetOwner().MountedCenter;


            if (odr.Count > 2600)
            {
                odr.RemoveAt(0);
            }
            if (Projectile.velocity.X > 0)
            {
                owner.direction = 1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            else
            {
                owner.direction = -1;
                owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)(Math.PI * 0.5f));
            }
            owner.heldProj = Projectile.whoAmI;
            if (counter > MaxUpdateTimes)
            {
                Projectile.Kill();
            }
            odr.Add(Projectile.rotation);
            if (odr.Count > 32)
            {
                odr.RemoveAt(0);
            }
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();

            int dir = (int)(Projectile.ai[0]);
            Vector2 origin = dir > 0 ? new Vector2(0, tex.Height) : new Vector2(tex.Width, tex.Height);
            SpriteEffects effect = dir > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rot = dir > 0 ? Projectile.rotation + MathHelper.PiOver4 : Projectile.rotation + MathHelper.Pi * 0.75f;

            float MaxUpdateTime = Projectile.GetOwner().itemTimeMax * Projectile.MaxUpdates;

            Main.EntitySpriteDraw(tex, Projectile.Center + Projectile.GetOwner().gfxOffY * Vector2.UnitY - Main.screenPosition, null, lightColor * alpha, rot, origin, Projectile.scale * scale, effect);

            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (160 * (Projectile.ai[0] == 2 ? 1.24f : 1)) * Projectile.scale * scale, targetHitbox, 64);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * (160 * (Projectile.ai[0] == 2 ? 1.24f : 1)) * Projectile.scale * scale, 54, DelegateMethods.CutTiles);
        }
    }
}
