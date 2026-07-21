using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TroopFormations_TOR;

/// <summary>
/// Keep Order of Battle available so assigned formations can be applied during deployment.
/// Only force-enable when the player actually has troop formation preferences,
/// so TOR battles without assignments keep their normal deploy path.
/// </summary>
[HarmonyPatch(typeof(BattleInitializationModel), nameof(BattleInitializationModel.CanPlayerSideDeployWithOrderOfBattle))]
public static class BattleInitializationModelPatch
{
    private static void Postfix(ref bool __result)
    {
        if (__result)
        {
            return;
        }

        if (Campaign.Current?.PlayerFormationPreferences is { Count: > 0 })
        {
            __result = true;
        }
    }
}
