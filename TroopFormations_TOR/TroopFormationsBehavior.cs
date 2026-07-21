using System.Collections.Generic;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace TroopFormations_TOR;

/// <summary>
/// Uses vanilla Campaign.PlayerFormationPreferences for persistence.
/// Avoids a custom SaveableTypeDefiner for Dictionary&lt;CharacterObject, FormationClass&gt;,
/// which collides with Campaign's own container definition and can break/OOM on save load.
/// </summary>
public class TroopFormationsBehavior : CampaignBehaviorBase
{
    public const int UnassignedIndex = 8;
    public const int FormationCount = 8;

    public static TroopFormationsBehavior? Instance { get; private set; }

    private List<(string Icon, TextObject Hint)>? _formationNames;

    public TroopFormationsBehavior()
    {
        Instance = this;
    }

    public List<(string Icon, TextObject Hint)> FormationNames
    {
        get
        {
            if (_formationNames != null)
            {
                return _formationNames;
            }

            _formationNames = new List<(string, TextObject)>(FormationCount + 1);
            for (int i = 0; i < FormationCount; i++)
            {
                string icon = $"<img src=\"PartyScreen\\FormationIcons\\{i + 1}\"/>";
                _formationNames.Add((icon, new TextObject($"Formation {i + 1}")));
            }

            _formationNames.Add(("<img src=\"PartyScreen\\FormationIcons\\0\"/>", new TextObject("Unassigned")));
            return _formationNames;
        }
    }

    public override void RegisterEvents()
    {
    }

    public override void SyncData(IDataStore dataStore)
    {
        // Intentionally empty — preferences live in Campaign.PlayerFormationPreferences.
    }

    public void UpdateCurrentCharacterFormationClass(int selectedIndex, CharacterObject character)
    {
        if (character == null || character.IsHero || Campaign.Current == null)
        {
            return;
        }

        if (selectedIndex < 0 || selectedIndex >= UnassignedIndex)
        {
            RemovePlayerFormationPreference(character);
            return;
        }

        Campaign.Current.SetPlayerFormationPreference(character, (FormationClass)selectedIndex);
    }

    public int CurrentCharacterFormation(CharacterObject? character)
    {
        if (character != null
            && Campaign.Current?.PlayerFormationPreferences != null
            && Campaign.Current.PlayerFormationPreferences.TryGetValue(character, out FormationClass formation))
        {
            int index = (int)formation;
            if (index >= 0 && index < FormationCount)
            {
                return index;
            }
        }

        return UnassignedIndex;
    }

    public static bool TryGetAssignedFormation(CharacterObject? character, out FormationClass formation)
    {
        formation = FormationClass.Infantry;
        if (character == null
            || Campaign.Current?.PlayerFormationPreferences == null
            || !Campaign.Current.PlayerFormationPreferences.TryGetValue(character, out formation))
        {
            return false;
        }

        int index = (int)formation;
        return index >= 0 && index < FormationCount;
    }

    private static void RemovePlayerFormationPreference(CharacterObject character)
    {
        Campaign? campaign = Campaign.Current;
        if (campaign == null)
        {
            return;
        }

        var prefs = Traverse.Create(campaign).Field("_playerFormationPreferences")
            .GetValue<Dictionary<CharacterObject, FormationClass>>();
        prefs?.Remove(character);
    }
}
