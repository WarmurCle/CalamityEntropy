using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using System.IO;
using Terraria;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.Enums;
using Terraria.ModLoader;
using ReLogic.Content;
using CalamityEntropy.Buffs.Pets;
using CalamityEntropy.Util;
using CalamityEntropy.Projectiles.Pets.Deus;
using CalamityMod.NPCs.DevourerofGods;
using System.Text;
using Terraria.Localization;
using Terraria.DataStructures;
namespace CalamityEntropy.Projectiles.Pets
{
    public class LostSoulProj : ModProjectile
    {
        public int counter = 0;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 1;
            Main.projPet[Projectile.type] = true;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.ZephyrFish);
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
            Projectile.width = 16;
            Projectile.height = 24;
            Projectile.scale = 2;
        }
        int dc = 0;
        public bool mantle = true;
        public List<NPC> needLoots = new List<NPC>();
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D txd = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/Pets/LostSoulProj").Value;
            if (Main.gameMenu)
            {
                
                Main.spriteBatch.Draw(txd, Projectile.Center - Main.screenPosition, Util.Util.GetCutTexRect(txd, 4, 0), lightColor, Projectile.rotation, new Vector2(txd.Width, txd.Height) / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0);

                return false;
            }
            dc++;
            if (spawnAnm)
            {
                txd = ModContent.Request<Texture2D>("CalamityEntropy/Projectiles/Pets/LostSoulSpawn").Value;
                Main.spriteBatch.Draw(txd, Projectile.Center - Main.screenPosition + new Vector2((spawnAnmFrame == 1 ? Main.rand.Next(-4, 5) : 0), (float)Math.Cos((float)dc * 0.06f) * 20), Util.Util.GetCutTexRect(txd, 6, spawnAnmFrame), lightColor * alpha, Projectile.rotation, new Vector2(32, txd.Height) / 2, Projectile.scale, SpriteEffects.None, 0);


                return false;
            }
            Main.spriteBatch.Draw(txd, Projectile.Center - Main.screenPosition + new Vector2(0, (float)Math.Cos((float)dc * 0.06f) * 20), Util.Util.GetCutTexRect(txd, 4, (counter / 4) % 4), lightColor * alpha, Projectile.rotation, new Vector2(32, txd.Height) / 2, Projectile.scale, SpriteEffects.None, 0);
            


            return false;

        }
        public bool std = false;
        void MoveToTarget(Vector2 targetPos)
        {
            if (Util.Util.getDistance(Projectile.Center, targetPos) > 3000)
            {
                Projectile.Center = Main.player[Projectile.owner].Center;
            }
            if (Util.Util.getDistance(Projectile.Center, targetPos) > 20)
            {
                Vector2 px = targetPos - Projectile.Center;
                px.Normalize();
                Projectile.velocity += px * 1.6f;

                Projectile.velocity *= 0.96f;

            }
            else
            {
                Projectile.velocity *= 0.76f;

            }

        }
        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.zephyrfish = false;

            return true;
        }
        public static string ConvertToUnicodeString(string text)
        {
            string result = "";
            foreach (char c in text)
            {
                result += "\\u" + ((int)c).ToString("x4");
            }
            return result;
        }
        public int hideVisualTime = 0;
        public float alpha = 1;
        public int mantleCd = 0;
        public List<int> bosses = new List<int>();
        public override void OnSpawn(IEntitySource source)
        {
            foreach(NPC n in Main.npc)
            {
                if (n.active && n.boss)
                {
                    hideVisualTime = 100;
                    alpha = 0;
                }
            }
        }
        public override void AI()
        {
            mantleCd--;
            if (hideVisualTime > 0)
            {
                alpha -= 0.025f;
            }
            else
            {
                alpha = 1;

            }
            counter++;
            Player player = Main.player[Projectile.owner];
            if (hideVisualTime > 0)
            {
                Projectile.velocity = new Vector2(0, -3.6f);
            }
            else
            {
                MoveToTarget(player.Center + new Vector2(0, 0) + new Vector2(-60 * player.direction, 0));
            }
            if (!player.dead && player.HasBuff(ModContent.BuffType<LostSoulBuff>()))
            {
                Projectile.timeLeft = 2;
            }

            hideVisualTime--;
            bool hasBoss = false;
            if(hideVisualTime > 0)
            {
                bosses.Clear();
            }
            foreach (NPC n in Main.npc)
            {
                if (n.active && n.boss)
                {
                    if (hideVisualTime <= 0 && !bosses.Contains(n.whoAmI) && n.realLife < 0)
                    {
                        bosses.Add(n.whoAmI);
                    }

                    hasBoss = true;
                }
            }
            if (hideVisualTime <= 0)
            {
                if (mantleCd <= 0 && mantle == false)
                {
                    mantleCd = 3600;
                    mantle = true;
                }
                for (int i = bosses.Count - 1; i >= 0; i--) {
                    int b = bosses[i];
                    if (!b.ToNPC().active)
                    {
                        bosses.RemoveAt(i);
                       needLoots.Add((NPC)b.ToNPC().Clone());
                    }
                }
            }
            
            if(Util.Util.getDistance(Projectile.Center, player.Center) < 300 && !spawnAnm && needLoots.Count > 0)
            {
                spawnAnm = true;
                spawnAnmFrame = 0;
                spawnAnmCount = 4;
                Projectile.netUpdate = true;
            }
            if (hasBoss)
            {
                if(hideVisualTime > 0)
                {
                    hideVisualTime = 20;
                }
            }
            if (hideVisualTime == 0 && alpha <= 0)
            {
                alpha = 0;
                Projectile.Center = player.Center;
                mantle = true;
            }
            if(hideVisualTime == 0)
            {
                Projectile.netUpdate = true;
            }
            hurtCd--;
            if (hideVisualTime < 0)
            {
                
                foreach (Projectile p in Main.projectile)
                {
                    if (p.active && p.damage > 0 && p.hostile)
                    {
                        if (p.Colliding(p.Hitbox, Projectile.getRect()))
                        {
                            if (p.penetrate > 0)
                            {
                                p.penetrate -= 1;
                            }
                            hurt();
                            return;
                        }
                        if (Util.Util.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center) < 1000 && Util.Util.getDistance(p.Center + p.velocity * 20, Projectile.Center + Projectile.velocity * 20) < 120 + Math.Max(p.width, p.height)) {
                            Projectile.velocity += (Projectile.Center + Projectile.velocity * 6 - p.Center + p.velocity * 6).SafeNormalize(Vector2.One) * 1.6f;
                        }
                    }
                }
                
                foreach (NPC n in Main.npc)
                {
                    if (n.active && n.damage > 0 && !n.friendly)
                    {
                        if (n.Hitbox.Intersects(Projectile.getRect()))
                        {
                            hurt();
                            return;
                        }
                        if (Util.Util.getDistance(Projectile.Center, Projectile.owner.ToPlayer().Center) < 1000 && Util.Util.getDistance(n.Center + n.velocity * 20, Projectile.Center + Projectile.velocity * 20) < 160 + Math.Max(n.width, n.height))
                        {
                            Projectile.velocity += (Projectile.Center + Projectile.velocity * 6 - n.Center + n.velocity * 6).SafeNormalize(Vector2.One) * 1.6f;
                        }
                    }
                }
            }
            if (spawnAnm)
            {
                Projectile.velocity *= 0;
                spawnAnmCount--;
                if (spawnAnmCount <= 0)
                {
                    spawnAnmCount = 4;
                    spawnAnmFrame++;
                    if(spawnAnmFrame == 1)
                    {
                        spawnAnmCount = 44;
                    }
                    if(spawnAnmFrame > 5)
                    {
                        Loots();
                        spawnAnm = false;
                        spawnAnmFrame = 0;
                    }
                }
            }

        }
        public bool spawnAnm = false;
        public int spawnAnmFrame = 0;
        public int spawnAnmCount = 0;
        private void Loots()
        {
            Projectile.netUpdate = true;
            if (!(Main.myPlayer == Projectile.owner))
            {
                return;
            }
            foreach(NPC n in needLoots)
            {
                n.Center = Projectile.Center;
                n.NPCLoot();
                
            }
            needLoots.Clear();
        }

        public int hurtCd = 0;

        public void hurt()
        {
            Projectile.netUpdate = true;
            if (hurtCd > 0)
            {
                return;
            }
            if (mantle)
            {
                if(Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MantleBreak>(), 0, 0, -1);
                }
                hurtCd = 120;
                mantle = false;
                mantleCd = 3600;
            }
            else {
                needLoots.Clear();
                bosses.Clear();
                hideVisualTime = 180;
                SoundStyle s = new SoundStyle("CalamityEntropy/Sounds/isaacdies");
                SoundEngine.PlaySound(s);
                CombatText.NewText(Projectile.getRect(), Color.White, Mod.GetLocalization("Uh").Value);
            } 
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(hurtCd);
            writer.Write(mantle);
            writer.Write(mantleCd);
            writer.Write(spawnAnm);
            writer.Write(spawnAnmFrame);
            writer.Write(spawnAnmCount);
            writer.Write(hideVisualTime); 
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            hurtCd = reader.ReadInt32();
            mantle = reader.ReadBoolean();
            mantleCd = reader.ReadInt32();
            spawnAnm = reader.ReadBoolean();
            spawnAnmFrame = reader.ReadInt32();
            spawnAnmCount = reader.ReadInt32();
            hideVisualTime = reader.ReadInt32();
        }
    }
}
