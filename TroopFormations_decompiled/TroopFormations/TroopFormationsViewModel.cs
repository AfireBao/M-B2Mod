using System;
using System.Collections.Generic;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;

namespace TroopFormations;

[ViewModelMixin]
public class TroopFormationsViewModel : BaseViewModelMixin<PartyVM>
{
	private SelectorVM<SelectorItemVM> _characterFormationSelector;

	private PartyVM _partyvm;

	private TroopFormationsBehavior formationbehavior = TroopFormationsBehavior.Instance;

	[DataSourceProperty]
	public SelectorVM<SelectorItemVM> CharacterFormationSelector
	{
		get
		{
			return _characterFormationSelector;
		}
		set
		{
			if (value != _characterFormationSelector)
			{
				_characterFormationSelector = value;
				((ViewModel)_partyvm).OnPropertyChangedWithValue<SelectorVM<SelectorItemVM>>(value, "CharacterFormationSelector");
			}
		}
	}

	public TroopFormationsViewModel(PartyVM vm)
		: base(vm)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		_partyvm = vm;
		((ViewModel)_partyvm).PropertyChangedWithValue += new PropertyChangedWithValueEventHandler(switchCharacter);
		CharacterFormationSelector = new SelectorVM<SelectorItemVM>((IEnumerable<string>)new List<string>(), formationbehavior.CurrentCharacterFormation(_partyvm.CurrentCharacter.Character), (Action<SelectorVM<SelectorItemVM>>)OnSelectorUpdate);
		for (int i = 0; i < formationbehavior.FormationNames.Count; i++)
		{
			CharacterFormationSelector.AddItem(new SelectorItemVM(formationbehavior.FormationNames[i].Item1, formationbehavior.FormationNames[i].Item2));
		}
	}

	private void OnSelectorUpdate(SelectorVM<SelectorItemVM> selector)
	{
		formationbehavior.UpdateCurrentCharacterFormationClass(selector.SelectedIndex, _partyvm.CurrentCharacter.Character);
	}

	private void switchCharacter(object sender, PropertyChangedWithValueEventArgs e)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		if (e.PropertyName == "CurrentCharacter")
		{
			PartyCharacterVM val = (PartyCharacterVM)e.Value;
			CharacterFormationSelector.SelectedIndex = formationbehavior.CurrentCharacterFormation(val.Character);
		}
	}
}
