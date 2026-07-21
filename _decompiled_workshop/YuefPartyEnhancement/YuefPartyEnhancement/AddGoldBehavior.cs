using System;
using MCM.Abstractions.Base.Global;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;

namespace YuefPartyEnhancement;

internal class AddGoldBehavior : CampaignBehaviorBase
{
	private MCMSetting settings;

	private int GoldAmount = 300000;

	private int MaxGoldLimit = 300000;

	private void InitializeSettings()
	{
		settings = GlobalSettings<MCMSetting>.Instance;
		if (settings != null)
		{
			GoldAmount = settings.dailyGoldAmount;
			MaxGoldLimit = settings.maxGoldLimit;
		}
	}

	private void SettlementGoldGiver(Town town)
	{
		if (town != null && ((SettlementComponent)town).Gold < MaxGoldLimit)
		{
			((SettlementComponent)town).ChangeGold(GoldAmount);
		}
	}

	public override void SyncData(IDataStore dataStore)
	{
	}

	public override void RegisterEvents()
	{
		InitializeSettings();
		if (settings != null)
		{
			CampaignEvents.DailyTickTownEvent.AddNonSerializedListener((object)this, (Action<Town>)SettlementGoldGiver);
		}
	}
}
