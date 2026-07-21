using HarmonyLib;
using MCM.Abstractions.Base.Global;
using TOR_Core.Models;

namespace YuefTORMechanism.General.ResistanceCap;

[HarmonyPatch]
internal class ResistanceCapPatch
{
	private static MCMSetting Settings => GlobalSettings<MCMSetting>.Instance;

	[HarmonyPostfix]
	[HarmonyPatch(typeof(TORAgentApplyDamageModel), "ApplyGeneralDamageModifiers")]
	private static void Postfix_ApplyGeneralDamageModifiers(float baseDamage, ref float __result)
	{
		MCMSetting settings = Settings;
		if (settings == null || !settings.Yuef_ResistanceCapEnabled || baseDamage <= 0f)
		{
			return;
		}
		float num = settings.Yuef_ResistanceCapValue / 100f;
		if (!(num >= 1f))
		{
			float num2 = baseDamage * (1f - num);
			if (__result < num2)
			{
				__result = num2;
			}
		}
	}
}
