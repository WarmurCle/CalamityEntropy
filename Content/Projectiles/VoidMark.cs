using CalamityEntropy.Common;
using CalamityEntropy.Content.Buffs;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class VoidMark : ModProjectile
    {
        List<Vector2> odp = new List<Vector2>();
        List<float> odr = new List<float>();

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = false;
            base.SetStaticDefaults();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

            if (Main.rand.NextDouble() < 0.12)
            {
                modifiers.SetCrit();
            }
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Summon;
            Projectile.width = 248;
            Projectile.height = 248;
            Projectile.friendly = true;
            Projectile.aiStyle = -1;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.light = 20f;
            Projectile.minion = true;
            Projectile.minionSlots = 0;
            Projectile.damage = 348;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.hide = true;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.minionSlots = Projectile.ai[2];

        }
        public override bool PreDrawExtras()
        {
            return base.PreDrawExtras();
        }
        public override bool? CanCutTiles()
        {
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[1] > 0)
                {
                    EGlobalNPC.AddVoidTouch(target, 200, 2, 600, 16);
                    int p = Projectile.NewProjectile(Projectile.owner.ToPlayer().GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<Slash2>(), Projectile.damage, 0, Projectile.owner);
                    p.ToProj().rotation = (float)(Main.rand.NextDouble() - 0.5f) * (float)Math.PI * 0.4f + (float)Math.PI * 0.5f;
                    Projectile.ai[1] -= 1;
                    Main.projectile[p].netUpdate = true;


                }
                else
                {
                    EGlobalNPC.AddVoidTouch(target, 40, 1, 600);
                    int p = Projectile.NewProjectile(Projectile.owner.ToPlayer().GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<Slash>(), 0, 0, Projectile.owner);
                    p.ToProj().rotation = (float)(Main.rand.NextDouble() - 0.5f) * (float)Math.PI * 0.4f + (float)Math.PI * 0.5f;
                    Main.projectile[p].netUpdate = true;
                }
            }
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                if (Projectile.ai[1] > 0)
                {
                    EGlobalNPC.AddVoidTouch(target, 200, 2, 600, 16);

                    int p = Projectile.NewProjectile(Projectile.owner.ToPlayer().GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<Slash2>(), Projectile.damage, 0, Projectile.owner);
                    p.ToProj().rotation = (float)(Main.rand.NextDouble() - 0.5f) * (float)Math.PI * 0.4f + (float)Math.PI * 0.5f;
                    Projectile.ai[1] -= 1;
                    Main.projectile[p].netUpdate = true;
                }
                else
                {
                    EGlobalNPC.AddVoidTouch(target, 40, 1, 600);
                    int p = Projectile.NewProjectile(Projectile.owner.ToPlayer().GetSource_FromAI(), target.Center, Vector2.Zero, ModContent.ProjectileType<Slash>(), 0, 0, Projectile.owner);
                    p.ToProj().rotation = (float)(Main.rand.NextDouble() - 0.5f) * (float)Math.PI * 0.4f + (float)Math.PI * 0.5f;
                    Main.projectile[p].netUpdate = true;
                }
            }
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (odp.Count > 1)
            {
            }
            if (Projectile.localNPCHitCooldown < 0)
            {
                Projectile.localNPCHitCooldown = 0;
            }
            foreach (Projectile p in Main.projectile)
            {
                if (p.active && Util.Util.getDistance(p.Center, Projectile.Center) < 248 * Projectile.scale * 1.5f && (p.owner != Projectile.owner || p.owner == -1 || p.hostile))
                {
                    p.velocity += (p.Center - Projectile.Center).SafeNormalize(new Vector2(1, 0)) * 0.1f * (Util.Util.getDistance(p.Center, Projectile.Center) / (248 * 1.5f));
                    Dust.NewDust(p.Center, 6, 6, DustID.MagicMirror, 0, 0);
                }
            }



            if (player.HasBuff(ModContent.BuffType<VoidStorm>()))
            {
                Projectile.timeLeft = 3;
            }
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.netUpdate = true;
                /*Projectile.velocity += (Main.MouseWorld - Projectile.Center).ToRotation().ToRotationVector2() * 3;
                Projectile.velocity *= 0.94f;
                if (Util.Util.getDistance(Projectile.Center, player.Center) > 3000)
                {*/
                Projectile.Center = Main.MouseWorld;
            }
            Projectile.rotation += 0.07f;
            Projectile.ai[0] += 1;
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindNPCs.Add(index);

        }
        public override bool PreDraw(ref Color lightColor)
        {
            odr.Add(Projectile.rotation);
            odp.Add(Projectile.Center);
            if (odp.Count > 60)
            {
                odp.RemoveAt(0);
                odr.RemoveAt(0);
            }

            List<Texture2D> flame1 = new List<Texture2D>();
            List<Texture2D> flame2 = new List<Texture2D>();
            for (int i = 0; i < 3; i++)
            {
                flame1.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VM/flame" + i.ToString()).Value);
            }
            for (int i = 1; i < 6; i++)
            {
                flame2.Add(ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VM/darkflame" + i.ToString()).Value);
            }




            Texture2D ft2 = flame2[(int)(Projectile.ai[0] / 5) % flame2.Count];



            Texture2D tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VoidMark").Value;
            float x = 0f;
            if (odp.Count > 16)
            {
                for (int i = odp.Count - 14; i < odp.Count; i++)
                {
                    Color tc = Color.White;

                    Main.spriteBatch.Draw(tx, odp[i] - Main.screenPosition, null, tc * x * 0.6f, odr[i], new Vector2(tx.Width, tx.Height) / 2, 1, SpriteEffects.None, 0);
                    x += 1 / 14f;
                }
            }
            x = 1f;
            float sz = 0f;
            float clb = 0f;
            for (int i = 0; i < odp.Count; i++)
            {
                Color tc = new Color((int)(255 * clb), (int)(255 * clb), (int)(255 * clb), 255);
                clb += 1f / odp.Count;
                Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, tc * x * 0.6f, odr[i], new Vector2(tx.Width, tx.Height) / 2, sz, SpriteEffects.None, 0);
                x += 0f / odp.Count;
                sz += 1f / odp.Count;
            }


            Main.spriteBatch.Draw(ft2, Projectile.Center - Main.screenPosition - new Vector2(0, 40), null, Color.White * 0.8f, 0, new Vector2(ft2.Width, ft2.Height) / 2, 2, SpriteEffects.None, 0);

            float ap = 1;
            Main.spriteBatch.Draw(tx, Projectile.Center - Main.screenPosition, null, Color.White * ap, Projectile.rotation, new Vector2(tx.Width, tx.Height) / 2, 1, SpriteEffects.None, 0);
            float ap2 = 0.6f + (float)(Math.Cos((float)Projectile.ai[0] / 26f) * 0.2f);
            Texture2D eye = ModContent.Request<Texture2D>("CalamityEntropy/Content/Projectiles/VM/Eye").Value;

            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 3.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 3.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 3f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.9f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.8f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.7f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.6f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.4f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.3f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2 * 0.1f, 0, new Vector2(eye.Width, eye.Height) / 2, 2.1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(eye, Projectile.Center - Main.screenPosition - new Vector2(0, 20), null, Color.White * ap2, 0, new Vector2(eye.Width, eye.Height) / 2, 2, SpriteEffects.None, 0);



            return false;

        }

    }
}

