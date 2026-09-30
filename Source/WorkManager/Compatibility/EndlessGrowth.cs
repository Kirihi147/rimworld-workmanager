using System;
using System.Linq;
using Verse;

namespace LordKuper.WorkManager.Compatibility;

/// <summary>
///     Provides compatibility integration with the EndlessGrowth mod.
/// </summary>
internal static class EndlessGrowth
{
    /// <summary>
    ///     Gets a value indicating whether the EndlessGrowth mod is active.
    /// </summary>
    internal static bool Active =>
        ModsConfig.IsActive("slimesenpai.endlessgrowth") ||
        (LoadedModManager.RunningModsListForReading != null &&
         LoadedModManager.RunningModsListForReading.Any(m =>
             "slimesenpai.endlessgrowth".Equals(m.PackageId, StringComparison.OrdinalIgnoreCase)));
}
