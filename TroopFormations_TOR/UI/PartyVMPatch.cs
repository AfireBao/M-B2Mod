using HarmonyLib;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;

namespace TroopFormations_TOR;

[HarmonyPatch(typeof(PartyVM), "RefreshCurrentCharacterInformation")]
public static class PartyVMPatch
{
    // Keep formation picker visible for regular troops; hide for heroes (same as original mod).
    private static void Postfix(PartyVM __instance)
    {
        if (__instance.CurrentCharacter == null)
        {
            return;
        }

        if (__instance.CurrentCharacter.IsHero)
        {
            __instance.IsCurrentCharacterFormationEnabled = false;
        }
    }
}
