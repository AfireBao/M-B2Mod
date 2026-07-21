using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace TroopFormations;

[PrefabExtension("PartyScreen", "descendant::PartyScreenWidget[@Id='PartyScreen']/Children")]
public class PartyScreenUIPatch : PrefabExtensionInsertPatch
{
	public override InsertType Type => InsertType.Child;

	public override int Index => 2;

	[PrefabExtensionFileName(true)]
	public string File => "PartyScreenFormationPatch";
}
