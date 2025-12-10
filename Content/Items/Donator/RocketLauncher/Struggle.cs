using CalamityEntropy.Content.Items.Donator.RocketLauncher;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator.RocketLauncher
{
    public class Struggle : ModItem
    {
        public static int MaxStick => 3;
        public static int ExplodeRadius => 120;
        public override void SetDefaults()
        {
            Item.DefaultToRangedWeapon(ModContent.ProjectileType<CharredMissleProj>(), BaseMissleProj.AmmoType, singleShotTime: 40, shotVelocity: 20f, hasAutoReuse: true);
            Item.width = 90;
            Item.height = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 106;
            Item.knockBack = 4f;
            Item.UseSound = CEUtils.GetSound("cannon", 1.3f);
            Item.value = Item.buyPrice(gold: 3); 
            Item.rare = ItemRarityID.Pink;
            Item.Entropy().tooltipStyle = 8;
            Item.Entropy().strokeColor = new Color(180, 20, 180);
            Item.Entropy().NameColor = Color.DarkBlue;
            Item.Entropy().NameLightColor = Color.Purple * 0.4f;
        }

        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 76f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = -HoldoutOffset().Value;
            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.5)
                rotation += (-0.2f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-27f, -4f);
        }
        public override bool RangedPrefix()
        {
            return true;
        }
        #endregion

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight ,15)
                .AddIngredient<UnholyCore>(10)
                .AddIngredient<CoreofHavoc>(5)
                .AddIngredient(ItemID.HallowedBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public static void struggleProjKilled(Projectile proj)
        {
            int type = ModContent.ProjectileType<BrimHomingBullet>();
            for(float i = 0; i < 360; i += 60)
            {
                Projectile.NewProjectile(proj.GetSource_FromAI(), proj.Center, i.ToRadians().ToRotationVector2() * 16, type, proj.damage / 6, 4, proj.owner);
            }
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            position += (new Vector2(54, -16) * new Vector2(1, player.direction)).RotatedBy(velocity.ToRotation());
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, MaxStick, ExplodeRadius);
            p.ToProj().Entropy().applyBuffs.Add(ModContent.BuffType<BrimstoneFlames>());
            p.ToProj().Entropy().OnKillActions += struggleProjKilled;
            return false;
        }
    }
    public class BrimHomingBullet : ModProjectile
    {
        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.localAI[0] > 15 ? null : false;
        }
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, 1);
            Projectile.width = Projectile.height = 12;
            Main.projFrames[Type] = 4;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ > 16)
            {
                Projectile.HomingToNPCNearby();
            }
            else
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(0.1f);
            }
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            lightColor.R = 255;
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }
}
