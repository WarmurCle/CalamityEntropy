using CalamityEntropy.Content.Projectiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class AzafureSurgeRod : RogueWeapon
    {
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.damage = 46;
            Item.ArmorPenetration = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 28;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 8f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityOrangeBuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<AzafureSurgeRodThrow>();
            Item.shootSpeed = 36f;
            Item.DamageType = CEUtils.RogueDC;
        }

        public static int ht = -1;
        public override void HoldItem(Player player)
        {
            if (ht == -1)
                ht = ModContent.ProjectileType<AzafureSurgeRodHeldEffect>();
            if (player.ownedProjectileCounts[ht] < 1 && Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ht, 0, 0, player.whoAmI);
            }
        }
        public override float StealthDamageMultiplier => 1f;
        public override float StealthVelocityMultiplier => 1.46f;
        public override float StealthKnockbackMultiplier => 3f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            var s = ((SoundStyle)Item.UseSound);
            s.MaxInstances = 6;
            s.Volume = 2;
            SoundEngine.PlaySound(Item.UseSound, position); SoundEngine.PlaySound(Item.UseSound, position);
            if (player.Calamity().StealthStrikeAvailable())
            {
                for (int i = 0; i < 8; i++)
                {
                    int p = Projectile.NewProjectile(source, position, velocity + CEUtils.randomPointInCircle(9), type, damage, knockback, player.whoAmI, 0f, 1f);
                    if (p.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[p].Calamity().stealthStrike = true;
                    }
                }
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            CreateRecipe().AddIngredient(ModContent.ItemType<HellIndustrialComponents>(), 6).AddIngredient(ItemID.CobaltBar, 8).AddIngredient<MysteriousCircuitry>(2).AddTile(TileID.Anvils).Register();
        }
    }

    public class AzafureSurgeRodThrow : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/AzafureSurgeRod";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, true, -1);
            Projectile.width = Projectile.height = 32;
            Projectile.timeLeft = 60 * 4;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
            {
                if(Projectile.Calamity().stealthStrike)
                {
                    Projectile.tileCollide = false;
                }
                int ht = ModContent.ProjectileType<AzafureSurgeRodHeldEffect>();
                foreach(var proj in Main.ActiveProjectiles)
                {
                    if(proj.owner == Projectile.owner && proj.type == ht)
                    {
                        proj.Kill();
                    }
                }
            }
            if (StickOnNPC && (stick == null || !stick.active))
                StickOnNPC = false;
            if(StickOnNPC)
            {
                Projectile.Center = stick.Center + StickPos.RotatedBy(stick.rotation);
                Projectile.rotation = stick.rotation + yr;
            }
            else if(StickOnGround)
            {

            }
            else
            {
                if (Projectile.Calamity().stealthStrike)
                {
                    Projectile.velocity *= 0.93f;
                    if(target == null || !target.active)
                        target = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 340);
                    if(target != null)
                    {
                        Projectile.velocity *= 0.9f;
                        Projectile.velocity += (target.Center - Projectile.Center).normalize() * 9f;
                    }
                }
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
            if (StrikeCounter > 0)
            {
                StrikeCounter--;
                if (StrikeCounter == 0)
                {
                    Func<int, bool> filter = (i) => true;
                    if (StickOnNPC)
                    {
                        filter = (i) => i != stick.whoAmI;
                    }
                    NPC npc = CEUtils.FindTarget_HomingProj(Projectile, Projectile.Center, 400, filter);
                    if(npc == null)
                    {
                        npc = stick;
                    }
                    if (npc != null)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), stick != null ? stick.Center : Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TeslaLightningRed>(), Projectile.damage, 0, Projectile.owner, npc.Center.X, npc.Center.Y, (Projectile.Calamity().stealthStrike ? 1 : 0)).ToProj().DamageType = Projectile.DamageType;
                        for (int i = 0; i < 8; i++)
                        {
                            GeneralParticleHandler.SpawnParticle(new AltSparkParticle(npc.Center, CEUtils.randomRot().ToRotationVector2() * Main.rand.NextFloat(4, 12), false, 60, Main.rand.NextFloat(0.9f, 1.4f), (Projectile.Calamity().stealthStrike ? Color.Red : Color.White)));
                        }
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        Vector2 velocity = ((MathHelper.TwoPi * i / 16f) - (MathHelper.Pi / 16f)).ToRotationVector2() * 12f;
                        Particle sparkle = new CritSpark(stick != null ? stick.Center : Projectile.Center, velocity, (Projectile.Calamity().stealthStrike ? Color.DarkRed : Color.White), (Projectile.Calamity().stealthStrike ? Color.DarkRed : Color.White), 0.8f, 30, 0.1f, 3f, Main.rand.NextFloat(0f, 0.01f));
                        GeneralParticleHandler.SpawnParticle(sparkle);
                    }
                    Projectile.timeLeft = 1;
                }
            }
        }
        NPC target = null;
        public int StrikeCounter = -1;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            if (Projectile.Calamity().stealthStrike)
                tex = this.getTextureAlt();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, new Vector2(36, 12), Projectile.scale, SpriteEffects.None);
            if(StrikeCounter >= 0)
            {
                float wa = 1 - StrikeCounter / 52f;
                tex = this.getTextureGlow();
                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, (Projectile.Calamity().stealthStrike ? Color.Red : Color.White) * wa, Projectile.rotation + MathHelper.PiOver4, new Vector2(36, 12), Projectile.scale, SpriteEffects.None);
                Main.spriteBatch.ExitShaderRegion();
            }
            return false;
        }
        public bool StickOnGround = false;
        public bool StickOnNPC = false;
        public Vector2 StickPos = Vector2.Zero;
        public NPC stick = null;
        public float yr = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(StickOnGround);
            writer.WriteVector2(StickPos);
            writer.Write(yr);
            writer.Write(StickOnNPC);
            if(StickOnNPC)
            {
                writer.Write(stick.whoAmI);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            StickOnGround = reader.ReadBoolean();
            StickPos = reader.ReadVector2();
            yr = reader.ReadSingle();
            StickOnNPC = reader.ReadBoolean();
            if(StickOnNPC)
            {
                stick = reader.ReadInt32().ToNPC();
            }
        }
        public override bool? CanHitNPC(NPC target)
        {
            if(StickOnGround || StickOnNPC)
            {
                return false;
            }
            return null;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if(!StickOnGround && !StickOnNPC)
            {
                StickOnNPC = true;
                StickPos = Projectile.Center - target.Center;
                StickPos = StickPos.RotatedBy(-target.rotation);
                stick = target;
                StrikeCounter = 52;
                yr = Projectile.rotation - target.rotation;
                Projectile.tileCollide = false;
            }
            Projectile.velocity *= 0;
            CEUtils.SyncProj(Projectile.whoAmI);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!StickOnGround)
            {
                Projectile.Center += oldVelocity * 1f;
                Projectile.velocity *= 0;
                StickOnNPC = false;
                StickOnGround = true;
                StickPos = Projectile.Center;
                Projectile.tileCollide = false;
                StrikeCounter = 52;

                if (Main.myPlayer == Projectile.owner)
                {
                    CEUtils.SyncProj(Projectile.whoAmI);
                }
            }
            return false;
        }
    }
    public class AzafureSurgeRodHeldEffect : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/AzafureSurgeRod";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
            Projectile.width = Projectile.height = 16;
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public float counter { get { return Projectile.localAI[0]; } set { Projectile.localAI[0] = value; } }
        public override void AI()
        {
            Player player = Projectile.GetOwner();
            if(player.HeldItem.ModItem is AzafureSurgeRod)
            {
                Projectile.timeLeft = 4;
            }
            else
            {
                Projectile.Kill();
                return;
            }
            float progress = counter / (player.HeldItem.useTime - 4);
            counter++;
            if (progress > 1)
                progress = 1;
            player.Calamity().mouseWorldListener = true;
            Projectile.Center = player.GetDrawCenter();
            if(progress > 0.4f)
            {
                float rj = 2.5f;
                float p = (progress - 0.5f) * 2;
                player.direction = (player.Calamity().mouseWorld - Projectile.Center).X > 0 ? 1 : -1;
                Projectile.rotation = (player.Calamity().mouseWorld - Projectile.Center).ToRotation() - (player.direction * (CEUtils.GetRepeatedCosFromZeroToOne(p, 2) * rj));
                
                player.SetHandRot(Projectile.rotation);
                player.direction = (player.Calamity().mouseWorld - Projectile.Center).X > 0 ? 1 : -1;
                player.heldProj = Projectile.whoAmI;
            }
            else
            {
                player.SetHandRot((player.Calamity().mouseWorld - Projectile.Center).ToRotation());
            }

        }
        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Projectile.GetOwner();
            float progress = counter / (player.HeldItem.useTime - 4);
            if (progress <= 0.5f)
                return false;
            Texture2D tex = Projectile.GetTexture();
            if (Projectile.GetOwner().Calamity().StealthStrikeAvailable())
                tex = this.getTextureAlt();
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathHelper.PiOver4, new Vector2(0, tex.Height), Projectile.scale, SpriteEffects.None);
            return false;
        }
    }
}
