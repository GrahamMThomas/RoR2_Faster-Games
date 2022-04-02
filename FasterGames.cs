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
    [BepInPlugin("com.CwakrJax.FasterGames", "FasterGames", "1.0.1")]
    public class FasterGames : BaseUnityPlugin
    {

        public static ConfigEntry<bool> IsModEnabled { get; set; }
        public static ConfigEntry<float> SpawnRate { get; set; }

        public static ConfigEntry<float> BaseExpMultiplier { get; set; }
        public static ConfigEntry<float> ExpPerPlayerMultiplier { get; set; }

        public static ConfigEntry<float> BaseMoney { get; set; }
        public static ConfigEntry<float> MoneyPerPlayer { get; set; }

        public static ConfigEntry<float> MoveSpeed { get; set; }

        public static ConfigEntry<float> ScalingPercentage { get; set; }

        public static ConfigEntry<float> BaseInteractableMultiplier { get; set; }
        public static ConfigEntry<float> PerPlayerInteractableMultiplier { get; set; }

        public static ConfigEntry<int> ChanceShrineItemCount { get; set; }
        public static ConfigEntry<float> ChanceShrineCostMultiplier { get; set; }

        public static ConfigEntry<float> TeleporterChargeMultiplier { get; set; }

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
                (ScalingPercentage.Value - 1) * 5, //0 is Normal mode. 2.5f is 50% which is monsoon
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
                    myHooks.IncreaseExpCoefficient(BaseExpMultiplier.Value, ExpPerPlayerMultiplier.Value);
                    myHooks.IncreaseMoneyMultiplier(BaseMoney.Value, MoneyPerPlayer.Value);
                    myHooks.IncreaseBaseStats(MoveSpeed.Value);
                    myHooks.IncreaseChestSpawnRate(BaseInteractableMultiplier.Value, PerPlayerInteractableMultiplier.Value);
                    myHooks.OverhaulChanceShrines(ChanceShrineItemCount.Value, ChanceShrineCostMultiplier.Value);
                    myHooks.IncreaseTeleporterChargeSpeed(TeleporterChargeMultiplier.Value);
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

            BaseExpMultiplier = Config.Bind<float>(
                "Player",
                "baseExpMultiplier",
                2.5f,
                "Base Exp Gain. \nBase Game value: 1"
            );

            ExpPerPlayerMultiplier = Config.Bind<float>(
                "Player",
                "expPerPlayerMultiplier",
                0.5f,
                "Extra Exp Per player. Helps with multiplayer scaling. \nBase Game value: 0"
            );

            BaseMoney = Config.Bind<float>(
                "Player",
                "baseMoneyMultiplier",
                1.5f,
                "Increases money gain for killing stuff. \nBase Game value: 1"
            );

            MoneyPerPlayer = Config.Bind<float>(
                "Player",
                "moneyPerPlayer",
                0.1f,
                "Increases money gain per player. Helps with multiplayer scaling. \nBase Game value: 0"
            );

            MoveSpeed = Config.Bind<float>(
                "Player",
                "moveSpeed",
                10f,
                "Increases base speed so you don't move so slow. \nBase Game value: 7"
            );

            ScalingPercentage = Config.Bind<float>(
                "Game",
                "scalingIncreaseMultiplier",
                2.75f,
                "Increases game scaling. \nNormal Mode value: 1 \nMonsoon value: 1.5"
            );

            BaseInteractableMultiplier = Config.Bind<float>(
                "Game",
                "interactableSpawnRateMultiplier",
                2.5f,
                "Increases Interactable spawn Rate. \nBase Game value: 1"
            );

            PerPlayerInteractableMultiplier = Config.Bind<float>(
                "Game",
                "interactableSpawnRatePerPlayerMultiplier",
                0.5f,
                "Increases Interactable spawn Rate per extra Player Past 1. \nBase Game value: 0"
            );

            ChanceShrineItemCount = Config.Bind<int>(
                "Game",
                "chanceShrineItemAmount",
                5,
                "Increases amount of items you get from chance shrine before it's disabled. \nBase Game value: 2"
            );

            ChanceShrineCostMultiplier = Config.Bind<float>(
                "Game",
                "chanceShrineCostMultiplier",
                1.2f,
                "Increases cost of chance shrine by this amount every use (exponential).  \nBase Game value: 1.4"
            );

            TeleporterChargeMultiplier = Config.Bind<float>(
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
                $"> Difficulty Scaling: <style=cDeath>+{(ScalingPercentage.Value - 1) * 100}%</style>\n",
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
