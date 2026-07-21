using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.OrderOfBattle;

namespace TroopFormations;

[HarmonyPatch(typeof(OrderOfBattleVM))]
public static class OrderOfBattlePatch
{
	public static OrderOfBattleVM Instance { get; set; }

	[HarmonyPrefix]
	[HarmonyPatch("Initialize")]
	private static void Prefix(OrderOfBattleVM __instance)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Expected I4, but got Unknown
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between I4 and Unknown
		Instance = __instance;
		Dictionary<CharacterObject, FormationClass> formationAssignment = TroopFormationsBehavior.FormationAssignment;
		foreach (Agent item in (List<Agent>)(object)Mission.Current.PlayerTeam.TeamAgents)
		{
			if (!formationAssignment.ContainsKey((CharacterObject)item.Character))
			{
				continue;
			}
			int FormationId = (int)formationAssignment[(CharacterObject)item.Character];
			if (item.Formation.Index != (int)formationAssignment[(CharacterObject)item.Character])
			{
				item.Formation = ((IEnumerable<Formation>)Mission.Current.PlayerTeam.FormationsIncludingEmpty).Where((Formation x) => x.Index == FormationId).First();
			}
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch("Initialize")]
	private static void Postfix(List<OrderOfBattleFormationItemVM> ____allFormations, ref Mission ____mission, OrderOfBattleVM __instance)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected I4, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between I4 and Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected I4, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Invalid comparison between I4 and Unknown
		//IL_01b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ba: Expected O, but got Unknown
		//IL_01d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Expected I4, but got Unknown
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Invalid comparison between I4 and Unknown
		Dictionary<CharacterObject, FormationClass> formationAssignment = TroopFormationsBehavior.FormationAssignment;
		foreach (Agent item in (List<Agent>)(object)Mission.Current.PlayerTeam.TeamAgents)
		{
			if (!formationAssignment.ContainsKey((CharacterObject)item.Character))
			{
				continue;
			}
			int FormationId = (int)formationAssignment[(CharacterObject)item.Character];
			if (item.Formation.Index != (int)formationAssignment[(CharacterObject)item.Character])
			{
				item.Formation = ((IEnumerable<Formation>)Mission.Current.PlayerTeam.FormationsIncludingEmpty).Where((Formation x) => x.Index == FormationId).First();
			}
		}
		foreach (Agent item2 in (List<Agent>)(object)____mission.PlayerTeam.TeamAgents)
		{
			if (formationAssignment.ContainsKey((CharacterObject)item2.Character))
			{
				int index = (int)formationAssignment[(CharacterObject)item2.Character];
				if (____allFormations[index].OrderOfBattleFormationClassInt != (int)formationAssignment[(CharacterObject)item2.Character])
				{
					item2.Formation = ____allFormations[index].Formation;
				}
			}
		}
		foreach (Agent item3 in (List<Agent>)(object)Mission.Current.PlayerTeam.TeamAgents)
		{
			if (!formationAssignment.ContainsKey((CharacterObject)item3.Character))
			{
				continue;
			}
			int FormationId2 = (int)formationAssignment[(CharacterObject)item3.Character];
			if (item3.Formation.Index != (int)formationAssignment[(CharacterObject)item3.Character])
			{
				item3.Formation = ((IEnumerable<Formation>)Mission.Current.PlayerTeam.FormationsIncludingEmpty).Where((Formation x) => x.Index == FormationId2).First();
			}
		}
	}

	[HarmonyPrefix]
	[HarmonyPatch("Tick")]
	private static void TickPrefix()
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected I4, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between I4 and Unknown
		Dictionary<CharacterObject, FormationClass> formationAssignment = TroopFormationsBehavior.FormationAssignment;
		foreach (Agent item in (List<Agent>)(object)Mission.Current.PlayerTeam.TeamAgents)
		{
			if (!formationAssignment.ContainsKey((CharacterObject)item.Character))
			{
				continue;
			}
			int FormationId = (int)formationAssignment[(CharacterObject)item.Character];
			if (item.Formation.Index != (int)formationAssignment[(CharacterObject)item.Character])
			{
				item.Formation = ((IEnumerable<Formation>)Mission.Current.PlayerTeam.FormationsIncludingEmpty).Where((Formation x) => x.Index == FormationId).First();
			}
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch("Tick")]
	private static void TickPostfix(List<OrderOfBattleFormationItemVM> ____allFormations, ref Mission ____mission)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected I4, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Invalid comparison between I4 and Unknown
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected I4, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Invalid comparison between I4 and Unknown
		Dictionary<CharacterObject, FormationClass> formationAssignment = TroopFormationsBehavior.FormationAssignment;
		foreach (Agent item in (List<Agent>)(object)Mission.Current.PlayerTeam.TeamAgents)
		{
			if (!formationAssignment.ContainsKey((CharacterObject)item.Character))
			{
				continue;
			}
			int FormationId = (int)formationAssignment[(CharacterObject)item.Character];
			if (item.Formation.Index != (int)formationAssignment[(CharacterObject)item.Character])
			{
				item.Formation = ((IEnumerable<Formation>)Mission.Current.PlayerTeam.FormationsIncludingEmpty).Where((Formation x) => x.Index == FormationId).First();
			}
		}
		foreach (Agent item2 in (List<Agent>)(object)____mission.PlayerTeam.TeamAgents)
		{
			if (formationAssignment.ContainsKey((CharacterObject)item2.Character))
			{
				int index = (int)formationAssignment[(CharacterObject)item2.Character];
				if (____allFormations[index].OrderOfBattleFormationClassInt != (int)formationAssignment[(CharacterObject)item2.Character])
				{
					item2.Formation = ____allFormations[index].Formation;
				}
			}
		}
	}
}
