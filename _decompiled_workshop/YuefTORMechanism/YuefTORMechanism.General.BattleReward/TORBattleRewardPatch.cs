using System.Collections.Generic;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.Models;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Library;

namespace YuefTORMechanism.General.BattleReward;

[HarmonyPatch]
internal class TORBattleRewardPatch
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(TORBattleRewardModel), "GetLootPrisonerChances")]
	private static bool Prefix_GetLootPrisonerChances(MBReadOnlyList<MapEventParty> winnerParties, TroopRosterElement prisonerElement, ref MBReadOnlyList<KeyValuePair<MapEventParty, float>> __result)
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (GlobalSettings<MCMSetting>.Instance.Yuef_BattleReward_adjustment)
		{
			__result = ((BattleRewardModel)new DefaultBattleRewardModel()).GetLootPrisonerChances(winnerParties, prisonerElement);
			return false;
		}
		return true;
	}
}
