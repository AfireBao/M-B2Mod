using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TroopFormations;

[HarmonyPatch(typeof(MissionReinforcementsHelper))]
public class GetReinforcementAssignmentsPatch
{
	[HarmonyPostfix]
	[HarmonyPatch("GetReinforcementAssignments")]
	private static void Postfix(ref List<(IAgentOriginBase, int)> __result)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected I4, but got Unknown
		Dictionary<CharacterObject, FormationClass> formationAssignment = TroopFormationsBehavior.FormationAssignment;
		List<(IAgentOriginBase, int)> list = new List<(IAgentOriginBase, int)>();
		foreach (var item2 in __result)
		{
			(IAgentOriginBase, int) item = item2;
			if (formationAssignment.ContainsKey((CharacterObject)item.Item1.Troop))
			{
				int num = (int)formationAssignment[(CharacterObject)item2.Item1.Troop];
				if (item.Item2 != num)
				{
					item.Item2 = num;
				}
			}
			list.Add(item);
		}
		__result = list;
	}
}
