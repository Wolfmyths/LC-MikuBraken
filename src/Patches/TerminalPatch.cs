using HarmonyLib;

namespace MikuBraken.Patches
{
    internal class TerminalPatch
    {
        [HarmonyPatch(typeof(Terminal), "Awake")]
        [HarmonyPostfix]
        static void OverrideTerminal(Terminal __instance)
        {

            if (ConfigManager.MikuTerminal.Value)
            {
                __instance.enemyFiles[1].creatureName = "Hatsune Miku";
                __instance.enemyFiles[1].displayText = "PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\n\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPi-\r\n\r\nPiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii!\r\n\r\n\r\nSaa nome, o mae suki darou? Yasai jusu\r\nWatashi ga kimeta, ima kimeta\r\nDakara nonde watashi no yasai JUICE,\r\nKakaku wa nihyaku en~!\r\n\r\nsoiya! soiya!\r\ndotsee! dotsee!\r\nsoiya! soiya!\r\ndotsee! dotsee!\r\n\r\n\r\nMaroyaka, yasai JUICE,\r\nFuwa fuwa yasai juice\r\nIchiban osusume na no wa\r\nMidori no jusuuu~!\r\n\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nVegitabura-na a-aa-aa-a-a-aaa!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nSeimei afureta, a-aa-aa-a-a-aaa!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nAnata mo ima a-aa-aa-a-a-aaa!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nYasai jusu ga suki ni naru!\r\n\r\n\r\nPiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiii!\r\n\r\nCome now, don't you love\r\nit? Vegetable juice\r\nThat's my decision, I've decided it now.\r\nThus you should drink\r\nmy vegetable juice,\r\nIt costs just 200 yen~!\r\n\r\nsoiya! soiya!\r\ndotsee! dotsee!\r\nsoiya! soiya!\r\ndotsee! dotsee!\r\n\r\n\r\nIt'll agree with you, vegetable juice,\r\nLight-weighted vegetable juice\r\nThe best juice I choose for you would be\r\nthe green vegtable juice~!\r\n\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nWe are vegetarian a-aa-aa-a-a-aaa!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nbursting with life, a-aa-aa-a-a-aaa!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nhappy vegetarians a-aa-aa-a-a-aaa!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nPoPiPoPiPo PoPiPo! PoPiPoPiPo PoPiPo!\r\nY.A.S.A Come here to love\r\n\r\nvegetable juice!";
                __instance.terminalNodes.allKeywords[36].word = "hatsune miku";
            }

            //__instance.enemyFiles[1].displayVideo = null; Video asset for later update
        }
    }
}
