using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace TroopFormations_TOR;

[HarmonyPatch(typeof(OrderOfBattleVM))]
public static class OrderOfBattlePatch
{
    public static OrderOfBattleVM? Instance { get; private set; }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OrderOfBattleVM.Initialize))]
    private static void InitializePostfix(OrderOfBattleVM __instance)
    {
        Instance = __instance;
        ApplyAssignments();
    }

    [HarmonyPostfix]
    [HarmonyPatch("Tick")]
    private static void TickPostfix(OrderOfBattleVM __instance)
    {
        // Only re-assert while deployment UI is active; avoid fighting TOR AI mid-battle every tick.
        if (!__instance.IsEnabled)
        {
            return;
        }

        ApplyAssignments();
    }

    internal static void ApplyAssignments()
    {
        Mission? mission = Mission.Current;
        if (mission?.PlayerTeam == null || !FormationsBehavior.IsSupportedMission())
        {
            return;
        }

        foreach (Agent agent in mission.PlayerTeam.TeamAgents)
        {
            FormationsBehavior.TryAssignAgent(agent);
        }

        if (Instance == null)
        {
            return;
        }

        var allFormations = Traverse.Create(Instance).Field("_allFormations").GetValue<List<OrderOfBattleFormationItemVM>>();
        if (allFormations == null)
        {
            return;
        }

        foreach (Agent agent in mission.PlayerTeam.TeamAgents)
        {
            if (agent.Character is not CharacterObject character
                || !TroopFormationsBehavior.TryGetAssignedFormation(character, out FormationClass formationClass))
            {
                continue;
            }

            int index = (int)formationClass;
            if (index < 0 || index >= allFormations.Count)
            {
                continue;
            }

            Formation target = allFormations[index].Formation;
            if (agent.Formation != target)
            {
                agent.Formation = target;
            }
        }
    }
}
