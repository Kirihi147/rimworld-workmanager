using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JetBrains.Annotations;
using LordKuper.WorkManager.Compatibility;

namespace LordKuper.WorkManager.Patches;

/// <summary>
///     Harmony patch to expand the maximum skill level in the pawn filter UI when EndlessGrowth is active.
/// </summary>
[HarmonyPatch]
[UsedImplicitly]
internal static class EndlessGrowthPatch
{
    /// <summary>
    ///     Returns the target method to patch.
    /// </summary>
    [HarmonyTargetMethod]
    [UsedImplicitly]
    private static MethodBase? TargetMethod()
    {
        var targetType = AccessTools.TypeByName("LordKuper.Common.UI.Widgets.PawnFilterWidget");
        return targetType != null ? AccessTools.Method(targetType, "DoPawnSkillsSection") : null;
    }

    /// <summary>
    ///     Transpiles the skill slider max value instruction in the UI widget.
    /// </summary>
    [HarmonyTranspiler]
    [UsedImplicitly]
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        foreach (var instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Ldc_I4_S &&
                instruction.operand is sbyte value &&
                value == 20)
            {
                yield return new CodeInstruction(
                    OpCodes.Call,
                    AccessTools.Method(
                        typeof(EndlessGrowthPatch),
                        nameof(GetMaxSkillLevel)
                    )
                );
                continue;
            }

            yield return instruction;
        }
    }

    /// <summary>
    ///     Returns the maximum skill level based on EndlessGrowth active state.
    /// </summary>
    private static int GetMaxSkillLevel()
    {
        return EndlessGrowth.Active ? 100 : 20;
    }
}
