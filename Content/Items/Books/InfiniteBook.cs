
using CalamityEntropy.Content.ArmorPrefixes;
using CalamityEntropy.Content.Projectiles.TwistedTwin;
using CalamityEntropy.Util;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.Pkcs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Content.Items.Books
{
    public class InfiniteBook : EntropyBook
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.useTime = Item.useAnimation = 6;
            Item.damage = 1600;
        }
        public override int HeldProjectileType => ModContent.ProjectileType<InfiniteBookHeld>();
        public override int SlotCount => 24;
    }

    public class InfiniteBookHeld : EntropyBookHeldProjectile
    {
        public override string OpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/AncientScriptures/AncientScripturesOpen";
        public override string PageAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/AncientScriptures/AncientScripturesPage";
        public override string UIOpenAnimationPath => "CalamityEntropy/Content/Items/Books/Textures/AncientScriptures/AncientScripturesUI";
        public override int frameChange => UIOpen ? 4 : 2;
    }
}