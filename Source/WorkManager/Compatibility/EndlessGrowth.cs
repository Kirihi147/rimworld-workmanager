using System;
using System.Linq;
using HarmonyLib;
using LordKuper.WorkManager.Patches;
using Verse;

namespace LordKuper.WorkManager.Compatibility;

/// <summary>
///     Provides compatibility integration with the EndlessGrowth mod.
/// </summary>
internal static class EndlessGrowth
{
    private static bool _isInitialized;

    /// <summary>
    ///     Gets a value indicating whether the EndlessGrowth mod is active.
    /// </summary>
    internal static bool Active =>
        ModsConfig.IsActive("slimesenpai.endlessgrowth") ||
        (LoadedModManager.RunningModsListForReading != null &&
         LoadedModManager.RunningModsListForReading.Any(m =>
             "slimesenpai.endlessgrowth".Equals(m.PackageId, StringComparison.OrdinalIgnoreCase)));

    /// <summary>
    ///     Initializes EndlessGrowth compatibility by applying UI patches after all game data and languages have finished loading.
    /// </summary>
    /// <param name="harmony">The Harmony instance used for patching.</param>
    internal static void Initialize(Harmony harmony)
    {
        if (_isInitialized) return;
        _isInitialized = true;

        if (!Active) return;

        LongEventHandler.ExecuteWhenFinished(() =>
        {
            try
            {
                EndlessGrowthPatch.Apply(harmony);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to apply EndlessGrowth patch.", ex);
            }
        });
    }
}

