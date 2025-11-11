using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Tiles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using Humanizer;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics.Metrics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Weapons
{
    public class CrossBorderPursuit : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 76;
            Item.useTime = 16;
            Item.useAnimation = 16;
            Item.useStyle = -1;
            Item.damage = 1000;
            Item.DamageType = CEUtils.RogueDC;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.value = CalamityGlobalItem.RarityRedBuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.shoot = ModContent.ProjectileType<CrossBorderPursuitProj>();
            Item.shootSpeed = 8;
        }
        public NPC castTarget = null;
        public override bool CanUseItem(Player player)
        {
            NPC target = CEUtils.FindTarget_HomingProj(player, Main.MouseWorld, 360, null);
            castTarget = target;
            return player.Calamity().StealthStrikeAvailable() && target != null;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (castTarget != null)
            {
                Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, castTarget.whoAmI);
                CEUtils.CostStealthForPlr(player);
            }
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<VoidBar>(8)
                .AddIngredient<DedicatedOracle>()
                .AddTile<VoidWellTile>()
                .Register();
        }
    }
    public class CrossBorderPursuitProj : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Weapons/TheoEye";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(CEUtils.RogueDC, false, -1);
        }
        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }
        public NPC target => ((int)Projectile.ai[0]).ToNPC();
        public Vector2 origPos = Vector2.Zero;
        public Vector2 lastPlrPos = Vector2.Zero;
        public Vector2 targetPos = Vector2.Zero;
        public override void AI()
        {
            if (target == null && !target.active)
            {
                Projectile.Kill();
                return;
            }
            Player player = Projectile.GetOwner();
            int counter = (int)Projectile.ai[1]++;
            if (counter == 0)
            {
                targetPos = target.Center;
                origPos = player.Center;
            }
            target.Center = targetPos;
            player.Entropy().immune = 52;
            player.itemTime = player.itemAnimation = 4;
            Projectile.Center = target.Center;
            if (counter <= 6)
            {
                Vector2 plrPos = Vector2.Lerp(origPos, Projectile.Center, counter / 6f);
                player.Center = plrPos;
                if (counter > 0)
                {
                    Vector2 from = lastPlrPos;
                    Vector2 to = plrPos;
                    for (float i = 0; i < 1; i += 0.05f)
                    {
                        EParticle.spawnNew(new SlashDarkRed() { scw = 1f, colorInside = Color.White }, Vector2.Lerp(from, to, i), (to - from) * 0.1f, Color.LightSkyBlue, Main.rand.NextFloat(0.06f, 0.07f), 1, true, BlendState.AlphaBlend, (to - from).ToRotation(), 7);
                    }
                }
                lastPlrPos = plrPos;
            }
            else
            {
                player.Entropy().DontDrawTime = 2;
                player.Center = Projectile.Center;
            }
            if (counter < 60)
            {
                Projectile.timeLeft = 2;
                if (counter > 6 && counter % 3 == 0)
                {
                    if (counter % 9 == 0)
                        EParticle.NewParticle(new PrismShard() { PixelShader = true}, target.Center + CEUtils.randomPointInCircle(128), Vector2.Zero, Color.White, 1, 1, true, BlendState.AlphaBlend, CEUtils.randomRot());
                    CEUtils.SetShake(Projectile.Center, 4);
                    CEUtils.SpawnExplotionFriendly(Projectile.GetSource_FromAI(), player, Projectile.Center, Projectile.damage / 10, 280, Projectile.DamageType).ArmorPenetration = 200;
                    float rot = CEUtils.randomRot();
                    for(int i = 0; i < 24; i++)
                    {
                        Particle p = new Particle();
                        p.position = Projectile.Center + -rot.ToRotationVector2() * 180;
                        p.alpha = Main.rand.NextFloat(0.3f, 0.4f);
                        p.shape = 4;
                        p.vd = 0.96f;
                        p.velocity = (rot.ToRotationVector2() * 38 + CEUtils.randomPointInCircle(4)) * Main.rand.NextFloat(0.2f, 1);
                        VoidParticles.particles.Add(p);
                    }
                    EParticle.spawnNew(new DOracleSlash() { centerColor = Color.White, PixelShader = true }, Projectile.Center, Vector2.Zero, new Color(180, 180, 255), Main.rand.NextFloat(250, 280), 0.6f, true, BlendState.NonPremultiplied, rot, 8);

                    CEUtils.PlaySound("AntivoidDash", Main.rand.NextFloat(1.4f, 1.8f), Projectile.Center, 16, 0.5f);
                }
            }
            if (counter == 0) CEUtils.PlaySound("AntivoidDashHit", 1.2f, Projectile.Center);
            if (counter == 60)
            {
                int stealthRegenDelay = 160.ApplyCdDec(player);
                player.Entropy().StealthRegenDelay = stealthRegenDelay;
                CEUtils.PlaySound("CastTriangles", 0.8f, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<NetherRiftCrack>(), Projectile.damage, 1, Projectile.owner).ToProj().DamageType = Projectile.DamageType;
                for (int i = 0; i < 64; i++)
                {
                    Particle p = new Particle();
                    p.position = Projectile.Center;
                    p.alpha = Main.rand.NextFloat(0.6f, 0.8f);
                    p.shape = 4;
                    p.vd = 0.96f;
                    p.velocity = CEUtils.randomPointInCircle(16);
                    VoidParticles.particles.Add(p);
                }
                player.velocity = ((origPos - Projectile.Center) * new Vector2(1, 0.6f)).normalize() * 46;
                if (player.velocity.Y < -24)
                    player.velocity.Y = -24;
                player.Entropy().XSpeedSlowdownTime = 34;
                player.Entropy().gravAddTime = 34;
                ScreenShaker.AddShake(new ScreenShaker.ScreenShake(player.velocity.normalize() * 4, 16));
                CalamityEntropy.Instance.screenShakeAmp = 4;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Projectile.GetTexture();
            int counter = (int)Projectile.ai[1];
            Texture2D circle = CEUtils.RequestTex("CalamityEntropy/Content/Items/Weapons/TheoCircle");
            if (target != null)
            {
                int adjustDrawingYPos = -20 - target.height / 2;
                Vector2 drawPos = Projectile.Center + new Vector2(0, adjustDrawingYPos);
                 float alpha = float.Min(counter / 12, 1);
                DrawChain(target.Center, alpha + 0.3f + (counter > 46 ? ((counter - 46) / 8f) : 0));
                Main.EntitySpriteDraw(circle, target.Center - Main.screenPosition, null, Color.White * (0.85f + 0.15f * (float)(Math.Cos(Main.GameUpdateCount * 0.15f))), Main.GlobalTimeWrappedHourly * 1.2f, circle.Size() / 2f, Projectile.scale * 1.5f, SpriteEffects.None);
                
            }
            return false;
        }
        public void DrawEye()
        {
            Texture2D tex = Projectile.GetTexture();
            int counter = (int)Projectile.ai[1];
            if (target != null)
            {
                int adjustDrawingYPos = -20 - target.height / 2;
                Vector2 drawPos = Projectile.Center + new Vector2(0, adjustDrawingYPos);
                Main.EntitySpriteDraw(tex, drawPos - Main.screenPosition, null, Color.White, 0, tex.Size() / 2f, CEUtils.CustomLerp2(float.Min(counter / 10f, 1)), SpriteEffects.None);
            }
        }
        public void DrawChain(Vector2 center, float alpha)
        {
            Texture2D chain = CEUtils.RequestTex("CalamityEntropy/Content/Items/Weapons/CrossBorderPursuitAlt");
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            while (alpha > 0)
            {
                for (int i = 0; i < 3; i++)
                {
                    float rot = -MathHelper.PiOver2 + MathHelper.TwoPi / 3 * i;
                    for (int j = 0; j < 32; j++)
                    {
                        Vector2 drawPos = center + rot.ToRotationVector2() * chain.Width * j;
                        Main.spriteBatch.Draw(chain, drawPos - Main.screenPosition, null, Color.White * ((32f - j) / 32f) * (float.Min(1, alpha)), rot, new Vector2(0, chain.Height / 2), 1, SpriteEffects.None, 0);
                    }
                }
                alpha -= 1;
            }
            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
        }
    }

}
