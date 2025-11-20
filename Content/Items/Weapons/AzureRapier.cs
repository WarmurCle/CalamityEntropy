using CalamityEntropy.Content.Cooldowns;
using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzureRapier : ModItem, IDevItem, IGetFromStarterBag
    {
        public string DevName => "Polaris";
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 68;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.useTime = 6;
            Item.useAnimation = 6;
            Item.autoReuse = true;
            Item.scale = 2f;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.damage = 9;
            Item.knockBack = 4;
            Item.crit = 6;
            Item.shoot = ModContent.ProjectileType<AzureRapierHeld>();
            Item.shootSpeed = 16;
            Item.value = CalamityGlobalItem.RarityGreenBuyPrice;
            Item.rare = ModContent.RarityType<Soulight>();
            Item.Calamity().devItem = true;
            Item.ArmorPenetration = 16;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<AzureRapierBlock>(), damage, knockback, player.whoAmI);
            }
            else
            {
                return true;
            }
            return false;
        }

        public override bool MeleePrefix()
        {
            return true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient<SeaPrism>(6)
                .AddIngredient<CalamityMod.Items.Placeables.PrismShard>(10)
                .AddIngredient<PearlShard>(4)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public bool OwnAble(Player player, ref int count)
        {
            return StartBagGItem.NameContains(player, "polaris") || StartBagGItem.NameContains(player, "cle");
        }
    }
    public class AzureRapierHeld: ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/AzureRapier";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(ModContent.GetInstance<TrueMeleeDamageClass>(), false, -1);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public int counter;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CEUtils.PlaySound("spearImpact", Main.rand.NextFloat(1.2f, 1.6f), target.Center);
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                Projectile.velocity = Projectile.velocity.RotatedByRandom(0.24f);
                CEUtils.PlaySound("powerwhip", Main.rand.NextFloat(2.4f, 2.8f), Projectile.Center, 12, 0.6f * CEUtils.WeapSound);
            }
            var player = Projectile.GetOwner();
            player.heldProj = Projectile.whoAmI;
            int MaxTime = (int)(player.itemTimeMax * 1.4f);
            counter++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center = player.HandPosition.Value;
            Projectile.Center += Projectile.rotation.ToRotationVector2() * (CEUtils.Parabola((float)counter / MaxTime, 60 * player.HeldItem.scale) - 16);
            if(counter >= MaxTime)
            {
                Projectile.timeLeft = 0;
                Projectile.Kill();
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center, Projectile.Center + 210 * Projectile.rotation.ToRotationVector2(), targetHitbox, 24);
        }
        public override void CutTiles()
        {
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 216, 16, DelegateMethods.CutTiles);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var player = Projectile.GetOwner();
            int MaxTime = (int)(player.itemTimeMax * 1.4f);
            Texture2D tex = Projectile.GetTexture();
            Texture2D glow = CEUtils.getExtraTex("SpearArrowGlow2");
            float alpha = CEUtils.Parabola((float)counter / MaxTime, 1);
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, new Vector2(4, tex.Height - 4), Projectile.scale, SpriteEffects.None);
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 50, null, Color.Aqua, Projectile.rotation, new Vector2(0, glow.Height / 2), new Vector2(0.7f, 0.16f) * Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * 50, null, Color.White, Projectile.rotation, new Vector2(0, glow.Height / 2), new Vector2(0.7f, 0.1f) * Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public static void OnBlock(Player player, Vector2 targetPos, Vector2 targetVel)
        {
            int pjtype = ModContent.ProjectileType<AzureRapierBlockSlash>();
            if (player.ownedProjectileCounts[pjtype] < 1)
            {
                player.AddCooldown(BlockingCooldown.ID, 600);
                CEUtils.PlaySound("metalhit", 1.4f, player.Center);
                CEUtils.PlaySound("SwordHit0", 1.5f, player.Center);
                CEUtils.PlaySound("metalhit", 1.4f, player.Center);
                CEUtils.PlaySound("SwordHit0", 1.5f, player.Center);
                player.velocity = targetVel - player.velocity;
                Projectile.NewProjectile(player.GetSource_ItemUse(player.HeldItem), targetPos, Vector2.Zero, pjtype, player.GetWeaponDamage(player.HeldItem) * 2 + 1, 2, player.whoAmI);
            }
        }
    }
    public class AzureRapierBlockSlash : ModProjectile
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(target.type != NPCID.WallofFlesh)
                target.velocity *= 0.24f;
        }
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/AzureRapier";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(ModContent.GetInstance<TrueMeleeDamageClass>(), false, -1);
            Projectile.timeLeft = 60;
            Projectile.localNPCHitCooldown = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.width = 700;
            Projectile.height = 400;
            Projectile.ArmorPenetration += 60;
        }
        public int counter = 0;
        public Vector2 plrPos = Vector2.Zero;
        public override bool? CanHitNPC(NPC target)
        {
            return counter > 16;
        }
        public override void AI()
        {
            var player = Projectile.GetOwner();
            if (player.dead)
                Projectile.Kill();
            counter++;
            if(counter == 2)
            {
                player.Entropy().immune = 100;
            }
            if (counter == 16)
                plrPos = player.position;
            if(counter >= 16)
            {
                player.Entropy().DontDrawTime = 2;
                if(counter % 3 == 0)
                {
                    Vector2 spawnPos = Projectile.Center + new Vector2(Main.rand.Next(160, 240) * (Main.rand.NextBool() ? 1 : -1), Main.rand.Next(-140, 140));
                    EParticle.spawnNew(new DOracleSlash() { centerColor = Color.White, widthMult = 0.8f}, spawnPos, Vector2.Zero, Color.Aqua, 300, 1, true, BlendState.Additive, (Projectile.Center + new Vector2(0, spawnPos.Y - Projectile.Center.Y + Main.rand.NextFloat(-60, 60)) - spawnPos).ToRotation(), 16);
                    CEUtils.PlaySound("SwiftSlice", Main.rand.NextFloat(1.4f, 2f), Projectile.Center);
                }
                player.Entropy().noItemTime = 4;
                player.velocity *= 0;
                if(plrPos != Vector2.Zero)
                    player.position = plrPos;
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
    public class AzureRapierBlock : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/AzureRapier";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(ModContent.GetInstance<TrueMeleeDamageClass>(), false, -1);
            Projectile.timeLeft = 24;
        }
        public float TScale = 0;
        public float TSpeed = 0.5f;
        public float TAlpha = 1;
        public override void AI()
        {
            var player = Projectile.GetOwner();
            if (Projectile.localAI[0]++ == 0)
            {
                
                CEUtils.PlaySound("metalhit", 2.4f, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item27 with { Pitch = 1 }, Projectile.Center);
            }
            Projectile.Center = player.Center;
            player.SetHandRot(new Vector2(player.direction, 0.15f).ToRotation());
            Projectile.Center = player.GetDrawCenter() + new Vector2(player.direction * 9, -12);
            Projectile.rotation = new Vector2(-player.direction, -9).ToRotation();
            TScale += TSpeed;
            TAlpha *= 0.9f;
            TSpeed *= 0.9f;
            if (Projectile.localAI[0]==1)
            {
                for (int i = 0; i < 6; i++)
                {
                    GeneralParticleHandler.SpawnParticle(new AltSparkParticle(Projectile.Center, player.velocity + Projectile.rotation.ToRotationVector2().RotatedByRandom(0.1f) * Main.rand.NextFloat(16, 28), false, 32, Main.rand.NextFloat(1, 1.4f), Color.Aqua));
                    GeneralParticleHandler.SpawnParticle(new AltSparkParticle(Projectile.Center, player.velocity + Projectile.rotation.ToRotationVector2().RotatedByRandom(0.1f) * -Main.rand.NextFloat(16, 28), false, 32, Main.rand.NextFloat(1, 1.4f), Color.Aqua));
                }
            }
            player.Entropy().AzureRapierBlock = 2;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = player.itemAnimation = 3;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2f, Projectile.scale, SpriteEffects.None);
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Texture2D glow = CEUtils.RequestTex("CalamityEntropy/Content/Particles/ThinEndedLine");
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.Aqua * TAlpha * 2, Projectile.rotation + MathHelper.PiOver2, glow.Size() / 2f, new Vector2(1.2f, TScale), SpriteEffects.None, 0);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition, null, Color.White * TAlpha * 2, Projectile.rotation + MathHelper.PiOver2, glow.Size() / 2f, new Vector2(0.8f, TScale), SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
    }
}
