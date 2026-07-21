using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.Models;
using TaleWorlds.CampaignSystem;

namespace YuefTORMechanism.General.Notable;

[HarmonyPatch]
internal class NotableRecruitPatch
{
	private static MCMSetting Settings => GlobalSettings<MCMSetting>.Instance;

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TORHiringCompatibilityModel), "CanPlayerHireTroopFromSeller")]
	private static bool Prefix_CanPlayerHireTroopFromSeller(Hero player, Hero seller, ref bool __result)
	{
		if (player == null || seller == null)
		{
			return true;
		}
		if (Settings == null)
		{
			return true;
		}
		if (Settings.Yuef_UnlockAllCultureTroopRecruit)
		{
			__result = true;
			return false;
		}
		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TORHiringCompatibilityModel), "CanPlayerHireWanderer")]
	private static bool Prefix_CanPlayerHireWanderer(Hero player, Hero wanderer, ref bool __result)
	{
		if (player == null || wanderer == null)
		{
			return true;
		}
		if (Settings == null)
		{
			return true;
		}
		if (Settings.Yuef_UnlockAllCultureWandererRecruit)
		{
			__result = true;
			return false;
		}
		return true;
	}
}
