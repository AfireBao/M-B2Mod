using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.Extensions;
using TOR_Core.Utilities;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace YuefTORMechanism.General.Battle;

[HarmonyPatch]
internal class SpellFriendlyFirePatch
{
	private static MCMSetting Settings => GlobalSettings<MCMSetting>.Instance;

	[HarmonyPrefix]
	[HarmonyPatch(typeof(AgentExtensions), "ApplyDamage")]
	private static bool Prefix_ApplyDamage(Agent agent, int damageAmount, Vec3 impactPosition, Agent damager, bool doBlow, bool hasShockWave, bool originatesFromAbility)
	{
		if (!originatesFromAbility)
		{
			return true;
		}
		if (agent == null)
		{
			return true;
		}
		if (Settings == null)
		{
			return true;
		}
		if (damager != null && Settings.Yuef_NoFriendlySpellDamage && damager.Team == agent.Team)
		{
			return false;
		}
		if (damager != null && Settings.Yuef_NoSelfSpellDamage && damager == agent)
		{
			return false;
		}
		if (damager == null && Settings.Yuef_NoFriendlySpellDamage && agent.IsMainAgent)
		{
			return false;
		}
		return true;
	}

	[HarmonyPrefix]
	[HarmonyPatch(typeof(TORDamageDisplay), "DisplayAggregateSpellFriendlyFire")]
	private static bool Prefix_DisplayAggregateSpellFriendlyFire()
	{
		return Settings == null || !Settings.Yuef_NoFriendlySpellDamage;
	}
}
