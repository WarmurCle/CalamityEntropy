using CalamityEntropy.Content.Particles;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureHealingTowerRemote : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 56;
            Item.damage = 5;
            Item.mana = 10;
            Item.useTime = Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.noMelee = true;
            Item.knockBack = 4f;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<AzafureHealingTowerSentry>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            player.FindSentryRestingSpot(type, out int XPosition, out int YPosition, out int YOffset);
            YOffset += 22;
            position = new Vector2((float)XPosition, (float)(YPosition - YOffset));
            int p = Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, 20f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<HellIndustrialComponents>(6)
                .AddIngredient<MysteriousCircuitry>()
                .AddIngredient(ItemID.HealingPotion)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    public class AzafureHealingTowerSentry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 102;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Player healing = null;
            float dist = 900;
            int p = -1;
            foreach (Player plr in Main.ActivePlayers)
            {
                if (CEUtils.getDistance(plr.Center, Projectile.Center) < dist && plr.statLife < plr.statLifeMax2)
                {
                    dist = CEUtils.getDistance(plr.Center, Projectile.Center);
                    p = plr.whoAmI;
                }
            }
            if (p >= 0 && Main.player[p].active)
            {
                healing = Main.player[p];
            }
            bool flag = false;


            if (CheckCD > 0)
                CheckCD--;
            if (healing != null)
            {
                flag = true;
                AttackMode = false;
                if (CheckCD <= 0)
                {
                    CheckCD = (int)(25f * (100f / player.GetTotalDamage(DamageClass.Summon).ApplyTo(100f)));
                    healing.Heal(1);
                    for (int i = 0; i < 3; i++)
                        EParticle.spawnNew(new HealingParticle(), healing.position + CEUtils.randomPoint(new Rectangle(-6, -6, 12 + healing.width, 12 + healing.height)), new Vector2(0, -2), Color.White, 0.8f, 1, true, BlendState.AlphaBlend);
                }
                LaserTargetPos = healing.Center;
            }
            else
            {
                AttackMode = true;
                NPC attack = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 900);
                if (attack != null)
                {
                    flag = true;
                    LaserTargetPos = attack.Center;
                    if (CheckCD <= 0)
                    {
                        CheckCD = 10;
                        CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), player, attack.Center + attack.velocity, Projectile.damage, 42, Projectile.DamageType);
                    }
                }
            }
            if (flag)
            {
                if (LineWidth < 1)
                {
                    LineWidth += 0.1f;
                }
                if (LineWidth > 1)
                    LineWidth = 1;
            }
            else
            {
                if (LineWidth > 0)
                {
                    LineWidth *= 0.7f;
                }
            }
        }
        public bool AttackMode = false;
        public int CheckCD = 10;
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        public Vector2 LaserTargetPos = Vector2.Zero;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        public float LineWidth = 0;
        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, 0, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            if (LineWidth > 0.01f)
            {
                Vector2 laserStart = Projectile.Center + new Vector2(10, -28);
                Texture2D ball = CEUtils.getExtraTex("BasicCircle");
                float scale = 1 + 0.16f * (float)(Math.Sin(Main.GlobalTimeWrappedHourly * 12));
                scale *= LineWidth;
                //Main.spriteBatch.UseBlendState(BlendState.Additive);
                CEUtils.drawLine(laserStart, LaserTargetPos, (AttackMode ? Color.Red : Color.LightGreen), 9 * scale);

                Main.spriteBatch.Draw(ball, laserStart - Main.screenPosition, null, (AttackMode ? Color.Red : Color.LightGreen), 0, ball.Size() / 2f, scale * 0.2f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(ball, LaserTargetPos - Main.screenPosition, null, (AttackMode ? Color.Red : Color.LightGreen), 0, ball.Size() / 2f, scale * 0.2f, SpriteEffects.None, 0);
                CEUtils.drawLine(laserStart, LaserTargetPos, Color.White, 4 * scale);
                Main.spriteBatch.Draw(ball, laserStart - Main.screenPosition, null, Color.White, 0, ball.Size() / 2f, scale * 0.12f, SpriteEffects.None, 0);
                Main.spriteBatch.Draw(ball, LaserTargetPos - Main.screenPosition, null, Color.White, 0, ball.Size() / 2f, scale * 0.12f, SpriteEffects.None, 0);

                //Main.spriteBatch.ExitShaderRegion();
            }
            return false;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
    }
}
