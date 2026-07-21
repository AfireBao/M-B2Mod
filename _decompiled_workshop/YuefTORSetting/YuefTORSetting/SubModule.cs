using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace YuefTORSetting;

public class SubModule : MBSubModuleBase
{
	private bool _patched = false;

	public static Harmony HarmonyInstance { get; private set; }

	protected override void OnSubModuleLoad()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		((MBSubModuleBase)this).OnSubModuleLoad();
		HarmonyInstance = new Harmony("com.yueftor.setting");
	}

	protected override void OnBeforeInitialModuleScreenSetAsRoot()
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		((MBSubModuleBase)this).OnBeforeInitialModuleScreenSetAsRoot();
		try
		{
			if (!_patched && HarmonyInstance != null)
			{
				PatchOnlyMyPatches();
				_patched = true;
				InformationManager.DisplayMessage(new InformationMessage("TOR 数值设置已加载", Colors.Green));
			}
		}
		catch
		{
			InformationManager.DisplayMessage(new InformationMessage("数值补丁加载失败", Colors.Red));
		}
	}

	private void PatchOnlyMyPatches()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		string[] array = new string[3] { "YuefTORSetting.TORBugFixPatch", "YuefTORSetting.TORConfigPatch", "YuefTORSetting.TORWoMPatch" };
		string[] array2 = array;
		foreach (string name in array2)
		{
			try
			{
				Type type = executingAssembly.GetType(name);
				if (type != null)
				{
					HarmonyInstance.CreateClassProcessor(type).Patch();
				}
			}
			catch
			{
			}
		}
	}

	protected override void OnSubModuleUnloaded()
	{
		((MBSubModuleBase)this).OnSubModuleUnloaded();
		try
		{
			Harmony harmonyInstance = HarmonyInstance;
			if (harmonyInstance != null)
			{
				harmonyInstance.UnpatchAll(HarmonyInstance.Id);
			}
			HarmonyInstance = null;
		}
		catch
		{
		}
	}
}
