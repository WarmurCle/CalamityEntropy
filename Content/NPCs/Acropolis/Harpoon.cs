using CalamityEntropy.Common;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Accessories;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Particles;
using CalamityEntropy.Content.Projectiles;
using CalamityEntropy.Utilities;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Particles;
using CalamityMod.World;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.NPCs.Acropolis
{
    public class Harpoon : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.MustAlwaysDraw[NPC.type] = true;
            this.HideFromBestiary();
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.width = 40;
            NPC.height = 40;
            NPC.damage = 30;
            NPC.dontTakeDamage = true;
            NPC.lifeMax = 1400;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = null;
            NPC.value = 0f;
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.dontCountMe = true;
            NPC.timeLeft *= 5;
        }
        public NPC owner => ((int)NPC.ai[0]).ToNPC();
        public bool OnLauncher = true;
        public int Back = 0;
        public override void AI()
        {
            if (NPC.ai[0] < 0 || !owner.active)
            {
                NPC.active = false;
                return;
            }
            NPC.scale = owner.scale;
            NPC.damage = owner.damage;
            if(owner.ModNPC is AcropolisMachine am)
            {
                if(OnLauncher)
                {
                    NPC.Center = am.HarpoonPos;
                    NPC.rotation = am.harpoon.Seg2Rot;
                }
                else
                {
                    NPC.rotation = (NPC.Center - am.HarpoonPos).ToRotation();
                    if(Back-- < 0)
                    {
                        NPC.velocity += (am.HarpoonPos - NPC.Center).normalize() * 8 * NPC.scale;
                        NPC.velocity *= 0.9f;
                        if(CEUtils.getDistance(NPC.Center, am.HarpoonPos) <= NPC.velocity.Length() + 6)
                        {
                            OnLauncher = true;
                            NPC.velocity *= 0;
                        }
                    }
                }
            }
            else
            {
                NPC.active = false;
            }
        }
        public override bool CheckActive()
        {
            return !owner.active;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (OnLauncher)
                return false;
            CEUtils.drawChain(NPC.Center, ((AcropolisMachine)owner.ModNPC).HarpoonPos - ((AcropolisMachine)owner.ModNPC).harpoon.Seg2Rot.ToRotationVector2() * 60, 18, "CalamityEntropy/Content/NPCs/Acropolis/HarpoonChain");
            Texture2D harpoon3 = NPC.getTexture();
            Main.EntitySpriteDraw(harpoon3, NPC.Center - Main.screenPosition, null, drawColor, NPC.rotation, new Vector2(70, harpoon3.Height / 2f), NPC.scale, ((AcropolisMachine)owner.ModNPC).dir > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically);
            return false;
        }
    }
}
