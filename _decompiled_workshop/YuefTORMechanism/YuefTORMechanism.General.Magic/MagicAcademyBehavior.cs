using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MCM.Abstractions.Base.Global;
using TOR_Core.AbilitySystem;
using TOR_Core.AbilitySystem.Spells;
using TOR_Core.CharacterDevelopment;
using TOR_Core.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace YuefTORMechanism.General.Magic;

public class MagicAcademyBehavior : CampaignBehaviorBase
{
	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static OnInitDelegate _003C_003E9__6_0;

		public static OnInitDelegate _003C_003E9__6_1;

		public static OnInitDelegate _003C_003E9__6_2;

		public static OnConditionDelegate _003C_003E9__6_3;

		public static OnConsequenceDelegate _003C_003E9__6_4;

		public static OnConditionDelegate _003C_003E9__6_5;

		public static OnConsequenceDelegate _003C_003E9__6_6;

		public static OnConditionDelegate _003C_003E9__6_7;

		public static OnConditionDelegate _003C_003E9__6_9;

		public static OnConsequenceDelegate _003C_003E9__6_10;

		public static OnConditionDelegate _003C_003E9__6_11;

		public static OnConsequenceDelegate _003C_003E9__6_12;

		public static OnConditionDelegate _003C_003E9__6_13;

		public static OnConsequenceDelegate _003C_003E9__6_14;

		public static OnConditionDelegate _003C_003E9__6_15;

		public static OnConsequenceDelegate _003C_003E9__6_16;

		internal void _003CAddGameMenus_003Eb__6_0(MenuCallbackArgs args)
		{
		}

		internal void _003CAddGameMenus_003Eb__6_1(MenuCallbackArgs args)
		{
		}

		internal void _003CAddGameMenus_003Eb__6_2(MenuCallbackArgs args)
		{
		}

		internal bool _003CAddGameMenus_003Eb__6_3(MenuCallbackArgs args)
		{
			return Hero.MainHero != null;
		}

		internal void _003CAddGameMenus_003Eb__6_4(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_academy");
		}

		internal bool _003CAddGameMenus_003Eb__6_5(MenuCallbackArgs args)
		{
			return true;
		}

		internal void _003CAddGameMenus_003Eb__6_6(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town");
		}

		internal bool _003CAddGameMenus_003Eb__6_7(MenuCallbackArgs args)
		{
			return true;
		}

		internal bool _003CAddGameMenus_003Eb__6_9(MenuCallbackArgs args)
		{
			return true;
		}

		internal void _003CAddGameMenus_003Eb__6_10(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("academy_learn_spells");
		}

		internal bool _003CAddGameMenus_003Eb__6_11(MenuCallbackArgs args)
		{
			return true;
		}

		internal void _003CAddGameMenus_003Eb__6_12(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("academy_unlearn_spells");
		}

		internal bool _003CAddGameMenus_003Eb__6_13(MenuCallbackArgs args)
		{
			return true;
		}

		internal void _003CAddGameMenus_003Eb__6_14(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_academy");
		}

		internal bool _003CAddGameMenus_003Eb__6_15(MenuCallbackArgs args)
		{
			return true;
		}

		internal void _003CAddGameMenus_003Eb__6_16(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_academy");
		}
	}

	private Hero _selectedHero = Hero.MainHero;

	private static MCMSetting Settings => GlobalSettings<MCMSetting>.Instance;

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunched);
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	private void OnSessionLaunched(CampaignGameStarter starter)
	{
		if (Settings != null && Settings.Yuef_EnableMagicAcademy)
		{
			AddGameMenus(starter);
		}
	}

	private void AddGameMenus(CampaignGameStarter s)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Expected O, but got Unknown
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Expected O, but got Unknown
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Expected O, but got Unknown
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_0158: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dc: Expected O, but got Unknown
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Expected O, but got Unknown
		//IL_01bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Expected O, but got Unknown
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Expected O, but got Unknown
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Expected O, but got Unknown
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0230: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Expected O, but got Unknown
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Expected O, but got Unknown
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Expected O, but got Unknown
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Expected O, but got Unknown
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02de: Expected O, but got Unknown
		//IL_02f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fd: Expected O, but got Unknown
		//IL_0379: Unknown result type (might be due to invalid IL or missing references)
		//IL_037f: Expected O, but got Unknown
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Expected O, but got Unknown
		//IL_03e8: Expected O, but got Unknown
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Expected O, but got Unknown
		//IL_0436: Unknown result type (might be due to invalid IL or missing references)
		//IL_043b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Expected O, but got Unknown
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0460: Expected O, but got Unknown
		//IL_04a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a8: Expected O, but got Unknown
		//IL_04f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_050d: Expected O, but got Unknown
		//IL_050d: Expected O, but got Unknown
		_selectedHero = Hero.MainHero;
		string text = ((object)new TextObject("{=Yuef_MagicAcademy_MainMenu}Magic Academy", (Dictionary<string, object>)null)).ToString();
		object obj = _003C_003Ec._003C_003E9__6_0;
		if (obj == null)
		{
			OnInitDelegate val = delegate
			{
			};
			_003C_003Ec._003C_003E9__6_0 = val;
			obj = (object)val;
		}
		s.AddGameMenu("town_academy", text, (OnInitDelegate)obj, (MenuOverlayType)0, (MenuFlags)0, (object)null);
		string text2 = ((object)new TextObject("{=Yuef_MagicAcademy_LearnMenu}Learn Spells", (Dictionary<string, object>)null)).ToString();
		object obj2 = _003C_003Ec._003C_003E9__6_1;
		if (obj2 == null)
		{
			OnInitDelegate val2 = delegate
			{
			};
			_003C_003Ec._003C_003E9__6_1 = val2;
			obj2 = (object)val2;
		}
		s.AddGameMenu("academy_learn_spells", text2, (OnInitDelegate)obj2, (MenuOverlayType)0, (MenuFlags)0, (object)null);
		string text3 = ((object)new TextObject("{=Yuef_MagicAcademy_UnlearnMenu}Remove Spells", (Dictionary<string, object>)null)).ToString();
		object obj3 = _003C_003Ec._003C_003E9__6_2;
		if (obj3 == null)
		{
			OnInitDelegate val3 = delegate
			{
			};
			_003C_003Ec._003C_003E9__6_2 = val3;
			obj3 = (object)val3;
		}
		s.AddGameMenu("academy_unlearn_spells", text3, (OnInitDelegate)obj3, (MenuOverlayType)0, (MenuFlags)0, (object)null);
		string text4 = ((object)new TextObject("{=Yuef_MagicAcademy_Enter_Title}★ Open Magic Academy", (Dictionary<string, object>)null)).ToString();
		object obj4 = _003C_003Ec._003C_003E9__6_3;
		if (obj4 == null)
		{
			OnConditionDelegate val4 = (MenuCallbackArgs args) => Hero.MainHero != null;
			_003C_003Ec._003C_003E9__6_3 = val4;
			obj4 = (object)val4;
		}
		object obj5 = _003C_003Ec._003C_003E9__6_4;
		if (obj5 == null)
		{
			OnConsequenceDelegate val5 = delegate
			{
				GameMenu.SwitchToMenu("town_academy");
			};
			_003C_003Ec._003C_003E9__6_4 = val5;
			obj5 = (object)val5;
		}
		s.AddGameMenuOption("town", "Academy_Main", text4, (OnConditionDelegate)obj4, (OnConsequenceDelegate)obj5, false, 5, false, (object)null);
		string text5 = ((object)new TextObject("{=Yuef_MagicAcademy_Return}Return", (Dictionary<string, object>)null)).ToString();
		object obj6 = _003C_003Ec._003C_003E9__6_5;
		if (obj6 == null)
		{
			OnConditionDelegate val6 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__6_5 = val6;
			obj6 = (object)val6;
		}
		object obj7 = _003C_003Ec._003C_003E9__6_6;
		if (obj7 == null)
		{
			OnConsequenceDelegate val7 = delegate
			{
				GameMenu.SwitchToMenu("town");
			};
			_003C_003Ec._003C_003E9__6_6 = val7;
			obj7 = (object)val7;
		}
		s.AddGameMenuOption("town_academy", "Academy_return", text5, (OnConditionDelegate)obj6, (OnConsequenceDelegate)obj7, true, 9, false, (object)null);
		string text6 = ((object)new TextObject("{=Yuef_MagicAcademy_SelectHero}Select Hero", (Dictionary<string, object>)null)).ToString();
		object obj8 = _003C_003Ec._003C_003E9__6_7;
		if (obj8 == null)
		{
			OnConditionDelegate val8 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__6_7 = val8;
			obj8 = (object)val8;
		}
		s.AddGameMenuOption("town_academy", "academy_select_hero", text6, (OnConditionDelegate)obj8, (OnConsequenceDelegate)delegate
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0084: Unknown result type (might be due to invalid IL or missing references)
			//IL_008e: Expected O, but got Unknown
			//IL_0094: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Expected O, but got Unknown
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Expected O, but got Unknown
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d0: Expected O, but got Unknown
			List<InquiryElement> list = new List<InquiryElement>();
			foreach (TroopRosterElement item in (List<TroopRosterElement>)(object)MobileParty.MainParty.MemberRoster.GetTroopRoster())
			{
				if (((BasicCharacterObject)item.Character).IsHero)
				{
					list.Add(new InquiryElement((object)item.Character.HeroObject, ((object)((BasicCharacterObject)item.Character).Name).ToString(), (ImageIdentifier)null));
				}
			}
			MultiSelectionInquiryData val19 = new MultiSelectionInquiryData(((object)new TextObject("{=Yuef_MagicAcademy_HeroSelectTitle}Hero", (Dictionary<string, object>)null)).ToString(), ((object)new TextObject("{=Yuef_MagicAcademy_HeroSelectDesc}Select a Hero", (Dictionary<string, object>)null)).ToString(), list, true, 1, 1, ((object)new TextObject("{=Yuef_MagicAcademy_Confirm}Confirm", (Dictionary<string, object>)null)).ToString(), "", (Action<List<InquiryElement>>)delegate(List<InquiryElement> x1)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Expected O, but got Unknown
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Expected O, but got Unknown
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Expected O, but got Unknown
				_selectedHero = (Hero)x1[0].Identifier;
				string text11 = ((object)new TextObject("{=Yuef_MagicAcademy_HeroSelected}Selected: ", (Dictionary<string, object>)null)).ToString();
				InformationManager.DisplayMessage(new InformationMessage(text11 + (object)_selectedHero.Name));
			}, (Action<List<InquiryElement>>)null, "", false);
			MBInformationManager.ShowMultiSelectionInquiry(val19, false, false);
		}, false, -1, false, (object)null);
		string text7 = ((object)new TextObject("{=Yuef_MagicAcademy_LearnAll}Learn All Spells", (Dictionary<string, object>)null)).ToString();
		object obj9 = _003C_003Ec._003C_003E9__6_9;
		if (obj9 == null)
		{
			OnConditionDelegate val9 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__6_9 = val9;
			obj9 = (object)val9;
		}
		object obj10 = _003C_003Ec._003C_003E9__6_10;
		if (obj10 == null)
		{
			OnConsequenceDelegate val10 = delegate
			{
				GameMenu.SwitchToMenu("academy_learn_spells");
			};
			_003C_003Ec._003C_003E9__6_10 = val10;
			obj10 = (object)val10;
		}
		s.AddGameMenuOption("town_academy", "academy_learn_spells", text7, (OnConditionDelegate)obj9, (OnConsequenceDelegate)obj10, false, 9, false, (object)null);
		string text8 = ((object)new TextObject("{=Yuef_MagicAcademy_UnlearnAll}Unlearn Spells", (Dictionary<string, object>)null)).ToString();
		object obj11 = _003C_003Ec._003C_003E9__6_11;
		if (obj11 == null)
		{
			OnConditionDelegate val11 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__6_11 = val11;
			obj11 = (object)val11;
		}
		object obj12 = _003C_003Ec._003C_003E9__6_12;
		if (obj12 == null)
		{
			OnConsequenceDelegate val12 = delegate
			{
				GameMenu.SwitchToMenu("academy_unlearn_spells");
			};
			_003C_003Ec._003C_003E9__6_12 = val12;
			obj12 = (object)val12;
		}
		s.AddGameMenuOption("town_academy", "academy_unlearn_spells", text8, (OnConditionDelegate)obj11, (OnConsequenceDelegate)obj12, false, 8, false, (object)null);
		string text9 = ((object)new TextObject("{=Yuef_MagicAcademy_Back}Return", (Dictionary<string, object>)null)).ToString();
		object obj13 = _003C_003Ec._003C_003E9__6_13;
		if (obj13 == null)
		{
			OnConditionDelegate val13 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__6_13 = val13;
			obj13 = (object)val13;
		}
		object obj14 = _003C_003Ec._003C_003E9__6_14;
		if (obj14 == null)
		{
			OnConsequenceDelegate val14 = delegate
			{
				GameMenu.SwitchToMenu("town_academy");
			};
			_003C_003Ec._003C_003E9__6_14 = val14;
			obj14 = (object)val14;
		}
		s.AddGameMenuOption("academy_learn_spells", "spell_return", text9, (OnConditionDelegate)obj13, (OnConsequenceDelegate)obj14, true, 9, false, (object)null);
		foreach (LoreObject lore in LoreObject.GetAll())
		{
			int cost = 10000;
			if (!HeroExtensions.HasAttribute(_selectedHero, "SpellCaster"))
			{
				cost *= 2;
			}
			TextObject val15 = new TextObject("{=Yuef_MagicAcademy_LearnSpell}Learn: {SPELL_NAME}", (Dictionary<string, object>)null);
			val15.SetTextVariable("SPELL_NAME", lore.Name.ToString());
			s.AddGameMenuOption("academy_learn_spells", "Master_" + lore.ID, ((object)val15).ToString(), (OnConditionDelegate)delegate(MenuCallbackArgs args)
			{
				//IL_0036: Unknown result type (might be due to invalid IL or missing references)
				//IL_003c: Expected O, but got Unknown
				//IL_008b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0091: Expected O, but got Unknown
				if (HeroExtensions.GetExtendedInfo(_selectedHero).HasKnownLore(lore.ID))
				{
					TextObject val19 = new TextObject("{=Yuef_MagicAcademy_AlreadyKnown}{HERO_NAME} already knows this spell.", (Dictionary<string, object>)null);
					val19.SetTextVariable("HERO_NAME", _selectedHero.Name);
					args.Tooltip = val19;
					args.IsEnabled = false;
				}
				else if (Hero.MainHero.Gold < cost)
				{
					TextObject val20 = new TextObject("{=Yuef_MagicAcademy_NeedGold}Need {GOLD} gold.", (Dictionary<string, object>)null);
					val20.SetTextVariable("GOLD", cost);
					args.Tooltip = val20;
					args.IsEnabled = false;
				}
				return true;
			}, (OnConsequenceDelegate)delegate
			{
				//IL_013b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0141: Expected O, but got Unknown
				//IL_0163: Unknown result type (might be due to invalid IL or missing references)
				//IL_016d: Expected O, but got Unknown
				if (!HeroExtensions.HasAttribute(_selectedHero, "SpellCaster"))
				{
					HeroExtensions.AddAttribute(_selectedHero, "SpellCaster");
					_selectedHero.AddSkillXp(TORSkills.Spellcraft, 5000f);
				}
				HeroExtensions.AddKnownLore(_selectedHero, lore.ID);
				foreach (string allSpellNamesAs in AbilityFactory.GetAllSpellNamesAsList())
				{
					AbilityTemplate template = AbilityFactory.GetTemplate(allSpellNamesAs);
					if (template != null && template.BelongsToLoreID == lore.ID)
					{
						HeroExtensions.AddAbility(_selectedHero, allSpellNamesAs);
						HeroExtensions.GetExtendedInfo(_selectedHero).AddSelectedAbility(allSpellNamesAs);
					}
				}
				Hero mainHero = Hero.MainHero;
				mainHero.Gold -= cost;
				TextObject val19 = new TextObject("{=Yuef_MagicAcademy_Learned}Learned: {SPELL_NAME}", (Dictionary<string, object>)null);
				val19.SetTextVariable("SPELL_NAME", lore.Name);
				InformationManager.DisplayMessage(new InformationMessage(((object)val19).ToString()));
			}, true, 9, false, (object)null);
		}
		string text10 = ((object)new TextObject("{=Yuef_MagicAcademy_Back}Return", (Dictionary<string, object>)null)).ToString();
		object obj15 = _003C_003Ec._003C_003E9__6_15;
		if (obj15 == null)
		{
			OnConditionDelegate val16 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__6_15 = val16;
			obj15 = (object)val16;
		}
		object obj16 = _003C_003Ec._003C_003E9__6_16;
		if (obj16 == null)
		{
			OnConsequenceDelegate val17 = delegate
			{
				GameMenu.SwitchToMenu("town_academy");
			};
			_003C_003Ec._003C_003E9__6_16 = val17;
			obj16 = (object)val17;
		}
		s.AddGameMenuOption("academy_unlearn_spells", "unspell_return", text10, (OnConditionDelegate)obj15, (OnConsequenceDelegate)obj16, true, 9, false, (object)null);
		foreach (LoreObject lore2 in LoreObject.GetAll())
		{
			TextObject val18 = new TextObject("{=Yuef_MagicAcademy_UnlearnSpell}Remove: {SPELL_NAME}", (Dictionary<string, object>)null);
			val18.SetTextVariable("SPELL_NAME", lore2.Name.ToString());
			s.AddGameMenuOption("academy_unlearn_spells", "UnMaster_" + lore2.ID, ((object)val18).ToString(), (OnConditionDelegate)delegate(MenuCallbackArgs args)
			{
				//IL_002f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0035: Expected O, but got Unknown
				if (!HeroExtensions.GetExtendedInfo(_selectedHero).HasKnownLore(lore2.ID))
				{
					TextObject val19 = new TextObject("{=Yuef_MagicAcademy_NotKnown}{HERO_NAME} does not know this spell.", (Dictionary<string, object>)null);
					val19.SetTextVariable("HERO_NAME", _selectedHero.Name);
					args.Tooltip = val19;
					args.IsEnabled = false;
				}
				return true;
			}, (OnConsequenceDelegate)delegate
			{
				//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ef: Expected O, but got Unknown
				//IL_010c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0116: Expected O, but got Unknown
				foreach (string allSpellNamesAs2 in AbilityFactory.GetAllSpellNamesAsList())
				{
					AbilityTemplate template = AbilityFactory.GetTemplate(allSpellNamesAs2);
					if (template != null && template.BelongsToLoreID == lore2.ID)
					{
						HeroExtensions.GetExtendedInfo(_selectedHero).RemoveAbility(allSpellNamesAs2);
					}
				}
				HeroExtensions.GetExtendedInfo(_selectedHero).RemoveKnownLore(lore2.ID);
				if (HeroExtensions.GetExtendedInfo(_selectedHero).GetKnownLoreCount() == 0 && HeroExtensions.HasAttribute(_selectedHero, "SpellCaster"))
				{
					HeroExtensions.RemoveAttribute(_selectedHero, "SpellCaster");
				}
				TextObject val19 = new TextObject("{=Yuef_MagicAcademy_Removed}Removed: {SPELL_NAME}", (Dictionary<string, object>)null);
				val19.SetTextVariable("SPELL_NAME", lore2.Name);
				InformationManager.DisplayMessage(new InformationMessage(((object)val19).ToString()));
				GameMenu.SwitchToMenu("academy_unlearn_spells");
			}, true, 9, false, (object)null);
		}
	}
}
