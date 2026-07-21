using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace TroopFormations_TOR;

/// <summary>
/// TOR replaces PartyScreen.xml and comments out formation UI.
/// Insert late in the Children list so the control draws above TOR overlays.
/// </summary>
[PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
public class PartyScreenUIPatch : PrefabExtensionInsertPatch
{
    public override InsertType Type => InsertType.Child;

    // High index => append near end of TOR PartyScreen children (drawn later / on top).
    public override int Index => 100;

    [PrefabExtensionFileName(true)]
    public string File => "PartyScreenFormationPatch";
}
