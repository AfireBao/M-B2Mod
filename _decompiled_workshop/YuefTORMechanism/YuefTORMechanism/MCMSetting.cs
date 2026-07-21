using System;
using System.Collections.Generic;
using MCM.Abstractions.Attributes;
using MCM.Abstractions.Attributes.v1;
using MCM.Abstractions.Base.Global;
using TaleWorlds.Localization;

namespace YuefTORMechanism;

internal class MCMSetting : AttributeGlobalSettings<MCMSetting>
{
	public override string Id => "YuefTORMechanism";

	public override string DisplayName
	{
		get
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			string text = ((object)new TextObject("{=YueF_Mechanism_Name}TOR:Mechanism", (Dictionary<string, object>)null)).ToString();
			Version version = typeof(MCMSetting).Assembly.GetName().Version;
			return text + ((version != null) ? version.ToString(3) : null);
		}
	}

	public override string FolderName => "Yuef_TOR_Mechanism";

	public override string FormatType => "json2";

	[SettingProperty("{=Yuef_CursedSite_Title}Cursed Land Mechanism Adjustment", RequireRestart = false, HintText = "{=Yuef_CursedSite_Hint}Enabling this will replace the original TOR health deduction injury mechanism with a speed reduction within the Cursed Land area.", Order = 1)]
	[SettingPropertyGroup("{=Yuef_General}General Mechanisms", GroupOrder = 0)]
	public bool Yuef_CursedSite_adjustment { get; set; } = false;

	[SettingProperty("{=Yuef_Castle_Title}Castle Mechanism Adjustment", RequireRestart = false, HintText = "{=Yuef_Castle_Hint}Enabling this will add a mechanism to the castle: when the team is hostile to a nearby castle, they will suffer a movement speed penalty; otherwise, they will receive a movement speed bonus.", Order = 2)]
	[SettingPropertyGroup("{=Yuef_General}General Mechanisms", GroupOrder = 0)]
	public bool Yuef_Castle_adjustment { get; set; } = true;

	[SettingProperty("{=Yuef_BattleReward_Title}Post-battle Captive Mechanism Adjustment", RequireRestart = false, HintText = "{=Yuef_BattleReward_Hint}Enabling this will restore the mechanism for saving captives.", Order = 3)]
	[SettingPropertyGroup("{=Yuef_General}General Mechanisms", GroupOrder = 0)]
	public bool Yuef_BattleReward_adjustment { get; set; } = false;

	[SettingProperty("{=Yuef_Treeman_Title}AI Treeman Recruitment Adjustment", RequireRestart = false, HintText = "{=Yuef_Treeman_Hint}Enabling this will strengthen the recruitment mechanism of the Wood Elf culture. When the AI is in the Wood Elf Forest, there will be a higher chance of receiving Treeman aid.", Order = 11)]
	[SettingPropertyGroup("{=Yuef_WoodElves}Wood Elf Mechanisms", GroupOrder = 1)]
	public bool Yuef_Treeman_adjustment { get; set; } = true;

	[SettingProperty("{=Yuef_DwarfMount_Title}Allow Dwarfs to Use Mounts", RequireRestart = false, HintText = "{=Yuef_DwarfMount_Hint}If enabled, Dwarf characters will be able to equip and ride mounts, bypassing TOR's racial restriction.", Order = 4)]
	[SettingPropertyGroup("{=Yuef_General}General Mechanisms", GroupOrder = 0)]
	public bool Yuef_DwarfCanUseMounts { get; set; } = false;

	[SettingProperty("{=Yuef_PrisonerRecruit_Title}Unlock All-Culture Prisoner Recruitment", RequireRestart = false, HintText = "{=Yuef_PrisonerRecruit_Hint}If enabled, prisoners from any culture can gain conformity and be recruited. Disable to restore TOR's original culture restriction.", Order = 12)]
	[SettingPropertyGroup("{=Yuef_PrisonerMechanism}Prisoner Mechanisms", GroupOrder = 2)]
	public bool Yuef_UnlockAllCulturePrisonerRecruit { get; set; } = false;

	[SettingProperty("{=Yuef_UnlockAllCulture_Multiplier_Title}Conformity Gain Multiplier", 0.1f, 10f, RequireRestart = false, HintText = "{=Yuef_UnlockAllCulture_Multiplier_Hint}Multiplier for conformity gain when 'Unlock All-Culture' is enabled. Below 1.0 slows down, above 1.0 speeds up, 1.0 is default speed.", Order = 13)]
	[SettingPropertyGroup("{=Yuef_PrisonerMechanism}Prisoner Mechanisms", GroupOrder = 2)]
	public float Yuef_UnlockAllCulture_ConformityMultiplier { get; set; } = 1f;

	[SettingProperty("{=Yuef_TroopRecruitNotable_Title}Unlock All-Culture Troop Recruitment from Notables", RequireRestart = false, HintText = "{=Yuef_TroopRecruitNotable_Hint}If enabled, you can recruit troops from any notable in towns or villages, regardless of culture.", Order = 22)]
	[SettingPropertyGroup("{=Yuef_RecruitmentMechanism}Recruitment Mechanisms", GroupOrder = 4)]
	public bool Yuef_UnlockAllCultureTroopRecruit { get; set; } = true;

	[SettingProperty("{=Yuef_WandererRecruit_Title}Unlock All-Culture Wanderer Recruitment", RequireRestart = false, HintText = "{=Yuef_WandererRecruit_Hint}If enabled, you can hire wanderers (companions) from any culture in taverns.", Order = 23)]
	[SettingPropertyGroup("{=Yuef_RecruitmentMechanism}Recruitment Mechanisms", GroupOrder = 4)]
	public bool Yuef_UnlockAllCultureWandererRecruit { get; set; } = true;

	[SettingProperty("{=Yuef_MagicAcademy_Title}Enable Magic Academy", RequireRestart = false, HintText = "{=Yuef_MagicAcademy_Hint}If enabled, a Magic Academy option will appear in town menus, allowing you to learn all spells for a gold cost.", Order = 31)]
	[SettingPropertyGroup("{=Yuef_MagicMechanism}Magic Mechanisms", GroupOrder = 5)]
	public bool Yuef_EnableMagicAcademy { get; set; } = false;

	[SettingProperty("{=Yuef_EnchantAcademy_Title}Enable Enchantment Academy", RequireRestart = false, HintText = "{=Yuef_EnchantAcademy_Hint}If enabled, an Enchantment Academy option will appear in town menus, allowing heroes who meet requirements to learn enchantments for gold (120% of original cost).", Order = 32)]
	[SettingPropertyGroup("{=Yuef_MagicMechanism}Magic Mechanisms", GroupOrder = 5)]
	public bool Yuef_EnableEnchantmentAcademy { get; set; } = false;

	[SettingProperty("{=Yuef_SpellFriendlyFire_Title}Disable Friendly Spell Damage", RequireRestart = false, HintText = "{=Yuef_SpellFriendlyFire_Hint}If enabled, spells will no longer damage allied troops.", Order = 41)]
	[SettingPropertyGroup("{=Yuef_SpellMechanism}Spell Mechanics", GroupOrder = 6)]
	public bool Yuef_NoFriendlySpellDamage { get; set; } = false;

	[SettingProperty("{=Yuef_SpellSelfDamage_Title}Disable Self Spell Damage", RequireRestart = false, HintText = "{=Yuef_SpellSelfDamage_Hint}If enabled, spells will no longer damage the caster themselves.", Order = 42)]
	[SettingPropertyGroup("{=Yuef_SpellMechanism}Spell Mechanics", GroupOrder = 6)]
	public bool Yuef_NoSelfSpellDamage { get; set; } = false;

	[SettingProperty("{=Yuef_ResistCapEn_Title}Enable Resistance Cap", RequireRestart = false, HintText = "{=Yuef_ResistCapEn_Hint}If enabled, the damage reduction from resistances is capped: a single hit can never be reduced below a configurable floor, restoring the Warhammer-style resistance ceiling (e.g. cap 90% means at least 10% damage always gets through).", Order = 51)]
	[SettingPropertyGroup("{=Yuef_ResistCap}Resistance Cap", GroupOrder = 7)]
	public bool Yuef_ResistanceCapEnabled { get; set; } = false;

	[SettingProperty("{=Yuef_ResistCapVal_Title}Resistance Damage-Reduction Cap (%)", 50f, 100f, RequireRestart = false, HintText = "{=Yuef_ResistCapVal_Hint}Max damage reduction a single hit can gain from resistances, as a percentage. 90 = resistances can block at most 90% of a hit (at least 10% gets through). Default 90.", Order = 52)]
	[SettingPropertyGroup("{=Yuef_ResistCap}Resistance Cap", GroupOrder = 7)]
	public float Yuef_ResistanceCapValue { get; set; } = 90f;
}
