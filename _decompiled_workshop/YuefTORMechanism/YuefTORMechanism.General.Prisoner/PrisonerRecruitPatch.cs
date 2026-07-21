using System;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.Models;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace YuefTORMechanism.General.Prisoner;

[HarmonyPatch]
internal class PrisonerRecruitPatch
{
	private static readonly DefaultPrisonerRecruitmentCalculationModel VanillaModel = new DefaultPrisonerRecruitmentCalculationModel();

	private static MCMSetting Settings => GlobalSettings<MCMSetting>.Instance;

	[HarmonyPatch(typeof(TORPrisonerRecruitmentCalculationModel), "IsPrisonerRecruitable")]
	[HarmonyPrefix]
	private static bool Prefix_IsPrisonerRecruitable(PartyBase party, CharacterObject prisoner, out int conformityNeeded, ref bool __result)
	{
		conformityNeeded = 0;
		if (Settings == null)
		{
			return true;
		}
		if (Settings.Yuef_UnlockAllCulturePrisonerRecruit)
		{
			__result = ((PrisonerRecruitmentCalculationModel)VanillaModel).IsPrisonerRecruitable(party, prisoner, ref conformityNeeded);
			return false;
		}
		return true;
	}

	[HarmonyPatch(typeof(TORPrisonerRecruitmentCalculationModel), "GetConformityChangePerHour")]
	[HarmonyPrefix]
	private static bool Prefix_GetConformityChangePerHour(PartyBase party, CharacterObject prisoner, ref ExplainedNumber __result)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (Settings == null)
		{
			return true;
		}
		if (Settings.Yuef_UnlockAllCulturePrisonerRecruit)
		{
			__result = ((PrisonerRecruitmentCalculationModel)VanillaModel).GetConformityChangePerHour(party, prisoner);
			return false;
		}
		return true;
	}

	[HarmonyPatch(typeof(TORPrisonerRecruitmentCalculationModel), "GetConformityChangePerHour")]
	[HarmonyPostfix]
	private static void Postfix_GetConformityChangePerHour(PartyBase party, CharacterObject prisoner, ref ExplainedNumber __result)
	{
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		MobileParty mobileParty = party.MobileParty;
		if (mobileParty != null && mobileParty.IsMainParty && Settings.Yuef_UnlockAllCulturePrisonerRecruit)
		{
			float yuef_UnlockAllCulture_ConformityMultiplier = Settings.Yuef_UnlockAllCulture_ConformityMultiplier;
			if (Math.Abs(yuef_UnlockAllCulture_ConformityMultiplier - 1f) > 0.0001f)
			{
				float resultNumber = ((ExplainedNumber)(ref __result)).ResultNumber;
				__result = new ExplainedNumber(resultNumber * yuef_UnlockAllCulture_ConformityMultiplier, false, (TextObject)null);
			}
		}
	}
}
