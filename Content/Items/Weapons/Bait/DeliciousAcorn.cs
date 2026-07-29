using CalamityEntropy.Content.Buffs;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons.Bait
{
    public class DeliciousAcorn : ModItem, IBaitItem
    {
        public static int TagDamage = 3;
        public static float DamageMult = 2.25f;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(TagDamage);

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.knockBack = 0;
            Item.shootSpeed = 30;
            Item.useAnimation = Item.useTime = 18;
            Item.value = CalamityGlobalItem.RarityWhiteBuyPrice;
            Item.rare = ItemRarityID.White;
            Item.width = 38;
            Item.height = 38; 
            Item.autoReuse = false;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.noMelee = true;
            Item.DamageType = DamageClass.SummonMeleeSpeed;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<DeliciousAcornProjectile>();
            Item.autoReuse = true;
        }
        public override bool CanUseItem(Player player)
        {
            return player.Entropy().BaitUsable;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, (int)(damage * DamageMult), knockback, player.whoAmI, 0, 0, TagDamage);
            return false;
        }

        public override void AddRecipes()
        {

        }

        public override bool MeleePrefix()
        {
            return true;
        }
    }
    public class DeliciousAcornProjectile : BaitProj
    {
        public override string Texture => CEUtils.ItemTexPath<DeliciousAcorn>();
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Summon, true, -1);
            Projectile.width = Projectile.height = 24;
        }

        public override void AI()
        {
            if (Projectile.Entropy().FirstFrames)
            {
                Projectile.GetOwner().Entropy().BaitCharge--;
            }
            Projectile.rotation += Projectile.velocity.X * 0.02f;
            if (StickNPC < 0)
            {
                if (Counter > 12)
                {
                    Projectile.velocity *= 0.99f;
                    Projectile.velocity.Y += 0.8f;
                }
            }
            else
            {
                NPC npc = StickNPC.ToNPC();
                if (!npc.active)
                {
                    Projectile.Kill();
                    return;
                }
                Main.player[Projectile.owner].MinionAttackTargetNPC = npc.whoAmI;
                npc.GetGlobalNPC<WhipDebuffNPC>().BaitStick = 2;
                if (IsActive)
                {
                    Projectile.GetOwner().Calamity().mouseWorldListener = true;
                    npc.GetGlobalNPC<WhipDebuffNPC>().ClearBaitTags();
                    npc.GetGlobalNPC<WhipDebuffNPC>().Tags.Add(new WhipTag(this.GetType().Name, 5, this.TagDamage, 1, 0, this.GetType().Name) { IsABaitTag = true });
                }
                Projectile.Center = npc.Center + StickOffset;
                ActiveCounter++;
                if(ActiveCounter > 120)
                {
                    if(IsActive)
                    {
                        CEUtils.SyncProj(Projectile.whoAmI);
                        SetActive();
                    }
                }
            }
            activeEffectAlpha = float.Lerp(activeEffectAlpha, (StickNPC >= 0 && IsActive) ? 1 : 0, 0.04f);
            Counter++;
        }
        public override void ActiveEffect(float damageMul)
        {
            if(Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Main.rand.NextFloat(-400, 400), 700), Vector2.Zero, ModContent.ProjectileType<DesertNuisanceFriendly>(), (int)(Projectile.damage * damageMul), 6, Projectile.owner);
            }
        }
        public float activeEffectAlpha = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            if (activeEffectAlpha >= 0.01f)
            {
                Main.spriteBatch.UseAdditiveClamp();
                Texture2D pulse = CEUtils.getExtraTex("SoftRoundExplosion");
                for(float i = 0; i < 1f; i += 0.5f)
                {
                    float scale = CEUtils.Frac(i + Main.GlobalTimeWrappedHourly);
                    Main.spriteBatch.Draw(pulse, Projectile.Center - Main.screenPosition, null, Color.SandyBrown * Projectile.Opacity * (1 - scale) * activeEffectAlpha, i * MathHelper.TwoPi, pulse.Size() * 0.5f, scale * Projectile.scale * 0.12f, SpriteEffects.None, 0);
                }
                Main.spriteBatch.ExitShaderRegion();
            }
            Main.EntitySpriteDraw(Projectile.getDrawData(lightColor));
            return false;
        }
        public override bool? CanDamage()
        {
            return StickNPC == -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffect(Projectile.Center);
            Projectile.velocity *= 0;
            StickNPC = target.whoAmI;
            StickOffset = Projectile.Center - target.Center;
            Projectile.timeLeft = 360;
            CEUtils.SyncProj(Projectile.whoAmI);
        }
        public void OnHitEffect(Vector2 pos)
        {
            CEUtils.PlaySound("corruptwhip_hit2", Main.rand.NextFloat(0.8f, 1.2f), pos);
            for(int i = 0; i < 20; i++)
            {
                var d = Dust.NewDustDirect(pos - Projectile.Size * 0.5f, Projectile.width, Projectile.height, DustID.Sand);
                d.velocity = CEUtils.randomPointInCircle(18);
                d.scale = Main.rand.NextFloat(1.2f, 1.7f);
                d.noGravity = true;
            }
            for(int i = 0; i < 2; i++)
            {
                float scale = 0.05f + 0.02f * i;
                GeneralParticleHandler.SpawnParticle(new CustomPulse(pos, Vector2.Zero, Color.Lerp(Color.SandyBrown, new Color(230, 198, 104), (i / 2f)), "CalamityMod/Particles/FlameExplosion2", Vector2.One, CEUtils.randomRot(), scale * 0.2f, scale, 12 + i * 4));
            }
        }
        public override void OnKill(int timeLeft)
        {
            if(timeLeft > 0)
            {
                OnHitEffect(Projectile.Center);
            }
        }
    }
    public class SquirrerMinion : ModProjectile
    {

    }
}
