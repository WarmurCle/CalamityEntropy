using CalamityEntropy.Content.Items.Accessories;
using System.IO;
using System;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class CEKeybinds : ModSystem
    {
        public static ModKeybind RetrieveVoidAnnihilateHotKey { get; private set; }
        public static ModKeybind ThrowPoopHotKey { get; set; }
        public static ModKeybind PoopHoldHotKey { get; set; }
        public override void Load()
        {
            //Register keybinds            
            RetrieveVoidAnnihilateHotKey = KeybindLoader.RegisterKeybind(Mod, "RetrieveVoidAnnihilate", "J");
            string MyGameFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games");
            string Isaac1 = Path.Combine(MyGameFolder, "Binding of Isaac Repentance").Replace("/", "\\");
            string Isaac2 = Path.Combine(MyGameFolder, "Binding of Isaac Repentance+").Replace("/", "\\");
            bool isaac = Directory.Exists(Isaac1) || Directory.Exists(Isaac2);
            if (isaac)
            {
                CEKeybinds.ThrowPoopHotKey = KeybindLoader.RegisterKeybind(Mod, "ThrowPoop", "LeftAlt");
                CEKeybinds.PoopHoldHotKey = KeybindLoader.RegisterKeybind(Mod, "KeepPoop", "Q");
            }
        }
        public override void Unload()
        {
            RetrieveVoidAnnihilateHotKey = null;
            ThrowPoopHotKey = null;
            PoopHoldHotKey = null;
        }
    }
}
