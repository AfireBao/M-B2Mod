using System.Collections.Generic;
using System.ComponentModel;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem.ViewModelCollection.Party;
using TaleWorlds.Core.ViewModelCollection.Selector;
using TaleWorlds.Library;

namespace TroopFormations_TOR;

[ViewModelMixin]
public class TroopFormationsViewModel : BaseViewModelMixin<PartyVM>
{
    private readonly PartyVM _partyVm;
    private SelectorVM<SelectorItemVM>? _characterFormationSelector;

    [DataSourceProperty]
    public SelectorVM<SelectorItemVM>? CharacterFormationSelector
    {
        get => _characterFormationSelector;
        set
        {
            if (value == _characterFormationSelector)
            {
                return;
            }

            _characterFormationSelector = value;
            _partyVm.OnPropertyChangedWithValue(value, nameof(CharacterFormationSelector));
        }
    }

    public TroopFormationsViewModel(PartyVM vm)
        : base(vm)
    {
        _partyVm = vm;

        TroopFormationsBehavior? behavior = TroopFormationsBehavior.Instance;
        if (behavior == null)
        {
            return;
        }

        int selected = behavior.CurrentCharacterFormation(_partyVm.CurrentCharacter?.Character);
        CharacterFormationSelector = new SelectorVM<SelectorItemVM>(
            new List<string>(),
            selected,
            OnSelectorUpdate);

        foreach ((string icon, var hint) in behavior.FormationNames)
        {
            CharacterFormationSelector.AddItem(new SelectorItemVM(icon, hint));
        }

        _partyVm.PropertyChangedWithValue += OnPartyPropertyChanged;
    }

    public override void OnFinalize()
    {
        _partyVm.PropertyChangedWithValue -= OnPartyPropertyChanged;
        base.OnFinalize();
    }

    private void OnSelectorUpdate(SelectorVM<SelectorItemVM> selector)
    {
        if (TroopFormationsBehavior.Instance == null || _partyVm.CurrentCharacter?.Character == null)
        {
            return;
        }

        TroopFormationsBehavior.Instance.UpdateCurrentCharacterFormationClass(
            selector.SelectedIndex,
            _partyVm.CurrentCharacter.Character);
    }

    private void OnPartyPropertyChanged(object? sender, PropertyChangedWithValueEventArgs e)
    {
        if (e.PropertyName != "CurrentCharacter" || CharacterFormationSelector == null)
        {
            return;
        }

        if (e.Value is PartyCharacterVM characterVm)
        {
            CharacterFormationSelector.SelectedIndex =
                TroopFormationsBehavior.Instance?.CurrentCharacterFormation(characterVm.Character)
                ?? TroopFormationsBehavior.UnassignedIndex;
        }
    }
}
