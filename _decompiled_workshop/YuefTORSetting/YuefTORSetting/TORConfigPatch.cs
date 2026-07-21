using System;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.CampaignMechanics.CustomResources;
using TOR_Core.Extensions;
using TOR_Core.Utilities;

namespace YuefTORSetting;

[HarmonyPatch]
public static class TORConfigPatch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORConfig), "get_MaximumNumberOfCareerPerkPoints")]
	private static void Postfix_MaximumNumberOfCareerPerkPoints(ref int __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null)
		{
			__result = instance.Yuef_MaximumNumberOfCareerPerkPoints;
		}
		else
		{
			__result = 30;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORConfig), "get_MaximumCustomResourceValue")]
	private static void Postfix_MaximumCustomResourceValue(ref int __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null)
		{
			__result = instance.Yuef_MaximumCustomResourceValue;
		}
		else
		{
			__result = 2500;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(CharacterObjectExtensions), "GetCustomResourceRequiredForUpgrade")]
	private static void Postfix_GetCustomResourceRequiredForUpgrade(ref Tuple<CustomResource, int> __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null && __result != null && __result.Item2 > 0)
		{
			float num = Math.Max(0f, Math.Min(100f, instance.Yuef_CustomResource_UpgradeCostReduction));
			int num2 = Math.Max(0, (int)((float)__result.Item2 * (1f - num / 100f)));
			if (num2 == 0)
			{
				__result = null;
			}
			else
			{
				__result = new Tuple<CustomResource, int>(__result.Item1, num2);
			}
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(CharacterObjectExtensions), "GetCustomResourceRequiredForUpkeep")]
	private static void Postfix_GetCustomResourceRequiredForUpkeep(ref Tuple<CustomResource, int> __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null && __result != null && __result.Item2 > 0)
		{
			float num = Math.Max(0f, Math.Min(100f, instance.Yuef_CustomResource_KeepCostReduction));
			int item = Math.Max(0, (int)((float)__result.Item2 * (1f - num / 100f)));
			__result = new Tuple<CustomResource, int>(__result.Item1, item);
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORConfig), "get_NumberOfInitialHideoutsAtEachBanditFaction")]
	private static void Postfix_NumberOfInitialHideoutsAtEachBanditFaction(ref int __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null)
		{
			__result = instance.Yuef_NumberOfInitialHideoutsAtEachBanditFaction;
		}
		else
		{
			__result = 10;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORConfig), "get_NumberOfMaximumHideoutsAtEachBanditFaction")]
	private static void Postfix_NumberOfMaximumHideoutsAtEachBanditFaction(ref int __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null)
		{
			__result = instance.Yuef_NumberOfMaximumHideoutsAtEachBanditFaction;
		}
		else
		{
			__result = 0;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORConfig), "get_NumberOfMaximumBanditPartiesAroundEachHideout")]
	private static void Postfix_NumberOfMaximumBanditPartiesAroundEachHideout(ref int __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null)
		{
			__result = instance.Yuef_NumberOfMaximumBanditPartiesAroundEachHideout;
		}
		else
		{
			__result = 0;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORConfig), "get_NumberOfMaximumBanditPartiesInEachHideout")]
	private static void Postfix_NumberOfMaximumBanditPartiesInEachHideout(ref int __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null)
		{
			__result = instance.Yuef_NumberOfMaximumBanditPartiesInEachHideout;
		}
		else
		{
			__result = 0;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORConfig), "get_MinPeaceDays")]
	private static void Postfix_MinPeaceDays(ref float __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null)
		{
			__result = instance.Yuef_MinPeaceDays;
		}
		else
		{
			__result = 0f;
		}
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORConfig), "get_MinWarDays")]
	private static void Postfix_MinWarDays(ref float __result)
	{
		MCMSetting instance = GlobalSettings<MCMSetting>.Instance;
		if (instance != null)
		{
			__result = instance.Yuef_MinWarDays;
		}
		else
		{
			__result = 0f;
		}
	}
}
