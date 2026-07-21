using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TroopFormations;

public class SubModule : MBSubModuleBase
{
	private Harmony _harmony = new Harmony("TroopFormations");

	private UIExtender _extender = UIExtender.Create("TroopFormations");

	protected override void OnSubModuleLoad()
	{
		_harmony.PatchAll();
		_extender.Register(typeof(SubModule).Assembly);
		_extender.Enable();
		((MBSubModuleBase)this).OnSubModuleLoad();
	}

	protected override void OnSubModuleUnloaded()
	{
		((MBSubModuleBase)this).OnSubModuleUnloaded();
	}

	protected override void InitializeGameStarter(Game game, IGameStarter gameStarter)
	{
		if (Game.Current.GameType is Campaign && gameStarter is CampaignGameStarter)
		{
			CampaignGameStarter val = (CampaignGameStarter)(object)((gameStarter is CampaignGameStarter) ? gameStarter : null);
			val.AddBehavior((CampaignBehaviorBase)(object)new TroopFormationsBehavior());
		}
	}

	protected override void OnBeforeInitialModuleScreenSetAsRoot()
	{
		((MBSubModuleBase)this).OnBeforeInitialModuleScreenSetAsRoot();
	}

	public override void OnMissionBehaviorInitialize(Mission mission)
	{
		mission.AddMissionBehavior((MissionBehavior)(object)new FormationsBehavior());
	}
}
