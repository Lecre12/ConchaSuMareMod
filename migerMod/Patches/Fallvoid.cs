using BepInEx;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace ConchaSuMare.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class FallvoidPatch : HarmonyPatch
    {
        static bool done;
        static AudioClip newSFX;
        static ManualLogSource mls;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void prepareAudio()
        {
            done = false;
            mls = BepInEx.Logging.Logger.CreateLogSource("Lecre.conchaSuMareMod");
            // Load the audio file
            string location = ((BaseUnityPlugin)ConchaSuMare.instance).Info.Location;
            string text = "conchaSuMare.dll";
            string text2 = location.TrimEnd(text.ToCharArray());
            string path = text2 + "CONCHA.wav";
            ((MonoBehaviour)ConchaSuMare.instance).StartCoroutine(LoadAudio("file:///" + path, clip =>
            {
                newSFX = clip;
            }));
        }

        static IEnumerator LoadAudio(string url, Action<AudioClip> callback)
        {
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError)
                {
                    mls.LogError("Failed to load audio assets!");
                }
                else
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                    if (clip == null)
                    {
                        mls.LogError("The audio clip is null after loading!");
                    }
                    else
                    {
                        callback(clip);
                        mls.LogInfo("Audio inserted");
                    }
                }
            }
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]

        static void SoundVoidPatch(ref PlayerControllerB __instance)
        {
            mls = BepInEx.Logging.Logger.CreateLogSource("Lecre.conchaSuMareMod");
            PlayerControllerB playerRef = __instance;
            CauseOfDeath causeOfDeath = __instance.causeOfDeath;
            if (playerRef.isPlayerDead && !done && (causeOfDeath == CauseOfDeath.Gravity || causeOfDeath == CauseOfDeath.Unknown))
            {
                done = true;
                // Play the audio file
                AudioSource audioSource = playerRef.gameObject.AddComponent<AudioSource>();
                audioSource.clip = newSFX;
                audioSource.Play();
                mls.LogInfo("CONCHA TU MAIIII, one memeber of the crew fell to the void, what a loser xD");
            }
            
        }

    }

}
