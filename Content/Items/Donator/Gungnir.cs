using CalamityEntropy.Content.Items.Weapons.Fractal;
using CalamityEntropy.Content.Particles;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Donator
{
    public class Gungnir : ModItem, IDonatorItem
    {
        public string DonatorName => "散尘化天心";

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = TextureAssets.Item[Type].Value;
            Vector2 position = Item.position - Main.screenPosition + tex.Size() / 2;
            Rectangle iFrame = tex.Frame();
            for (int i = 0; i < 16; i++)
                spriteBatch.Draw(tex, position + MathHelper.ToRadians(i * 60f).ToRotationVector2() * 2f, null, Color.DarkBlue with { A = 0 }, rotation, tex.Size() / 2, scale, 0, 0f);

            spriteBatch.Draw(tex, position, iFrame, Color.White, rotation, tex.Size() / 2, scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.Blue);
            return false;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Gungnir)
                .AddIngredient<DivineGeode>(12)
                .AddIngredient<GalacticaSingularity>(6)
                .AddIngredient<AscendantSpiritEssence>(2)
                .AddIngredient<CosmiliteBar>(12)
                .AddTile<CosmicAnvil>()
                .Register();
        }

        public override void SetDefaults()
        {
            Item.width = 120;
            Item.height = 120;
            Item.damage = 1000;
            Item.crit = 10;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = CalamityGlobalItem.RarityCalamityRedBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<GungnirThrow>();
            Item.shootSpeed = 28;
            Item.DamageType = DamageClass.Melee;
        }
    }
    public class GungnirThrow : ModProjectile
    {
        public override string Texture => "CalamityEntropy/Content/Items/Donator/Gungnir";
        public override void SetDefaults()
        {
            Projectile.FriendlySetDefaults(DamageClass.Melee, false, -1);
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.MaxUpdates = 4;
            Projectile.penetrate = 7;
            Projectile.timeLeft = 240;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return CEUtils.LineThroughRect(Projectile.Center - Projectile.rotation.ToRotationVector2() * 170 * Projectile.scale, Projectile.Center, targetHitbox, 32);
        }
        public List<Vector2> oldPos = new List<Vector2>();
        public List<float> oldRot = new List<float>();
        public override void AI()
        {
            if (Projectile.localAI[0]++ == 0)
                CEUtils.PlaySound("xswing", Main.rand.NextFloat(3, 3.6f), Projectile.Center);
            Projectile.rotation = Projectile.velocity.ToRotation();
            oldPos.Add(Projectile.Center);
            oldRot.Add(Projectile.rotation);
            if (oldPos.Count > 36)
            {
                oldPos.RemoveAt(0);
                oldRot.RemoveAt(0);
            }
            if (Projectile.localAI[0] > 1)
            {
                for (float i = 0; i < 1; i += 1f)
                {
                    EParticle.spawnNew(new HeavenfallStar2() { drawScale = new Vector2(0.4f, 3) }, Projectile.Center - Projectile.velocity * i, Projectile.velocity * 0.6f, new Color(60, 60, 255), 0.4f, 1, true, BlendState.Additive, Projectile.rotation, 5);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ScreenShaker.AddShake(new ScreenShaker.ScreenShake(Vector2.Zero, 6));
            EParticle.spawnNew(new DOracleSlash() { centerColor = Color.White }, Projectile.Center - Projectile.velocity * 0.8f, Vector2.Zero, new Color(122, 122, 255), Main.rand.NextFloat(380, 420), 1f, true, BlendState.Additive, Projectile.rotation + MathHelper.Pi, 8);
            if (Projectile.ai[1]-- > -1)
            {
                CEUtils.PlaySound("ThunderStrike", Main.rand.NextFloat(0.8f, 1.2f), target.Center, 6, 0.4f);
                CEUtils.PlaySound("ystn_hit", 2.7f, target.Center);
                for (int i = 0; i < 8; i++)
                {
                    Vector2 pos = target.Center + new Vector2(0, -900) + CEUtils.randomPointInCircle(600);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), pos, (target.Center - pos).normalize() * 42, ModContent.ProjectileType<AstralStarMelee>(), Projectile.damage / 6, Projectile.owner);
                }
                for (int i = 0; i < 1; i++)
                {
                    int lightningDamage = (int)(Projectile.damage * 1.25f);
                    Vector2 lightningSpawnPosition = Projectile.Center - Vector2.UnitY.RotatedByRandom(0.2f) * 1000f;
                    Vector2 lightningShootVelocity = (target.Center - lightningSpawnPosition + target.velocity * 7.5f).SafeNormalize(Vector2.UnitY) * 15f;
                    int lightning = Projectile.NewProjectile(Projectile.GetSource_FromThis(), lightningSpawnPosition, lightningShootVelocity, ModContent.ProjectileType<StormfrontLightning>(), lightningDamage, 0f, Projectile.owner);
                    if (Main.projectile.IndexInRange(lightning))
                    {
                        Main.projectile[lightning].CritChance = Projectile.CritChance;
                        Main.projectile[lightning].ai[0] = lightningShootVelocity.ToRotation();
                        Main.projectile[lightning].ai[1] = Main.rand.Next(100);
                        Main.projectile[lightning].DamageType = Projectile.DamageType;
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Effect shader = ModContent.Request<Effect>("CalamityEntropy/Assets/Effects/SwordTrail3", AssetRequestMode.ImmediateLoad).Value;
            List<ColoredVertex> ve = new();
            {
                for (int i = 0; i < oldPos.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(12 * Projectile.scale * 1, 0).RotatedBy(oldRot[i] + MathHelper.PiOver2)),
                          new Vector3((i) / ((float)oldPos.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(12 * Projectile.scale * 1, 0).RotatedBy(oldRot[i] - MathHelper.PiOver2)),
                          new Vector3((i) / ((float)oldPos.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color1"].SetValue(Color.DarkBlue.ToVector4());
                    shader.Parameters["color2"].SetValue(new Color(220, 220, 255).ToVector4());
                    shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 2.4f);
                    shader.Parameters["alpha"].SetValue(1);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak2");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            ve.Clear();
            {
                for (int i = 0; i < oldPos.Count; i++)
                {
                    Color b = new Color(255, 255, 255);
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(9 * Projectile.scale * 1, 0).RotatedBy(oldRot[i] + MathHelper.PiOver2)),
                          new Vector3((i) / ((float)oldPos.Count - 1), 1, 1),
                          b));
                    ve.Add(new ColoredVertex(oldPos[i] - Main.screenPosition + (new Vector2(9 * Projectile.scale * 1, 0).RotatedBy(oldRot[i] - MathHelper.PiOver2)),
                          new Vector3((i) / ((float)oldPos.Count - 1), 0, 1),
                          b));
                }
                if (ve.Count >= 3)
                {
                    var gd = Main.graphics.GraphicsDevice;
                    SpriteBatch sb = Main.spriteBatch;

                    sb.End();
                    sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);
                    shader.Parameters["color1"].SetValue(new Vector4(1, 1, 1, 0));
                    shader.Parameters["color2"].SetValue(Color.White.ToVector4() * 0.82f);
                    shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 3f);
                    shader.Parameters["alpha"].SetValue(1);
                    shader.CurrentTechnique.Passes["EffectPass"].Apply();
                    gd.Textures[0] = CEUtils.getExtraTex("Streak1");
                    gd.DrawUserPrimitives(PrimitiveType.TriangleStrip, ve.ToArray(), 0, ve.Count - 2);
                    Main.spriteBatch.ExitShaderRegion();
                }
            }

            Main.spriteBatch.UseBlendState(BlendState.AlphaBlend);
            Texture2D tex = Projectile.GetTexture();
            Vector2 position = Projectile.Center - Main.screenPosition;
            for (int i = 0; i < 16; i++)
                Main.spriteBatch.Draw(tex, position + MathHelper.ToRadians(i * 60f).ToRotationVector2() * 2f, null, Color.Blue with { A = 0 }, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2, Projectile.scale, 0, 0f);

            Main.spriteBatch.Draw(tex, position, null, Color.White, Projectile.rotation + MathHelper.PiOver4, tex.Size() / 2, Projectile.scale, 0f, 0f);
            Lighting.AddLight(position, TorchID.Blue);
            return false;
        }
    }
}