using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.SaveSystem;

namespace TroopFormations;

public class TroopFormationsBehavior : CampaignBehaviorBase
{
	public static TroopFormationsBehavior Instance;

	[SaveableField(1)]
	public static Dictionary<CharacterObject, FormationClass> FormationAssignment = new Dictionary<CharacterObject, FormationClass>();

	private List<Tuple<string, TextObject>> _formationNames;

	public List<Tuple<string, TextObject>> FormationNames
	{
		get
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Expected O, but got Unknown
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Expected O, but got Unknown
			if (_formationNames == null)
			{
				int num = 8;
				_formationNames = new List<Tuple<string, TextObject>>(num + 1);
				for (int i = 0; i < num; i++)
				{
					string item = "<img src=\"PartyScreen\\FormationIcons\\" + (i + 1) + "\"/>";
					TextObject item2 = new TextObject("Formation " + (i + 1), (Dictionary<string, object>)null);
					_formationNames.Add(new Tuple<string, TextObject>(item, item2));
				}
				_formationNames.Add(new Tuple<string, TextObject>("<img src=\"PartyScreen\\FormationIcons\\0\"/>", new TextObject("Unassigned", (Dictionary<string, object>)null)));
			}
			return _formationNames;
		}
	}

	public TroopFormationsBehavior()
	{
		Instance = this;
	}

	public override void RegisterEvents()
	{
	}

	public override void SyncData(IDataStore dataStore)
	{
		dataStore.SyncData<Dictionary<CharacterObject, FormationClass>>("FormationAssignment", ref FormationAssignment);
	}

	public void UpdateCurrentCharacterFormationClass(int selectedIndex, CharacterObject character)
	{
		if (selectedIndex == 8 || ((BasicCharacterObject)character).IsHero)
		{
			FormationAssignment.Remove(character);
		}
		else
		{
			FormationAssignment[character] = (FormationClass)selectedIndex;
		}
	}

	public int CurrentCharacterFormation(CharacterObject character)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected I4, but got Unknown
		if (FormationAssignment.TryGetValue(character, out var value))
		{
			return (int)value;
		}
		return 8;
	}
}
