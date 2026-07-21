using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace TroopFormations_TOR;

/// <summary>
/// TOR comments out the vanilla formation picker. Re-insert after a late sibling
/// so the control draws above TOR overlays — never use an Index past child count
/// (TOR PartyScreen has ~14 children; Index=100 caused native AV on startup).
/// </summary>
[PrefabExtension("PartyScreen", "descendant::PartyTroopManagerPopUp[@Id='RecruitPopup']")]
public class PartyScreenUIPatch : PrefabExtensionInsertPatch
{
    public override InsertType Type => InsertType.Append;

    [PrefabExtensionFileName(true)]
    public string File => "PartyScreenFormationPatch";
}
