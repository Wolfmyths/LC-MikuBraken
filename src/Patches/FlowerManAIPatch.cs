using HarmonyLib;
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
            if (ConfigManager.MikuModel.Value)
            {

                // Clone miku prefab
                GameObject MikuClone = Object.Instantiate(MikuBrakenBase.Miku, __instance.gameObject.transform);
                MikuClone.name = "Miku(Clone)";
                MikuClone.transform.localPosition = Vector3.zero; // Possible fix for weird collision on stairwells?
                MikuClone.SetActive(true);

                if (ConfigManager.MikuGlowingEyes.Value)
                {
                    // Clone glowing eyes prefab
                    GameObject MikuEyes = Object.Instantiate(MikuBrakenBase.Miku_Eyes, MikuClone.gameObject.transform);
                    MikuEyes.name = "MikuEyes(Clone)";

                    MikuEyes.SetActive(true);
                    MikuEyes.transform.localPosition = new Vector3(-0.0817f, 2.5481f, 0.1302f);
                }

                // Hide braken model
                Renderer[] renderers = __instance.transform.Find("FlowermanModel").GetComponentsInChildren<Renderer>();
                foreach (Renderer mesh in renderers)
                {
                    mesh.enabled = false;
                }
            }

            if (ConfigManager.MikuScanTag.Value)
            {
                // Replace braken's nametag on scan
                __instance.GetComponentInChildren<ScanNodeProperties>().headerText = "Hatsune Miku";
            }

            // Replace SFX
            if (ConfigManager.MikuAngry.Value)
            {
                // Everyone can hear her terror
                __instance.creatureAngerVoice.maxDistance = 40;
            }


            if (ConfigManager.MikuDies.Value)
            {
                __instance.dieSFX = MikuBrakenBase.Miku_Dies; // Miku_Dies
            }

            if (ConfigManager.MikuStun.Value)
            {
                __instance.enemyType.stunSFX = MikuBrakenBase.Miku_Stun; // Miku_Stun
            }

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
                    Quaternion rotation = transform.rotation;
                    float y = rotation.eulerAngles.y;
                    rotation = __instance.transform.rotation;
                    transform.rotation = Quaternion.Euler(-90f, y, rotation.eulerAngles.z);
                }
                else
                {
                    __instance.transform.Find("Miku(Clone)").gameObject.SetActive(false);
                }
            }
        }
    }
}
