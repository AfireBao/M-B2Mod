using System.Collections.Generic;
using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.CampaignMechanics;
using TOR_Core.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace YuefTORMechanism.WoodElves;

[Harmony]
internal class TORTreemanRecruitmentPatch
{
	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORAIRecruitmentCampaignBehavior), "AddDryadsToPartyOnEnteringSettlement")]
	private static void Postfix_AddDryadsToPartyOnEnteringSettlement(MobileParty party, Settlement settlement, Hero hero)
	{
		if (!GlobalSettings<MCMSetting>.Instance.Yuef_Treeman_adjustment || party == null || settlement == null || hero == null || !HeroExtensions.IsSpellCaster(hero) || ((MBObjectBase)hero.Culture).StringId != "battania" || ((BasicCharacterObject)hero.CharacterObject).IsPlayerCharacter || settlement.IsHideout || !((MBObjectBase)settlement).StringId.Contains("AL"))
		{
			return;
		}
		CharacterObject val = MBObjectManager.Instance.GetObject<CharacterObject>("tor_we_treeman");
		CharacterObject val2 = val.UpgradeTargets[0];
		TroopRoster memberRoster = party.MemberRoster;
		if (!(MBRandom.RandomFloat < 0.3f) || val == null || party.MemberRoster.GetTroopCount(val) + party.MemberRoster.GetTroopCount(val2) > 5)
		{
			return;
		}
		bool flag = false;
		for (int i = 0; i < memberRoster.Count; i++)
		{
			CharacterObject characterAtIndex = memberRoster.GetCharacterAtIndex(i);
			if (((MBObjectBase)characterAtIndex.Culture).StringId != "battania" && characterAtIndex != val)
			{
				party.MemberRoster.AddToCounts(characterAtIndex, -1, false, 0, 0, true, -1);
				flag = true;
				break;
			}
		}
		if (!flag && memberRoster.Count > 0)
		{
			List<int> list = new List<int>();
			for (int j = 0; j < memberRoster.Count; j++)
			{
				CharacterObject characterAtIndex2 = memberRoster.GetCharacterAtIndex(j);
				if (characterAtIndex2 != val && !((BasicCharacterObject)characterAtIndex2).IsHero)
				{
					list.Add(j);
				}
			}
			if (list.Count > 0)
			{
				int num = list[MBRandom.RandomInt(0, list.Count)];
				CharacterObject characterAtIndex3 = memberRoster.GetCharacterAtIndex(num);
				party.MemberRoster.AddToCounts(characterAtIndex3, -1, false, 0, 0, true, -1);
			}
		}
		party.MemberRoster.AddToCounts(val, 1, false, 0, 0, true, -1);
	}
}
