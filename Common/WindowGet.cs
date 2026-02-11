using CalamityEntropy.Content.Items.Donator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    //我脑袋抽了做的这玩意
    public class ChargingYuzu : ModItem
    {
        public override string Texture => "CalamityEntropy/Assets/Extra/ChargingYuzu";
        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = Item.height = 32;
            Item.rare = ItemRarityID.Yellow;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<PGetPlayer>().accEquiped = true;
            player.GetModPlayer<PGetPlayer>().accVnTime = 3;
            player.endurance += 0.05f * player.GetModPlayer<PGetPlayer>().count;
            player.GetDamage(DamageClass.Generic) += player.GetModPlayer<PGetPlayer>().count * 0.1f;
        }
        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<PGetPlayer>().accVnTime = 3;
        }
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Replace("[A]", Main.LocalPlayer.GetModPlayer<PGetPlayer>().count);
        }
    }
    public class PGetPlayer : ModPlayer
    {
        public static List<string> yuzuGames = new() { "SenrenBanka", "RiddleJoker", "SabbatOfTheWitch", "CafeStella", "tenshi", "Angelic", "DracuRiot", "PARQUET", "NobleWorks" };
        public int count = 0;
        public bool accEquiped = false;
        public bool accVanity = false;
        public int accVnTime = 0;
        public override void ResetEffects()
        {
            accVnTime--;
            accEquiped = false;
            accVanity = accVnTime > 0;
        }
        public override void OnEnterWorld()
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                int g = RunningGames();
                count = g;
                if (!Player.Entropy().yuzuCheck)
                {
                    Player.Entropy().yuzuCheck = true;
                    if (g > 0)
                    {
                        Player.QuickSpawnItem(Player.GetSource_FromThis(), ModContent.ItemType<ChargingYuzu>());
                    }
                }
            }
        }
        public static int RunningGames()
        {
            int s = 0;
            var windows = WindowGet.GetAllVisibleWindows();
            foreach (var win in windows)
            {
                bool flag = false;
                foreach (string name in yuzuGames)
                {
                    if (win.ProcessName.ToLower().Contains(name.ToLower()))
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    s++;
            }
            return s;
        }
        public override void PostUpdate()
        {
            if (Main.LocalPlayer != null && !Main.gameMenu)
                TextureAssets.Item[ModContent.ItemType<TlipocasScythe>()] = TlipocasScythe.GetTexture(Main.LocalPlayer);
            if (Main.GameUpdateCount % 120 == 0)
            {
                if (accEquiped || accVanity)
                {
                    ThreadPool.QueueUserWorkItem(_ =>
                    {
                        count = RunningGames();
                    });
                }
            }
        }
        public static class WindowGet
        {
            [DllImport("user32.dll")]
            private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

            [DllImport("user32.dll")]
            private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

            [DllImport("user32.dll")]
            private static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll")]
            private static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll")]
            private static extern IntPtr GetShellWindow();

            [DllImport("user32.dll")]
            private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

            private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

            public static List<WindowInfo> GetAllVisibleWindows()
            {
                if (!OperatingSystem.IsWindows())
                {
                    return new List<WindowInfo>();
                }
                List<WindowInfo> windows = new List<WindowInfo>();
                IntPtr shellWindow = GetShellWindow();

                EnumWindows((IntPtr hWnd, IntPtr lParam) =>
                {
                    // 跳过不可见窗口和Shell窗口
                    if (hWnd == shellWindow) return true;
                    if (!IsWindowVisible(hWnd)) return true;

                    int length = GetWindowTextLength(hWnd);
                    if (length == 0) return true; // 跳过无标题窗口

                    // 获取窗口标题
                    StringBuilder builder = new StringBuilder(length + 1);
                    GetWindowText(hWnd, builder, builder.Capacity);

                    string title = builder.ToString();
                    if (string.IsNullOrEmpty(title)) return true;

                    // 获取进程ID
                    GetWindowThreadProcessId(hWnd, out uint processId);

                    // 获取进程信息
                    try
                    {
                        Process process = Process.GetProcessById((int)processId);
                        windows.Add(new WindowInfo
                        {
                            Handle = hWnd,
                            Title = title,
                            ProcessName = process.ProcessName,
                            ProcessId = process.Id,
                            FilePath = process.MainModule?.FileName ?? "Unknown"
                        });
                    }
                    catch
                    {
                        // 无法获取进程信息
                        windows.Add(new WindowInfo
                        {
                            Handle = hWnd,
                            Title = title,
                            ProcessName = "Unknown",
                            ProcessId = (int)processId,
                            FilePath = "Unknown"
                        });
                    }

                    return true;
                }, IntPtr.Zero);

                return windows;
            }

            /// <summary>
            /// 获取前台活动窗口
            /// </summary>
            [DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();

            public static WindowInfo GetActiveWindow()
            {
                IntPtr hWnd = GetForegroundWindow();
                if (hWnd == IntPtr.Zero) return null;

                int length = GetWindowTextLength(hWnd);
                if (length == 0) return null;

                StringBuilder builder = new StringBuilder(length + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                string title = builder.ToString();

                GetWindowThreadProcessId(hWnd, out uint processId);

                try
                {
                    Process process = Process.GetProcessById((int)processId);
                    return new WindowInfo
                    {
                        Handle = hWnd,
                        Title = title,
                        ProcessName = process.ProcessName,
                        ProcessId = process.Id,
                        FilePath = process.MainModule?.FileName ?? "Unknown"
                    };
                }
                catch
                {
                    return new WindowInfo
                    {
                        Handle = hWnd,
                        Title = title,
                        ProcessName = "Unknown",
                        ProcessId = (int)processId,
                        FilePath = "Unknown"
                    };
                }
            }
        }

        public class WindowInfo
        {
            public IntPtr Handle { get; set; }
            public string Title { get; set; }
            public string ProcessName { get; set; }
            public int ProcessId { get; set; }
            public string FilePath { get; set; }

            public override string ToString()
            {
                return $"{Title} ({ProcessName}) [PID: {ProcessId}]";
            }
        }
    }
}
