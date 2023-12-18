using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using ConchaSuMare.Patches;
using System.Collections.Generic;
using conchaSuMare.Objects;
using System.Threading;
using System.IO;
using System.Reflection;

namespace ConchaSuMare
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class ConchaSuMare : BaseUnityPlugin
    {
        private const string modGUID = "Lecre.ConchaSuMareMod";
        private const string modName = "LC ConchaSuMareMod";
        private const string modVersion = "1.2.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ConchaSuMare instance;

        internal ManualLogSource mls;
        

        void Awake()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            instance = this;
            DontDestroyOnLoad (gameObject);
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad (gameObject);
            }else
            {
                mls.LogInfo("Happy kirbi :)");
            }
            
            mls.LogInfo("Lecre.ConchaSuMareMod is loading");

            
            

            if(instance == null)
            {
                mls.LogError("INSTANCE IS NULL 2 TIMES IN A ROW, CONCHASUMARE WILL BE DISABLED");
            }

            harmony.PatchAll(typeof(ConchaSuMare));
            harmony.PatchAll(typeof(FallvoidPatch));
        }


    }
}
