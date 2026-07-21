using System;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TOR_Core.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Party.PartyComponents;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ObjectSystem;

namespace TOR_Core.CampaignMechanics.Assimilation;

public class AssimilationCampaignBehavior : CampaignBehaviorBase
{
	private Dictionary<Settlement, CultureObject> _settlementCulturePairs = new Dictionary<Settlement, CultureObject>();

	private Dictionary<Settlement, CultureObject> _originalSettlementCulturePairs = new Dictionary<Settlement, CultureObject>();

	private readonly Dictionary<string, List<CharacterObject>> _mercenaryTroopsByCulture = new Dictionary<string, List<CharacterObject>>();

	private bool _isMercenaryTroopCacheInitialized;

	public static CultureObject GetOriginalCultureForSettlement(Settlement settlement)
	{
		if (Campaign.Current == null || Campaign.Current.GetCampaignBehavior<AssimilationCampaignBehavior>() == null)
		{
			return settlement.Culture;
		}
		AssimilationCampaignBehavior campaignBehavior = Campaign.Current.GetCampaignBehavior<AssimilationCampaignBehavior>();
		if (campaignBehavior._originalSettlementCulturePairs.ContainsKey(settlement))
		{
			return campaignBehavior._originalSettlementCulturePairs[settlement];
		}
		return settlement.Culture;
	}

	public override void RegisterEvents()
	{
		CampaignEvents.OnSettlementOwnerChangedEvent.AddNonSerializedListener((object)this, (Action<Settlement, bool, Hero, Hero, Hero, ChangeOwnerOfSettlementDetail>)SettlementOwnerChanged);
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
		CampaignEvents.OnNewGameCreatedPartialFollowUpEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter, int>)OnNewGameStart);
		CampaignEvents.OnBeforeSaveEvent.AddNonSerializedListener((object)this, (Action)BeforeSave);
		CampaignEvents.OnTroopRecruitedEvent.AddNonSerializedListener((object)this, (Action<Hero, Settlement, Hero, CharacterObject, int>)OnTroopRecruited);
		CampaignEvents.DailyTickSettlementEvent.AddNonSerializedListener((object)this, (Action<Settlement>)DailyTickSettlement);
		CampaignEvents.DailyTickPartyEvent.AddNonSerializedListener((object)this, (Action<MobileParty>)DailyTickParty);
		CampaignEvents.OnLootDistributedToPartyEvent.AddNonSerializedListener((object)this, (Action<PartyBase, PartyBase, ItemRoster>)OnDistributeLootToParty);
		CampaignEvents.HeroKilledEvent.AddNonSerializedListener((object)this, (Action<Hero, Hero, KillCharacterActionDetail, bool>)OnNotableKilled);
	}

	private void OnNotableKilled(Hero victim, Hero killer, KillCharacterActionDetail detail, bool showNotification)
	{
		if (!victim.IsNotable)
		{
			return;
		}
		foreach (CaravanPartyComponent item in victim.OwnedCaravans.ToList())
		{
			DestroyPartyAction.Apply((PartyBase)null, ((PartyComponent)item).MobileParty);
		}
	}

	private void OnDistributeLootToParty(PartyBase winnerParty, PartyBase defeatedParty, ItemRoster roster)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		foreach (TroopRosterElement item in ((IEnumerable<TroopRosterElement>)winnerParty.MemberRoster.GetTroopRoster()).ToList())
		{
			TroopRosterElement current = item;
			SwapTroopsIfNeeded(winnerParty.LeaderHero, winnerParty.MemberRoster, current.Character, ((TroopRosterElement)(ref current)).Number);
		}
	}

	private void DailyTickSettlement(Settlement settlement)
	{
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		object obj;
		if (settlement == null)
		{
			obj = null;
		}
		else
		{
			Town town = settlement.Town;
			if (town == null)
			{
				obj = null;
			}
			else
			{
				MobileParty garrisonParty = ((Fief)town).GarrisonParty;
				obj = ((garrisonParty != null) ? garrisonParty.MemberRoster : null);
			}
		}
		if (obj == null || ((settlement != null) ? settlement.Owner : null) == null || settlement.IsUnderSiege || settlement.InRebelliousState || ((object)settlement.OwnerClan).Equals((object)Clan.PlayerClan))
		{
			return;
		}
		foreach (TroopRosterElement item in ((IEnumerable<TroopRosterElement>)((Fief)settlement.Town).GarrisonParty.MemberRoster.GetTroopRoster()).ToList())
		{
			TroopRosterElement current = item;
			Hero owner = settlement.Owner;
			MobileParty garrisonParty2 = ((Fief)settlement.Town).GarrisonParty;
			SwapTroopsIfNeeded(owner, (garrisonParty2 != null) ? garrisonParty2.MemberRoster : null, current.Character, ((TroopRosterElement)(ref current)).Number);
		}
	}

	private void DailyTickParty(MobileParty party)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Invalid comparison between Unknown and I4
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Invalid comparison between Unknown and I4
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		if (!party.IsCaravan)
		{
			return;
		}
		EnsureMercenaryTroopCacheInitialized();
		object obj = party.HomeSettlement?.Culture;
		if (obj == null)
		{
			Clan actualClan = party.ActualClan;
			obj = ((actualClan != null) ? actualClan.Culture : null);
			if (obj == null)
			{
				IFaction mapFaction = party.Party.MapFaction;
				obj = ((mapFaction != null) ? mapFaction.Culture : null) ?? party.LeaderHero?.Culture;
			}
		}
		CultureObject val = (CultureObject)obj;
		if (val == null)
		{
			return;
		}
		foreach (TroopRosterElement item in ((IEnumerable<TroopRosterElement>)party.MemberRoster.GetTroopRoster()).ToList())
		{
			TroopRosterElement current = item;
			CharacterObject character = current.Character;
			if (!((BasicCharacterObject)character).IsHero && ((MBObjectBase)character).StringId.StartsWith("tor_") && character.Culture != val && ((int)character.Occupation == 2 || (int)character.Occupation == 30))
			{
				CharacterObject val2 = FindMercenaryEquivalent(val, character);
				if (val2 != null)
				{
					int number = ((TroopRosterElement)(ref current)).Number;
					party.MemberRoster.RemoveTroop(character, number, default(UniqueTroopDescriptor), 0);
					party.MemberRoster.AddToCounts(val2, number, false, 0, 0, true, -1);
				}
			}
		}
		party.MemberRoster.RemoveZeroCounts();
	}

	private void EnsureMercenaryTroopCacheInitialized()
	{
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Invalid comparison between Unknown and I4
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Invalid comparison between Unknown and I4
		if (_isMercenaryTroopCacheInitialized)
		{
			return;
		}
		foreach (CharacterObject item in (List<CharacterObject>)(object)MBObjectManager.Instance.GetObjectTypeList<CharacterObject>())
		{
			if (!((BasicCharacterObject)item).IsHero && item.Culture != null && ((int)item.Occupation == 2 || (int)item.Occupation == 30) && ((MBObjectBase)item).StringId.StartsWith("tor_"))
			{
				if (!_mercenaryTroopsByCulture.TryGetValue(((MBObjectBase)item.Culture).StringId, out var value))
				{
					value = new List<CharacterObject>();
					_mercenaryTroopsByCulture.Add(((MBObjectBase)item.Culture).StringId, value);
				}
				value.Add(item);
			}
		}
		_isMercenaryTroopCacheInitialized = true;
	}

	private CharacterObject FindMercenaryEquivalent(CultureObject targetCulture, CharacterObject sourceTroop)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (!_mercenaryTroopsByCulture.TryGetValue(((MBObjectBase)targetCulture).StringId, out var value))
		{
			return null;
		}
		FormationClass desiredFallbackClass = FormationClassExtensions.FallbackClass(((BasicCharacterObject)sourceTroop).DefaultFormationClass);
		List<CharacterObject> list = value.Where((CharacterObject x) => x.Occupation == sourceTroop.Occupation).ToList();
		List<CharacterObject> list2 = ((list.Count > 0) ? list : value);
		List<CharacterObject> list3 = list2.Where((CharacterObject x) => FormationClassExtensions.FallbackClass(((BasicCharacterObject)x).DefaultFormationClass) == desiredFallbackClass && x.Tier == sourceTroop.Tier).ToList();
		if (list3.Count > 0)
		{
			return list3[MBRandom.RandomInt(list3.Count)];
		}
		List<CharacterObject> list4 = list2.Where((CharacterObject x) => FormationClassExtensions.FallbackClass(((BasicCharacterObject)x).DefaultFormationClass) == desiredFallbackClass).ToList();
		if (list4.Count > 0)
		{
			return list4[MBRandom.RandomInt(list4.Count)];
		}
		List<CharacterObject> list5 = list2.Where((CharacterObject x) => x.Tier == sourceTroop.Tier).ToList();
		if (list5.Count > 0)
		{
			return list5[MBRandom.RandomInt(list5.Count)];
		}
		return (list2.Count > 0) ? list2[MBRandom.RandomInt(list2.Count)] : null;
	}

	private void OnTroopRecruited(Hero recruiter, Settlement settlement, Hero recruitmentSource, CharacterObject troop, int count)
	{
		object roster;
		if (recruiter == null)
		{
			roster = null;
		}
		else
		{
			MobileParty partyBelongedTo = recruiter.PartyBelongedTo;
			roster = ((partyBelongedTo != null) ? partyBelongedTo.MemberRoster : null);
		}
		SwapTroopsIfNeeded(recruiter, (TroopRoster)roster, troop, count);
		MobileParty val = ((recruiter != null) ? recruiter.PartyBelongedTo : null);
		if (val != null && val.IsCaravan)
		{
			DailyTickParty(val);
		}
	}

	private void OnNewGameStart(CampaignGameStarter starter, int index)
	{
		if (index != 0)
		{
			return;
		}
		foreach (Settlement item in (List<Settlement>)(object)Settlement.All)
		{
			_settlementCulturePairs.Add(item, item.Culture);
			_originalSettlementCulturePairs.Add(item, item.Culture);
		}
	}

	private void BeforeSave()
	{
		foreach (Settlement item in (List<Settlement>)(object)Settlement.All)
		{
			if (_settlementCulturePairs.ContainsKey(item))
			{
				_settlementCulturePairs[item] = item.Culture;
			}
			else
			{
				_settlementCulturePairs.Add(item, item.Culture);
			}
		}
	}

	private void OnSessionLaunched(CampaignGameStarter starter)
	{
		foreach (Settlement key in _settlementCulturePairs.Keys)
		{
			CultureObject val = _settlementCulturePairs[key];
			if (key.Culture == val)
			{
				continue;
			}
			key.Culture = val;
			foreach (Hero item in (List<Hero>)(object)key.Notables)
			{
				if (item.Culture != key.Culture)
				{
					item.Culture = key.Culture;
				}
			}
		}
	}

	private void SwapTroopsIfNeeded(Hero owner, TroopRoster roster, CharacterObject troop, int count)
	{
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		if (troop.Culture == owner?.Culture || owner == null || ((((MBObjectBase)troop.Culture).StringId == "khuzait" || ((MBObjectBase)troop.Culture).StringId == "mousillon") && (((MBObjectBase)owner.Culture).StringId == "khuzait" || ((MBObjectBase)owner.Culture).StringId == "mousillon")) || owner.Clan == null || owner == Hero.MainHero)
		{
			return;
		}
		MobileParty partyBelongedTo = owner.PartyBelongedTo;
		if (((partyBelongedTo != null) ? partyBelongedTo.Party : null) == null || roster == null || ((BasicCharacterObject)troop).IsHero)
		{
			return;
		}
		roster.ValidateTroopListCache();
		int troopCount = roster.GetTroopCount(troop);
		if (troopCount < 1)
		{
			return;
		}
		count = troopCount;
		if (owner.Clan.DefaultPartyTemplate == null)
		{
			return;
		}
		List<CharacterObject> list = new List<CharacterObject>();
		foreach (PartyTemplateStack item in (List<PartyTemplateStack>)(object)owner.Clan.DefaultPartyTemplate.Stacks)
		{
			List<CharacterObject> list2 = new List<CharacterObject>();
			list2.AddRange(list);
			list2.AddRange(CharacterHelper.GetTroopTree(item.Character, -1f, float.MaxValue));
			list = list2;
		}
		FormationClass troopClass = FormationClassExtensions.FallbackClass(((BasicCharacterObject)troop).DefaultFormationClass);
		List<CharacterObject> templateCharacters = list.Where((CharacterObject c) => FormationClassExtensions.FallbackClass(((BasicCharacterObject)c).DefaultFormationClass) == troopClass).ToList();
		CharacterObject val = DetermineReplacement(templateCharacters, troop.Tier, IsEliteTroop(troop));
		if (val == null)
		{
			val = DetermineReplacement(list, troop.Tier, IsEliteTroop(troop));
		}
		if (val == null)
		{
			val = DetermineReplacement(templateCharacters, troop.Tier, !IsEliteTroop(troop));
		}
		if (val == null)
		{
			val = DetermineReplacement(list, troop.Tier, !IsEliteTroop(troop));
		}
		if (val != null)
		{
			roster.RemoveTroop(troop, count, default(UniqueTroopDescriptor), 0);
			roster.AddToCounts(val, count, false, 0, 0, true, -1);
			roster.RemoveZeroCounts();
			if (val.Tier != troop.Tier || IsEliteTroop(val) != IsEliteTroop(troop))
			{
				ExplainedNumber troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(troop, owner, false);
				int num = (int)((ExplainedNumber)(ref troopRecruitmentCost)).ResultNumber;
				troopRecruitmentCost = Campaign.Current.Models.PartyWageModel.GetTroopRecruitmentCost(val, owner, false);
				int num2 = (int)((ExplainedNumber)(ref troopRecruitmentCost)).ResultNumber;
				GiveGoldAction.ApplyBetweenCharacters((Hero)null, owner, (num - num2) * count, false);
			}
		}
	}

	private CharacterObject DetermineReplacement(List<CharacterObject> templateCharacters, int troopTier, bool useElite)
	{
		CharacterObject val = null;
		val = templateCharacters.Where((CharacterObject t) => t.Tier == troopTier && IsEliteTroop(t) == useElite).TakeRandom(1).FirstOrDefault();
		if (val == null)
		{
			val = templateCharacters.Where((CharacterObject t) => t.Tier == troopTier).TakeRandom(1).FirstOrDefault();
		}
		if (val == null)
		{
			val = templateCharacters.TakeRandom(1).FirstOrDefault();
		}
		return val;
	}

	private bool IsEliteTroop(CharacterObject unit)
	{
		IEnumerable<CharacterObject> troopTree = CharacterHelper.GetTroopTree(unit.Culture.EliteBasicTroop, -1f, float.MaxValue);
		return troopTree.Contains(unit);
	}

	private void SettlementOwnerChanged(Settlement settlement, bool openToClaim, Hero newOwner, Hero oldOwner, Hero capturerHero, ChangeOwnerOfSettlementDetail detail)
	{
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Invalid comparison between Unknown and I4
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Unknown result type (might be due to invalid IL or missing references)
		if (IsSpecialSettlement(settlement))
		{
			return;
		}
		if (settlement.IsFortification && ((Fief)settlement.Town).GarrisonParty != null)
		{
			((Fief)settlement.Town).GarrisonParty.RecentEventsMorale = 0f;
		}
		if (newOwner.MapFaction != null && oldOwner.MapFaction != null && newOwner.MapFaction.Culture != settlement.Culture)
		{
			settlement.Culture = newOwner.MapFaction.Culture;
			foreach (Hero item in ((IEnumerable<Hero>)settlement.Notables).ToList())
			{
				if (item.Culture != settlement.Culture)
				{
					Occupation occupation = item.Occupation;
					KillCharacterAction.ApplyByRemove(item, false, true);
					EnterSettlementAction.ApplyForCharacterOnly(HeroCreator.CreateNotable(occupation, settlement), settlement);
				}
			}
			if (settlement.BoundVillages != null && ((List<Village>)(object)settlement.BoundVillages).Count > 0)
			{
				foreach (Village item2 in (List<Village>)(object)settlement.BoundVillages)
				{
					((SettlementComponent)item2).Settlement.Culture = settlement.Culture;
					foreach (Hero item3 in ((IEnumerable<Hero>)((SettlementComponent)item2).Settlement.Notables).ToList())
					{
						if (item3.Culture != settlement.Culture)
						{
							Occupation occupation2 = item3.Occupation;
							KillCharacterAction.ApplyByRemove(item3, false, true);
							EnterSettlementAction.ApplyForCharacterOnly(HeroCreator.CreateNotable(occupation2, ((SettlementComponent)item2).Settlement), ((SettlementComponent)item2).Settlement);
						}
					}
				}
			}
		}
		if ((int)detail != 4 && (!newOwner.IsKingdomLeader || newOwner != Hero.MainHero))
		{
			return;
		}
		foreach (Hero item4 in (List<Hero>)(object)settlement.Notables)
		{
			item4.SetPersonalRelation(newOwner, 20);
		}
	}

	private bool IsSpecialSettlement(Settlement settlement)
	{
		return ((MBObjectBase)settlement).StringId == "castle_BK1";
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData<Dictionary<Settlement, CultureObject>>("_settlementCulturePairs", ref _settlementCulturePairs);
		dataStore.SyncData<Dictionary<Settlement, CultureObject>>("_originalSettlementCulturePairs", ref _originalSettlementCulturePairs);
	}
}
