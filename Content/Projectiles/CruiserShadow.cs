using System.Collections.Generic;
using System.IO;
using CalamityEntropy.Common;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class CruiserShadow : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
        }
        float mouthRot = 0;
        public bool bite = false;
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 156;
            Projectile.height = 156;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.light = 1f;
            Projectile.timeLeft = 180;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.ArmorPenetration = 60;
            Projectile.localNPCHitCooldown = 8;
        }
        public Vector2 spawnPos;
        public float spawnRot = 0;
        public float alphaPor = 1;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            EGlobalNPC.AddVoidTouch(target, 90, 3.6f, 1000, 16);
            Projectile.netUpdate = true;
            bite = true;
            if (noChase < -10)
            {
                noChase = 10;
            }
        }
        public List<Vector2> bodies = new List<Vector2>();
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < 27; i++)
            {
                bodies.Add(Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * -20);
            }
            Projectile.Center += Projectile.velocity * 16;
            targetPos = Projectile.Center;
            spawnPos = Projectile.Center;
            spawnRot = Projectile.velocity.ToRotation();
            Projectile.Center += Projectile.velocity * 10;
        }
        float counter = 0;
        public void DrawPortal(Vector2 pos, Color color, float rot, float size, float xmul = 0.3f, float aj = 0)
        {
            
            Texture2D tx = Util.Util.getExtraTex("SoulVortex");
            float angle = MathHelper.ToDegrees(counter * 0.2f + aj);
            Vector2 lu = new Vector2(size, 0).RotatedBy(MathHelper.ToRadians(angle - 135));
            Vector2 ru = new Vector2(size, 0).RotatedBy(MathHelper.ToRadians(angle - 45));
            Vector2 ld = new Vector2(size, 0).RotatedBy(MathHelper.ToRadians(angle + 135));
            Vector2 rd = new Vector2(size, 0).RotatedBy(MathHelper.ToRadians(angle + 45));

            lu.X *= xmul;
            ru.X *= xmul;
            ld.X *= xmul;
            rd.X *= xmul;

            Vector2 dp = pos - Main.screenPosition;
            float rangle = rot;
            lu = lu.RotatedBy(rangle);
            ru = ru.RotatedBy(rangle);
            ld = ld.RotatedBy(rangle);
            rd = rd.RotatedBy(rangle);

            Util.Util.drawTextureToPoint(Main.spriteBatch, tx, color, dp + lu, dp + ru, dp + ld, dp + rd);
        }
        public override void AI(){
            alphaPor *= 0.88f;
            counter++;
            if(counter % 20 == 0 && Main.myPlayer == Projectile.owner)
            {
                for(int i = 0; i < 6; i++)
                {
                    Projectile p = Main.projectile[Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.6f + new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11)), ModContent.ProjectileType<VoidStarF>(), (int)(Projectile.damage * 0.16f), 5, Projectile.owner)];
                    p.DamageType = Projectile.DamageType;
                }
            }
            Player player = Projectile.owner.ToPlayer();
            Projectile.rotation = Projectile.velocity.ToRotation();
            updateBodies();
            if (bite)
            {
                mouthRot -= 12;
                if (mouthRot < -48)
                {
                    bite = false;
                }
            }
            else
            {
                mouthRot *= 0.9f;
            }
            spawnParticles();
            if(Main.myPlayer == Projectile.owner)
            {
                if(targetPos != Main.MouseWorld)
                {
                    Projectile.netUpdate = true;
                }
                targetPos = Main.MouseWorld;
            }

            Vector2 c = Projectile.Center;
            Projectile.Center = player.Center;
            NPC n = Projectile.FindTargetWithinRange(2400);
            Projectile.Center = c;
            noChase--;
            
            if(Projectile.timeLeft < 40)
            {
                Projectile.velocity.Y -= 1;
                Projectile.velocity *= 0.98f;
                return;
            }
            if (n != null)
            {
                targetPos = n.Center;
                if(Util.Util.getDistance(targetPos, Projectile.Center) > 60)
                {
                    Projectile.velocity *= 0.9f;
                    Projectile.rotation = Projectile.velocity.ToRotation();
                    Projectile.rotation = Util.Util.rotatedToAngle(Projectile.rotation, (targetPos - Projectile.Center).ToRotation(), 36);

                    Projectile.velocity = new Vector2(Projectile.velocity.Length() + 10, 0).RotatedBy(Projectile.rotation);
                }
                else {
                    Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 20f;
                    Projectile.velocity *= 0.8f;
                }
            }
            else
            {
                if (Util.Util.getDistance(Projectile.Center, targetPos) > 1000)
                {
                    Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 40f;
                    Projectile.velocity *= 0.8f;
                }
                else
                if (Util.Util.getDistance(Projectile.Center, targetPos) > 100)
                {
                    Projectile.velocity += (targetPos - Projectile.Center).SafeNormalize(Vector2.Zero) * 0.9f;
                    Projectile.velocity *= 0.996f;
                }
            }
        }
        public int noChase = 0;
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(targetPos);
            writer.Write(bite);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            targetPos = reader.ReadVector2();
            bite = reader.ReadBoolean();
        }
        Vector2 targetPos;

        public void updateBodies()
        {
            for (int i = 0; i < bodies.Count; i++)
            {
                Vector2 oPos;
                float oRot;

                if (i == 0)
                {
                    oPos = Projectile.Center;
                    oRot = Projectile.rotation;
                }
                else
                {
                    oPos = bodies[i - 1];
                    if (i == 1)
                    {
                        oRot = (Projectile.Center - bodies[0]).ToRotation();
                    }
                    else
                    {
                        oRot = (bodies[i - 2] - bodies[i - 1]).ToRotation();
                    }
                }
                float rot = (oPos - bodies[i]).ToRotation();
                rot = Util.Util.rotatedToAngle(rot, oRot, 0.12f, false);
                int spacing = 54;
                bodies[i] = oPos - rot.ToRotationVector2() * spacing * Projectile.scale;
            }
        }

        public void spawnParticles()
        {
            for (int i = 0; i < 4; i++)
            {
                Particle p = new Particle();
                p.shape = 4;
                p.position = Projectile.Center - Projectile.rotation.ToRotationVector2() * 60;
                p.alpha = 1.6f;
                p.ad = 0.013f;
                var r = Main.rand;
                p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * .3), (float)((r.NextDouble() - 0.5) * 1.3));
                VoidParticles.particles.Add(p);
            }
            for (int i = 0; i < 4; i++)
            {
                Particle p = new Particle();
                p.shape = 4;
                p.position = Projectile.Center - Projectile.rotation.ToRotationVector2() * 60 - Projectile.velocity * 0.5f;
                p.alpha = 1.6f;
                p.ad = 0.013f;
                var r = Main.rand;
                p.velocity = new Vector2((float)((r.NextDouble() - 0.5) * .3), (float)((r.NextDouble() - 0.5) * 1.3));
                VoidParticles.particles.Add(p);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int bd = 0;
            Vector2 vtodraw = Projectile.Center;
            SpriteBatch spriteBatch = Main.spriteBatch;
            float alpha = 1;
            if(Projectile.timeLeft < 40)
            {
                alpha = (float)Projectile.timeLeft / 40f;
            }
            for (int d = 0; d < 9; d++)
            {
                if (d == 0 || d == 2)
                {
                    continue;
                }
                float rot = 0;
                if (bd == 0)
                {
                    rot = (vtodraw - bodies[d]).ToRotation();
                }
                else
                {
                    rot = (bodies[d - 1] - bodies[d]).ToRotation();
                }
                Vector2 pos = bodies[d];

                Texture2D tx;
                tx = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/P2b" + (bd + 1).ToString()).Value;

                spriteBatch.Draw(tx, pos - Main.screenPosition, null, Color.White * alpha, rot, new Vector2(tx.Width, tx.Height) / 2, Projectile.scale, SpriteEffects.None, 0f);

                bd += 1;


            }
            Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/Head2").Value;
            Texture2D j2 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserJawUp2").Value;
            Texture2D j1 = ModContent.Request<Texture2D>("CalamityEntropy/Content/NPCs/Cruiser/CruiserJawDown2").Value;
            Vector2 joffset = new Vector2(60, 62);
            Vector2 ofs2 = joffset * new Vector2(1, -1);
            float roth = mouthRot * 0.8f;

            spriteBatch.Draw(j1, vtodraw - Main.screenPosition + joffset.RotatedBy(Projectile.rotation) * Projectile.scale, null, Color.White * alpha, Projectile.rotation + MathHelper.ToRadians(roth), new Vector2(40, 28), Projectile.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(j2, vtodraw - Main.screenPosition + ofs2.RotatedBy(Projectile.rotation) * Projectile.scale, null, Color.White * alpha, Projectile.rotation - MathHelper.ToRadians(roth), new Vector2(40, j2.Height - 28), Projectile.scale, SpriteEffects.None, 0);

            spriteBatch.Draw(txd, vtodraw - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
    

}