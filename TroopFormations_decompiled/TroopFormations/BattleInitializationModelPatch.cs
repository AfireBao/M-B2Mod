using HarmonyLib;
using TaleWorlds.MountAndBlade.ComponentInterfaces;

namespace TroopFormations;

[HarmonyPatch(typeof(BattleInitializationModel))]
public class BattleInitializationModelPatch
{
	[HarmonyPostfix]
	[HarmonyPatch("CanPlayerSideDeployWithOrderOfBattle")]
	private static void Postfix(ref bool __result)
	{
		__result = true;
	}
}
