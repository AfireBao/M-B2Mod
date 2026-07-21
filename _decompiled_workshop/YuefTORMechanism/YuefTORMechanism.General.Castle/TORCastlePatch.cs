using System;
using System.Collections.Generic;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.Models;
using TOR_Core.Utilities;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Localization;

namespace YuefTORMechanism.General.Castle;

[HarmonyPatch]
internal class TORCastlePatch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORPartySpeedCalculatingModel), "CalculateFinalSpeed")]
	private static void Postfix_Castle_CalculateFinalSpeed(MobileParty mobileParty, ref ExplainedNumber __result)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Expected O, but got Unknown
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Expected O, but got Unknown
		if (!GlobalSettings<MCMSetting>.Instance.Yuef_Castle_adjustment || !mobileParty.IsLordParty || mobileParty.LeaderHero == null)
		{
			return;
		}
		Clan clan = mobileParty.LeaderHero.Clan;
		if (clan == null)
		{
			return;
		}
		IEnumerable<Settlement> enumerable = (IEnumerable<Settlement>)TORCommon.FindSettlementsAroundPosition(mobileParty.GetPosition2D, 15f, (Func<Settlement, bool>)((Settlement settlement) => settlement.IsCastle));
		foreach (Settlement item in enumerable)
		{
			if (item == null || !item.IsCastle)
			{
				continue;
			}
			Clan ownerClan = item.OwnerClan;
			string text = ((object)item.Name).ToString();
			if (ownerClan != null && clan != null)
			{
				if (FactionManager.IsAtWarAgainstFaction(item.MapFaction, mobileParty.MapFaction))
				{
					((ExplainedNumber)(ref __result)).AddFactor(-0.4f, new TextObject(text, (Dictionary<string, object>)null));
					break;
				}
				if (ownerClan.Kingdom != null && clan.Kingdom != null && ownerClan.Kingdom == clan.Kingdom)
				{
					((ExplainedNumber)(ref __result)).AddFactor(0.4f, new TextObject(text, (Dictionary<string, object>)null));
					break;
				}
				if (item.Owner == mobileParty.LeaderHero)
				{
					((ExplainedNumber)(ref __result)).AddFactor(0.4f, new TextObject(text, (Dictionary<string, object>)null));
					break;
				}
			}
		}
	}
}
