using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using YuefTORMechanism.General.Enchantment;
using YuefTORMechanism.General.HideOut;
using YuefTORMechanism.General.Magic;

namespace YuefTORMechanism;

public class SubModule : MBSubModuleBase
{
	private bool _shouldLoadPatches = true;

	public static Harmony HarmonyInstance { get; private set; }

	protected override void OnSubModuleLoad()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		((MBSubModuleBase)this).OnSubModuleLoad();
		HarmonyInstance = new Harmony("com.yueftor.patch");
	}

	protected override void OnBeforeInitialModuleScreenSetAsRoot()
	{
		((MBSubModuleBase)this).OnBeforeInitialModuleScreenSetAsRoot();
		DisplayModuleInfo();
		if (ShouldLoadPatches())
		{
			ApplyPatches();
		}
	}

	protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
	{
		((MBSubModuleBase)this).InitializeGameStarter(game, gameStarterObject);
		if (game.GameType is Campaign)
		{
			CampaignGameStarter val = (CampaignGameStarter)(object)((gameStarterObject is CampaignGameStarter) ? gameStarterObject : null);
			if (val != null)
			{
				AddBehaviors(val);
			}
		}
	}

	private void AddBehaviors(CampaignGameStarter campaignGameStarter)
	{
		try
		{
			campaignGameStarter.AddBehavior((CampaignBehaviorBase)(object)new HideoutBehavior());
		}
		catch
		{
		}
		try
		{
			campaignGameStarter.AddBehavior((CampaignBehaviorBase)(object)new MagicAcademyBehavior());
		}
		catch
		{
		}
		try
		{
			campaignGameStarter.AddBehavior((CampaignBehaviorBase)(object)new EnchantmentAcademyBehavior());
		}
		catch
		{
		}
	}

	private void DisplayModuleInfo()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		InformationManager.DisplayMessage(new InformationMessage("TOR_Mechanism 加载成功", Colors.Green));
	}

	private bool ShouldLoadPatches()
	{
		return _shouldLoadPatches && HarmonyInstance != null;
	}

	private void ApplyPatches()
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_00aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Expected O, but got Unknown
		try
		{
			if (HarmonyInstance == null)
			{
				return;
			}
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type[] types = executingAssembly.GetTypes();
			foreach (Type type in types)
			{
				try
				{
					if (((MemberInfo)type).GetCustomAttribute<HarmonyPatch>() != null)
					{
						HarmonyInstance.CreateClassProcessor(type).Patch();
					}
				}
				catch
				{
					InformationManager.DisplayMessage(new InformationMessage("补丁失败：" + type.Name, Colors.Red));
				}
			}
			_shouldLoadPatches = false;
			InformationManager.DisplayMessage(new InformationMessage("[TOR] 所有补丁加载完成", Colors.Blue));
		}
		catch
		{
			InformationManager.DisplayMessage(new InformationMessage("整体补丁加载失败！", Colors.Red));
		}
	}
}
