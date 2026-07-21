using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace TroopFormations_TOR;

public class FormationsBehavior : MissionView
{
    public override void OnAgentCreated(Agent agent)
    {
        base.OnAgentCreated(agent);
        TryAssignAgent(agent, refreshUi: false, switchFormationType: false);
    }

    public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
    {
        base.OnMissionModeChange(oldMissionMode, atStart);
        // Leaving deployment/battle: drop OOB VM so load/campaign teardown cannot touch it.
        if (Mission.Current != null
            && Mission.Current.Mode != MissionMode.Deployment
            && Mission.Current.Mode != MissionMode.Battle)
        {
            OrderOfBattlePatch.ClearSessionState();
        }
    }

    protected override void OnEndMission()
    {
        OrderOfBattlePatch.ClearSessionState();
        base.OnEndMission();
    }

    public override void OnTeamDeployed(Team team)
    {
        base.OnTeamDeployed(team);
        if (!IsSupportedMission() || team != Mission.Current?.PlayerTeam)
        {
            return;
        }

        var prefs = Campaign.Current?.PlayerFormationPreferences;
        if (prefs == null || prefs.Count == 0)
        {
            return;
        }

        OrderOfBattlePatch.ApplyAssignments(moveAgents: true, heavyUiRefresh: false);
    }

    internal static bool IsSupportedMission()
    {
        Mission? mission = Mission.Current;
        return mission != null && (mission.IsFieldBattle || mission.IsSiegeBattle);
    }

    /// <summary>
    /// Troop combat role (infantry/ranged/...) for switching OOB formation type.
    /// Preference FormationClass is a slot index, not this role.
    /// </summary>
    internal static FormationClass GetTroopRoleClass(CharacterObject character)
    {
        return FormationClassExtensions.FallbackClass(character.DefaultFormationClass);
    }

    /// <summary>
    /// Switch formation slot X to match troop A's combat type before moving troops into it.
    /// </summary>
    internal static void EnsureFormationMatchesTroopType(
        OrderOfBattleFormationItemVM? item,
        Formation formation,
        CharacterObject character)
    {
        if (formation == null || character == null || item == null)
        {
            return;
        }

        FormationClass troopRole = GetTroopRoleClass(character);
        DeploymentFormationClass desired =
            OrderOfBattleFormationExtensions.GetOrderOfBattleFormationClass(troopRole);

        if (item.GetOrderOfBattleClass() == desired
            && item.OrderOfBattleFormationClassInt == (int)desired)
        {
            return;
        }

        try
        {
            item.RefreshFormation(formation, desired, false);
            item.OrderOfBattleFormationClassInt = (int)desired;

            if (item.Classes is { Count: > 0 })
            {
                item.Classes[0].Class = troopRole;
                item.Classes[0].IsUnset = false;
            }

            item.UpdateAdjustable();
        }
        catch (Exception)
        {
            // OOB UI may not be fully ready; type switch is best-effort.
        }
    }

    internal static void ApplyFormationClassToSlot(
        OrderOfBattleFormationItemVM? item,
        Formation formation,
        FormationClass troopRole)
    {
        if (item == null || formation == null)
        {
            return;
        }

        DeploymentFormationClass desired =
            OrderOfBattleFormationExtensions.GetOrderOfBattleFormationClass(troopRole);

        if (item.GetOrderOfBattleClass() == desired
            && item.OrderOfBattleFormationClassInt == (int)desired)
        {
            return;
        }

        try
        {
            item.RefreshFormation(formation, desired, false);
            item.OrderOfBattleFormationClassInt = (int)desired;
            if (item.Classes is { Count: > 0 })
            {
                item.Classes[0].Class = troopRole;
                item.Classes[0].IsUnset = false;
            }

            item.UpdateAdjustable();
        }
        catch (Exception)
        {
        }
    }

    /// <summary>
    /// Move agent to preferred slot. agent.Formation setter internally calls RemoveUnit,
    /// which can throw IndexOutOfRangeException during OOB init — catch and skip.
    /// </summary>
    internal static void TryAssignAgent(Agent agent, bool refreshUi, bool switchFormationType = true)
    {
        if (OrderOfBattlePatch.IsApplying && refreshUi)
        {
            return;
        }

        if (!IsSupportedMission()
            || agent == null
            || !agent.IsHuman
            || agent.IsHero
            || !agent.IsActive()
            || agent.Team != Mission.Current.PlayerTeam
            || agent.Character is not CharacterObject character
            || !TroopFormationsBehavior.TryGetAssignedFormation(character, out FormationClass formationSlot))
        {
            return;
        }

        int index = (int)formationSlot;
        if (index < 0 || index >= TroopFormationsBehavior.FormationCount)
        {
            return;
        }

        Formation? target = Mission.Current.PlayerTeam.FormationsIncludingEmpty
            .FirstOrDefault(f => f.Index == index);
        if (target == null || agent.Formation == target)
        {
            return;
        }

        if (switchFormationType)
        {
            OrderOfBattleFormationItemVM? item = GetFormationItem(index);
            EnsureFormationMatchesTroopType(item, target, character);
        }

        try
        {
            agent.Formation = target;
        }
        catch (Exception)
        {
            // Never RemoveUnit/AddUnit — that corrupts LineFormation and can AV on later save load.
            return;
        }

        if (refreshUi && OrderOfBattlePatch.Instance != null)
        {
            try
            {
                OrderOfBattleFormationExtensions.Refresh(target);
            }
            catch (Exception)
            {
            }
        }
    }

    /// <summary>
    /// Move any non-hero agent to a formation index (used to park unassigned troops
    /// outside preferred slots).
    /// </summary>
    internal static void TryMoveAgentToSlot(Agent agent, int slotIndex, bool switchFormationType)
    {
        if (!IsSupportedMission()
            || agent == null
            || !agent.IsHuman
            || agent.IsHero
            || !agent.IsActive()
            || agent.Team != Mission.Current.PlayerTeam
            || slotIndex < 0
            || slotIndex >= TroopFormationsBehavior.FormationCount)
        {
            return;
        }

        Formation? target = Mission.Current.PlayerTeam.FormationsIncludingEmpty
            .FirstOrDefault(f => f.Index == slotIndex);
        if (target == null || agent.Formation == target)
        {
            return;
        }

        if (switchFormationType && agent.Character is CharacterObject character)
        {
            OrderOfBattleFormationItemVM? item = GetFormationItem(slotIndex);
            EnsureFormationMatchesTroopType(item, target, character);
        }

        try
        {
            agent.Formation = target;
        }
        catch (Exception)
        {
            return;
        }
    }

    internal static OrderOfBattleFormationItemVM? GetFormationItem(int index)
    {
        if (OrderOfBattlePatch.Instance == null)
        {
            return null;
        }

        var allFormations = Traverse.Create(OrderOfBattlePatch.Instance)
            .Field("_allFormations")
            .GetValue<List<OrderOfBattleFormationItemVM>>();
        if (allFormations == null || index < 0 || index >= allFormations.Count)
        {
            return null;
        }

        return allFormations[index];
    }

    internal static void RefreshOrderOfBattleUi(bool invokeTick)
    {
        OrderOfBattleVM? oob = OrderOfBattlePatch.Instance;
        if (oob == null)
        {
            return;
        }

        var allFormations = Traverse.Create(oob)
            .Field("_allFormations")
            .GetValue<List<OrderOfBattleFormationItemVM>>();
        if (allFormations == null)
        {
            return;
        }

        try
        {
            // Avoid RefreshFormation here — it re-triggers mass redistribute / native churn.
            foreach (OrderOfBattleFormationItemVM item in allFormations)
            {
                item.OnSizeChanged();
                item.UpdateAdjustable();
            }

            Traverse.Create(oob).Method("RefreshWeights").GetValue();
            Traverse.Create(oob).Method("OnUnitDeployed").GetValue();
            Traverse.Create(oob).Field("_isMissingFormationsDirty").SetValue(false);
        }
        catch (Exception)
        {
        }
    }
}
