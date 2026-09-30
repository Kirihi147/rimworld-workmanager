using FluentAssertions;
using NUnit.Framework;

namespace LordKuper.WorkManager.Tests;

/// <summary>
///     Tests for <see cref="WorkTypeAssignmentRule" /> priority override logic and combining.
/// </summary>
[TestFixture]
public class WorkTypeAssignmentRuleTests
{
    /// <summary>
    ///     Tests that Combine preserves main's dedicated and guaranteed priority overrides when defined.
    /// </summary>
    [Test]
    public void Combine_PriorityOverrides_UsesMainWhenDefined()
    {
        var main = new WorkTypeAssignmentRule
        {
            EnsureWorkerAssigned = true,
            DedicatedWorkerSettings = new DedicatedWorkerSettings { AllowDedicated = true, Mode = DedicatedWorkerMode.Constant },
            UseDedicatedPriorityOverride = true,
            DedicatedOverridePriority = 2,
            UseGuaranteedPriorityOverride = true,
            GuaranteedOverridePriority = 1
        };
        var fallback = new WorkTypeAssignmentRule
        {
            EnsureWorkerAssigned = true,
            DedicatedWorkerSettings = new DedicatedWorkerSettings { AllowDedicated = true, Mode = DedicatedWorkerMode.Constant },
            UseDedicatedPriorityOverride = false,
            DedicatedOverridePriority = 4,
            UseGuaranteedPriorityOverride = false,
            GuaranteedOverridePriority = 5
        };

        var combined = WorkTypeAssignmentRule.Combine(main, fallback);

        combined.UseDedicatedPriorityOverride.Should().BeTrue();
        combined.DedicatedOverridePriority.Should().Be(2);
        combined.UseGuaranteedPriorityOverride.Should().BeTrue();
        combined.GuaranteedOverridePriority.Should().Be(1);
    }

    /// <summary>
    ///     Tests that Combine falls back to fallback priority overrides when main has null AllowDedicated / EnsureWorkerAssigned.
    /// </summary>
    [Test]
    public void Combine_PriorityOverrides_FallsBackWhenMainUnspecified()
    {
        var main = new WorkTypeAssignmentRule
        {
            EnsureWorkerAssigned = null,
            DedicatedWorkerSettings = new DedicatedWorkerSettings { AllowDedicated = null, Mode = DedicatedWorkerMode.Constant },
            UseDedicatedPriorityOverride = false,
            DedicatedOverridePriority = 3,
            UseGuaranteedPriorityOverride = false,
            GuaranteedOverridePriority = 3
        };
        var fallback = new WorkTypeAssignmentRule
        {
            EnsureWorkerAssigned = true,
            DedicatedWorkerSettings = new DedicatedWorkerSettings { AllowDedicated = true, Mode = DedicatedWorkerMode.Constant },
            UseDedicatedPriorityOverride = true,
            DedicatedOverridePriority = 2,
            UseGuaranteedPriorityOverride = true,
            GuaranteedOverridePriority = 1
        };

        var combined = WorkTypeAssignmentRule.Combine(main, fallback);

        combined.UseDedicatedPriorityOverride.Should().BeTrue();
        combined.DedicatedOverridePriority.Should().Be(2);
        combined.UseGuaranteedPriorityOverride.Should().BeTrue();
        combined.GuaranteedOverridePriority.Should().Be(1);
    }

    /// <summary>
    ///     Tests that EndlessGrowthPatch finds the target method in LordKuper.Common.
    /// </summary>
    [Test]
    public void EndlessGrowthPatch_TargetMethod_IsFound()
    {
        var patchType = typeof(WorkPriorityUpdater).Assembly.GetType("LordKuper.WorkManager.Patches.EndlessGrowthPatch");
        patchType.Should().NotBeNull();
        var targetMethod = patchType!.GetMethod("TargetMethod", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        targetMethod.Should().NotBeNull();
        var result = targetMethod!.Invoke(null, null);
        result.Should().NotBeNull();
    }

    /// <summary>
    ///     Tests that WorkPriorityUpdater contains the expected assignment methods.
    /// </summary>
    [TestCase("AssignGuaranteedWorkers")]
    [TestCase("AssignDedicatedWorkers")]
    [TestCase("AssignDedicatedWorkersForDay")]
    [TestCase("TryAssignDedicatedWorkersBySchedule")]
    [TestCase("UpdateWorkPriorities")]
    public void WorkPriorityUpdater_Methods_Exist(string methodName)
    {
        var method = typeof(WorkPriorityUpdater).GetMethod(methodName,
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method.Should().NotBeNull($"WorkPriorityUpdater should have method {methodName}");
    }

    /// <summary>
    ///     Tests that setting Mode on DedicatedWorkerSettings resets ConstantWorkerCount when switching away from Constant.
    /// </summary>
    [Test]
    public void DedicatedWorkerSettings_ModeChange_ResetsIrrelevantSettings()
    {
        var settings = new DedicatedWorkerSettings
        {
            Mode = DedicatedWorkerMode.Constant,
            ConstantWorkerCount = 5
        };
        settings.ConstantWorkerCount.Should().Be(5);

        settings.Mode = DedicatedWorkerMode.CapablePawnRatio;
        settings.ConstantWorkerCount.Should().Be(1); // Reset to default
    }
}
