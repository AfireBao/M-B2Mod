using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace TroopFormations;

public class FormationsBehavior : MissionView
{
	public override void OnAgentCreated(Agent agent)
	{
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected I4, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Invalid comparison between I4 and Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		((MissionBehavior)this).OnAgentCreated(agent);
		if (!Mission.Current.IsFieldBattle && !Mission.Current.IsSiegeBattle)
		{
			return;
		}
		Dictionary<CharacterObject, FormationClass> formationAssignment = TroopFormationsBehavior.FormationAssignment;
		if (agent.IsHuman && !agent.IsHero && agent.Character is CharacterObject && agent.Team == Mission.Current.PlayerTeam && formationAssignment.ContainsKey((CharacterObject)agent.Character))
		{
			int index = (int)formationAssignment[(CharacterObject)agent.Character];
			if (agent.Formation.Index != (int)formationAssignment[(CharacterObject)agent.Character])
			{
				Formation formation = agent.Formation;
				Formation val = ((List<Formation>)(object)Mission.Current.PlayerTeam.FormationsIncludingEmpty)[index];
				formation.RemoveUnit(agent);
				val.AddUnit(agent);
				OrderOfBattleFormationExtensions.Refresh(formation);
				OrderOfBattleFormationExtensions.Refresh(val);
				agent.Formation = ((List<Formation>)(object)Mission.Current.PlayerTeam.FormationsIncludingEmpty)[index];
				object value = AccessTools.Field(typeof(OrderOfBattleVM), "_allFormations").GetValue(OrderOfBattlePatch.Instance);
				List<OrderOfBattleFormationItemVM> list = (List<OrderOfBattleFormationItemVM>)value;
				agent.Formation = list[index].Formation;
			}
		}
	}

	public override void OnTeamDeployed(Team team)
	{
		((MissionBehavior)this).OnTeamDeployed(team);
		if ((Mission.Current.IsFieldBattle || Mission.Current.IsSiegeBattle) && team == Mission.Current.PlayerTeam)
		{
			RefreshThisDumbScreen();
			RefreshThisDumbScreen();
			RefreshThisDumbScreen();
			RefreshThisDumbScreen();
		}
	}

	private void RefreshThisDumbScreen()
	{
		object value = AccessTools.Field(typeof(OrderOfBattleVM), "_allFormations").GetValue(OrderOfBattlePatch.Instance);
		List<OrderOfBattleFormationItemVM> list = (List<OrderOfBattleFormationItemVM>)value;
		foreach (OrderOfBattleFormationItemVM item in list)
		{
			item.RefreshFormation(item.Formation, (DeploymentFormationClass)0, false);
		}
		MethodInfo methodInfo = AccessTools.Method(typeof(OrderOfBattleVM), "RefreshWeights", (Type[])null, (Type[])null);
		MethodInfo methodInfo2 = AccessTools.Method(typeof(OrderOfBattleVM), "OnUnitDeployed", (Type[])null, (Type[])null);
		methodInfo.Invoke(OrderOfBattlePatch.Instance, null);
		methodInfo2.Invoke(OrderOfBattlePatch.Instance, null);
		list.ForEach(delegate(OrderOfBattleFormationItemVM f)
		{
			f.UpdateAdjustable();
		});
		AccessTools.Field(typeof(OrderOfBattleVM), "_isMissingFormationsDirty").SetValue(OrderOfBattlePatch.Instance, false);
		MethodInfo methodInfo3 = AccessTools.Method(typeof(OrderOfBattleVM), "Tick", (Type[])null, (Type[])null);
		methodInfo3.Invoke(OrderOfBattlePatch.Instance, null);
	}
}
