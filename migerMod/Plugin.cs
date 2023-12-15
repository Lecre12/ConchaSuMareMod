using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ConchaSuMare.Patches;
using System.Collections.Generic;
using conchaSuMare.Objects;

namespace ConchaSuMare
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ConchaSuMare : BaseUnityPlugin
    {
        private const string modGUID = "Lecre.ConchaSuMareMod";
        private const string modName = "LC ConchaSuMareMod";
        private const string modVersion = "1.1.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ConchaSuMare instance;

        internal ManualLogSource mls;
        

        void Awake()
        {

            if (instance == null)
            {
                instance = this;
            }
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            mls.LogInfo("Lecre.ConchaSuMareMod is loading");

            harmony.PatchAll(typeof(ConchaSuMare));

            FallvoidPatch.playerSoundStatusList = new List<PlayerSoundStatus>();
            harmony.PatchAll(typeof(FallvoidPatch));
            mls.LogInfo("TIPO3");
        }

    }
}
