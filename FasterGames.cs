using BepInEx;
using BepInEx.Configuration;
using RoR2;
// using R2API.Utils;
using UnityEngine;

namespace FasterGames
{
    //[R2APISubmoduleDependency("DifficultyAPI")]
    //[BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.CwakrJax.FasterGames", "FasterGames", "0.3.0")]
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
            // myHooks.IncreaseDifficultyScaling();
            myHooks.OverrideDifficulties();
            myHooks.IncreaseChestSpawnRate();
            myHooks.OverhaulChanceShrines();
            myHooks.IncreaseTeleporterChargeSpeed();


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
