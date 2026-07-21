using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace YuefPartyEnhancement;

public class SubModule : MBSubModuleBase
{
	private bool _shouldLoadPatches = true;

	public static Harmony HarmonyInstance { get; private set; }

	protected override void OnSubModuleLoad()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Expected O, but got Unknown
		((MBSubModuleBase)this).OnSubModuleLoad();
		HarmonyInstance = new Harmony("com.yuefparty.patch");
	}

	protected override void OnBeforeInitialModuleScreenSetAsRoot()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		((MBSubModuleBase)this).OnBeforeInitialModuleScreenSetAsRoot();
		InformationManager.DisplayMessage(new InformationMessage("PartyEnhancement Author:YuefChen", Colors.Green));
		if (_shouldLoadPatches && HarmonyInstance != null)
		{
			ApplyPatches();
		}
	}

	protected override void InitializeGameStarter(Game game, IGameStarter gameStarterObject)
	{
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
		campaignGameStarter.AddBehavior((CampaignBehaviorBase)(object)new AddGoldBehavior());
		campaignGameStarter.AddBehavior((CampaignBehaviorBase)(object)new LordPartyEnhancement());
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
					InformationManager.DisplayMessage(new InformationMessage("PartyEnhancement patch failed: " + type.Name, Colors.Red));
				}
			}
			_shouldLoadPatches = false;
			InformationManager.DisplayMessage(new InformationMessage("[PartyEnhancement] All patches loaded", Colors.Blue));
		}
		catch
		{
			InformationManager.DisplayMessage(new InformationMessage("PartyEnhancement patch loading failed!", Colors.Red));
		}
	}
}
