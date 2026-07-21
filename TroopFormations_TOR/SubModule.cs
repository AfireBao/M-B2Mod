using System;
using Bannerlord.UIExtenderEx;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TroopFormations_TOR;

public class SubModule : MBSubModuleBase
{
    public const string HarmonyId = "com.troopformations.tor";

    private readonly Harmony _harmony = new(HarmonyId);
    private readonly UIExtender _extender = UIExtender.Create("TroopFormations_TOR");
    private bool _uiRegistered;
    private bool _patched;

    protected override void OnSubModuleLoad()
    {
        base.OnSubModuleLoad();
        if (!_uiRegistered)
        {
            _extender.Register(typeof(SubModule).Assembly);
            _extender.Enable();
            _uiRegistered = true;
        }
    }

    protected override void OnBeforeInitialModuleScreenSetAsRoot()
    {
        base.OnBeforeInitialModuleScreenSetAsRoot();
        ApplyPatches();
        InformationManager.DisplayMessage(
            new InformationMessage("Troop Formations TOR loaded (fork of Better Troop Formations)", Colors.Green));
    }

    protected override void InitializeGameStarter(Game game, IGameStarter gameStarter)
    {
        if (game.GameType is Campaign && gameStarter is CampaignGameStarter campaignStarter)
        {
            campaignStarter.AddBehavior(new TroopFormationsBehavior());
        }
    }

    public override void OnMissionBehaviorInitialize(Mission mission)
    {
        mission.AddMissionBehavior(new FormationsBehavior());
    }

    private void ApplyPatches()
    {
        if (_patched)
        {
            return;
        }

        try
        {
            _harmony.PatchAll();
            _patched = true;
        }
        catch (Exception ex)
        {
            InformationManager.DisplayMessage(
                new InformationMessage($"Troop Formations TOR patch failed: {ex.Message}", Colors.Red));
        }
    }
}
