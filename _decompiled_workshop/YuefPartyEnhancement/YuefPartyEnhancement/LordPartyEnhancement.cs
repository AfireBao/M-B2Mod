using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;

namespace YuefPartyEnhancement;

internal class LordPartyEnhancement : CampaignBehaviorBase
{
	private Dictionary<CharacterObject, List<CharacterObject>> basicTroopCache = new Dictionary<CharacterObject, List<CharacterObject>>();

	private Dictionary<CharacterObject, List<CharacterObject>> eliteTroopCache = new Dictionary<CharacterObject, List<CharacterObject>>();

	private MCMSetting settings;

	private Random random = new Random();

	private Dictionary<FormationClass, double> targetRatios = new Dictionary<FormationClass, double>
	{
		{
			(FormationClass)0,
			0.2
		},
		{
			(FormationClass)1,
			0.3
		},
		{
			(FormationClass)2,
			0.1
		},
		{
			(FormationClass)3,
			0.4
		}
	};

	public override void RegisterEvents()
	{
		settings = GlobalSettings<MCMSetting>.Instance;
		if (settings != null)
		{
			CampaignEvents.DailyTickEvent.AddNonSerializedListener((object)this, (Action)DailyTick);
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener((object)this, (Action)WeeklyTick);
			CampaignEvents.AfterSettlementEntered.AddNonSerializedListener((object)this, (Action<MobileParty, Settlement, Hero>)AddSoliderToParty);
		}
	}

	private void DailyTick()
	{
		if (settings == null)
		{
			return;
		}
		if (settings.isPlayerAutoUpgradeEnabled)
		{
			MobileParty mainParty = MobileParty.MainParty;
			if (((mainParty != null) ? mainParty.Party : null) != null)
			{
				UpgradeAIParty(MobileParty.MainParty.Party);
			}
		}
		foreach (MobileParty item in (List<MobileParty>)(object)MobileParty.AllLordParties)
		{
			if (item.IsMainParty)
			{
				continue;
			}
			Hero leaderHero = item.LeaderHero;
			if (leaderHero != null && leaderHero.Clan != null)
			{
				if (leaderHero.Clan != Clan.PlayerClan)
				{
					HandleDailyPartyGrain(item);
					UpgradeAIParty(item.Party);
				}
				if (leaderHero.Clan == Clan.PlayerClan && settings.isFamilyTroopAutoUpgradeEnabled && item.Party != null)
				{
					UpgradeAIParty(item.Party);
				}
			}
		}
		if (settings.isUpgradeForBanditsEnabled)
		{
			foreach (MobileParty item2 in (List<MobileParty>)(object)MobileParty.AllBanditParties)
			{
				if (item2.Party != null)
				{
					UpgradeAIParty(item2.Party);
				}
			}
		}
		if (settings.isUpgradeForCaravansEnabled)
		{
			foreach (MobileParty item3 in (List<MobileParty>)(object)MobileParty.AllCaravanParties)
			{
				if (item3.Party != null)
				{
					UpgradeAIParty(item3.Party);
				}
			}
		}
		if (settings.isUpgradeForVillagersEnabled)
		{
			foreach (MobileParty item4 in (List<MobileParty>)(object)MobileParty.AllVillagerParties)
			{
				if (item4.Party != null)
				{
					UpgradeAIParty(item4.Party);
				}
			}
		}
		if (settings.isUpgradeForGarrisonsEnabled)
		{
			foreach (MobileParty item5 in (List<MobileParty>)(object)MobileParty.AllGarrisonParties)
			{
				if (item5.CurrentSettlement != null && item5.CurrentSettlement.OwnerClan != null && (item5.CurrentSettlement.OwnerClan != Clan.PlayerClan || settings.isPlayerCityTroopsUpgradeEnabled) && item5.Party != null)
				{
					UpgradeAIParty(item5.Party);
				}
			}
		}
		if (!settings.isUpgradeForMilitiaEnabled)
		{
			return;
		}
		foreach (MobileParty item6 in (List<MobileParty>)(object)MobileParty.AllMilitiaParties)
		{
			if (item6.CurrentSettlement != null && item6.CurrentSettlement.OwnerClan != null && (item6.CurrentSettlement.OwnerClan != Clan.PlayerClan || settings.isPlayerCityTroopsUpgradeEnabled) && item6.Party != null)
			{
				UpgradeAIParty(item6.Party);
			}
		}
	}

	private void HandleDailyPartyGrain(MobileParty mobileParty)
	{
		if (settings.IsRecruitmentEnabled && settings.AIDailyPartyGrainAmount != 0)
		{
			ItemRoster itemRoster = mobileParty.ItemRoster;
			if (itemRoster.GetItemNumber(DefaultItems.Grain) <= 300)
			{
				itemRoster.AddToCounts(DefaultItems.Grain, settings.AIDailyPartyGrainAmount);
			}
		}
	}

	private void WeeklyTick()
	{
		if (settings.LordWeeklyGold == 0 || !settings.IsRecruitmentEnabled)
		{
			return;
		}
		foreach (Hero item in (List<Hero>)(object)Hero.AllAliveHeroes)
		{
			if (item != null && item.IsLord && item != Hero.MainHero && item.Clan != null && item.Clan != Clan.PlayerClan && item.Gold <= 1000000)
			{
				item.ChangeHeroGold(settings.LordWeeklyGold);
			}
		}
	}

	private void UpgradeAIParty(PartyBase party)
	{
		if (party != null)
		{
			TroopRoster memberRoster = party.MemberRoster;
			if (memberRoster != null && memberRoster.Count != 0)
			{
				ExecuteUpgrades(memberRoster, party, new Random());
			}
		}
	}

	public void ExecuteUpgrades(TroopRoster memberRoster, PartyBase party, Random random)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		if (memberRoster == null || party == null)
		{
			return;
		}
		List<TroopRosterElement> list = new List<TroopRosterElement>();
		for (int i = 0; i < memberRoster.Count; i++)
		{
			TroopRosterElement elementCopyAtIndex = memberRoster.GetElementCopyAtIndex(i);
			if (elementCopyAtIndex.Character != null)
			{
				list.Add(elementCopyAtIndex);
			}
		}
		ConcurrentDictionary<HashSet<CharacterObject>, Dictionary<FormationClass, List<CharacterObject>>> categorizedCache = new ConcurrentDictionary<HashSet<CharacterObject>, Dictionary<FormationClass, List<CharacterObject>>>();
		foreach (TroopRosterElement item in list)
		{
			CharacterObject character = item.Character;
			if (character != null && character.UpgradeTargets != null && character.UpgradeTargets.Length != 0)
			{
				CharacterObject[] array = FilterUpgradeTargets(character);
				if (array.Length != 0)
				{
					Dictionary<FormationClass, List<CharacterObject>> categorizedTargets = GetCategorizedTargets(array, categorizedCache);
					Dictionary<FormationClass, double> finalRatios = GetFinalRatios(categorizedTargets);
					double adjustedProbability = CalculateAdjustedProbability(character);
					int finalUpgradeCount = CalculateFinalUpgradeCount(item, adjustedProbability, random);
					PerformUpgrades(item, categorizedTargets, finalRatios, finalUpgradeCount, party, random);
				}
			}
		}
	}

	private CharacterObject[] FilterUpgradeTargets(CharacterObject character)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		CharacterObject[] array = character.UpgradeTargets;
		if (!settings.isUpgradeToRegularAllowed && (int)character.Occupation != 7)
		{
			array = array.Where((CharacterObject target) => (int)target.Occupation != 7).ToArray();
		}
		return array;
	}

	private Dictionary<FormationClass, List<CharacterObject>> GetCategorizedTargets(CharacterObject[] upgradeTargets, ConcurrentDictionary<HashSet<CharacterObject>, Dictionary<FormationClass, List<CharacterObject>>> categorizedCache)
	{
		HashSet<CharacterObject> key = new HashSet<CharacterObject>(upgradeTargets);
		if (!categorizedCache.TryGetValue(key, out Dictionary<FormationClass, List<CharacterObject>> value))
		{
			value = (categorizedCache[key] = CategorizeUpgradeTargets(upgradeTargets));
		}
		return value;
	}

	private Dictionary<FormationClass, List<CharacterObject>> CategorizeUpgradeTargets(CharacterObject[] upgradeTargets)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<FormationClass, List<CharacterObject>> dictionary = new Dictionary<FormationClass, List<CharacterObject>>();
		foreach (CharacterObject val in upgradeTargets)
		{
			if (val != null)
			{
				if (!dictionary.ContainsKey(((BasicCharacterObject)val).DefaultFormationClass))
				{
					dictionary[((BasicCharacterObject)val).DefaultFormationClass] = new List<CharacterObject>();
				}
				dictionary[((BasicCharacterObject)val).DefaultFormationClass].Add(val);
			}
		}
		return dictionary;
	}

	private double CalculateAdjustedProbability(CharacterObject character)
	{
		return Math.Max(0.0, Math.Min((double)settings.upgradeProbability * (1.0 - (double)character.Tier / 15.0), 100.0));
	}

	private int CalculateFinalUpgradeCount(TroopRosterElement element, double adjustedProbability, Random random)
	{
		int num = (int)Math.Floor((double)((TroopRosterElement)(ref element)).Number * adjustedProbability / 100.0);
		int num2 = (int)Math.Floor((double)num * 0.1);
		return Math.Max(0, Math.Min(num + random.Next(-num2, num2 + 1), ((TroopRosterElement)(ref element)).Number));
	}

	private Dictionary<FormationClass, double> GetFinalRatios(Dictionary<FormationClass, List<CharacterObject>> categorizedTargets)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		Dictionary<FormationClass, double> dictionary = new Dictionary<FormationClass, double>(targetRatios);
		List<FormationClass> list = categorizedTargets.Keys.ToList();
		foreach (FormationClass item in dictionary.Keys.ToList())
		{
			if (!list.Contains(item))
			{
				dictionary.Remove(item);
			}
		}
		double num = dictionary.Values.Sum();
		if (num > 0.0 && num != 1.0 && num != 1.0)
		{
			foreach (FormationClass item2 in dictionary.Keys.ToList())
			{
				dictionary[item2] /= num;
			}
		}
		return dictionary;
	}

	private void PerformUpgrades(TroopRosterElement element, Dictionary<FormationClass, List<CharacterObject>> categorizedTargets, Dictionary<FormationClass, double> finalRatios, int finalUpgradeCount, PartyBase party, Random random)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		foreach (KeyValuePair<FormationClass, List<CharacterObject>> categorizedTarget in categorizedTargets)
		{
			FormationClass key = categorizedTarget.Key;
			List<CharacterObject> value = categorizedTarget.Value;
			double num = finalRatios[key];
			int num2 = (int)Math.Floor((double)finalUpgradeCount * num);
			if (num2 <= 0)
			{
				continue;
			}
			int num3 = num2 / value.Count;
			if (num3 == 0 && num2 > 0)
			{
				CharacterObject val = value[random.Next(value.Count)];
				party.AddMember(element.Character, -1, 0);
				party.AddMember(val, 1, 0);
				continue;
			}
			foreach (CharacterObject item in value)
			{
				party.AddMember(element.Character, -num3, 0);
				party.AddMember(item, num3, 0);
			}
		}
	}

	public void AddSoliderToParty(MobileParty mobileParty, Settlement settlement, Hero hero)
	{
		if (settings.IsRecruitmentEnabled && mobileParty != null && settlement != null && hero != null && !((BasicCharacterObject)hero.CharacterObject).IsPlayerCharacter && !settlement.IsHideout && mobileParty.LeaderHero != null)
		{
			CultureObject culture = mobileParty.LeaderHero.Culture;
			if (((culture != null) ? culture.BasicTroop : null) != null)
			{
				AddSoldiersToParty(mobileParty, mobileParty.LeaderHero, settlement);
			}
		}
	}

	private void AddSoldiersToParty(MobileParty mobileParty, Hero lord, Settlement settlement)
	{
		if (settlement == null || mobileParty == null || lord == null || Hero.MainHero.IsPrisoner || lord.Culture != settlement.Culture || (settings.ExcludePlayerClanFromRecruitment && lord.Clan == Clan.PlayerClan))
		{
			return;
		}
		int partySizeLimit = mobileParty.Party.PartySizeLimit;
		int currentNumber = mobileParty.Party.NumberOfAllMembers;
		if (currentNumber + 20 > partySizeLimit || lord.Gold < settings.MinRecruitmentCost)
		{
			return;
		}
		var (dictionary, dictionary2) = GetAllSoldiers(lord.Culture);
		if (dictionary != null && dictionary2 != null)
		{
			lord.ChangeHeroGold(-settings.SingleRecruitmentCost);
			if (settlement.IsTown)
			{
				AddTroopsToParty(mobileParty, dictionary, ref currentNumber, partySizeLimit);
			}
			else if (settlement.IsVillage)
			{
				AddTroopsToParty(mobileParty, dictionary2, ref currentNumber, partySizeLimit);
			}
		}
	}

	private CharacterObject GetTroopWithLeastMembers(MobileParty mobileParty, List<CharacterObject> tierTroops)
	{
		if (tierTroops.Count == 0)
		{
			return null;
		}
		CharacterObject result = null;
		int num = int.MaxValue;
		foreach (CharacterObject tierTroop in tierTroops)
		{
			int troopCount = mobileParty.MemberRoster.GetTroopCount(tierTroop);
			if (troopCount < num)
			{
				num = troopCount;
				result = tierTroop;
			}
		}
		return result;
	}

	private int CalculateSoldierCount(int tier)
	{
		if (tier >= 3)
		{
			return Math.Max(0, (settings.AIMaxBasicTroopsTier - tier + 1) * 2);
		}
		return 0;
	}

	private void AddTroopsToParty(MobileParty mobileParty, Dictionary<int, List<CharacterObject>> TroopsByTier, ref int currentNumber, int maxNumber)
	{
		int basicSoldierOneTimeRecruitmentAmount = settings.BasicSoldierOneTimeRecruitmentAmount;
		int num = 0;
		for (int num2 = settings.AIMaxBasicTroopsTier; num2 >= 0; num2--)
		{
			List<CharacterObject> tierTroops = (List<CharacterObject>)(TroopsByTier.ContainsKey(num2) ? ((IList)TroopsByTier[num2]) : ((IList)new List<CharacterObject>()));
			int val = CalculateSoldierCount(num2);
			CharacterObject troopWithLeastMembers = GetTroopWithLeastMembers(mobileParty, tierTroops);
			if (troopWithLeastMembers != null)
			{
				int troopCount = mobileParty.MemberRoster.GetTroopCount(troopWithLeastMembers);
				int num3 = Math.Min(val, basicSoldierOneTimeRecruitmentAmount - num);
				mobileParty.MemberRoster.AddToCounts(troopWithLeastMembers, num3, false, 0, 0, true, -1);
				num += num3;
			}
			if (num >= basicSoldierOneTimeRecruitmentAmount)
			{
				break;
			}
		}
		currentNumber = num;
	}

	private (Dictionary<int, List<CharacterObject>> basicTroopsByTier, Dictionary<int, List<CharacterObject>> eliteTroopsByTier) GetAllSoldiers(CultureObject culture)
	{
		if (basicTroopCache.TryGetValue(culture.BasicTroop, out List<CharacterObject> value) && eliteTroopCache.TryGetValue(culture.EliteBasicTroop, out List<CharacterObject> value2))
		{
			return (basicTroopsByTier: GetTroopsByTier(value), eliteTroopsByTier: GetTroopsByTier(value2));
		}
		HashSet<CharacterObject> hashSet = new HashSet<CharacterObject>();
		HashSet<CharacterObject> hashSet2 = new HashSet<CharacterObject>();
		if (culture.BasicTroop != null)
		{
			AddTroopAndUpgrades(culture.BasicTroop, hashSet);
		}
		if (culture.EliteBasicTroop != null)
		{
			AddTroopAndUpgrades(culture.EliteBasicTroop, hashSet2);
		}
		List<CharacterObject> list = hashSet.ToList();
		List<CharacterObject> list2 = hashSet2.ToList();
		basicTroopCache[culture.BasicTroop] = list;
		eliteTroopCache[culture.EliteBasicTroop] = list2;
		return (basicTroopsByTier: GetTroopsByTier(list), eliteTroopsByTier: GetTroopsByTier(list2));
		static void AddTroopAndUpgrades(CharacterObject troop, HashSet<CharacterObject> troopList)
		{
			if (troop != null)
			{
				Stack<CharacterObject> stack = new Stack<CharacterObject>();
				stack.Push(troop);
				while (stack.Count > 0)
				{
					CharacterObject val = stack.Pop();
					if (!troopList.Contains(val))
					{
						troopList.Add(val);
						CharacterObject[] upgradeTargets = val.UpgradeTargets;
						foreach (CharacterObject item in upgradeTargets)
						{
							stack.Push(item);
						}
					}
				}
			}
		}
	}

	private Dictionary<int, List<CharacterObject>> GetTroopsByTier(List<CharacterObject> troops)
	{
		if (troops == null || troops.Count == 0)
		{
			return new Dictionary<int, List<CharacterObject>>();
		}
		return (from t in troops
			group t by t.Tier into g
			orderby g.Key descending
			select g).ToDictionary((IGrouping<int, CharacterObject> g) => g.Key, (IGrouping<int, CharacterObject> g) => g.ToList());
	}

	public override void SyncData(IDataStore dataStore)
	{
	}
}
