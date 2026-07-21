using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;

namespace TroopFormations;

[HarmonyPatch(typeof(PartyVM))]
public class PartyVMPatch
{
	[HarmonyPostfix]
	[HarmonyPatch("RefreshCurrentCharacterInformation")]
	private static void Postfix(PartyVM __instance)
	{
		if (__instance.CurrentCharacter.IsHero)
		{
			__instance.IsCurrentCharacterFormationEnabled = false;
		}
	}
}
