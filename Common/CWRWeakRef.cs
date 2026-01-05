using CalamityEntropy.Content.ILEditing;
using CalamityEntropy.Content.Items.Books;
using CalamityOverhaul.Content;
using CalamityOverhaul.Content.LegendWeapon.HalibutLegend;
using InnoVault;
using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityEntropy.Common
{
    internal static partial class CWRWeakRef
    {
        [ExtendsFromMod("CalamityOverhaul")]
        [JITWhenModsEnabled("CalamityOverhaul")]
        internal static class CWRRef
        {
            public static float GetPlayersPressure(Player plr)
            {
                return plr.GetModPlayer<CWRPlayer>().PressureIncrease;
            }
            public static void ActiveFS(int type, Item item, Projectile proj, int style)
            {
                if(FishSkill.IDToInstance.TryGetValue(type, out var value) && proj.ModProjectile is EntropyBookHeldProjectile eb)
                {
                    if(style == 0)
                    {
                        if (value.Cooldown <= 0)
                        {
                            var player = proj.GetOwner();
                            bool set = false;
                            int org = 0;
                            if(player.TryGetOverride<HalibutPlayer>(out var hp))
                            {
                                set = true;
                                org = hp.SeaDomainLayers;
                                hp.SeaDomainLayers = player.GetMyMaxActiveBookMarks(item);
                            }
                            int oafu = player.altFunctionUse;
                            player.altFunctionUse = 2;
                            value.AltFunctionUse(item, proj.GetOwner());
                            value.ShootAlt(item, proj.GetOwner(), (EntitySource_ItemUse_WithAmmo)proj.GetOwner().GetSource_ItemUse_WithPotentialAmmo(item, AmmoID.None), proj.Center, proj.rotation.ToRotationVector2() * proj.GetOwner().HeldItem.shootSpeed, eb.getShootProjectileType(), eb.CauculateProjectileDamage(), item.knockBack);
                            player.altFunctionUse = oafu;
                            value.Use(item, proj.GetOwner());
                            value.Shoot(item, proj.GetOwner(), (EntitySource_ItemUse_WithAmmo)proj.GetOwner().GetSource_ItemUse_WithPotentialAmmo(item, AmmoID.None), proj.Center, proj.rotation.ToRotationVector2() * proj.GetOwner().HeldItem.shootSpeed, eb.getShootProjectileType(), eb.CauculateProjectileDamage(), item.knockBack);
                            if(set)
                            {
                                hp.SeaDomainLayers = org;
                            }
                        }
                    }
                }
            }
            public static void SetupFishSkillBM()
            {
                foreach (var kv in FishSkill.IDToInstance)
                {
                    int type = kv.Value.UnlockFishID;
                    Main.instance.LoadItem(type);
                    void bu(Projectile p, bool o)
                    {
                        if (p.ModProjectile is EntropyBookHeldProjectile eb)
                        {
                            ActiveFS(kv.Key, p.GetOwner().HeldItem, p, 0);
                        }
                    }
                    BookMarkLoader.RegisterBookmarkEffect("FishBM" + type, bookUpdate:bu);
                    BookMarkLoader.RegisterBookmark(type, TextureAssets.Item[type], "FishBM" + type);

                }
            }
            public static void HookFSActive()
            {
                var method = typeof(FishSkill).GetMethod("Active", BindingFlags.Instance | BindingFlags.Public);
                bool hook(Func<FishSkill, Player, bool> orig, FishSkill skill, Player plr)
                {
                    if(BookMarkLoader.GetPlayerHeldEntropyBook(plr, out var eb))
                    {
                        for(int i = 0; i < plr.GetMyMaxActiveBookMarks(eb.bookItem); i++)
                        {
                            if (plr.Entropy().EBookStackItems[i].type == skill.UnlockFishID)
                                return true;
                        }
                    }
                    return orig(skill, plr);
                }
                EModHooks.Add(method, hook);
            }
        }
    }
}
