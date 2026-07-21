using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MCM.Abstractions.Base.Global;
using TOR_Core.AbilitySystem.Spells;
using TOR_Core.CampaignMechanics.CustomResources;
using TOR_Core.CharacterDevelopment;
using TOR_Core.Extensions;
using TOR_Core.Extensions.ExtendedInfoSystem;
using TOR_Core.Items;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;
using TaleWorlds.Core.ImageIdentifiers;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace YuefTORMechanism.General.Enchantment;

public class EnchantmentAcademyBehavior : CampaignBehaviorBase
{
	private struct BlueprintData
	{
		public ItemObject Item;

		public string BlueprintId;

		public SkillObject RequiredSkill;

		public int RequiredSkillValue;

		public string Restriction;
	}

	[Serializable]
	[CompilerGenerated]
	private sealed class _003C_003Ec
	{
		public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

		public static Func<ItemTrait, bool> _003C_003E9__8_0;

		public static OnInitDelegate _003C_003E9__10_0;

		public static OnInitDelegate _003C_003E9__10_1;

		public static OnConditionDelegate _003C_003E9__10_2;

		public static OnConsequenceDelegate _003C_003E9__10_3;

		public static OnConditionDelegate _003C_003E9__10_4;

		public static OnConsequenceDelegate _003C_003E9__10_5;

		public static OnConditionDelegate _003C_003E9__10_6;

		public static OnConditionDelegate _003C_003E9__10_8;

		public static OnConsequenceDelegate _003C_003E9__10_9;

		public static OnConditionDelegate _003C_003E9__10_10;

		public static OnConsequenceDelegate _003C_003E9__10_11;

		internal bool _003CDiscoverAllBlueprints_003Eb__8_0(ItemTrait t)
		{
			return t.OnInventoryUseScript != (InventoryScriptTuple)null && t.OnInventoryUseScript.InventoryScriptName != null && t.OnInventoryUseScript.InventoryScriptName.Contains("EnchantmentBlueprintScript");
		}

		internal void _003CAddGameMenus_003Eb__10_0(MenuCallbackArgs args)
		{
		}

		internal void _003CAddGameMenus_003Eb__10_1(MenuCallbackArgs args)
		{
		}

		internal bool _003CAddGameMenus_003Eb__10_2(MenuCallbackArgs args)
		{
			return Hero.MainHero != null;
		}

		internal void _003CAddGameMenus_003Eb__10_3(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_enchant_academy");
		}

		internal bool _003CAddGameMenus_003Eb__10_4(MenuCallbackArgs args)
		{
			return true;
		}

		internal void _003CAddGameMenus_003Eb__10_5(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town");
		}

		internal bool _003CAddGameMenus_003Eb__10_6(MenuCallbackArgs args)
		{
			return true;
		}

		internal bool _003CAddGameMenus_003Eb__10_8(MenuCallbackArgs args)
		{
			return true;
		}

		internal void _003CAddGameMenus_003Eb__10_9(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("academy_enchant_learn");
		}

		internal bool _003CAddGameMenus_003Eb__10_10(MenuCallbackArgs args)
		{
			return true;
		}

		internal void _003CAddGameMenus_003Eb__10_11(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("town_enchant_academy");
		}
	}

	private Hero _selectedHero = Hero.MainHero;

	private List<BlueprintData> _allBlueprints;

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
		if (Settings != null && Settings.Yuef_EnableEnchantmentAcademy)
		{
			_selectedHero = Hero.MainHero;
			_allBlueprints = DiscoverAllBlueprints();
			AddGameMenus(starter);
		}
	}

	private static List<BlueprintData> DiscoverAllBlueprints()
	{
		List<BlueprintData> list = new List<BlueprintData>();
		List<SkillObject> defaultSkills = SkillExtensions.GetDefaultSkills(Game.Current.DefaultSkills);
		defaultSkills.AddRange(SkillExtensions.GetTorSkills(TORSkills.Instance));
		foreach (ItemObject item in (List<ItemObject>)(object)MBObjectManager.Instance.GetObjectTypeList<ItemObject>())
		{
			if (!ItemObjectExtensions.IsInventoryUsable(item))
			{
				continue;
			}
			ItemTrait val = ItemObjectExtensions.GetTraits(item).FirstOrDefault((ItemTrait t) => t.OnInventoryUseScript != (InventoryScriptTuple)null && t.OnInventoryUseScript.InventoryScriptName != null && t.OnInventoryUseScript.InventoryScriptName.Contains("EnchantmentBlueprintScript"));
			if (val == (ItemTrait)null)
			{
				continue;
			}
			List<string> args = val.OnInventoryUseScript.InventoryScriptArguments;
			if (args != null && args.Count >= 3)
			{
				SkillObject val2 = defaultSkills.FirstOrDefault((SkillObject s) => ((MBObjectBase)s).StringId == args[1]);
				if (val2 != null && int.TryParse(args[2], out var result))
				{
					list.Add(new BlueprintData
					{
						Item = item,
						BlueprintId = args[0],
						RequiredSkill = val2,
						RequiredSkillValue = result,
						Restriction = ((args.Count > 3) ? args[3] : null)
					});
				}
			}
		}
		return list;
	}

	private bool CanHeroLearnBlueprint(Hero hero, BlueprintData bp)
	{
		if (HeroExtensions.HasKnownEnchantmentBlueprint(hero, bp.BlueprintId))
		{
			return false;
		}
		if (!string.IsNullOrEmpty(bp.Restriction))
		{
			HeroExtendedInfo extendedInfo = HeroExtensions.GetExtendedInfo(hero);
			bool valueOrDefault = ((extendedInfo == null) ? ((bool?)null) : extendedInfo.KnownLores?.Any((LoreObject l) => l != null && l.ID == bp.Restriction)) == true;
			bool flag = HeroExtensions.HasAttribute(hero, bp.Restriction);
			if (!valueOrDefault && !flag)
			{
				return false;
			}
		}
		return true;
	}

	private void AddGameMenus(CampaignGameStarter s)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Expected O, but got Unknown
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Expected O, but got Unknown
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Expected O, but got Unknown
		//IL_0185: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Expected O, but got Unknown
		//IL_01a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Expected O, but got Unknown
		//IL_0209: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Expected O, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Expected O, but got Unknown
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Expected O, but got Unknown
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Expected O, but got Unknown
		//IL_03ba: Expected O, but got Unknown
		string text = ((object)new TextObject("{=Yuef_EnchantAcademy_MainMenu}Enchantment Academy", (Dictionary<string, object>)null)).ToString();
		object obj = _003C_003Ec._003C_003E9__10_0;
		if (obj == null)
		{
			OnInitDelegate val = delegate
			{
			};
			_003C_003Ec._003C_003E9__10_0 = val;
			obj = (object)val;
		}
		s.AddGameMenu("town_enchant_academy", text, (OnInitDelegate)obj, (MenuOverlayType)0, (MenuFlags)0, (object)null);
		string text2 = ((object)new TextObject("{=Yuef_EnchantAcademy_LearnMenu}Learn Enchantments", (Dictionary<string, object>)null)).ToString();
		object obj2 = _003C_003Ec._003C_003E9__10_1;
		if (obj2 == null)
		{
			OnInitDelegate val2 = delegate
			{
			};
			_003C_003Ec._003C_003E9__10_1 = val2;
			obj2 = (object)val2;
		}
		s.AddGameMenu("academy_enchant_learn", text2, (OnInitDelegate)obj2, (MenuOverlayType)0, (MenuFlags)0, (object)null);
		string text3 = ((object)new TextObject("{=Yuef_EnchantAcademy_Enter_Title}★ Enter Enchantment Academy", (Dictionary<string, object>)null)).ToString();
		object obj3 = _003C_003Ec._003C_003E9__10_2;
		if (obj3 == null)
		{
			OnConditionDelegate val3 = (MenuCallbackArgs args) => Hero.MainHero != null;
			_003C_003Ec._003C_003E9__10_2 = val3;
			obj3 = (object)val3;
		}
		object obj4 = _003C_003Ec._003C_003E9__10_3;
		if (obj4 == null)
		{
			OnConsequenceDelegate val4 = delegate
			{
				GameMenu.SwitchToMenu("town_enchant_academy");
			};
			_003C_003Ec._003C_003E9__10_3 = val4;
			obj4 = (object)val4;
		}
		s.AddGameMenuOption("town", "EnchantAcademy_Main", text3, (OnConditionDelegate)obj3, (OnConsequenceDelegate)obj4, false, 4, false, (object)null);
		string text4 = ((object)new TextObject("{=Yuef_EnchantAcademy_Return}Return", (Dictionary<string, object>)null)).ToString();
		object obj5 = _003C_003Ec._003C_003E9__10_4;
		if (obj5 == null)
		{
			OnConditionDelegate val5 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__10_4 = val5;
			obj5 = (object)val5;
		}
		object obj6 = _003C_003Ec._003C_003E9__10_5;
		if (obj6 == null)
		{
			OnConsequenceDelegate val6 = delegate
			{
				GameMenu.SwitchToMenu("town");
			};
			_003C_003Ec._003C_003E9__10_5 = val6;
			obj6 = (object)val6;
		}
		s.AddGameMenuOption("town_enchant_academy", "EnchantAcademy_return", text4, (OnConditionDelegate)obj5, (OnConsequenceDelegate)obj6, true, 9, false, (object)null);
		string text5 = ((object)new TextObject("{=Yuef_EnchantAcademy_SelectHero}Select Hero", (Dictionary<string, object>)null)).ToString();
		object obj7 = _003C_003Ec._003C_003E9__10_6;
		if (obj7 == null)
		{
			OnConditionDelegate val7 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__10_6 = val7;
			obj7 = (object)val7;
		}
		s.AddGameMenuOption("town_enchant_academy", "enchant_select_hero", text5, (OnConditionDelegate)obj7, (OnConsequenceDelegate)delegate
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
			MultiSelectionInquiryData val13 = new MultiSelectionInquiryData(((object)new TextObject("{=Yuef_EnchantAcademy_HeroSelectTitle}Hero", (Dictionary<string, object>)null)).ToString(), ((object)new TextObject("{=Yuef_EnchantAcademy_HeroSelectDesc}Select a Hero", (Dictionary<string, object>)null)).ToString(), list, true, 1, 1, ((object)new TextObject("{=Yuef_EnchantAcademy_Confirm}Confirm", (Dictionary<string, object>)null)).ToString(), "", (Action<List<InquiryElement>>)delegate(List<InquiryElement> x1)
			{
				//IL_000e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0018: Expected O, but got Unknown
				//IL_001e: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Expected O, but got Unknown
				//IL_0046: Unknown result type (might be due to invalid IL or missing references)
				//IL_0050: Expected O, but got Unknown
				_selectedHero = (Hero)x1[0].Identifier;
				string text9 = ((object)new TextObject("{=Yuef_EnchantAcademy_HeroSelected}Selected: ", (Dictionary<string, object>)null)).ToString();
				InformationManager.DisplayMessage(new InformationMessage(text9 + (object)_selectedHero.Name));
			}, (Action<List<InquiryElement>>)null, "", false);
			MBInformationManager.ShowMultiSelectionInquiry(val13, false, false);
		}, false, -1, false, (object)null);
		string text6 = ((object)new TextObject("{=Yuef_EnchantAcademy_LearnAll}Learn Enchantments", (Dictionary<string, object>)null)).ToString();
		object obj8 = _003C_003Ec._003C_003E9__10_8;
		if (obj8 == null)
		{
			OnConditionDelegate val8 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__10_8 = val8;
			obj8 = (object)val8;
		}
		object obj9 = _003C_003Ec._003C_003E9__10_9;
		if (obj9 == null)
		{
			OnConsequenceDelegate val9 = delegate
			{
				GameMenu.SwitchToMenu("academy_enchant_learn");
			};
			_003C_003Ec._003C_003E9__10_9 = val9;
			obj9 = (object)val9;
		}
		s.AddGameMenuOption("town_enchant_academy", "academy_enchant_learn", text6, (OnConditionDelegate)obj8, (OnConsequenceDelegate)obj9, false, 9, false, (object)null);
		string text7 = ((object)new TextObject("{=Yuef_EnchantAcademy_Back}Return", (Dictionary<string, object>)null)).ToString();
		object obj10 = _003C_003Ec._003C_003E9__10_10;
		if (obj10 == null)
		{
			OnConditionDelegate val10 = (MenuCallbackArgs args) => true;
			_003C_003Ec._003C_003E9__10_10 = val10;
			obj10 = (object)val10;
		}
		object obj11 = _003C_003Ec._003C_003E9__10_11;
		if (obj11 == null)
		{
			OnConsequenceDelegate val11 = delegate
			{
				GameMenu.SwitchToMenu("town_enchant_academy");
			};
			_003C_003Ec._003C_003E9__10_11 = val11;
			obj11 = (object)val11;
		}
		s.AddGameMenuOption("academy_enchant_learn", "enchant_return", text7, (OnConditionDelegate)obj10, (OnConsequenceDelegate)obj11, true, 9, false, (object)null);
		foreach (BlueprintData bp in _allBlueprints)
		{
			int goldCost = (int)((double)bp.Item.Value * 1.2);
			ItemTrait val12 = ItemTrait.All.FirstOrDefault((ItemTrait t) => t.ItemTraitStringId == bp.BlueprintId);
			string displayName = ((val12 != (ItemTrait)null) ? val12.ItemTraitName.ToString() : bp.BlueprintId);
			CustomResource resource = HeroExtensions.GetCultureSpecificCustomResource(Hero.MainHero);
			int resourceCost = 0;
			string resourceIcon = "";
			if (resource != null)
			{
				resourceCost = (int)(resource.GetCustomResourceGeneralizedFactor() * (float)bp.RequiredSkillValue);
				resourceIcon = resource.GetCustomResourceIconAsText(false);
			}
			string text8 = "Enchant_" + bp.BlueprintId;
			s.AddGameMenuOption("academy_enchant_learn", text8, displayName, (OnConditionDelegate)delegate(MenuCallbackArgs args)
			{
				//IL_0031: Unknown result type (might be due to invalid IL or missing references)
				//IL_0037: Expected O, but got Unknown
				//IL_0181: Unknown result type (might be due to invalid IL or missing references)
				//IL_0188: Expected O, but got Unknown
				//IL_0116: Unknown result type (might be due to invalid IL or missing references)
				//IL_011d: Expected O, but got Unknown
				//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
				//IL_01e8: Expected O, but got Unknown
				if (HeroExtensions.HasKnownEnchantmentBlueprint(_selectedHero, bp.BlueprintId))
				{
					TextObject val13 = new TextObject("{=Yuef_EnchantAcademy_AlreadyKnown}{HERO_NAME} already knows this enchantment.", (Dictionary<string, object>)null);
					val13.SetTextVariable("HERO_NAME", _selectedHero.Name);
					args.Tooltip = val13;
					args.IsEnabled = false;
				}
				else if (!CanHeroLearnBlueprint(_selectedHero, bp))
				{
					string text9;
					if (!string.IsNullOrEmpty(bp.Restriction))
					{
						text9 = bp.Restriction;
					}
					else
					{
						string obj12 = ((object)((PropertyObject)bp.RequiredSkill).Name)?.ToString();
						int requiredSkillValue = bp.RequiredSkillValue;
						text9 = obj12 + " " + requiredSkillValue;
					}
					string text10 = text9;
					TextObject val14 = new TextObject("{=Yuef_EnchantAcademy_Requirement}{HERO_NAME} does not meet requirement: {REQ}", (Dictionary<string, object>)null);
					val14.SetTextVariable("HERO_NAME", _selectedHero.Name);
					val14.SetTextVariable("REQ", text10);
					args.Tooltip = val14;
					args.IsEnabled = false;
				}
				else if (Hero.MainHero.Gold < goldCost)
				{
					TextObject val15 = new TextObject("{=Yuef_EnchantAcademy_NeedGold}Need {GOLD} gold.", (Dictionary<string, object>)null);
					val15.SetTextVariable("GOLD", goldCost);
					args.Tooltip = val15;
					args.IsEnabled = false;
				}
				else if (resource != null && resourceCost > 0 && HeroExtensions.GetCultureSpecificCustomResourceValue(Hero.MainHero) < (float)resourceCost)
				{
					TextObject val16 = new TextObject("{=Yuef_EnchantAcademy_NeedResource}Not enough {RESOURCE}.", (Dictionary<string, object>)null);
					val16.SetTextVariable("RESOURCE", resourceIcon);
					args.Tooltip = val16;
					args.IsEnabled = false;
				}
				return true;
			}, (OnConsequenceDelegate)delegate
			{
				//IL_006b: Unknown result type (might be due to invalid IL or missing references)
				//IL_0071: Expected O, but got Unknown
				//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
				//IL_00b4: Expected O, but got Unknown
				HeroExtensions.AddEnchantmentBlueprint(_selectedHero, bp.BlueprintId, true);
				Hero.MainHero.ChangeHeroGold(-goldCost);
				if (resource != null && resourceCost > 0)
				{
					HeroExtensions.AddCultureSpecificCustomResource(Hero.MainHero, (float)(-resourceCost));
				}
				TextObject val13 = new TextObject("{=Yuef_EnchantAcademy_Learned}{HERO_NAME} learned: {ENCHANT}", (Dictionary<string, object>)null);
				val13.SetTextVariable("HERO_NAME", _selectedHero.Name);
				val13.SetTextVariable("ENCHANT", displayName);
				InformationManager.DisplayMessage(new InformationMessage(((object)val13).ToString()));
				GameMenu.SwitchToMenu("academy_enchant_learn");
			}, true, 9, false, (object)null);
		}
	}
}
