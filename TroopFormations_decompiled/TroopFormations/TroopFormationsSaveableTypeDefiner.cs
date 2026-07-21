using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace TroopFormations;

public class TroopFormationsSaveableTypeDefiner : SaveableTypeDefiner
{
	public TroopFormationsSaveableTypeDefiner()
		: base(188359168)
	{
	}

	protected override void DefineClassTypes()
	{
	}

	protected override void DefineContainerDefinitions()
	{
		((SaveableTypeDefiner)this).ConstructContainerDefinition(typeof(Dictionary<CharacterObject, FormationClass>));
	}
}
