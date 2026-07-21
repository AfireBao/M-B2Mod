#define TRACE
using System;
using System.Diagnostics;
using System.Reflection;

namespace HarmonyLib.BUTR.Extensions;

internal static class HarmonyExtensions
{
	public static bool TryPatch(this Harmony harmony, MethodBase? original, MethodInfo? prefix = null, MethodInfo? postfix = null, MethodInfo? transpiler = null, MethodInfo? finalizer = null)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		if ((object)original == null || ((object)prefix == null && (object)postfix == null && (object)transpiler == null && (object)finalizer == null))
		{
			Trace.TraceError("HarmonyExtensions.TryPatch: 'original' or all methods are null");
			return false;
		}
		HarmonyMethod val = (((object)prefix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(prefix));
		HarmonyMethod val2 = (((object)postfix == null) ? ((HarmonyMethod)null) : new HarmonyMethod(postfix));
		HarmonyMethod val3 = (((object)transpiler == null) ? ((HarmonyMethod)null) : new HarmonyMethod(transpiler));
		HarmonyMethod val4 = (((object)finalizer == null) ? ((HarmonyMethod)null) : new HarmonyMethod(finalizer));
		try
		{
			harmony.Patch(original, val, val2, val3, val4);
		}
		catch (Exception arg)
		{
			Trace.TraceError($"HarmonyExtensions.TryPatch: Exception occurred: {arg}, original '{original}'");
			return false;
		}
		return true;
	}

	public static ReversePatcher? TryCreateReversePatcher(this Harmony harmony, MethodBase? original, MethodInfo? standin)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		if ((object)original == null || (object)standin == null)
		{
			Trace.TraceError("HarmonyExtensions.TryCreateReversePatcher: 'original' or 'standin' is null");
			return null;
		}
		try
		{
			return harmony.CreateReversePatcher(original, new HarmonyMethod(standin));
		}
		catch (Exception arg)
		{
			Trace.TraceError($"HarmonyExtensions.TryCreateReversePatcher: Exception occurred: {arg}, original '{original}'");
			return null;
		}
	}

	public static bool TryCreateReversePatcher(this Harmony harmony, MethodBase? original, MethodInfo? standin, out ReversePatcher? result)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		if ((object)original == null || (object)standin == null)
		{
			Trace.TraceError("HarmonyExtensions.TryCreateReversePatcher: 'original' or 'standin' is null");
			result = null;
			return false;
		}
		try
		{
			result = harmony.CreateReversePatcher(original, new HarmonyMethod(standin));
			return true;
		}
		catch (Exception arg)
		{
			Trace.TraceError($"HarmonyExtensions.TryCreateReversePatcher: Exception occurred: {arg}, original '{original}'");
			result = null;
			return false;
		}
	}
}
