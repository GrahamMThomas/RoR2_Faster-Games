using BepInEx;
using BepInEx.Configuration;
using RoR2;
using R2API.Utils;
using UnityEngine;
using System.Reflection;
using System.Security.Permissions;

#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

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

        [System.Obsolete]
        public void Awake()
        {
            InitConfig();
            if (!IsModEnabled.Value)
            {
                return;
            }
            string InitMessage = "Your game is Faster!";
            Logger.LogInfo(InitMessage);

            Sprite diffIcon;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FasterGames.assets.fastergames"))
            {
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                diffIcon = bundle.LoadAsset<Sprite>("Assets/Import/icons/FasterDifficultyIcon.png");
            }

            Color DifficultyColor = new Color(0.875f, 0.875f, 0.14f);

            DifficultyDef FasterDifficulty = new DifficultyDef(
                (scalingPercentage.Value - 1) * 5, //0 is Normal mode. 2.5f is 50% which is monsoon
                "Faster",
                "",
                GenerateDifficultyDescription(),
                DifficultyColor,
                "Faster",
                true
                );

            DifficultyIndex diffIndex = R2API.DifficultyAPI.AddDifficulty(FasterDifficulty, diffIcon);

            RoR2.Run.onRunStartGlobal += (RoR2.Run run) => {
                if (run.selectedDifficulty == diffIndex)
                {
                    ChatMessage.SendColored(InitMessage, new Color(0.78f, 0.788f, 0.3f));
                    Hooks myHooks = new Hooks();
                    myHooks.pluginLogger = Logger;
                    myHooks.IncreaseSpawnRate();
                    myHooks.IncreaseExpCoefficient(baseExpMultiplier.Value, expPerPlayerMultiplier.Value);
                    myHooks.IncreaseMoneyMultiplier(baseMoney.Value, moneyPerPlayer.Value);
                    myHooks.IncreaseBaseStats(moveSpeed.Value);
                    myHooks.IncreaseChestSpawnRate(baseInteractableMultiplier.Value, perPlayerInteractableMultiplier.Value);
                    myHooks.OverhaulChanceShrines(chanceShrineItemCount.Value, chanceShrineCostMultiplier.Value);
                    myHooks.IncreaseTeleporterChargeSpeed(teleporterChargeMultiplier.Value);
                    myHooks.NoCoolDown3dPrinter();
                    ChatMessage.SendColored("[FasterGames] If you plan on playing different difficulty, you must restart your game!", new Color(0.78f, 0.788f, 0.3f));
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

        public string GenerateDifficultyDescription()
        {
            string desc = "Games go 3x faster. For those who love the game, but not how long it takes.\n<style=cStack>";
            desc = string.Join("\n",
                desc,
                $"> Difficulty Scaling: <style=cDeath>+{(scalingPercentage.Value - 1) * 100}%</style>\n",
                "<style=cIsHealing>+</style> Exp Rate",
                "<style=cIsHealing>+</style> Money on Kill",
                "<style=cIsHealing>+</style> Base Move Speed",
                "<style=cIsHealing>+</style> Interactable Spawn Rate",
                "<style=cIsHealing>+</style> Max items from Chance Shrine\n",
                "> Reduces Teleporter Charge Time", 
                "> Spammable Printers and Chance Shrines</style>",
                "\nThese values can be changed in the mod config.");

            return desc;
        }
    }
}
