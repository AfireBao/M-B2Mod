using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Roster;
using TOR_Core.CampaignMechanics.Assimilation;

namespace TORCompanionAssimilationSkip
{
    /// <summary>
    /// Skips TOR cultural troop assimilation for player-recruited wanderer companions.
    /// AI lords and the main hero are unchanged (main hero is already skipped by TOR).
    /// </summary>
    [HarmonyPatch(typeof(AssimilationCampaignBehavior), "SwapTroopsIfNeeded")]
    internal static class AssimilationCompanionSkipPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Hero owner, TroopRoster roster, CharacterObject troop, int count)
        {
            // Only block player companions (wanderers hired into the player clan).
            if (owner != null && owner.IsPlayerCompanion)
            {
                return false;
            }

            return true;
        }
    }
}
