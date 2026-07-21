using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.Extensions;
using TOR_Core.Items;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace YuefTORMechanism.General.Dwarf;

[HarmonyPatch]
internal class DwarfMountPatch
{
	private static MCMSetting Settings => GlobalSettings<MCMSetting>.Instance;

	[HarmonyPatch(typeof(ExtendedItemObjectManager), "CanCharacterUseItem")]
	[HarmonyPrefix]
	private static bool Prefix_CanCharacterUseItem(ItemObject item, CharacterObject character, ref bool __result)
	{
		if (Settings == null || !Settings.Yuef_DwarfCanUseMounts)
		{
			return true;
		}
		if (CharacterObjectExtensions.IsDwarf(character) && item.IsMountable)
		{
			__result = true;
			return false;
		}
		return true;
	}
}
