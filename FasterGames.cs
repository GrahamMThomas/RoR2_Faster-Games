using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API.Utils;

namespace FasterGames
{
    [R2APISubmoduleDependency("AssetPlus")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.CwakrJax.FasterGames", "FasterGames", "0.0.1")]
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
