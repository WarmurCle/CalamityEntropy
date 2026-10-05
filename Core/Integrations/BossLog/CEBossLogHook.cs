using CalamityEntropy.Common;
using CalamityEntropy.Content.ILEditing;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityEntropy.Core.Integrations.BossLog
{
    /// <summary>
    /// 钩 BossLogUI.Draw 后置书缘,书外不画;LogPanel.Draw 里书封和 PageOne 跳过 orig,PageTwo 放行
    /// 反射缺失、挂钩失败、绘制抛错,或 BossLogTakeover 关掉,退回 customPortrait
    /// </summary>
    internal sealed class CEBossLogHook : ICELoader
    {
        private delegate void OrigDraw(object self, SpriteBatch sb);
        private delegate void DrawDetour(OrigDraw orig, object self, SpriteBatch sb);

        /// <summary>钩子在位且未因异常停用</summary>
        public static bool Armed { get; private set; }

        /// <summary>翻到本模组页后皮的入场演出时长(秒)</summary>
        private const float SettleTime = 0.45f;

        //==================== 帧态 ====================

        /// <summary>当前选中的本模组条目(null = 原版页 / 未开书)</summary>
        private static CEBossLogEntry active;
        /// <summary>当前页 EntryInfo 反射对象</summary>
        private static object activeEntry;
        private static float settle;
        private static float time;
        private static long lastStamp;
        private static Rectangle bookRect;
        private static Rectangle leftRect;
        private static Rectangle rightRect;
        /// <summary>上游页字段缺失:页矩形改由书矩形按版式常量推算(书封钩子里拿到真书矩形后再算)</summary>
        private static bool pagesDerived;

        void ICELoader.SetupData() {
            if (Main.dedServ || !ModLoader.TryGetMod("BossChecklist", out Mod checklist)) {
                return;
            }
            if (!CEBossLogReflect.Resolve(checklist, out string missing)) {
                CalamityEntropy.Instance.Logger.Warn($"CEBossLogHook: BossChecklist 反射面缺失({missing}),整本书接管停用,图鉴走 customPortrait 回退");
                return;
            }
            try {
                Attach(CEBossLogReflect.PanelDraw, new DrawDetour(OnPanelDraw));
                Attach(CEBossLogReflect.LogDraw, new DrawDetour(OnLogDraw));
                Armed = true;
                CalamityEntropy.Instance.Logger.Info("CEBossLogHook: 图鉴整本书接管已就位(LogPanel.Draw / BossLogUI.Draw)");
            } catch (Exception e) {
                Armed = false;
                CalamityEntropy.Instance.Logger.Warn($"CEBossLogHook: 挂钩失败,整本书接管停用: {e.Message}");
            }
        }

        /// <summary>钩子由 EModHooks.UnLoadData 统一撤,这里只清状态</summary>
        void ICELoader.UnLoadData() {
            Armed = false;
            CEBossLogReflect.Clear();
            CEBossLogRegistry.Clear();
            active = null;
            activeEntry = null;
            settle = time = 0f;
            lastStamp = 0;
        }

        private static void Attach(MethodInfo method, Delegate detour) {
            if (EModHooks.Add(method, detour) == null) {
                throw new InvalidOperationException($"无法挂钩 {method?.DeclaringType?.Name}.{method?.Name}");
            }
        }

        /// <summary>记一次日志并停用,本帧余下交回 orig</summary>
        private static void Disarm(Exception e) {
            Armed = false;
            active = null;
            CalamityEntropy.Instance.Logger.Warn($"CEBossLogHook: 绘制异常,整本书接管停用: {e}");
        }

        //==================== BossLogUI.Draw ====================

        private static void OnLogDraw(OrigDraw orig, object self, SpriteBatch sb) {
            if (!Armed) {
                orig(self, sb);
                return;
            }
            try {
                BeginFrame();
            } catch (Exception e) {
                Disarm(e);
                orig(self, sb);
                return;
            }

            orig(self, sb);

            if (Armed && active != null) {
                try {
                    Rectangle spine = CEBossLogSkin.SpineOf(bookRect, leftRect, rightRect);
                    Rectangle bottom = CEBossLogSkin.BottomMarginOf(bookRect, leftRect, rightRect);
                    active.Actor.Theme.DrawOrnament(sb, bookRect, spine, bottom, time, CEBossLogSkin.Ease(settle));
                } catch (Exception e) {
                    Disarm(e);
                }
            }
        }

        private static void BeginFrame() {
            object logUi = CEBossLogReflect.LogUI();
            bool visible = CEBossLogReflect.LogVisible(logUi);
            bool enabled = Config.Instance == null || Config.Instance.BossLogTakeover;

            CEBossLogEntry now = null;
            object entry = null;
            if (enabled && visible && CEBossLogReflect.PageNum(logUi) >= 0) {
                entry = CEBossLogReflect.CurrentEntry(logUi);
                if (CEBossLogRegistry.TryGet(CEBossLogReflect.EntryKey(entry), out CEBossLogEntry found)) {
                    now = found;
                }
            }

            //游戏暂停时计时也走墙钟,合书后再开时把单帧增量限在 0.1 秒,避免跳变
            long stamp = Stopwatch.GetTimestamp();
            float dt = lastStamp == 0 ? 1f / 60f
                : MathHelper.Clamp((float)((stamp - lastStamp) / (double)Stopwatch.Frequency), 0f, 0.1f);
            lastStamp = stamp;
            time += dt;

            if (now != null) {
                if (now != active) {
                    settle = 0f;
                }
                settle = MathF.Min(1f, settle + dt / SettleTime);
            }
            active = now;
            activeEntry = now != null ? entry : null;

            if (now == null) {
                return;
            }
            //页矩形优先用上游字段,缺了就按版式常量从书矩形推算
            UIElement book = CEBossLogReflect.BookArea(logUi);
            if (book != null) {
                bookRect = book.GetInnerDimensions().ToRectangle();
            }
            UIElement left = CEBossLogReflect.LeftPage(logUi);
            UIElement right = CEBossLogReflect.RightPage(logUi);
            pagesDerived = left == null || right == null;
            if (!pagesDerived) {
                leftRect = left.GetInnerDimensions().ToRectangle();
                rightRect = right.GetInnerDimensions().ToRectangle();
            }
            else {
                leftRect = CEBossLogSkin.LeftPageOf(bookRect);
                rightRect = CEBossLogSkin.RightPageOf(bookRect);
            }
        }

        //==================== LogPanel.Draw ====================

        private static void OnPanelDraw(OrigDraw orig, object self, SpriteBatch sb) {
            if (!Armed || active == null || self is not UIElement panel) {
                orig(self, sb);
                return;
            }
            string id;
            try {
                id = CEBossLogReflect.PanelId(panel);
            } catch (Exception e) {
                Disarm(e);
                orig(self, sb);
                return;
            }

            try {
                switch (id) {
                    case "":
                        DrawBook(panel, sb);
                        return;
                    case "PageOne":
                        DrawPageOne(panel, sb);
                        return;
                    default:
                        orig(self, sb);
                        return;
                }
            } catch (Exception e) {
                Disarm(e);
                orig(self, sb);
            }
        }

        /// <summary>跳过上游书皮,页面坐标不动</summary>
        private static void DrawBook(UIElement panel, SpriteBatch sb) {
            HideMouseOver(panel);
            Rectangle book = panel.GetInnerDimensions().ToRectangle();
            bookRect = book;
            if (pagesDerived) {
                leftRect = CEBossLogSkin.LeftPageOf(book);
                rightRect = CEBossLogSkin.RightPageOf(book);
            }
            CEBossLogSkin.DrawBook(sb, active.Actor.Theme, book, leftRect, rightRect, time, settle);
        }

        /// <summary>
        /// 场景、子元素、标题块按这个顺序画,上游是子元素先画,不透明场景会盖住按钮
        /// </summary>
        private static void DrawPageOne(UIElement panel, SpriteBatch sb) {
            HideMouseOver(panel);
            CEBossLogEntry entry = active;
            CEBossLogTheme theme = entry.Actor.Theme;
            Rectangle page = panel.GetInnerDimensions().ToRectangle();
            Color mask = CEBossLogReflect.MaskBoss(activeEntry);

            Rectangle canvas = CEBossLogSkin.SceneCanvas(page, theme);
            CEBossPortraitStage.DrawScene(sb, canvas, mask, entry.Actor);
            CEBossLogSkin.DrawSceneFrame(sb, theme, canvas, time, settle);

            CEBossLogReflect.DrawChildren(panel, sb);

            string name = CEBossLogReflect.EntryDisplayName(activeEntry) ?? entry.FallbackName;
            IReadOnlyList<Asset<Texture2D>> heads = CEBossLogReflect.EntryHeads(activeEntry) ?? entry.FallbackHeads();
            bool downed = CEBossLogReflect.EntryDowned(activeEntry) ?? entry.Downed();
            Rectangle hover = CEBossLogSkin.DrawTitleBlock(sb, theme, page, name, CalamityEntropy.Instance.DisplayNameClean,
                heads, downed, mask, settle);

            //沿用上游 Log.EntryPage 文案键,悬停层在 BossChecklist 里画
            if (hover != Rectangle.Empty && hover.Contains(Main.MouseScreen.ToPoint())) {
                bool marked = CEBossLogReflect.EntryMarked(activeEntry);
                CEBossLogReflect.SetHoverText(downed ? "Log.EntryPage.Defeated" : "Log.EntryPage.Undefeated",
                    [Main.worldName, marked ? "*" : ""], downed ? Colors.RarityGreen : Colors.RarityRed);
            }
        }

        /// <summary>镜像上游 LogUIElement.Draw,悬停在书上时挡住世界交互和物块提示</summary>
        private static void HideMouseOver(UIElement element) {
            if (!element.ContainsPoint(Main.MouseScreen) || PlayerInput.IgnoreMouseInterface) {
                return;
            }
            Main.LocalPlayer.mouseInterface = true;
            Main.mouseText = true;
            Main.LocalPlayer.cursorItemIconEnabled = false;
            Main.LocalPlayer.cursorItemIconID = -1;
            Main.ItemIconCacheUpdate(0);
        }
    }
}
