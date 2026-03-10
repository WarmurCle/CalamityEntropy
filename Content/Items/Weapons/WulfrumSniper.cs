using CalamityEntropy.Content.Items.Books;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class WulfrumSniper : ModItem
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override bool RangedPrefix()
        {
            return true;
        }
        public override void SetDefaults()
        {
            Item.width = 234;
            Item.height = 70;
            Item.damage = 56;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 42;
            Item.reuseDelay = 10;
            Item.useAnimation = 38;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.ArmorPenetration = 15;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = CEUtils.GetSound("gunshot");
            Item.autoReuse = true;
            Item.shoot = ProjectileID.Bullet;
            Item.shootSpeed = 6f;
            Item.useAmmo = AmmoID.Bullet;
            Item.crit = 8;
            Item.scale = 0.5f;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-24, 0);
        }

        public int ShootCount = 0;
        #region Shooting
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (ShootCount < 5)
            {
                Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.Zero) * 50, velocity, type, damage, knockback, player.whoAmI);
                ShootCount++;
            }
            else
            {
                Item.noUseGraphic = true;
                ShootCount = 0;
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<WulfrumSniperSpecialAttack>(), damage, knockback, player.whoAmI, 0, Item.scale);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EnergyCore>(2)
                .AddIngredient<WulfrumMetalScrap>(8)
                .AddTile(TileID.Anvils)
                .Register();
        }
        #endregion

        #region Animations
        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 76f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-24, 0);



            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;
            if (animProgress < 0.5)
                rotation += (-0.15f) * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);

            if (animProgress > 0.5f)
            {
                float backArmRotation = rotation + 0.52f * player.direction;

                Player.CompositeArmStretchAmount stretch = ((float)Math.Sin(MathHelper.Pi * (animProgress - 0.5f) / 0.36f)).ToStretchAmount();
                player.SetCompositeArmBack(true, stretch, backArmRotation);
            }

        }
        #endregion
    }
    public class WulfrumSniperSpecialAttack : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/WulfrumSniper";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, false, -1);
            Projectile.timeLeft = 60;
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public int counter = 0;
        public float offset = 0;
        public bool thrown = false;
        public override void AI()
        {
            Projectile.scale = Projectile.ai[1];
            if(counter == 0 || counter == 16)
            {
                offset = -12;
                CEUtils.PlaySound("gunshot_small" + Main.rand.Next(1, 4).ToString(), 1, Projectile.Center);
            }
            if(counter == 20)
            {
                thrown = true;
                Projectile.GetOwner().HeldItem.noUseGraphic = false;
                Vector2 fpos = Projectile.Center + Projectile.velocity.normalize() * 36 * Projectile.scale;
                for (int i = 0; i < 12; i++)
                {
                    EParticle.NewParticle(new EMediumSmoke(), fpos, Projectile.velocity.normalize().RotatedByRandom(1) * Main.rand.NextFloat(2, 9), Color.Lerp(new Color(255, 255, 0), Color.White, (float)Main.rand.NextDouble()), Main.rand.NextFloat(0.7f, 1f), 1, true, BlendState.AlphaBlend, CEUtils.randomRot());
                }
                CEUtils.PlaySound("chainsaw_break", 1.4f, Projectile.Center);
                if(Main.myPlayer == Projectile.owner)
                {
                    int type = ModContent.ProjectileType<SniperWulfrumScrap>();
                    for(int i = 0; i < 4; i++)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), fpos, Projectile.velocity.normalize().RotatedByRandom(0.16f) * Main.rand.NextFloat(36, 42), type, (int)(Projectile.damage * 0.7f), Projectile.knockBack, Projectile.owner);
                    }
                }
                Projectile.GetOwner().velocity -= Projectile.velocity.normalize() * 8;
                Projectile.velocity = Projectile.velocity.RotatedBy(-2.7f * Projectile.GetOwner().direction).RotatedByRandom(0.32f).normalize() * 12;
            }
            if (!thrown)
            {
                Projectile.timeLeft = 200;
                Projectile.StickToPlayer();
                Projectile.GetOwner().SetHandRot(Projectile.rotation);
                Projectile.position += Projectile.rotation.ToRotationVector2() * (offset + 52 * Projectile.scale);
                Projectile.GetOwner().itemTime = Projectile.GetOwner().itemAnimation = 30;
                dir = Projectile.GetOwner().direction;
            }
            else
            {
                Projectile.velocity.Y += 0.36f;
                Projectile.rotation += Projectile.velocity.X * 0.003f;
            }
            offset *= 0.8f;
            counter++;
            if (Main.myPlayer == Projectile.owner && !Main.mouseLeft)
                flag = false;
            if(counter == 20)
            {
                if (Main.myPlayer != Projectile.owner || !Main.mouseLeft || flag)
                    counter--;
                else
                {
                    CEUtils.SyncProj(Projectile.whoAmI);
                }
            }
        }
        public bool flag = true;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(counter);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            counter = reader.ReadInt32();
        }
        public int dir = 1;
        public override bool ShouldUpdatePosition()
        {
            return thrown;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.spriteDirection = dir;
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
    }
    public class SniperWulfrumScrap : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Ranged, true, 1);
            Projectile.timeLeft = 480;
            Projectile.width = Projectile.height = 16;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center - Projectile.velocity, Projectile.Center, targetHitbox, Projectile.height);        }
        public override void AI()
        {
            if (Projectile.localAI[2]++ > 6)
            {
                Projectile.velocity.Y += 0.3f;
                Projectile.velocity *= 0.998f;
            }
            Projectile.rotation += Projectile.velocity.X * 0.01f;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            int type = ModContent.ItemType<WulfrumMetalScrap>();
            Main.instance.LoadItem(type);
            Texture2D tex = TextureAssets.Item[type].Value;
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor, tex));
            return false;
        }
    }
}
