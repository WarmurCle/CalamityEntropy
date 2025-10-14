using CalamityEntropy.Content.Buffs.Pets;
using CalamityEntropy.Content.Items.Donator;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Content.Projectiles.Pets.Deus;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Pets
{
    public class MomosLightPet : ModItem, IDonatorItem
    {
        public string DonatorName => "Momodzmz";
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
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/Molightpet").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/mo/mo2").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/mo/mo3").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/mo/mo4").Value);
            list.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Items/mo/mo5").Value);

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
            Main.spriteBatch.Draw(CEUtils.getExtraTex("GlowCone"), Projectile.Center - Main.screenPosition, null, Color.White * 0.7f, (Projectile.GetOwner().Calamity().mouseWorld - Projectile.Center).ToRotation(), new Vector2(0, 250), new Vector2(1.4f, 0.8f), SpriteEffects.None, 0);
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
            SpawnLighting();
            return true;
        }
        public void SpawnLighting()
        {
            float rot = (Projectile.GetOwner().Calamity().mouseWorld - Projectile.Center).ToRotation();
            for (float i = 0; i < 700; i += 10)
            {
                CEUtils.AddLight(Projectile.Center + Projectile.velocity + rot.ToRotationVector2() * i, Color.White * 0.14f, float.Max(4, i * 0.06f));
            }
        }
        public int shotCd = 0;

        public override void AI()
        {
            counter++;
            Player player = Main.player[Projectile.owner];
            MoveToTarget(player.Center + new Vector2(0, -200));
            if (!player.dead && player.HasBuff(ModContent.BuffType<AstrumDeusBuff>()))
            {
                Projectile.timeLeft = 2;
            }
        }


    }
}