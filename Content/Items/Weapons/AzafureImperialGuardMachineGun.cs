using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Content.Items.Armor.Azafure;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Rarities;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureImperialGuardMachineGun : ModItem, IAzafureEnhancable
    {
        public override void SetDefaults()
        {
            Item.damage = 23;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 76;
            Item.height = 46;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ModContent.RarityType<AzafureOrange>();
            Item.UseSound = null;
            Item.noMelee = true;
            Item.shoot = ModContent.ProjectileType<AzafureImperialGuardMachineGunHeld>();
            Item.shootSpeed = 20;
            Item.channel = true;
            Item.noUseGraphic = true;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Minishark)
                .AddIngredient<HellIndustrialComponents>(6)
                .AddIngredient(ItemID.SoulofLight, 6)
                .AddTile(TileID.Anvils)
                .Register();
        }
        public int AmmoLeft = 0;
        public static int MaxAmmo = 100;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[AMMOMAX]", MaxAmmo);
        }
        public static bool TryReload(Player player)
        {
            if (player.HeldItem.ModItem is AzafureImperialGuardMachineGun aigm)
            {
                int hiType = ModContent.ItemType<HellIndustrialComponents>();
                for (int i = 0; i < player.inventory.Length; i++)
                {
                    Item item = player.inventory[i];
                    if (!item.IsAir && item.stack > 0 && item.type == hiType)
                    {
                        if(ItemLoader.CanConsumeAmmo(player.HeldItem, item, player))
                            item.Shrink(1);
                        aigm.AmmoLeft = AzafureImperialGuardMachineGun.MaxAmmo;
                        return true;
                    }
                }
            }
            return false;
        }
        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
            float p = ((float)AmmoLeft / MaxAmmo);
            CEUtils.DrawChargeBar(scale * 2f, position + new Vector2(8, 40) * scale, p, Color.Lerp(Color.Firebrick, Color.Orange, p));
            Vector2 drawPos = position + Vector2.UnitY * (frame.Height - 2 + 6f) * scale + Vector2.UnitX * (frame.Width - barBG.Width * 1.2f) * scale * 0.5f;
            CalamityUtils.DrawBorderStringEightWay(spriteBatch, FontAssets.MouseText.Value, AmmoLeft.ToString(), drawPos + new Vector2(-68, -30) * scale, Color.Orange, Color.Black, scale * 2);

        }

        public override void NetSend(BinaryWriter writer)
        {
            writer.Write(AmmoLeft);
        }
        public override void NetReceive(BinaryReader reader)
        {
            AmmoLeft = reader.ReadInt32();
        }
        public override void SaveData(TagCompound tag)
        {
            tag["Ammos"] = AmmoLeft;
        }
        public override void LoadData(TagCompound tag)
        {
            if(tag.TryGet<int>("Ammos", out int ammo))
                AmmoLeft = ammo;
        }
    }
    public class AzafureImperialGuardMachineGunHeld : ModProjectile
    {
        public float rotup = 0;
        public float rotv = 0f;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.HeldProjSetDefaults(DamageClass.Ranged);
        }
        public override bool? CanDamage()
        {
            return false;
        }
        public override void AI()
        {
            Projectile.GetOwner().Calamity().mouseWorldListener = true;
            Player player = Projectile.GetOwner();
            if (player.dead)
            {
                Projectile.Kill();
                return;
            }
            rotup += rotv;
            rotv *= 0.5f;
            rotup *= 0.7f;
            Projectile.StickToPlayer();
            int dir = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.rotation += dir * rotup;
            player.SetHandRotWithDir(Projectile.rotation, dir);
            player.itemAnimation = player.itemTime = 3;
            Projectile.timeLeft = 3;
            if(Projectile.Entropy().FirstFrames)
            {
                ShootDelay = 0;
            }
            if (ShootDelay > 0)
                ShootDelay -= player.GetWeaponAttackSpeed(player.HeldItem);

            if (!player.channel)
            {
                if (ShootDelay <= 0)
                    Projectile.Kill();
                return;
            }
            if (player.HeldItem.ModItem is AzafureImperialGuardMachineGun modItem)
            {
                if (ShootDelay <= 0)
                {
                    bool hasAmmo = modItem.AmmoLeft > 0;

                    if (!hasAmmo)
                    {
                        bool reload = AzafureImperialGuardMachineGun.TryReload(player);
                        ShootDelay = 60;
                        float pitch = (reload ? 0 : 1.4f);
                        SoundEngine.PlaySound(SoundID.Item149 with { Pitch = pitch }, base.Projectile.Center);
                        rotv = -0.5f;
                        if (!reload)
                            CombatText.NewText(player.Hitbox, Color.Orange, Mod.GetLocalization("NoAmmo").Value, false);
                    }
                    else
                    {
                        ShootDelay += 4;
                        CEUtils.PlaySound(Main.rand.NextBool() ? "GunShotSmall" : "GunShotSmallAlt", Main.rand.NextFloat(2f, 2.4f), Projectile.Center, 16, 0.3f);
                        modItem.AmmoLeft--;
                        Projectile.rotation += Main.rand.NextFloat(-0.15f, 0.15f);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center + new Vector2(16, -14 * dir).RotatedBy(Projectile.rotation), Projectile.velocity.RotatedByRandom(Main.rand.NextFloat(0, player.AzafureEnhance() ? 0.3f : 0.6f)) * Main.rand.NextFloat(1.2f, 1.42f) * (player.AzafureEnhance() ? 1.4f : 1), ModContent.ProjectileType<ImperialGuardShot>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner);
                            }
                        }
                        EParticle.spawnNew(new ShellParticle(), Projectile.Center + new Vector2(12, 4 * dir).RotatedBy(Projectile.rotation), Projectile.velocity.RotatedBy(dir * -2.2f) * 0.7f + CEUtils.randomPointInCircle(5), Color.White, 1, 1, false, BlendState.AlphaBlend, CEUtils.randomRot());
                    }
                }
            }
        }
        public float ShootDelay { get { return Projectile.ai[0]; } set { Projectile.ai[0] = value; } }
        public override void OnKill(int timeLeft)
        {
            Projectile.GetOwner().itemTime = Projectile.GetOwner().itemAnimation = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D t = Projectile.GetTexture();
            Main.EntitySpriteDraw(t, Projectile.Center - Main.screenPosition + new Vector2(18, -14 * (Projectile.velocity.X > 0 ? 1 : -1)).RotatedBy(Projectile.rotation), CEUtils.GetCutTexRect(t, 3, (int)Main.GameUpdateCount / 4 % 2, false), lightColor, Projectile.rotation, t.Size() * new Vector2(0.5f, 0.5f / 3f), Projectile.scale, (Projectile.velocity.X > 0) ? SpriteEffects.None : SpriteEffects.FlipVertically);

            return false;
        }
    }
    public class ImperialGuardShot : ModProjectile
    {
        public override string Texture => CEUtils.WhiteTexPath;
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 60;
            Projectile.MaxUpdates = 4;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public TrailParticle trail = null;

        public override void AI()
        {
            Projectile.velocity *= 0.95f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (trail == null)
            {
                trail = new TrailParticle();
                trail.maxLength = 18;
                trail.TimeLeftMax = 12;
                trail.ShouldDraw = false;
                EParticle.spawnNew(trail, Projectile.Center, Vector2.Zero, Color.Orange, 0.8f, 1f, true, BlendState.Additive);
            }
            if (Projectile.timeLeft < 20)
                Projectile.Opacity -= 1 / 20f;
            trail.Lifetime = (int)(12 * Projectile.Opacity);
            trail.AddPoint(Projectile.Center + Projectile.velocity);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            trail?.Draw();
            Texture2D tex = CEUtils.getExtraTex("Diamond");
            float scale = 0.06f * Projectile.scale;
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Orange * Projectile.Opacity, Projectile.rotation, tex.Size().Half(), new Vector2(2, 1) * scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, tex.Size().Half(), new Vector2(2, 1) * scale * 0.5f, SpriteEffects.None, 0);

            Main.spriteBatch.ExitShaderRegion();
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff<MechanicalTrauma>(3 * 60);
        }
        public override void OnKill(int timeLeft)
        {
            if (timeLeft > 6)
            {
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.Orange, 0.4f, 1, true, BlendState.Additive, 0, 8);
                EParticle.spawnNew(new ShineParticle(), Projectile.Center, Vector2.Zero, Color.White, 0.3f, 1, true, BlendState.Additive, 0, 8);
                CEUtils.PlaySound("beast_lavaball_rise1", Main.rand.NextFloat(2.4f, 2.8f), Projectile.Center, 36, 0.4f);
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 60;
        }
    }
}
