using HarmonyLib;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TroopFormations_TOR;

/// <summary>
/// Keep Order of Battle available so assigned formations can be applied during deployment.
/// </summary>
[HarmonyPatch(typeof(BattleInitializationModel), nameof(BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle))]
public static class BattleInitializationModelPatch
{
    private static void Postfix(ref bool __result)
    {
        __result = true;
    }
}
