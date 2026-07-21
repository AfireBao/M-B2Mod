using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.CharacterDevelopment;
using TaleWorlds.CampaignSystem.ComponentInterfaces;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.LinQuick;
using TaleWorlds.ObjectSystem;

namespace TOR_Core.CampaignMechanics;

public class TORPartyUpgraderCampaignBehavior : CampaignBehaviorBase
{
	private readonly struct TORTroopUpgradeArgs
	{
		public readonly CharacterObject UpgradeSource;

		public readonly CharacterObject UpgradeTarget;

		public readonly int PossibleUpgradeCount;

		public readonly int UpgradeGoldCost;

		public readonly int UpgradeXpCost;

		public readonly float UpgradeChance;

		public TORTroopUpgradeArgs(CharacterObject upgradeSource, CharacterObject upgradeTarget, int possibleUpgradeCount, int upgradeGoldCost, int upgradeXpCost, float upgradeChance)
		{
			UpgradeSource = upgradeSource;
			UpgradeTarget = upgradeTarget;
			PossibleUpgradeCount = possibleUpgradeCount;
			UpgradeGoldCost = upgradeGoldCost;
			UpgradeXpCost = upgradeXpCost;
			UpgradeChance = upgradeChance;
		}
	}

	private float _offTemplateRatio = 0.05f;

	private int _cutoffLevel = 25;

	private static Dictionary<string, Tuple<int, List<PartyTemplateStack>>> _cultureTemplateData = new Dictionary<string, Tuple<int, List<PartyTemplateStack>>>();

	public override void RegisterEvents()
	{
		CampaignEvents.MapEventEnded.AddNonSerializedListener((object)this, (Action<MapEvent>)MapEventEnded);
		CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener((object)this, (Action<MobileParty>)DailyTickParty);
		CampaignEvents.WeeklyTickEvent.AddNonSerializedListener((object)this, (Action)WeeklyGarrisonUpgrade);
		CampaignEvents.OnAfterSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)CalculateTemplateDataOnLoad);
	}

	private void CalculateTemplateDataOnLoad(CampaignGameStarter starter)
	{
		IEnumerable<BasicCultureObject> enumerable = LinQuick.WhereQ<BasicCultureObject>((List<BasicCultureObject>)(object)MBObjectManager.Instance.GetObjectTypeList<BasicCultureObject>(), (Func<BasicCultureObject, bool>)((BasicCultureObject x) => !x.IsBandit));
		foreach (BasicCultureObject item3 in enumerable)
		{
			if (_cultureTemplateData.TryGetValue(((MBObjectBase)item3).StringId, out var _))
			{
				continue;
			}
			CultureObject val = (CultureObject)(object)((item3 is CultureObject) ? item3 : null);
			MBList<PartyTemplateStack> val2 = val.DefaultPartyTemplate?.Stacks;
			if (val2 != null)
			{
				List<PartyTemplateStack> item = (from highFirst in LinQuick.WhereQ<PartyTemplateStack>((List<PartyTemplateStack>)(object)val2, (Func<PartyTemplateStack, bool>)((PartyTemplateStack tier) => ((BasicCharacterObject)tier.Character).Level > _cutoffLevel))
					orderby ((BasicCharacterObject)highFirst.Character).Level descending
					select highFirst).ToList();
				int item2 = ((IEnumerable<PartyTemplateStack>)val2).Sum((PartyTemplateStack x) => x.MaxValue);
				_cultureTemplateData.Add(((MBObjectBase)item3).StringId, new Tuple<int, List<PartyTemplateStack>>(item2, item));
			}
		}
	}

	private void WeeklyGarrisonUpgrade()
	{
		foreach (Town item in Town.AllFiefs.Where((Town x) => !x.IsUnderSiege))
		{
			MobileParty garrisonParty = ((Fief)item).GarrisonParty;
			if (garrisonParty != null && garrisonParty.MapEvent == null)
			{
				UpgradeReadyTroops(garrisonParty.Party);
			}
		}
	}

	private void MapEventEnded(MapEvent mapEvent)
	{
		foreach (PartyBase involvedParty in mapEvent.InvolvedParties)
		{
			if (involvedParty.MobileParty != null && involvedParty.MobileParty != MobileParty.MainParty)
			{
				UpgradeReadyTroops(involvedParty);
			}
		}
	}

	private void DailyTickParty(MobileParty party)
	{
		if (party.IsLordParty && party.MapEvent == null && party != MobileParty.MainParty)
		{
			UpgradeReadyTroops(party.Party);
		}
	}

	public void UpgradeReadyTroops(PartyBase party)
	{
		if (!party.IsActive)
		{
			return;
		}
		IOrderedEnumerable<TORTroopUpgradeArgs> orderedEnumerable = from troopArg in GetTroopUpgradeList(party)
			orderby ((BasicCharacterObject)troopArg.UpgradeTarget).Level
			select troopArg;
		PartyWageModel partyWageModel = Campaign.Current.Models.PartyWageModel;
		foreach (TORTroopUpgradeArgs item in orderedEnumerable)
		{
			UpgradeTroop(partyWageModel, party, party.MemberRoster.FindIndexOfTroop(item.UpgradeSource), item);
		}
	}

	private List<TORTroopUpgradeArgs> GetTroopUpgradeList(PartyBase party)
	{
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_020d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0399: Unknown result type (might be due to invalid IL or missing references)
		//IL_039e: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		List<TORTroopUpgradeArgs> list = new List<TORTroopUpgradeArgs>();
		PartyTroopUpgradeModel partyTroopUpgradeModel = Campaign.Current.Models.PartyTroopUpgradeModel;
		TroopRoster memberRoster = party.MemberRoster;
		IEnumerable<TroopRosterElement> enumerable = from x in (IEnumerable<TroopRosterElement>)memberRoster.GetTroopRoster()
			where !((BasicCharacterObject)x.Character).IsHero && x.Character.UpgradeTargets.Length != 0
			orderby ((BasicCharacterObject)x.Character).Level descending
			select x;
		float num = memberRoster.TotalRegulars;
		Clan actualClan = party.MobileParty.ActualClan;
		object obj = ((actualClan != null) ? actualClan.DefaultPartyTemplate : null);
		if (obj == null)
		{
			IFaction mapFaction = party.MapFaction;
			if (mapFaction == null)
			{
				obj = null;
			}
			else
			{
				CultureObject culture = mapFaction.Culture;
				obj = ((culture != null) ? culture.DefaultPartyTemplate : null);
			}
		}
		MBList<PartyTemplateStack> val = ((PartyTemplateObject)(obj?)).Stacks;
		Dictionary<CharacterObject, float> dictionary = new Dictionary<CharacterObject, float>();
		MobileParty mobileParty = party.MobileParty;
		object obj2;
		if (mobileParty == null)
		{
			obj2 = null;
		}
		else
		{
			Clan actualClan2 = mobileParty.ActualClan;
			obj2 = ((actualClan2 != null) ? actualClan2.Culture : null);
		}
		if (obj2 == null)
		{
			IFaction mapFaction2 = party.MapFaction;
			obj2 = ((mapFaction2 != null) ? mapFaction2.Culture : null);
		}
		CultureObject val2 = (CultureObject)obj2;
		if (val2 != null && _cultureTemplateData.TryGetValue(((MBObjectBase)val2).StringId, out var value) && value != null)
		{
			float num2 = value.Item1;
			List<PartyTemplateStack> item = value.Item2;
			float num3 = num / num2;
			foreach (PartyTemplateStack item2 in item)
			{
				float num4 = (float)item2.MaxValue * num3;
				if (memberRoster.GetTroopCount(item2.Character) > 0)
				{
					num4 -= (float)memberRoster.GetTroopCount(item2.Character);
				}
				CharacterObject[] upgradeTargets = item2.Character.UpgradeTargets;
				foreach (CharacterObject key in upgradeTargets)
				{
					if (dictionary.TryGetValue(key, out var value2))
					{
						num4 += value2;
					}
				}
				dictionary.Add(item2.Character, num4);
			}
		}
		foreach (TroopRosterElement item3 in enumerable)
		{
			TroopRosterElement current2 = item3;
			CharacterObject upgradingCharacter = current2.Character;
			int number = ((TroopRosterElement)(ref current2)).Number;
			int healthyCount = number - ((TroopRosterElement)(ref current2)).WoundedNumber;
			IEnumerable<CharacterObject> enumerable2 = upgradingCharacter.UpgradeTargets.Where((CharacterObject possibleTarget) => BanditPerkAndItemCheck(party, upgradingCharacter, possibleTarget, partyTroopUpgradeModel) && healthyCount > 0);
			if (!enumerable2.Any())
			{
				continue;
			}
			ExplainedNumber goldCostForUpgrade;
			if (!enumerable2.Where((CharacterObject target) => ((BasicCharacterObject)target).Level > _cutoffLevel).Any())
			{
				CharacterObject randomElementInefficiently = Extensions.GetRandomElementInefficiently<CharacterObject>(enumerable2);
				int xpCostForUpgrade = partyTroopUpgradeModel.GetXpCostForUpgrade(party, upgradingCharacter, randomElementInefficiently);
				int xp = ((TroopRosterElement)(ref current2)).Xp;
				if (xpCostForUpgrade <= xp)
				{
					int val3 = xp / xpCostForUpgrade;
					val3 = Math.Min(val3, healthyCount);
					CharacterObject upgradeSource = upgradingCharacter;
					int possibleUpgradeCount = val3;
					goldCostForUpgrade = partyTroopUpgradeModel.GetGoldCostForUpgrade(party, upgradingCharacter, randomElementInefficiently);
					list.Add(new TORTroopUpgradeArgs(upgradeSource, randomElementInefficiently, possibleUpgradeCount, (int)((ExplainedNumber)(ref goldCostForUpgrade)).ResultNumber, xpCostForUpgrade, 1f));
				}
				continue;
			}
			List<CharacterObject> list2 = new List<CharacterObject>();
			foreach (CharacterObject item4 in enumerable2)
			{
				float value3;
				bool flag = dictionary.TryGetValue(item4, out value3);
				if (((BasicCharacterObject)item4).Level <= _cutoffLevel || (flag && value3 >= 1f) || (!flag && (int)(num * _offTemplateRatio) - memberRoster.GetTroopCount(item4) >= 1))
				{
					list2.Add(item4);
				}
			}
			if (!list2.Any())
			{
				continue;
			}
			CharacterObject randomElementInefficiently2 = Extensions.GetRandomElementInefficiently<CharacterObject>((IEnumerable<CharacterObject>)list2);
			int xpCostForUpgrade2 = partyTroopUpgradeModel.GetXpCostForUpgrade(party, upgradingCharacter, randomElementInefficiently2);
			int xp2 = ((TroopRosterElement)(ref current2)).Xp;
			if (xpCostForUpgrade2 > xp2)
			{
				continue;
			}
			float value4;
			bool flag2 = dictionary.TryGetValue(randomElementInefficiently2, out value4);
			if (((BasicCharacterObject)randomElementInefficiently2).Level > _cutoffLevel && !flag2)
			{
				value4 = num * _offTemplateRatio - (float)memberRoster.GetTroopCount(randomElementInefficiently2);
			}
			if (value4 != 0f && value4 < 1f)
			{
				continue;
			}
			int num6 = Math.Min(xp2 / xpCostForUpgrade2, healthyCount);
			if (((BasicCharacterObject)randomElementInefficiently2).Level > _cutoffLevel)
			{
				num6 = Math.Min(num6, (int)value4);
			}
			if (num6 >= 1)
			{
				if (((TroopRosterElement)(ref current2)).Number > 30)
				{
					num6 = Math.Min(num6, (int)((double)number * 0.25));
				}
				CharacterObject upgradeSource2 = upgradingCharacter;
				int possibleUpgradeCount2 = num6;
				goldCostForUpgrade = partyTroopUpgradeModel.GetGoldCostForUpgrade(party, upgradingCharacter, randomElementInefficiently2);
				list.Add(new TORTroopUpgradeArgs(upgradeSource2, randomElementInefficiently2, possibleUpgradeCount2, (int)((ExplainedNumber)(ref goldCostForUpgrade)).ResultNumber, xpCostForUpgrade2, 1f));
			}
		}
		return list;
	}

	private bool BanditPerkAndItemCheck(PartyBase party, CharacterObject troopCharacter, CharacterObject upgradeTargetCharacter, PartyTroopUpgradeModel partyTroopUpgradeModel)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Invalid comparison between Unknown and I4
		if ((int)upgradeTargetCharacter.Occupation == 15)
		{
			int result;
			if (party == null)
			{
				result = 0;
			}
			else
			{
				CultureObject culture = party.Culture;
				result = ((((culture != null) ? new bool?(((BasicCultureObject)culture).IsBandit) : ((bool?)null)) == true) ? 1 : 0);
			}
			return (byte)result != 0;
		}
		return partyTroopUpgradeModel.CanPartyUpgradeTroopToTarget(party, troopCharacter, upgradeTargetCharacter);
	}

	private List<TORTroopUpgradeArgs> GetPossibleUpgradeTargets(PartyBase party, TroopRosterElement rosterElement)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Invalid comparison between Unknown and I4
		PartyWageModel partyWageModel = Campaign.Current.Models.PartyWageModel;
		List<TORTroopUpgradeArgs> list = new List<TORTroopUpgradeArgs>();
		CharacterObject character = rosterElement.Character;
		int num = ((TroopRosterElement)(ref rosterElement)).Number - ((TroopRosterElement)(ref rosterElement)).WoundedNumber;
		if (num > 0)
		{
			PartyTroopUpgradeModel partyTroopUpgradeModel = Campaign.Current.Models.PartyTroopUpgradeModel;
			for (int i = 0; i < character.UpgradeTargets.Length; i++)
			{
				CharacterObject val = character.UpgradeTargets[i];
				int upgradeXpCost = character.GetUpgradeXpCost(party, i);
				if (upgradeXpCost > 0)
				{
					num = MathF.Min(num, ((TroopRosterElement)(ref rosterElement)).Xp / upgradeXpCost);
				}
				int upgradeGoldCost = character.GetUpgradeGoldCost(party, i);
				if (upgradeGoldCost > 0 && party.LeaderHero != null && num * upgradeGoldCost > party.LeaderHero.Gold)
				{
					num = party.LeaderHero.Gold / upgradeGoldCost;
					if (num <= 1)
					{
						continue;
					}
				}
				int num2 = partyWageModel.GetCharacterWage(val) - partyWageModel.GetCharacterWage(character);
				if (num2 > 0 && val.Tier > character.Tier && party.MobileParty.HasLimitedWage() && !party.MobileParty.IsWageLimitExceeded() && party.MobileParty.TotalWage + num * num2 > party.MobileParty.PaymentLimit)
				{
					num = (party.MobileParty.PaymentLimit - party.MobileParty.TotalWage) / num2;
					if (num <= 1)
					{
						continue;
					}
				}
				if ((!((BasicCultureObject)party.Culture).IsBandit || ((BasicCultureObject)val.Culture).IsBandit) && ((int)character.Occupation != 15 || partyTroopUpgradeModel.CanPartyUpgradeTroopToTarget(party, character, val)))
				{
					float upgradeChanceForTroopUpgrade = Campaign.Current.Models.PartyTroopUpgradeModel.GetUpgradeChanceForTroopUpgrade(party, character, i);
					list.Add(new TORTroopUpgradeArgs(character, val, num, upgradeGoldCost, upgradeXpCost, upgradeChanceForTroopUpgrade));
				}
			}
		}
		return list;
	}

	private TORTroopUpgradeArgs SelectPossibleUpgrade(List<TORTroopUpgradeArgs> possibleUpgrades)
	{
		TORTroopUpgradeArgs result = possibleUpgrades[0];
		if (possibleUpgrades.Count > 1)
		{
			float num = 0f;
			foreach (TORTroopUpgradeArgs possibleUpgrade in possibleUpgrades)
			{
				num += possibleUpgrade.UpgradeChance;
			}
			float num2 = num * MBRandom.RandomFloat;
			foreach (TORTroopUpgradeArgs possibleUpgrade2 in possibleUpgrades)
			{
				num2 -= possibleUpgrade2.UpgradeChance;
				if (num2 <= 0f)
				{
					result = possibleUpgrade2;
					break;
				}
			}
		}
		return result;
	}

	private void UpgradeTroop(PartyWageModel partyWageModel, PartyBase party, int rosterIndex, TORTroopUpgradeArgs upgradeArgs)
	{
		TroopRoster memberRoster = party.MemberRoster;
		CharacterObject upgradeSource = upgradeArgs.UpgradeSource;
		CharacterObject upgradeTarget = upgradeArgs.UpgradeTarget;
		int num = upgradeArgs.PossibleUpgradeCount;
		int totalWage = party.MobileParty.TotalWage;
		int num2 = partyWageModel.GetCharacterWage(upgradeTarget) - partyWageModel.GetCharacterWage(upgradeSource);
		if (num2 > 0 && upgradeTarget.Tier > upgradeSource.Tier && !party.MobileParty.IsWageLimitExceeded() && totalWage + num * num2 > party.MobileParty.PaymentLimit)
		{
			num = (party.MobileParty.PaymentLimit - totalWage) / num2;
			if (num < 1)
			{
				return;
			}
		}
		int num3 = upgradeArgs.UpgradeXpCost * num;
		if (num3 > 0)
		{
			memberRoster.SetElementXp(rosterIndex, memberRoster.GetElementXp(rosterIndex) - num3);
			party.AddMember(upgradeArgs.UpgradeSource, -num, 0);
			party.AddMember(upgradeArgs.UpgradeTarget, num, 0);
			ApplyEffects(party, upgradeArgs);
		}
	}

	private void ApplyEffects(PartyBase party, TORTroopUpgradeArgs upgradeArgs)
	{
		if (party.Owner != null && party.Owner.IsAlive)
		{
			SkillLevelingManager.OnUpgradeTroops(party, upgradeArgs.UpgradeSource, upgradeArgs.UpgradeTarget, upgradeArgs.PossibleUpgradeCount);
			GiveGoldAction.ApplyBetweenCharacters(party.Owner, (Hero)null, upgradeArgs.UpgradeGoldCost * upgradeArgs.PossibleUpgradeCount, true);
		}
		else if (party.LeaderHero != null && party.LeaderHero.IsAlive)
		{
			SkillLevelingManager.OnUpgradeTroops(party, upgradeArgs.UpgradeSource, upgradeArgs.UpgradeTarget, upgradeArgs.PossibleUpgradeCount);
			GiveGoldAction.ApplyBetweenCharacters(party.LeaderHero, (Hero)null, upgradeArgs.UpgradeGoldCost * upgradeArgs.PossibleUpgradeCount, true);
		}
	}

	public override void SyncData(IDataStore dataStore)
	{
	}
}
