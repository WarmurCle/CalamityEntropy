using Terraria.ModLoader;

namespace CalamityEntropy.Common
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
