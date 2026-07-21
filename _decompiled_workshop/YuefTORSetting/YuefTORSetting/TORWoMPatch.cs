using System;
using System.Collections.Generic;
using HarmonyLib;
using Helpers;
using MCM.Abstractions.Base.Global;
using TOR_Core.CampaignMechanics.CustomResources;
using TOR_Core.CharacterDevelopment;
using TOR_Core.CharacterDevelopment.CareerSystem;
using TOR_Core.Extensions;
using TOR_Core.Models;
using TOR_Core.Utilities;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.TwoDimension;

namespace YuefTORSetting;

[Harmony]
internal class TORWoMPatch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORAbilityModel), "GetMaximumWindsOfMagic")]
	private static void Postfix_GetMaximumWindsOfMagic(CharacterObject baseCharacter, ref float __result)
	{
		Hero val = ((baseCharacter != null) ? baseCharacter.HeroObject : null);
		if (val == null)
		{
			return;
		}
		if (val != Hero.MainHero)
		{
			MobileParty partyBelongedTo = val.PartyBelongedTo;
			if (partyBelongedTo == null || !partyBelongedTo.IsMainParty || !HeroExtensions.IsSpellCaster(val))
			{
				return;
			}
		}
		__result += GlobalSettings<MCMSetting>.Instance.Yuef_WOM_ValueToAdd;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TORAbilityModel), "GetWindsRechargeRate")]
	public unsafe static bool Prefix_GetWindsRechargeRate(CharacterObject baseCharacter, ref float __result)
	{
		//IL_010e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Invalid comparison between Unknown and I4
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Expected O, but got Unknown
		if (((baseCharacter != null) ? baseCharacter.HeroObject : null) != Hero.MainHero)
		{
			return true;
		}
		Hero heroObject = baseCharacter.HeroObject;
		ExplainedNumber val = default(ExplainedNumber);
		((ExplainedNumber)(ref val))._002Ector(1f, false, (TextObject)null);
		SkillHelper.AddSkillBonusForCharacter(TORSkillEffects.WindsRechargeRate, baseCharacter, ref val);
		if (MobilePartyExtensions.HasAnyActiveBlessing(MobileParty.MainParty) && MobilePartyExtensions.HasBlessing(MobileParty.MainParty, "cult_of_isha"))
		{
			((ExplainedNumber)(ref val)).AddFactor(0.25f, (TextObject)null);
		}
		CareerHelper.ApplyBasicCareerPassives(heroObject, ref val, (PassiveEffectType)15, false, (CharacterObject)null);
		float num = ((BasicCharacterObject)baseCharacter).Equipment.GetTotalWeightOfArmor(true) / 25f;
		num = Mathf.Min(num, 0.85f);
		if (HeroExtensions.HasCareerChoice(heroObject, "ArkaynePassive1") || GlobalSettings<MCMSetting>.Instance.Yuef_RemoveArmorWeightPenalty)
		{
			num = 0f;
		}
		((ExplainedNumber)(ref val)).AddFactor(0f - num, (TextObject)null);
		if (((MBObjectBase)baseCharacter.Culture).StringId == "battania")
		{
			if (!HeroExtensions.HasAttribute(heroObject, "WEWandererSymbol"))
			{
				ForestHarmonyLevel forestHarmonyLevel = HeroExtensions.GetForestHarmonyLevel(heroObject);
				if (1 == 0)
				{
				}
				float num2 = (((int)forestHarmonyLevel == 0) ? ForestHarmonyHelper.WindsDebuffUnbound : (((int)forestHarmonyLevel != 1) ? 0f : ForestHarmonyHelper.WindsDebuffBound));
				if (1 == 0)
				{
				}
				float num3 = num2;
				((ExplainedNumber)(ref val)).AddFactor(num3, new TextObject(((object)(*(ForestHarmonyLevel*)(&forestHarmonyLevel))/*cast due to .constrained prefix*/).ToString(), (Dictionary<string, object>)null));
			}
			if (HeroExtensions.HasAttribute(heroObject, "WEArielSymbol"))
			{
				Settlement val2 = TORCommon.FindNearestSettlement(MobileParty.MainParty, 500f, (Func<Settlement, bool>)((Settlement x) => SettlementExtensions.IsOakOfTheAges(x)));
				if (val2 != null)
				{
					((ExplainedNumber)(ref val)).AddFactor(1f, ForestHarmonyHelper.TreeSymbolText("WEArielSymbol"));
				}
			}
		}
		((ExplainedNumber)(ref val)).AddFactor((float)GlobalSettings<MCMSetting>.Instance.Yuef_WOM_RechargeSpeedBonus, (TextObject)null);
		__result = ((ExplainedNumber)(ref val)).ResultNumber;
		return false;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORAbilityModel), "GetWindsRechargeRate")]
	private static void Postfix_GetWindsRechargeRate(CharacterObject baseCharacter, ref float __result)
	{
		if (((baseCharacter != null) ? baseCharacter.HeroObject : null) == null)
		{
			return;
		}
		Hero heroObject = baseCharacter.HeroObject;
		if (heroObject == Hero.MainHero || !HeroExtensions.IsSpellCaster(heroObject))
		{
			return;
		}
		MobileParty partyBelongedTo = heroObject.PartyBelongedTo;
		if (partyBelongedTo != null && partyBelongedTo.IsMainParty && GlobalSettings<MCMSetting>.Instance.Yuef_RemoveArmorWeightPenalty && !HeroExtensions.IsVampire(heroObject))
		{
			float totalWeightOfArmor = ((BasicCharacterObject)baseCharacter).Equipment.GetTotalWeightOfArmor(true);
			ExplainedNumber val = default(ExplainedNumber);
			((ExplainedNumber)(ref val))._002Ector(totalWeightOfArmor, false, (TextObject)null);
			PerkHelper.AddPerkBonusForCharacter(Athletics.FormFittingArmor, baseCharacter, true, ref val, false);
			float num = Mathf.Max((((ExplainedNumber)(ref val)).ResultNumber - 5f) / 15f, 0f);
			float num2 = 0f - num;
			if (Math.Abs(num2 + 1f) > 0.0001f)
			{
				__result /= 1f + num2;
			}
		}
	}
}
