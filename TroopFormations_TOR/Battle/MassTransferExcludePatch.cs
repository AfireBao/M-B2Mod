using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TroopFormations_TOR;

/// <summary>
/// Exclude preferred agents from vanilla class-weight mass transfer so two Ranged
/// troop types are not reshuffled across preferred slots.
/// </summary>
[HarmonyPatch]
public static class MassTransferExcludePatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(
            AccessTools.TypeByName(
                "TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle.OrderOfBattleUIHelper"),
            "GetExcludedAgentsForTransfer");
    }

    private static void Postfix(List<Agent> __result)
    {
        try
        {
            if (__result == null
                || OrderOfBattlePatch.IsApplying
                || Mission.Current?.PlayerTeam == null
                || !FormationsBehavior.IsSupportedMission())
            {
                return;
            }

            var prefs = Campaign.Current?.PlayerFormationPreferences;
            if (prefs == null || prefs.Count == 0)
            {
                return;
            }

            foreach (Agent agent in Mission.Current.PlayerTeam.TeamAgents)
            {
                if (agent == null
                    || agent.IsHero
                    || agent.Character is not CharacterObject character
                    || !TroopFormationsBehavior.TryGetAssignedFormation(character, out _))
                {
                    continue;
                }

                if (!__result.Contains(agent))
                {
                    __result.Add(agent);
                }
            }
        }
        catch (Exception)
        {
        }
    }
}
