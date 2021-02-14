using System;
using UnityEngine;
using BepInEx;
using HarmonyLib;


namespace DSPMOD
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInProcess(GAME_PROCESS)]
    public class TEST : BaseUnityPlugin
    {
        public const string GUID = "ABRACADABRA";
        public const string NAME = "DSPTESTMOD";
        public const string VERSION = "1.0";
        private const string GAME_PROCESS = "DSPGAME.exe";
        private static UIGalaxySelect uiGalaxySelect;
        private bool Searching = false;
        public int SearchCount = 0;

        void Start() => Harmony.CreateAndPatchAll(typeof(DSPMOD.TEST), (string)null);

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {

                Searching = !Searching;
            }

            if (Searching)
            {
                SearchCount++;
                //System.Console.WriteLine("Search " + SearchCount);
                DSPMOD.TEST.uiGalaxySelect.Rerand();
            }

        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIGalaxySelect), "SetStarmapGalaxy")]
        public static void getUIGalaxySelect(UIGalaxySelect __instance) => DSPMOD.TEST.uiGalaxySelect = __instance;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIVirtualStarmap), "OnGalaxyDataReset")]

        public static void Postfix(UIVirtualStarmap __instance)
        {
            if (__instance.galaxyData == null)
            {
            }
            else
            {
                //System.Console.WriteLine("Refreshed");
                bool TideLockFound = false;
                bool CloseMagFound = false;
                string sTideLock = "潮汐锁定";
                var vResourceType = new string[] { "无", "铁矿", "铜矿", "硅矿", "钛矿", "石矿", "煤矿", "石油", "可燃冰", "金伯利", "分形硅", "有机晶", "光栅石", "刺笋", "磁石" };
                var vPlanetType = new string[] { "None", "Vocano", "Ocean", "Desert", "Ice", "Gas", };
                var vPlanetTheme = new string[] { "无", "地中海", "气态巨星", "气态巨星", "冰巨星", "冰巨星", "干旱荒漠", "灰烬冻土", "海洋丛林", "熔岩", "冰原冻土", "贫瘠荒漠", "戈壁", "火山灰", "红石", "草原", "水世界" };
                StarData[] StarList = __instance.galaxyData.stars;
                StarData BirthStar = StarList[0];
                string BirthStarName = BirthStar.name;
                int Seed = __instance.galaxyData.seed;

                var sPlanetData = new string[4];
                for (int PlanetIndex = 0; PlanetIndex < BirthStar.planetCount; PlanetIndex++)
                {
                    string pTheme = vPlanetTheme[BirthStar.planets[PlanetIndex].theme];
                    string sSin = BirthStar.planets[PlanetIndex].singularityString;
                    if (sSin.Contains(sTideLock))
                    {
                        TideLockFound = true;
                    }
                    sPlanetData[PlanetIndex] = "[" + (PlanetIndex + 1).ToString() + "]" + pTheme + "\t" + sSin;
                }

                int StarCount = 0;
                int NearStar = 0;
                int BlackHoleMagCount = 0;
                int NeutronStarMagCount = 0;
                double BlackHoleDistance = 0;
                double NeutronStarDistance = 0;
                int AcidCount = 0;
                var vRareCount = new int[15];
                Array.Clear(vRareCount, 0, vRareCount.Length);

                for (int StarIndex = 0; StarIndex < StarList.Length; StarIndex++)
                {
                    StarCount++;
                    PlanetData[] PlanetList = StarList[StarIndex].planets;

                    if (StarList[StarIndex].type == EStarType.BlackHole)
                    {
                        BlackHoleDistance = StarList[StarIndex].position.magnitude;
                        BlackHoleMagCount += PlanetList[0].veinSpotsSketch[14];
                        if (BlackHoleDistance < 10)
                        {
                            CloseMagFound = true;
                        }
                    }

                    if (StarList[StarIndex].type == EStarType.NeutronStar)
                    {
                        NeutronStarDistance = StarList[StarIndex].position.magnitude;
                        NeutronStarMagCount += PlanetList[0].veinSpotsSketch[14];
                        if (NeutronStarDistance < 10)
                        {
                            CloseMagFound = true;
                        }
                    }

                    if (StarList[StarIndex].position.magnitude < 6 && StarIndex != 0)
                    {
                        NearStar++;
                        foreach (PlanetData oPlanet in PlanetList)
                        {
                            if (oPlanet.type != EPlanetType.Gas)
                            {
                                for (int RareIndex = 0; RareIndex < vRareCount.Length; RareIndex++)
                                {
                                    vRareCount[RareIndex] += oPlanet.veinSpotsSketch[RareIndex];
                                }
                            }
                            if (oPlanet.theme == 13)
                            {
                                AcidCount += 1;
                            }

                        }
                    }
                }

                string sRareName = "六光年内\t";
                for (int i = 8; i < 15; i++)
                {
                    sRareName += vResourceType[i] + "\t";
                }
                sRareName += "硫酸";


                string sRareCount = "数量:\t";
                for (int i = 8; i < 15; i++)
                {
                    sRareCount += vRareCount[i] + "\t";
                }
                sRareCount += AcidCount;

                if (TideLockFound && CloseMagFound)
                {
                    System.Console.WriteLine("种子: " + Seed);
                    //System.Console.WriteLine("初始恒星: " + BirthStarName);
                    foreach (string sLine in sPlanetData)
                    {
                        System.Console.WriteLine(sLine);
                    }
                    System.Console.WriteLine(sRareName);
                    System.Console.WriteLine(sRareCount);
                    System.Console.WriteLine("恒星数目\t" + NearStar + "/" + StarCount);
                    System.Console.WriteLine("黑洞距离\t" + BlackHoleDistance.ToString("0.##") + "\t磁石数目:\t" + BlackHoleMagCount);
                    System.Console.WriteLine("中子距离\t" + NeutronStarDistance.ToString("0.##") + "\t磁石数目:\t" + NeutronStarMagCount);
                    System.Console.WriteLine("");
                }
            }
        }
    }
}
