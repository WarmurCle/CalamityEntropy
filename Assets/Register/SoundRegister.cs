using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityEntropy.Assets.Register
{
    public class SoundRegister
    {
        private static string SetPath => "CalamityEntropy/Assets/Sounds/";
        //下面的字段名最好和资源名一致……
        public static SoundStyle SmashToGroundHeavy => new(SetPath + nameof(SmashToGroundHeavy));
        public static SoundStyle Pipes => new(SetPath + nameof(Pipes));

    }
}
