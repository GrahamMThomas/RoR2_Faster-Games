using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API.Utils;
using UnityEngine;

namespace FasterGames
{
    [R2APISubmoduleDependency("DifficultyAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.CwakrJax.FasterGames", "FasterGames", "0.2.0")]
    public class FasterGames : BaseUnityPlugin
    {
        public static ConfigWrapper<bool> ModIsEnabled;
        public void Awake()
        {
            InitConfig();
            string InitMesssage = "Your game is Faster!";
            Chat.AddMessage(InitMesssage);
            Logger.LogInfo(InitMesssage);

            Hooks myHooks = new Hooks();
            myHooks.pluginLogger = Logger;
            myHooks.IncreaseSpawnRate();
            myHooks.IncreaseExpCoefficient();
            myHooks.IncreaseMoneyMultiplier();
            myHooks.IncreaseBaseStats();
            myHooks.IncreaseDifficultyScaling();
            myHooks.IncreaseChestSpawnRate();


            On.RoR2.DifficultyCatalog.GetDifficultyDef += (orig, self) =>
            {
                return hackedDiffDef();
                orig(self);
            };
        }


        public DifficultyDef hackedDiffDef()
        {
            Color DifficultyColor = new Color(0.94f, 0.51f, 0.15f);
            return new DifficultyDef(
                9f, //0 is Normal mode. 2.5f is 50% which is monsoon
                "Faster",
                ":Assets/FasterGames/DifficultyIcon.png",
                "Gotta go Faster!",
                DifficultyColor,
                "Hurr Durr Dunno what to put here",
                false
                );
        }
        public void InitConfig()
        {
            ModIsEnabled = Config.Wrap(
                "Settings",
                "ModEnabled",
                "Toggles mod.",
            true);
        }
    }
}
