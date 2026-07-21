using System;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace TORCompanionAssimilationSkip
{
    public class SubModule : MBSubModuleBase
    {
        public const string HarmonyId = "com.tor.companion.assimilation.skip";

        private bool _shouldLoadPatches = true;

        public static Harmony? HarmonyInstance { get; private set; }

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            HarmonyInstance = new Harmony(HarmonyId);
        }

        protected override void OnSubModuleUnloaded()
        {
            base.OnSubModuleUnloaded();
            if (HarmonyInstance != null)
            {
                HarmonyInstance.UnpatchAll(HarmonyId);
                HarmonyInstance = null;
            }
        }

        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            InformationManager.DisplayMessage(
                new InformationMessage("TOR Companion Assimilation Skip loaded", Colors.Green));

            if (_shouldLoadPatches && HarmonyInstance != null)
            {
                try
                {
                    HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
                    _shouldLoadPatches = false;
                }
                catch (Exception ex)
                {
                    InformationManager.DisplayMessage(
                        new InformationMessage(
                            $"Companion Assimilation Skip patch failed: {ex.Message}",
                            Colors.Red));
                }
            }
        }
    }
}
