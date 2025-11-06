using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Items.Vanity;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Pets
{
    public class MomosLightPet : ModItem, IDonatorItem
    {
        public string DonatorName => "Momodzmz";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<MosHat>();
        }
        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.ZephyrFish);
            Item.UseSound = SoundID.Item58;
            Item.shoot = ModContent.ProjectileType<Molightpet>();
            Item.buffType = ModContent.BuffType<MomosLightPetBuff>();
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                player.AddBuff(Item.buffType, 3600);
            }
            return true;
        }
    }
    public class MomosLightPetBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.lightPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool unused = false;
            player.BuffHandle_SpawnPetIfNeededAndSetTime(buffIndex, ref unused, ModContent.ProjectileType<Molightpet>());
        }
    }
    public class Molightpet : ModProjectile
    {
        public int counter = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            ProjectileID.Sets.LightPet[Projectile.type] = true;
            base.SetStaticDefaults();

        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 24;
            Projectile.height = 58;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Main.gameMenu)
            {
                Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/Pets/Deus/AstrumDeus").Value;
                Main.EntitySpriteDraw(txd, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            List<Texture2D> list = new List<Texture2D>();
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Pets/Molightpet").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Pets/mo/mo2").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Pets/mo/mo3").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Pets/mo/mo4").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Pets/mo/mo5").Value);

            Texture2D tx = list[(counter / 4) % list.Count];
            if (Main.player[Projectile.owner].Calamity().mouseWorld.X > Projectile.Center.X)
            {
                Projectile.direction = 1;
            }
            else
            {
                Projectile.direction = -1;
            }
            if (Projectile.direction == -1)
            {
                Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

            }
            else
            {
                Main.EntitySpriteDraw(tx, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
            }


            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.spriteBatch.Draw(CEUtils.getExtraTex("GlowCone"), Projectile.Center - Main.screenPosition, null, Color.White * 0.2f, (Projectile.GetOwner().Calamity().mouseWorld - Projectile.Center).ToRotation(), new Vector2(0, 250), new Vector2(1.4f, 0.8f), SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;

        }
        void MoveToTarget(Vector2 targetPos)
        {
            Projectile.GetOwner().Calamity().mouseWorldListener = true;
            if (CEUtils.getDistance(Projectile.Center, targetPos) > 1600)
            {
                Projectile.Center = Main.player[Projectile.owner].Center - new Vector2(0, 200);
            }
            Projectile.velocity = (targetPos - Projectile.Center) * 0.06f;
        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.zephyrfish = false;
            CEUtils.AddLight(Projectile.Center, Color.White, 4);
            Vector2 lp = Projectile.Center;
            bool addlight = false;
            float r = (Projectile.GetOwner().Calamity().mouseWorld - Projectile.Center).ToRotation();
            for (int i = 0; i < 1000; i += 8)
            {
                Point tpos = ((lp + r.ToRotationVector2() * i) / 16f).ToPoint();
                if (CEUtils.inWorld(tpos.X, tpos.Y))
                {
                    if (Main.tile[tpos].IsTileSolid())
                    {
                        addlight = true;
                        lp = tpos.ToVector2() * 16;
                        break;
                    }
                }
            }
            if (addlight)
            {
                CEUtils.AddLight(lp, Color.White, 3);
            }
            SpawnLighting();
            return true;
        }
        public void SpawnLighting()
        {
            for (float r = -0.12f; r <= 0.12f; r += 0.01f)
            {
                float rot = (Projectile.GetOwner().Calamity().mouseWorld - Projectile.Center).ToRotation() + r;

                for (float i = 0; i < 900; i += 8)
                {
                    CEUtils.AddLight(Projectile.Center + Projectile.velocity + rot.ToRotationVector2() * i, Color.White, ((910 - i) / 900f) * (5f * (0.2f - Math.Abs(r))));
                }
            }
        }
        public int shotCd = 0;

        public override void AI()
        {
            counter++;
            Player player = Main.player[Projectile.owner];
            MoveToTarget(player.Center + new Vector2(0, -100));
            if (!player.dead && player.HasBuff(ModContent.BuffType<MomosLightPetBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }


    }
}