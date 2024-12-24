using System;
using System.Collections.Generic;
using CalamityEntropy.Content.Items;
using CalamityEntropy.Content.Items.Weapons;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Util;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Projectiles
{
    public class PoopRainbowProjectile : PoopProj
    {
        public override void OnKill(int timeLeft)
        {
            foreach(Player player in Main.ActivePlayers)
            {
                player.Heal(160);
            }
            if (!Main.dedServ)
            {
                Util.Util.PlaySound("happy rainbow with giggle", 1);
            }
        }
        public override int dustType => DustID.RainbowTorch;
    }

}