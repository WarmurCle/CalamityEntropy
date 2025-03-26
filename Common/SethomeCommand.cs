using CalamityEntropy.Util;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    public class SethomeCommand : ModCommand
    {
        public override CommandType Type
            => CommandType.World;
        public override string Command => "sethome";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!(ModContent.GetInstance<ServerConfig>().EnableSethomeCommand))
            {
                Main.NewText("This command is not enabled!", Color.Red);
                return;
            }
            if (args.Length == 1)
            {
                (Main.LocalPlayer.Entropy().homes as Dictionary<string, Vector2>)[args[0]] = Main.LocalPlayer.Center;
            }
            else if (args.Length == 0)
            {
                (Main.LocalPlayer.Entropy().homes as Dictionary<string, Vector2>)["_default"] = Main.LocalPlayer.Center;
            }
            else
            {
                Main.NewText(Mod.GetLocalization("SetHomeCommandError").Value, Color.Red);
            }
        }

    }
    public class HomeCommand : ModCommand
    {
        public override CommandType Type
            => CommandType.World;
        public override string Command => "home";
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (!(ModContent.GetInstance<ServerConfig>().EnableSethomeCommand))
            {
                Main.NewText("This command is not enabled!", Color.Red);
                return;
            }
            string name = "_default";
            if (args.Length == 1)
            {
                name = args[0];
            }
            if ((Main.LocalPlayer.Entropy().homes as Dictionary<string, Vector2>).ContainsKey(name))
            {
                Main.LocalPlayer.Center = (Main.LocalPlayer.Entropy().homes as Dictionary<string, Vector2>)[name];
                Main.LocalPlayer.velocity *= 0;
            }
            else
            {
                Main.NewText(Mod.GetLocalization("HomeCommandError").Value + ": " + name, Color.Red);
            }
        }

    }
}
