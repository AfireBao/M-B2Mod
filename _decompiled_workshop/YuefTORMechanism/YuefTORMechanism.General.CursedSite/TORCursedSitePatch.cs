using System;
using System.Collections.Generic;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.CampaignMechanics.TORCustomSettlement;
using TOR_Core.Extensions;
using TOR_Core.Models;
using TOR_Core.Utilities;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace YuefTORMechanism.General.CursedSite;

[HarmonyPatch]
internal class TORCursedSitePatch
{
	[HarmonyPrefix]
	[HarmonyPatch(typeof(TORCustomSettlementCampaignBehavior), "OnSettlementHourlyTick")]
	private static bool Prefix_OnSettlementHourlyTick(Settlement settlement)
	{
		if (GlobalSettings<MCMSetting>.Instance.Yuef_CursedSite_adjustment)
		{
			return false;
		}
		return true;
	}

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORPartySpeedCalculatingModel), "CalculateFinalSpeed")]
	private static void Postfix_CalculateFinalSpeed(MobileParty mobileParty, ref ExplainedNumber __result)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Expected O, but got Unknown
		if (!GlobalSettings<MCMSetting>.Instance.Yuef_CursedSite_adjustment || !mobileParty.IsLordParty || mobileParty.LeaderHero == null || !(((MBObjectBase)mobileParty.LeaderHero.Culture).StringId != "mousillon"))
		{
			return;
		}
		MBReadOnlyList<Settlement> val = (MBReadOnlyList<Settlement>)(object)TORCommon.FindSettlementsAroundPosition(mobileParty.GetPosition2D, 25f, (Func<Settlement, bool>)null);
		foreach (Settlement item in (List<Settlement>)(object)val)
		{
			SettlementComponent settlementComponent = item.SettlementComponent;
			CursedSiteComponent val2 = (CursedSiteComponent)(object)((settlementComponent is CursedSiteComponent) ? settlementComponent : null);
			if (val2 != null && ((TORBaseSettlementComponent)val2).IsActive && HeroExtensions.GetDominantReligion(mobileParty.LeaderHero) != ((TORBaseSettlementComponent)val2).Religion)
			{
				((ExplainedNumber)(ref __result)).AddFactor(-0.5f, new TextObject("诅咒之地惩罚！", (Dictionary<string, object>)null));
				break;
			}
		}
	}
}
