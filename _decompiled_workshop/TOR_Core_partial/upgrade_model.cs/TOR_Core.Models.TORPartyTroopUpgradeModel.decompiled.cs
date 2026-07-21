using TOR_Core.CharacterDevelopment.CareerSystem;
using TOR_Core.Extensions;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameComponents;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;

namespace TOR_Core.Models;

public class TORPartyTroopUpgradeModel : DefaultPartyTroopUpgradeModel
{
	public override ExplainedNumber GetGoldCostForUpgrade(PartyBase party, CharacterObject characterObject, CharacterObject upgradeTarget)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		if (characterObject.IsUndead())
		{
			return new ExplainedNumber(0f, false, (TextObject)null);
		}
		ExplainedNumber number = ((DefaultPartyTroopUpgradeModel)this).GetGoldCostForUpgrade(party, characterObject, upgradeTarget);
		if (party.LeaderHero != null && party.LeaderHero == Hero.MainHero)
		{
			CareerHelper.ApplyBasicCareerPassives(party.LeaderHero, ref number, PassiveEffectType.TroopUpgradeCost, asFactor: true, characterObject);
		}
		if (((MBObjectBase)characterObject.Culture).StringId == "sturgia" && party == PartyBase.MainParty)
		{
			if (((BasicCharacterObject)(object)characterObject).HasAttribute("DwarfGun") || ((BasicCharacterObject)(object)upgradeTarget).HasAttribute("DwarfGun"))
			{
				if (Hero.MainHero.HasAttribute("DwarfEngineersII"))
				{
					((ExplainedNumber)(ref number)).AddFactor(-0.25f, (TextObject)null);
				}
				else if (Hero.MainHero.HasAttribute("DwarfEngineersI"))
				{
					((ExplainedNumber)(ref number)).AddFactor(-0.15f, (TextObject)null);
				}
			}
			if (((BasicCharacterObject)(object)characterObject).HasAttribute("DwarfWarrior"))
			{
				if (Hero.MainHero.HasAttribute("DwarfWarriorIII"))
				{
					((ExplainedNumber)(ref number)).AddFactor(-0.3f, (TextObject)null);
				}
				else if (Hero.MainHero.HasAttribute("DwarfWarriorII"))
				{
					((ExplainedNumber)(ref number)).AddFactor(-0.2f, (TextObject)null);
				}
				else if (Hero.MainHero.HasAttribute("DwarfWarriorI"))
				{
					((ExplainedNumber)(ref number)).AddFactor(-0.1f, (TextObject)null);
				}
			}
			if (((BasicCharacterObject)(object)characterObject).HasAttribute("Ironbreaker"))
			{
				((ExplainedNumber)(ref number)).AddFactor(3f, (TextObject)null);
				if (Hero.MainHero.HasAttribute("RuneSmithIII"))
				{
					((ExplainedNumber)(ref number)).AddFactor(-0.2f, (TextObject)null);
				}
				else if (Hero.MainHero.HasAttribute("RuneSmithII"))
				{
					((ExplainedNumber)(ref number)).AddFactor(-0.1f, (TextObject)null);
				}
			}
		}
		return number;
	}

	public override bool CanPartyUpgradeTroopToTarget(PartyBase upgradingParty, CharacterObject upgradeableCharacter, CharacterObject upgradeTarget)
	{
		bool flag = ((DefaultPartyTroopUpgradeModel)this).CanPartyUpgradeTroopToTarget(upgradingParty, upgradeableCharacter, upgradeTarget);
		if (!flag)
		{
			return flag;
		}
		return flag;
	}
}
