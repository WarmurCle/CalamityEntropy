using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityEntropy
{
    public class CEKeybinds : ModSystem
    {
        public static ModKeybind RetrieveVoidAnnihilateHotKey { get; private set; }
        public override void Load()
        {
            //Register keybinds            
            RetrieveVoidAnnihilateHotKey = KeybindLoader.RegisterKeybind(Mod, "RetrieveVoidAnnihilate", "LeftAlt");
        }
        public override void Unload()
        {
            RetrieveVoidAnnihilateHotKey = null;
        }
    }
}
