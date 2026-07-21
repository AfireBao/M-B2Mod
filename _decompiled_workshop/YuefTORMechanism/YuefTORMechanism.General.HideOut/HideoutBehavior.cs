using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CampaignBehaviors;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Roster;
using TaleWorlds.Core;

namespace YuefTORMechanism.General.HideOut;

internal class HideoutBehavior : CampaignBehaviorBase
{
	private HideoutCampaignBehavior HideoutCampaignBehavior => Campaign.Current.GetCampaignBehavior<HideoutCampaignBehavior>();

	public override void RegisterEvents()
	{
		CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener((object)this, (Action<CampaignGameStarter>)OnSessionLaunchedEvent);
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	private void OnSessionLaunchedEvent(CampaignGameStarter campaignGameStarter)
	{
		AddGameMenus(campaignGameStarter);
	}

	private void AddGameMenus(CampaignGameStarter campaignGameStarter)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		campaignGameStarter.AddGameMenuOption("hideout_place", "str_order_attack", "{=TtGJqRI5}Send Troops", (OnConditionDelegate)null, new OnConsequenceDelegate(OnSendTroopsConsequence), true, 2, false, (object)null);
	}

	private void OnSendTroopsConsequence(MenuCallbackArgs args)
	{
		if (PlayerEncounter.Battle == null)
		{
			if (MobileParty.MainParty.MapEvent != null)
			{
				PlayerEncounter.Init();
			}
			else
			{
				PlayerEncounter.StartBattle();
			}
		}
		PlayerEncounter.Update();
		ExecuteHideOutFightLogic(args);
	}

	private static void ExecuteHideOutFightLogic(MenuCallbackArgs args)
	{
		EncounterHideOutOrderAttack();
	}

	private static void EncounterHideOutOrderAttack()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if (PlayerEncounter.Current != null)
		{
			GameMenu.ExitToLast();
			PlayerEncounter.InitSimulation((FlattenedTroopRoster)null, (FlattenedTroopRoster)null);
			if (PlayerEncounter.Current?.BattleSimulation != null)
			{
				((MapState)Game.Current.GameStateManager.ActiveState).StartBattleSimulation();
			}
		}
	}
}
