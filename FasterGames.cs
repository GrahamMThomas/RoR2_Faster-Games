using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API.Utils;
using UnityEngine;

namespace FasterGames
{
    [R2APISubmoduleDependency("DifficultyAPI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.CwakrJax.FasterGames", "FasterGames", "1.0.0")]
    public class FasterGames : BaseUnityPlugin
    {

        public static ConfigEntry<bool> IsModEnabled { get; set; }
        public static ConfigEntry<float> spawnRate { get; set; }

        public static ConfigEntry<float> baseExpMultiplier { get; set; }
        public static ConfigEntry<float> expPerPlayerMultiplier { get; set; }

        public static ConfigEntry<float> baseMoney { get; set; }
        public static ConfigEntry<float> moneyPerPlayer { get; set; }

        public static ConfigEntry<float> moveSpeed { get; set; }

        public static ConfigEntry<float> scalingPercentage { get; set; }

        public static ConfigEntry<float> baseInteractableMultiplier { get; set; }
        public static ConfigEntry<float> perPlayerInteractableMultiplier { get; set; }

        public static ConfigEntry<int> chanceShrineItemCount { get; set; }
        public static ConfigEntry<float> chanceShrineCostMultiplier { get; set; }

        public static ConfigEntry<float> teleporterChargeMultiplier { get; set; }
        public void Awake()
        {
            InitConfig();
            if (!IsModEnabled.Value)
            {
                return;
            }
            string InitMesssage = "Your game is Faster!";
            bool hasGameUpdated = false;

            Chat.AddMessage(InitMesssage);
            Logger.LogInfo(InitMesssage);

            Color DifficultyColor = new Color(0.94f, 0.51f, 0.15f);
            DifficultyDef FasterDifficulty = new DifficultyDef(
                (scalingPercentage.Value - 1) * 5, //0 is Normal mode. 2.5f is 50% which is monsoon
                "Faster",
                ":Assets/FasterGames/DifficultyIcon.png",
                "Gotta go Fast!",
                DifficultyColor,
                "Gotta go Faster!",
                true
                );

            DifficultyIndex diffIndex = R2API.DifficultyAPI.AddDifficulty(FasterDifficulty);

            RoR2.Run.onRunStartGlobal += (RoR2.Run run) => {
                if (run.selectedDifficulty == diffIndex)
                {
                    Hooks myHooks = new Hooks();
                    myHooks.pluginLogger = Logger;
                    myHooks.IncreaseExpCoefficient(baseExpMultiplier.Value, expPerPlayerMultiplier.Value);
                    myHooks.IncreaseMoneyMultiplier(baseMoney.Value, moneyPerPlayer.Value);
                    myHooks.IncreaseBaseStats(moveSpeed.Value);
                    myHooks.IncreaseChestSpawnRate(baseInteractableMultiplier.Value, perPlayerInteractableMultiplier.Value);
                    myHooks.OverhaulChanceShrines(chanceShrineItemCount.Value, chanceShrineCostMultiplier.Value);
                    myHooks.IncreaseTeleporterChargeSpeed(teleporterChargeMultiplier.Value);
                    myHooks.NoCoolDown3dPrinter();
                    hasGameUpdated = true;
                }
            };

            RoR2.SteamworksLobbyManager.onLobbiesUpdated += () =>
            {
                if (hasGameUpdated) { 
                    Chat.AddMessage("[WARN] If you plan on playing another difficulty, you must restart your game!");
                }
            };
        }

        public void InitConfig()
        {
            IsModEnabled = Config.Bind<bool>(
                "Enabled",
                "isModEnabled",
                true,
                "Turn this off if you want to play the game normally again. \nBut why would you?"
            );

            spawnRate = Config.Bind<float>(
                "Game",
                "mobSpawnRate",
                3f,
                "It's unclear how this works to me. \nBase Game value: 1"
            );

            baseExpMultiplier = Config.Bind<float>(
                "Player",
                "baseExpMultiplier",
                2.5f,
                "Base Exp Gain. \nBase Game value: 1"
            );

            expPerPlayerMultiplier = Config.Bind<float>(
                "Player",
                "expPerPlayerMultiplier",
                0.5f,
                "Extra Exp Per player. Helps with multiplayer scaling. \nBase Game value: 0"
            );

            baseMoney = Config.Bind<float>(
                "Player",
                "baseMoneyMultiplier",
                1.5f,
                "Increases money gain for killing stuff. \nBase Game value: 1"
            );

            moneyPerPlayer = Config.Bind<float>(
                "Player",
                "moneyPerPlayer",
                0.1f,
                "Increases money gain per player. Helps with multiplayer scaling. \nBase Game value: 0"
            );

            moveSpeed = Config.Bind<float>(
                "Player",
                "moveSpeed",
                10f,
                "Increases base speed so you don't move so slow. \nBase Game value: 7"
            );

            scalingPercentage = Config.Bind<float>(
                "Game",
                "scalingIncreaseMultiplier",
                2.75f,
                "Increases game scaling. \nNormal Mode value: 1 \nMonsoon value: 1.5"
            );

            baseInteractableMultiplier = Config.Bind<float>(
                "Game",
                "interactableSpawnRateMultiplier",
                2.5f,
                "Increases Interactable spawn Rate. \nBase Game value: 1"
            );

            perPlayerInteractableMultiplier = Config.Bind<float>(
                "Game",
                "interactableSpawnRatePerPlayerMultiplier",
                0.5f,
                "Increases Interactable spawn Rate per extra Player Past 1. \nBase Game value: 0"
            );

            chanceShrineItemCount = Config.Bind<int>(
                "Game",
                "chanceShrineItemAmount",
                5,
                "Increases amount of items you get from chance shrine before it's disabled. \nBase Game value: 2"
            );

            chanceShrineCostMultiplier = Config.Bind<float>(
                "Game",
                "chanceShrineCostMultiplier",
                1.2f,
                "Increases cost of chance shrine by this amount every use (exponential).  \nBase Game value: 1.4"
            );

            teleporterChargeMultiplier = Config.Bind<float>(
                "Game",
                "teleporterChargeSpeedMultiplier",
                1.5f,
                "Increases Speed of Teleporter Charge.  \nBase Game value: 1"
            );
        }
    }
}
