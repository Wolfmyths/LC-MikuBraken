using HarmonyLib;
using LCSoundTool;
using UnityEngine;

namespace MikuBraken.Patches
{
    internal class MikuProperties : MonoBehaviour
    {
        private bool isDead { get; set; }
        private GameObject MikuClone { get; set; }
        private FlowermanAI Flowerman { get; set; }

        private void Start()
        {
            Flowerman = GetComponent<FlowermanAI>();

            isDead = false;

            // Clone miku prefab
            MikuClone = Instantiate(MikuBrakenBase.Miku, Flowerman.gameObject.transform);
            MikuClone.name = "Miku(Clone)";

            // Replace braken prefab
            Flowerman.enemyType.enemyPrefab = MikuClone;
            MikuClone.SetActive(true);

            // Hide braken model
            Renderer[] renderers = Flowerman.transform.Find("FlowermanModel").GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }

            // Replace braken's nametag on scan
            Flowerman.GetComponentInChildren<ScanNodeProperties>().headerText = "Hatsune Miku";
        }

        private void Update()
        {

            // Death animation
            if (Flowerman.isEnemyDead && isDead == false)
            {

                Transform transform = MikuClone.transform;
                Quaternion rotation = transform.rotation;
                float y = rotation.eulerAngles.y;
                rotation = MikuClone.transform.rotation;
                transform.rotation = Quaternion.Euler(-90f, y, rotation.eulerAngles.z);

                isDead = true;
            }
        }
    }

    [HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowerManAIPatch
    {

        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        static void OverrideStart(FlowermanAI __instance)
        {
            __instance.gameObject.AddComponent<MikuProperties>();
        }

        [HarmonyPatch("killAnimation")]
        [HarmonyPostfix]

        static void OverrideKillAnimation(FlowermanAI __instance)
        {
            // Miku will play an audio clip while still playing the neck crack sfx
            __instance.crackNeckAudio.PlayOneShot(MikuBrakenBase.SoundFX[2]); // Miku_CrackNeck
        }

        [HarmonyPatch("Update")]
        [HarmonyPostfix]

        static void OverrideUpdate(FlowermanAI __instance)
        {
            // Everyone can hear her terror
            __instance.creatureAngerVoice.maxDistance = 40;
            __instance.creatureAngerVoice.clip = MikuBrakenBase.SoundFX[0]; // Miku_Angry

            __instance.dieSFX = MikuBrakenBase.SoundFX[3]; // Miku_Dies
            __instance.enemyType.stunSFX = MikuBrakenBase.SoundFX[8]; // Miku_Stun

        }
    }
}
