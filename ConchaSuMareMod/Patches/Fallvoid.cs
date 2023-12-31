﻿using BepInEx;
using BepInEx.Logging;
using conchaSuMare.Objects;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace ConchaSuMare.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class FallvoidPatch : HarmonyPatch
    {
        public static List<PlayerSoundStatus> playerSoundStatusList = new List<PlayerSoundStatus>();
        public static AudioClip newSFX;
        static ManualLogSource mls;

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void startEventListener()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource("Lecre.conchaSuMareMod");
            mls.LogInfo("Starting to upload audio");
            // Load the audio file
            string location;

            if (ConchaSuMare.instance != null)
            {
                location = ((BaseUnityPlugin)ConchaSuMare.instance).Info.Location;
                string modFileName = "conchaSuMare.dll";
                string modPath = location.TrimEnd(modFileName.ToCharArray());
                string soundPath = modPath + "CONCHA.wav";
                mls.LogInfo("IS THE AUDIO FILE LOCATED HERE??: " + soundPath);
                ((MonoBehaviour)ConchaSuMare.instance).StartCoroutine(LoadAudio("file:///" + soundPath, sound =>
                {
                    newSFX = sound;
                }));                
                
            }
            else
            {
                mls.LogWarning("Instance was readed like <null> retriying to load audio files");
                string assemblyLocation = Assembly.GetExecutingAssembly().Location;
                string modPath = Path.GetDirectoryName(assemblyLocation);
                string soundPath = Path.Combine(modPath, "CONCHA.wav");
                mls.LogInfo("(else)IS THE AUDIO FILE LOCATED HERE??: " + soundPath);
                CoroutineHelper.StartCoroutine(LoadAudio("file:///" + soundPath, sound =>
                {
                    newSFX = sound;
                }));
            }

            
            
        }

        public static IEnumerator LoadAudio(string url, Action<AudioClip> callback)
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
            PlayerSoundStatus playerSoundStatus = new PlayerSoundStatus(playerRef);
            CauseOfDeath causeOfDeath = playerRef.causeOfDeath;
            
            if(__instance == null)
            {
                mls.LogInfo("We have a problem... playerRef = null :(");
            }

            if (playerRef.isPlayerDead && (causeOfDeath == CauseOfDeath.Gravity))
            {
                if (!playerSoundStatusList.Contains(playerSoundStatus))
                {
                    playerSoundStatusList.Add(playerSoundStatus);
                }
                else
                {
                    playerSoundStatus = playerSoundStatusList[playerSoundStatusList.IndexOf(playerSoundStatus)];
                    if (!playerSoundStatus._status)
                    {
                        AudioSource audioSource = playerRef.gameObject.AddComponent<AudioSource>();
                        audioSource.clip = newSFX;
                        audioSource.Play();
                        playerSoundStatus._status = true;
                        mls.LogInfo("CONCHA TU MAIIII, one memeber of the crew fell to the void, what a loser xD");
                    }
                }
                
            }else if (!playerRef.isPlayerDead && playerSoundStatusList.IndexOf(playerSoundStatus) != -1)
            {
                playerSoundStatusList.Remove(playerSoundStatus);
            }
        }



    }

}
