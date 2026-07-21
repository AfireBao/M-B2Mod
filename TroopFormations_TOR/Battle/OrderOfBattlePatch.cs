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

    internal static bool IsApplying { get; private set; }

    private static int _freeSlotCursor;

    public static void ClearSessionState()
    {
        Instance = null;
        IsApplying = false;
        _freeSlotCursor = 0;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(OrderOfBattleVM.Initialize))]
    private static void InitializePostfix(OrderOfBattleVM __instance)
    {
        Instance = __instance;
        // After vanilla DistributeAllTroops — seat prefs then evict unassigned.
        ApplyAssignments(moveAgents: true, heavyUiRefresh: true);
    }

    [HarmonyPostfix]
    [HarmonyPatch("DistributeAllTroops")]
    private static void DistributeAllTroopsPostfix()
    {
        if (IsApplying)
        {
            return;
        }

        ApplyAssignments(moveAgents: true, heavyUiRefresh: false);
    }

    [HarmonyPostfix]
    [HarmonyPatch("DistributeTroops")]
    private static void DistributeTroopsPostfix()
    {
        if (IsApplying)
        {
            return;
        }

        ApplyAssignments(moveAgents: true, heavyUiRefresh: false);
    }

    // Tick re-assert removed: continuous Formation moves during OOB corrupted native state
    // and caused Access Violation when loading a save after retreat.

    internal static void ApplyAssignments(bool moveAgents, bool heavyUiRefresh)
    {
        if (IsApplying)
        {
            return;
        }

        Mission? mission = Mission.Current;
        if (mission?.PlayerTeam == null
            || !FormationsBehavior.IsSupportedMission()
            || mission.Mode is not (MissionMode.Deployment or MissionMode.Battle))
        {
            return;
        }

        var prefs = Campaign.Current?.PlayerFormationPreferences;
        if (prefs == null || prefs.Count == 0)
        {
            return;
        }

        IsApplying = true;
        try
        {
            List<OrderOfBattleFormationItemVM>? allFormations = null;
            if (Instance != null)
            {
                allFormations = Traverse.Create(Instance)
                    .Field("_allFormations")
                    .GetValue<List<OrderOfBattleFormationItemVM>>();
            }

            HashSet<int> preferredSlots = CollectPreferredSlots();
            var roleVotes = new Dictionary<int, Dictionary<FormationClass, int>>();

            foreach (Agent agent in mission.PlayerTeam.TeamAgents)
            {
                if (agent.Character is not CharacterObject character
                    || agent.IsHero
                    || !TroopFormationsBehavior.TryGetAssignedFormation(character, out FormationClass slot))
                {
                    continue;
                }

                int index = (int)slot;
                if (index < 0 || index >= TroopFormationsBehavior.FormationCount)
                {
                    continue;
                }

                FormationClass role = FormationsBehavior.GetTroopRoleClass(character);
                if (!roleVotes.TryGetValue(index, out Dictionary<FormationClass, int>? votes))
                {
                    votes = new Dictionary<FormationClass, int>();
                    roleVotes[index] = votes;
                }

                votes.TryGetValue(role, out int count);
                votes[role] = count + 1;
            }

            foreach (KeyValuePair<int, Dictionary<FormationClass, int>> slotVotes in roleVotes)
            {
                int index = slotVotes.Key;
                Dictionary<FormationClass, int> votes = slotVotes.Value;

                FormationClass winningRole = FormationClass.Infantry;
                int best = -1;
                foreach (KeyValuePair<FormationClass, int> vote in votes)
                {
                    if (vote.Value > best)
                    {
                        best = vote.Value;
                        winningRole = vote.Key;
                    }
                }

                Formation? formation = FindFormation(mission.PlayerTeam, index);
                if (formation == null)
                {
                    continue;
                }

                OrderOfBattleFormationItemVM? item =
                    allFormations != null && index < allFormations.Count ? allFormations[index] : null;

                FormationsBehavior.ApplyFormationClassToSlot(item, formation, winningRole);
            }

            if (moveAgents)
            {
                SeatPreferredThenEvictUnassigned(mission.PlayerTeam, preferredSlots, allFormations);
            }

            if (heavyUiRefresh)
            {
                FormationsBehavior.RefreshOrderOfBattleUi(invokeTick: false);
            }
        }
        finally
        {
            IsApplying = false;
        }
    }

    private static void SeatPreferredThenEvictUnassigned(
        Team team,
        HashSet<int> preferredSlots,
        List<OrderOfBattleFormationItemVM>? allFormations)
    {
        foreach (Agent agent in team.TeamAgents)
        {
            FormationsBehavior.TryAssignAgent(agent, refreshUi: false, switchFormationType: false);
        }

        EvictUnassignedFromPreferredSlots(team, preferredSlots, allFormations);
    }

    private static void EvictUnassignedFromPreferredSlots(
        Team team,
        HashSet<int> preferredSlots,
        List<OrderOfBattleFormationItemVM>? allFormations)
    {
        if (preferredSlots.Count == 0)
        {
            return;
        }

        var freeSlots = new List<int>(TroopFormationsBehavior.FormationCount);
        for (int i = 0; i < TroopFormationsBehavior.FormationCount; i++)
        {
            if (!preferredSlots.Contains(i))
            {
                freeSlots.Add(i);
            }
        }

        if (freeSlots.Count == 0)
        {
            return;
        }

        var toEvict = new List<Agent>();
        foreach (Agent agent in team.TeamAgents)
        {
            if (agent == null
                || !agent.IsHuman
                || agent.IsHero
                || !agent.IsActive()
                || agent.Formation == null)
            {
                continue;
            }

            int current = agent.Formation.Index;
            if (!preferredSlots.Contains(current))
            {
                continue;
            }

            if (agent.Character is CharacterObject character
                && TroopFormationsBehavior.TryGetAssignedFormation(character, out FormationClass assigned)
                && (int)assigned == current)
            {
                continue;
            }

            if (agent.Character is CharacterObject c
                && TroopFormationsBehavior.TryGetAssignedFormation(c, out _))
            {
                continue;
            }

            toEvict.Add(agent);
        }

        if (toEvict.Count == 0)
        {
            return;
        }

        var freeRoleVotes = new Dictionary<int, Dictionary<FormationClass, int>>();

        foreach (Agent agent in toEvict)
        {
            int targetIndex = PickFreeSlot(team, freeSlots, agent);
            FormationsBehavior.TryMoveAgentToSlot(agent, targetIndex, switchFormationType: false);

            if (agent.Character is not CharacterObject character)
            {
                continue;
            }

            FormationClass role = FormationsBehavior.GetTroopRoleClass(character);
            if (!freeRoleVotes.TryGetValue(targetIndex, out Dictionary<FormationClass, int>? votes))
            {
                votes = new Dictionary<FormationClass, int>();
                freeRoleVotes[targetIndex] = votes;
            }

            votes.TryGetValue(role, out int count);
            votes[role] = count + 1;
        }

        foreach (KeyValuePair<int, Dictionary<FormationClass, int>> slotVotes in freeRoleVotes)
        {
            FormationClass winningRole = FormationClass.Infantry;
            int best = -1;
            foreach (KeyValuePair<FormationClass, int> vote in slotVotes.Value)
            {
                if (vote.Value > best)
                {
                    best = vote.Value;
                    winningRole = vote.Key;
                }
            }

            Formation? formation = FindFormation(team, slotVotes.Key);
            if (formation == null)
            {
                continue;
            }

            OrderOfBattleFormationItemVM? item =
                allFormations != null && slotVotes.Key < allFormations.Count
                    ? allFormations[slotVotes.Key]
                    : null;

            FormationsBehavior.ApplyFormationClassToSlot(item, formation, winningRole);
        }
    }

    private static int PickFreeSlot(Team team, List<int> freeSlots, Agent agent)
    {
        FormationClass role = FormationClass.Infantry;
        if (agent.Character is CharacterObject character)
        {
            role = FormationsBehavior.GetTroopRoleClass(character);
        }

        int bestMatch = -1;
        int bestEmpty = -1;
        foreach (int index in freeSlots)
        {
            Formation? f = FindFormation(team, index);
            if (f == null)
            {
                continue;
            }

            bool empty = f.CountOfUnits == 0;
            bool roleMatch = f.PhysicalClass == role;
            if (empty && roleMatch)
            {
                return index;
            }

            if (empty && bestEmpty < 0)
            {
                bestEmpty = index;
            }

            if (roleMatch && bestMatch < 0)
            {
                bestMatch = index;
            }
        }

        if (bestEmpty >= 0)
        {
            return bestEmpty;
        }

        if (bestMatch >= 0)
        {
            return bestMatch;
        }

        int pick = freeSlots[_freeSlotCursor % freeSlots.Count];
        _freeSlotCursor++;
        return pick;
    }

    private static HashSet<int> CollectPreferredSlots()
    {
        var slots = new HashSet<int>();
        var prefs = Campaign.Current?.PlayerFormationPreferences;
        if (prefs == null)
        {
            return slots;
        }

        foreach (KeyValuePair<CharacterObject, FormationClass> kv in prefs)
        {
            int index = (int)kv.Value;
            if (index >= 0 && index < TroopFormationsBehavior.FormationCount)
            {
                slots.Add(index);
            }
        }

        return slots;
    }

    private static Formation? FindFormation(Team team, int index)
    {
        foreach (Formation f in team.FormationsIncludingEmpty)
        {
            if (f.Index == index)
            {
                return f;
            }
        }

        return null;
    }
}
