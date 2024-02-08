using HarmonyLib;
using LCSoundTool;
using UnityEngine;

namespace MikuBraken.Patches
{

    [HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowerManAIPatch
    {

        [HarmonyPatch(typeof(FlowermanAI), "Start")]
        [HarmonyPostfix]
        static void OverrideStart(FlowermanAI __instance)
        {
            // Clone miku prefab
            GameObject MikuClone = Object.Instantiate(MikuBrakenBase.Miku, __instance.gameObject.transform);
            MikuClone.name = "Miku(Clone)";
            MikuClone.transform.localPosition = Vector3.zero; // Possible fix for weird collision on stairwells?

            // Make Miku Visble
            MikuClone.SetActive(true);

            // Hide braken model
            Renderer[] renderers = __instance.transform.Find("FlowermanModel").GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }

            // Replace braken's nametag on scan
            __instance.GetComponentInChildren<ScanNodeProperties>().headerText = "Hatsune Miku";

            // Replace SFX
            // Everyone can hear her terror
            __instance.creatureAngerVoice.maxDistance = 40;
            __instance.creatureAngerVoice.clip = MikuBrakenBase.SoundFX[0]; // Miku_Angry

            __instance.dieSFX = MikuBrakenBase.SoundFX[3]; // Miku_Dies
            __instance.enemyType.stunSFX = MikuBrakenBase.SoundFX[8]; // Miku_Stun
        }

        [HarmonyPatch(typeof(FlowermanAI), "killAnimation")]
        [HarmonyPostfix]

        static void OverrideKillAnimation(FlowermanAI __instance)
        {
            // Miku will play an audio clip while still playing the neck crack sfx
            __instance.crackNeckAudio.PlayOneShot(MikuBrakenBase.SoundFX[2]); // Miku_CrackNeck
        }

        [HarmonyPatch(typeof(FlowermanAI), "KillEnemy")]
        [HarmonyPostfix]
        static void KillEnemyOverride(FlowermanAI __instance)
        {
            Transform transform = __instance.transform;
            Quaternion rotation = transform.rotation;
            float y = rotation.eulerAngles.y;
            rotation = __instance.transform.rotation;
            transform.rotation = Quaternion.Euler(-90f, y, rotation.eulerAngles.z);
        }

    }
}
