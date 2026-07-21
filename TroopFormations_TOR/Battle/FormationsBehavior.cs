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
        TryAssignAgent(agent);
    }

    public override void OnTeamDeployed(Team team)
    {
        base.OnTeamDeployed(team);
        if (!IsSupportedMission() || team != Mission.Current?.PlayerTeam)
        {
            return;
        }

        // OOB UI can lag behind agent moves; refresh a few times like the original mod.
        for (int i = 0; i < 3; i++)
        {
            RefreshOrderOfBattleUi();
        }
    }

    internal static bool IsSupportedMission()
    {
        Mission? mission = Mission.Current;
        return mission != null && (mission.IsFieldBattle || mission.IsSiegeBattle);
    }

    internal static void TryAssignAgent(Agent agent)
    {
        if (!IsSupportedMission()
            || agent == null
            || !agent.IsHuman
            || agent.IsHero
            || agent.Team != Mission.Current.PlayerTeam
            || agent.Character is not CharacterObject character
            || !TroopFormationsBehavior.TryGetAssignedFormation(character, out FormationClass formationClass))
        {
            return;
        }

        int index = (int)formationClass;
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

        Formation? previous = agent.Formation;
        if (previous != null && previous != target)
        {
            previous.RemoveUnit(agent);
        }

        target.AddUnit(agent);
        agent.Formation = target;

        if (OrderOfBattlePatch.Instance != null)
        {
            if (previous != null)
            {
                OrderOfBattleFormationExtensions.Refresh(previous);
            }

            OrderOfBattleFormationExtensions.Refresh(target);
        }
    }

    private static void RefreshOrderOfBattleUi()
    {
        OrderOfBattleVM? oob = OrderOfBattlePatch.Instance;
        if (oob == null)
        {
            return;
        }

        var allFormations = Traverse.Create(oob).Field("_allFormations").GetValue<List<OrderOfBattleFormationItemVM>>();
        if (allFormations == null)
        {
            return;
        }

        foreach (OrderOfBattleFormationItemVM item in allFormations)
        {
            item.RefreshFormation(item.Formation, DeploymentFormationClass.Unset, false);
            item.UpdateAdjustable();
        }

        Traverse.Create(oob).Method("RefreshWeights").GetValue();
        Traverse.Create(oob).Method("OnUnitDeployed").GetValue();
        Traverse.Create(oob).Field("_isMissingFormationsDirty").SetValue(false);
        Traverse.Create(oob).Method("Tick").GetValue();
    }
}
