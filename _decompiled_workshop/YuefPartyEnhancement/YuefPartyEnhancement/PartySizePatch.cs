using System.Collections.Generic;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Localization;

namespace YuefPartyEnhancement;

[HarmonyPatch]
internal class PartySizePatch
{
	private static MCMSetting Settings => GlobalSettings<MCMSetting>.Instance;

	[HarmonyPostfix]
	[HarmonyPatch(typeof(DefaultPartySizeLimitModel), "GetPartyMemberSizeLimit")]
	private static void Postfix_GetPartyMemberSizeLimit(PartyBase party, ref ExplainedNumber __result)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		MCMSetting settings = Settings;
		if (settings != null && settings.EnablePartySizeBoost && party != null && party.IsMobile && !party.MobileParty.IsMainParty)
		{
			((ExplainedNumber)(ref __result)).Add((float)settings.PartySizeBoostAmount, new TextObject("{=yuef_party_size_bonus}Party Size Boost", (Dictionary<string, object>)null), (TextObject)null);
		}
	}
}
