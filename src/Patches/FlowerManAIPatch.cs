using HarmonyLib;
using System.CodeDom;
using UnityEngine;
using UnityEngine.UIElements;

namespace MikuBraken.Patches
{

    [HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowerManAIPatch
    {

        const int ENRAGE_BEHAVIOR_INDEX = 2;

        [HarmonyPatch(typeof(FlowermanAI), "Start")]
        [HarmonyPostfix]
        static void OverrideStart(FlowermanAI __instance)
        {
            const string SCAN_NODE_NAME = "ScanNode";
            const string SCAN_NODE_TEXT = "Hatsune Miku";
            const string FLOWERMAN_MODEL_NAME = "FlowermanModel";

            if (ConfigManager.MikuModel.Value)
            {

                // Clone miku prefab
                GameObject MikuClone = Object.Instantiate(MikuBrakenBase.Miku, __instance.transform);
                MikuClone.name = "Miku(Clone)";
                MikuClone.SetActive(true);

                if (ConfigManager.MikuGlowingEyes.Value)
                {
                    // Clone glowing eyes prefab
                    GameObject MikuEyes = Object.Instantiate(MikuBrakenBase.Miku_Eyes, MikuClone.transform);
                    MikuEyes.name = "MikuEyes(Clone)";
                    MikuEyes.transform.localPosition = new Vector3(-0.0817f, 2.5481f, 0.1302f);
                    MikuEyes.SetActive(true);

                }

                Renderer[] renderers = __instance.transform.Find(FLOWERMAN_MODEL_NAME).GetComponentsInChildren<Renderer>();
                // Hide braken model
                foreach (Renderer renderer in renderers)
                {
                    renderer.enabled = false;
                }
            }

            if (ConfigManager.MikuScanTag.Value)
            {
                // Replace braken's nametag on scan
                __instance.transform.Find(SCAN_NODE_NAME).GetComponent<ScanNodeProperties>().headerText = SCAN_NODE_TEXT;
            }

            // Replace SFX
            if (ConfigManager.MikuAngry.Value)
            {
                // Everyone can hear her terror
                __instance.creatureAngerVoice.maxDistance = 35f;
                
            }

            if (ConfigManager.MikuDies.Value)
            {
                __instance.dieSFX = MikuBrakenBase.Miku_Dies; // Miku_Dies
            }

            if (ConfigManager.MikuStun.Value)
            {
                __instance.enemyType.stunSFX = MikuBrakenBase.Miku_Stun; // Miku_Stun
            }

            // For debugging
            //MikuBrakenBase.Log_Info("Printing Bracken Children in Scenetree:");
            //foreach (Transform transform in __instance.transform)
            //{
            //    MikuBrakenBase.Log_Info($"Name: {transform.name}, Type: {transform.gameObject.GetType()}");
            //    foreach (Component comp in transform.GetComponents<AudioSource>())
            //    {
            //        MikuBrakenBase.Log_Info($"\tName: {comp.name}, Type: {comp.GetType()}");
            //    }
            //}

        }

        // Function call is used as an event to make Miku's Angry sound more random
        [HarmonyPatch(typeof(EnemyAI), "PlayAudioOfCurrentState")]
        [HarmonyPrefix]
        static void PreOverridePlayAudioOfCurrentState(FlowermanAI __instance)
        {
            if (!ConfigManager.MikuAngry.Value || __instance.currentBehaviourStateIndex != ENRAGE_BEHAVIOR_INDEX) { return; }
            __instance.creatureAngerVoice.clip = MikuBrakenBase.Roll_Next_Angry_Clip();
        }

        [HarmonyPatch(typeof(FlowermanAI), "killAnimation")]
        [HarmonyPostfix]

        static void OverrideKillAnimation(FlowermanAI __instance)
        {
            if (ConfigManager.MikuCrackNeck.Value)
            {
                // Miku will play an audio clip while still playing the neck crack sfx
                __instance.crackNeckAudio.PlayOneShot(MikuBrakenBase.Miku_CrackNeck); // Miku_CrackNeck
            }
        }

        [HarmonyPatch(typeof(FlowermanAI), "KillEnemy")]
        [HarmonyPostfix]
        static void overrideKillEnemy(FlowermanAI __instance)
        {
            if (ConfigManager.MikuModel.Value)
            {
                if (!ConfigManager.MikuDeleteOnKilled.Value)
                {
                    Transform transform = __instance.transform.Find("Miku(Clone)");

                    transform.Rotate(-90f, 0f, 0f);
                }
                else
                {
                    __instance.transform.Find("Miku(Clone)").gameObject.SetActive(false);
                }
            }

        }
    }
}
