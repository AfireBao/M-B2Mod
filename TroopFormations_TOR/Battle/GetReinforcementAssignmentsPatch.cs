using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TroopFormations_TOR;

[HarmonyPatch(typeof(MissionReinforcementsHelper), nameof(MissionReinforcementsHelper.GetReinforcementAssignments))]
public static class GetReinforcementAssignmentsPatch
{
    private static void Postfix(ref List<(IAgentOriginBase, int)> __result)
    {
        if (__result == null || __result.Count == 0)
        {
            return;
        }

        var rewritten = new List<(IAgentOriginBase, int)>(__result.Count);
        foreach ((IAgentOriginBase origin, int formationIndex) in __result)
        {
            int index = formationIndex;
            if (origin?.Troop is CharacterObject character
                && TroopFormationsBehavior.TryGetAssignedFormation(character, out FormationClass assigned))
            {
                index = (int)assigned;
            }

            rewritten.Add((origin!, index));
        }

        __result = rewritten;
    }
}
