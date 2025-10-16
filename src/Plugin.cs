using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using LCSoundTool;
using MikuBraken.Patches;
using System;
using System.CodeDom;
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
        private const string modVersion = "1.2.2";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static ConfigManager myConfig { get; internal set; }

        internal static MikuBrakenBase Instance;

        internal static ManualLogSource mls;

        internal static AudioClip Miku_Angry;
        internal static AudioClip Miku_Angry1;
        internal static AudioClip Miku_Caught;
        internal static AudioClip Miku_Caught1;
        internal static AudioClip Miku_Caught2;
        internal static AudioClip Miku_CrackNeck;
        internal static AudioClip Miku_Dies;
        internal static AudioClip Miku_Footstep;
        internal static AudioClip Miku_Footstep1;
        internal static AudioClip Miku_Footstep2;
        internal static AudioClip Miku_Footstep3;
        internal static AudioClip Miku_Stun;

        internal static List<GameObject> Prefabs;
        internal static GameObject Miku;
        internal static GameObject Miku_Eyes;

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

            string FolderLocation = Mod_Folder();

            PrefabBundle = AssetBundle.LoadFromFile(FolderLocation + "model");
            if (PrefabBundle != null)
            {
                Log_Info("Prefab Mesh Asset bundles loaded");

                Prefabs = PrefabBundle.LoadAllAssets<GameObject>().ToList();
                Miku_Eyes = Prefabs[0];
                Miku      = Prefabs[1];

                // Log_Info(Prefabs);

            }
            else
            {
                Log_Error("Failed to load Prefab bundle, Miku's model will not work :(");
            }

            harmony.PatchAll(typeof(MikuBrakenBase));
            harmony.PatchAll(typeof(FlowerManAIPatch));
            harmony.PatchAll(typeof(TerminalPatch));
        }

        void Start()
        {
            const string FLOWERMAN_NAME = "Flowerman(Clone)";
            const string SFX_FOLDER = "sfx";

            string mod_folder = Mod_Folder();

            // Assign sounds from file
            Miku_Angry     = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Angry.wav", SoundTool.AudioType.wav);
            Miku_Angry1    = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Angry1.wav", SoundTool.AudioType.wav);
            Miku_Caught    = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Caught.wav", SoundTool.AudioType.wav);
            Miku_Caught1   = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Caught1.wav", SoundTool.AudioType.wav);
            Miku_Caught2   = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Caught2.wav", SoundTool.AudioType.wav);
            Miku_CrackNeck = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_CrackNeck.wav", SoundTool.AudioType.wav);
            Miku_Dies      = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Dies.wav", SoundTool.AudioType.wav);
            Miku_Footstep  = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Footstep.wav", SoundTool.AudioType.wav);
            Miku_Footstep1 = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Footstep1.wav", SoundTool.AudioType.wav);
            Miku_Footstep2 = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Footstep2.wav", SoundTool.AudioType.wav);
            Miku_Footstep3 = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Footstep3.wav", SoundTool.AudioType.wav);
            Miku_Stun      = SoundTool.GetAudioClip(mod_folder, SFX_FOLDER, "Miku_Stun.wav", SoundTool.AudioType.wav);

            if (ConfigManager.MikuFootsteps.Value)
            {
                // Replace Braken Footsteps
                // Miku_Footstep 0-3
                SoundTool.ReplaceAudioClip("Step1", Miku_Footstep, 0.25f, FLOWERMAN_NAME);
                SoundTool.ReplaceAudioClip("Step2", Miku_Footstep1, 0.25f, FLOWERMAN_NAME);
                SoundTool.ReplaceAudioClip("Step3", Miku_Footstep2, 0.25f, FLOWERMAN_NAME);
                SoundTool.ReplaceAudioClip("Step4", Miku_Footstep3, 0.25f, FLOWERMAN_NAME);
            }

            if (ConfigManager.MikuCaught.Value)
            {
                // Replace Miku getting caught
                // Miku_Caught 0-2
                SoundTool.ReplaceAudioClip("Found1", Miku_Caught, 0.35f, FLOWERMAN_NAME);
                SoundTool.ReplaceAudioClip("Found1", Miku_Caught1, 0.35f, FLOWERMAN_NAME);
                SoundTool.ReplaceAudioClip("Found1", Miku_Caught2, 0.30f, FLOWERMAN_NAME);
            }

        }

        public static void Log_Info(object msg) { mls.LogInfo(msg); }

        public static void Log_Warning(object msg) { mls.LogWarning(msg); }

        public static void Log_Error(object msg) { mls.LogError(msg); }

        public static string Mod_Folder() { return Instance.Info.Location.TrimEnd("MikuBraken.dll".ToCharArray()); }

        // LCSoundTool does not allow for ObjectID-specific replacements, see FlowerManAIPatch.cs for implimentation
        public static AudioClip Roll_Next_Angry_Clip()
        {
            float rand_chance = UnityEngine.Random.Range(0f, 1f);

            if (rand_chance >= 0.5f)
            {
                return Miku_Angry;
            }
            else
            {
                return Miku_Angry1;
            }
        }
    }
}
