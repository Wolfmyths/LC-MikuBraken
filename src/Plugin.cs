using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LCSoundTool;
using MikuBraken.Patches;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MikuBraken
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MikuBrakenBase : BaseUnityPlugin
    {
        private const string modGUID = "Wolfmyths.MikuBraken";
        private const string modName = "Miku Braken";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static MikuBrakenBase Instance;

        internal static ManualLogSource mls;

        internal static List<AudioClip> SoundFX;
        internal static List<GameObject> Prefabs;
        internal static GameObject Miku;
        internal static AssetBundle SFXBundle;
        internal static AssetBundle PrefabBundle;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Miku Braken has awakened");

            harmony.PatchAll(typeof(FlowerManAIPatch));
            harmony.PatchAll(typeof(TerminalPatch));

            mls = Logger;

            // Sound Files in order
            // Miku_Angry, Miku_Caught, Miku_CrackNeck, Miku_Dies, Miku_Footsteps 1 to 4, Miku_Stun
            SoundFX = new List<AudioClip>();

            string FolderLocation = Instance.Info.Location;
            FolderLocation = FolderLocation.TrimEnd("MikuBraken.dll".ToCharArray());
            SFXBundle = AssetBundle.LoadFromFile(FolderLocation + "sounds");
            PrefabBundle = AssetBundle.LoadFromFile(FolderLocation + "model");
            if (SFXBundle != null && PrefabBundle != null)
            {
                mls.LogInfo("Asset bundles loaded");
                SoundFX = SFXBundle.LoadAllAssets<AudioClip>().ToList();

                Prefabs = PrefabBundle.LoadAllAssets<GameObject>().ToList();
                Miku = Prefabs[0];
            }
            else
            {
                if (SFXBundle == null)
                {
                    mls.LogError("Failed to load SoundFX bundle");
                }
                if (PrefabBundle == null)
                {
                    mls.LogError("Failed to load Prefab bundle");
                }
            }
        }

        void Start()
        {
            // Replace Braken Footsteps
            // Miku_Footstep 1-4
            SoundTool.ReplaceAudioClip("Step1", SoundFX[4], "Flowerman(Clone)");
            SoundTool.ReplaceAudioClip("Step2", SoundFX[5], "Flowerman(Clone)");
            SoundTool.ReplaceAudioClip("Step3", SoundFX[6], "Flowerman(Clone)");
            SoundTool.ReplaceAudioClip("Step4", SoundFX[7], "Flowerman(Clone)");

            // Replace Miku getting caught
            SoundTool.ReplaceAudioClip("Found1", SoundFX[1]); // Miku_Caught

        }
    }
}
