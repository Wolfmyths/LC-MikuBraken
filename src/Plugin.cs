using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LCSoundTool;
using MikuBraken.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MikuBraken
{
    public class ConfigManager
    {
        private const string SectionGeneral = "General";
        private const string SectionSound = "Sound";

        // General
        public static ConfigEntry<bool> MikuModel;
        public static ConfigEntry<bool> MikuTerminal;
        public static ConfigEntry<bool> MikuDeleteOnKilled;
        public static ConfigEntry<bool> MikuScanTag;
        public static ConfigEntry<bool> MikuGlowingEyes;

        // Sound
        public static ConfigEntry<bool> MikuAngry;
        public static ConfigEntry<bool> MikuCaught;
        public static ConfigEntry<bool> MikuDies;
        public static ConfigEntry<bool> MikuFootsteps;
        public static ConfigEntry<bool> MikuCrackNeck;
        public static ConfigEntry<bool> MikuStun;

        public ConfigManager(ConfigFile cfg)
        {
            MikuModel = cfg.Bind(
            SectionGeneral,
            "MikuModel",
            true,
            "The Braken's model will be replaced by Miku"
            );

            MikuGlowingEyes = cfg.Bind(
            SectionGeneral,
            "MikuGlowingEyes",
            true,
            "Miku will have glowing eyes similar to the Braken"
            );

            MikuTerminal = cfg.Bind(
            SectionGeneral,
            "Terminal",
            true,
            "The Braken's bestiary entry will be replaced by Miku"
            );

            MikuDeleteOnKilled = cfg.Bind(
            SectionGeneral,
            "DeleteOnKilled",
            false,
            "Miku's model is deleted on death"
            );

            MikuScanTag = cfg.Bind(
            SectionGeneral,
            "ScanTag",
            true,
            "The Braken's scan tag is replaced with 'Hatune Miku'"
            );

            MikuAngry = cfg.Bind(
            SectionSound,
            "MikuAngry",
            true,
            "Replaces the sound when the Braken begins to chase the player"
            );

            MikuCaught = cfg.Bind(
            SectionSound,
            "MikuCaught",
            true,
            "Replaces the sound when a player spots the Braken in stealth"
            );

            MikuDies = cfg.Bind(
            SectionSound,
            "MikuDies",
            true,
            "Replaces the Braken's death sound"
            );

            MikuFootsteps = cfg.Bind(
            SectionSound,
            "MikuFootsteps",
            true,
            "Replaces the Braken's footstep sounds"
            );

            MikuCrackNeck = cfg.Bind(
            SectionSound,
            "MikuCrackNeck",
            true,
            "Adds a sound after Miku kills a player"
            );

            MikuStun = cfg.Bind(
            SectionSound,
            "MikuStun",
            true,
            "Replaces the Braken's stun sound"
            );
        }
    }

    [BepInPlugin(modGUID, modName, modVersion)]
    public class MikuBrakenBase : BaseUnityPlugin
    {
        private const string modGUID = "Wolfmyths.MikuBraken";
        private const string modName = "Miku Braken";
        private const string modVersion = "1.2.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ConfigManager myConfig { get; internal set; }

        internal static MikuBrakenBase Instance;

        internal static ManualLogSource mls;

        private static List<AudioClip> SoundFX;
        internal static AudioClip Miku_Angry;
        internal static AudioClip Miku_Angry1;
        internal static AudioClip Miku_Caught;
        internal static AudioClip Miku_Caught1;
        internal static AudioClip Miku_Caught2;
        internal static AudioClip Miku_CrackNeck;
        internal static AudioClip Miku_Dies;
        internal static AudioClip Miku_Footsteps;
        internal static AudioClip Miku_Footsteps1;
        internal static AudioClip Miku_Footsteps2;
        internal static AudioClip Miku_Footsteps3;
        internal static AudioClip Miku_Stun;

        internal static List<GameObject> Prefabs;
        internal static GameObject Miku;
        internal static GameObject Miku_Eyes;

        internal static AssetBundle SFXBundle;
        internal static AssetBundle PrefabBundle;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            myConfig = new ConfigManager(Config);

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            Log_Info("Mikudayooo");

            mls = Logger;

            SoundFX = new List<AudioClip>();

            string FolderLocation = Instance.Info.Location;
            FolderLocation = FolderLocation.TrimEnd("MikuBraken.dll".ToCharArray());
            SFXBundle = AssetBundle.LoadFromFile(FolderLocation + "sounds");
            PrefabBundle = AssetBundle.LoadFromFile(FolderLocation + "model");
            if (SFXBundle != null && PrefabBundle != null)
            {
                Log_Info("Asset bundles loaded");
                SoundFX = SFXBundle.LoadAllAssets<AudioClip>().ToList();
                Miku_Angry      = SoundFX[0];
                Miku_Angry1     = SoundFX[1];
                Miku_Caught     = SoundFX[2];
                Miku_Caught1    = SoundFX[3];
                Miku_Caught2    = SoundFX[4];
                Miku_CrackNeck  = SoundFX[5];
                Miku_Dies       = SoundFX[6];
                Miku_Footsteps  = SoundFX[7];
                Miku_Footsteps1 = SoundFX[8];
                Miku_Footsteps2 = SoundFX[9];
                Miku_Footsteps3 = SoundFX[10];
                Miku_Stun       = SoundFX[11];

                // Log_UnityObject_List(SoundFX);

                Prefabs = PrefabBundle.LoadAllAssets<GameObject>().ToList();
                Miku_Eyes = Prefabs[0];
                Miku      = Prefabs[1];

                // Log_UnityObject_List(Prefabs);

            }
            else
            {
                if (SFXBundle == null)
                {
                    Log_Error("Failed to load SoundFX bundle");
                }
                if (PrefabBundle == null)
                {
                    Log_Error("Failed to load Prefab bundle");
                }
            }

            harmony.PatchAll(typeof(MikuBrakenBase));
            harmony.PatchAll(typeof(FlowerManAIPatch));
            harmony.PatchAll(typeof(TerminalPatch));
        }

        void Start()
        {
            if (ConfigManager.MikuFootsteps.Value)
            {
                // Replace Braken Footsteps
                // Miku_Footstep 1-4
                SoundTool.ReplaceAudioClip("Step1", Miku_Footsteps, 1f, "Flowerman(Clone)");
                SoundTool.ReplaceAudioClip("Step2", Miku_Footsteps1, 1f, "Flowerman(Clone)");
                SoundTool.ReplaceAudioClip("Step3", Miku_Footsteps2, 1f, "Flowerman(Clone)");
                SoundTool.ReplaceAudioClip("Step4", Miku_Footsteps3, 1f, "Flowerman(Clone)");
            }

            if (ConfigManager.MikuCaught.Value)
            {
                // Replace Miku getting caught
                // Miku_Caught 0-2
                SoundTool.ReplaceAudioClip("Found1", Miku_Caught, 0.35f, "Flowerman(Clone)");
                SoundTool.ReplaceAudioClip("Found1", Miku_Caught1, 0.35f, "Flowerman(Clone)");
                SoundTool.ReplaceAudioClip("Found1", Miku_Caught2, 0.30f, "Flowerman(Clone)");
            }

            if (ConfigManager.MikuAngry.Value)
            {
                // Replace Miku getting angry
                // Miku_Caught 0-1
                SoundTool.ReplaceAudioClip("Angered", Miku_Angry, 0.5f);
                SoundTool.ReplaceAudioClip("Angered", Miku_Angry1, 0.5f);
            }

        }

        public static void Log_UnityObject_List(List<AudioClip> list)
        {
            foreach (AudioClip a in list)
            {
                Log_Info(a.name);
            }
        }

        public static void Log_UnityObject_List(List<GameObject> list)
        {
            foreach (GameObject a in list)
            {
                Log_Info(a.name);
            }
        }

        public static void Log_Info(string msg) { mls.LogInfo(msg); }

        public static void Log_Warning(string msg) { mls.LogWarning(msg); }

        public static void Log_Error(string msg) { mls.LogError(msg); }
    }
}
