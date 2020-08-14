using R2API.Utils;
using RoR2;
using UnityEngine;
//using MonoMod.Cil;

namespace FasterGames
{
    public class Hooks
    {
        public BepInEx.Logging.ManualLogSource pluginLogger;

        public void IncreaseSpawnRate()
        {
            // Dunno if this is actually working...
            pluginLogger.LogInfo("Increasing Spawn Rate");
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.minSeriesSpawnInterval = 2f; // Default is 0.1
                self.maxSeriesSpawnInterval = 3f; // Default is 1
                orig(self, deltaTime);
            };
        }

        public void IncreaseExpCoefficient()
        {
            pluginLogger.LogInfo("Increasing Exp Gain");
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.expRewardCoefficient = new float();
                self.expRewardCoefficient = 0.5f + Run.instance.participatingPlayerCount * 0.1f; // Default is 0.2
                orig(self, deltaTime);
            };
        }

        public void IncreaseMoneyMultiplier()
        {
            pluginLogger.LogInfo("Increasing Money on Kill");
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.creditMultiplier = new float();
                self.creditMultiplier = 1.5f + Run.instance.participatingPlayerCount * 0.5f; // Default is 1
                orig(self, deltaTime);
            };
        }

        public void IncreaseBaseStats()
        {
            pluginLogger.LogInfo("Increasing Base Move Speed");
            foreach (string bodyName in new string[] { "CommandoBody", "ToolbotBody", "HuntressBody", "EngiBody", "MageBody", "MercBody", "TreebotBody", "LoaderBody", "CrocoBody", "CaptainBody" })
            {
                GameObject obj = Resources.Load<GameObject>($"Prefabs/CharacterBodies/{bodyName}");
                if (obj)
                {
                    CharacterBody body = obj.GetComponent<CharacterBody>();
                    if (body)
                    {
                        body.baseMoveSpeed = 10f; // Default is 7
                    }
                    else
                    {
                        pluginLogger.LogError("The prefab loaded has no character body");
                    }
                }
                else
                {
                    pluginLogger.LogError("That is not a valid prefab");
                }
            }
        }


        public void OverrideDifficulties()
        {
            pluginLogger.LogInfo("Overriding All Difficulties with Faster");

            Color DifficultyColor = new Color(0.94f, 0.51f, 0.15f);
            DifficultyDef FasterDifficulty = new DifficultyDef(
                9f, //0 is Normal mode. 2.5f is 50% which is monsoon
                "Faster",
                ":Assets/FasterGames/DifficultyIcon.png",
                "Gotta go Fast!",
                DifficultyColor,
                "Gotta go Faster!",
                true
                );
            On.RoR2.DifficultyCatalog.GetDifficultyDef += (orig, self) =>
            {
                return FasterDifficulty;
                // orig(self);
            };
        }

        //Currently Borked
        public void IncreaseDifficultyScaling()
        {
            pluginLogger.LogInfo("Created Faster Difficulty");

            Color DifficultyColor = new Color(0.94f, 0.51f, 0.15f);

            DifficultyDef FasterDef = new DifficultyDef(
                            9f, //0 is Normal mode. 2.5f is 50% which is monsoon
                            "Faster",
                            ":Assets/FasterGames/DifficultyIcon.png",
                            "Gotta go Faster!",
                            DifficultyColor,
                            "Hurr Durr Dunno what to put here",
                            false
                            );
            DifficultyIndex DelugeIndex = R2API.DifficultyAPI.AddDifficulty(FasterDef);
            pluginLogger.LogInfo(DelugeIndex);
        }

        public void IncreaseChestSpawnRate()
        {
            pluginLogger.LogInfo("Increasing Chest Spawn Rate");
            On.RoR2.ClassicStageInfo.Awake += (On.RoR2.ClassicStageInfo.orig_Awake orig, ClassicStageInfo self) =>
            {
                self.sceneDirectorInteractibleCredits = 400 + (Run.instance.participatingPlayerCount * 100); // Default is 200
                orig(self);
            };
        }

        // Credit to https://github.com/HimeTwyla/TwylaRoR2Mods/blob/master/InfiniteChance/InfiniteChance.cs
        public void OverhaulChanceShrines()
        {
            pluginLogger.LogInfo("Overhauling Chance Shrine");

            int maxPurchases = 5;
            float CostMult = 1.4f;

            // Increase Max Purchases and update Cost Scaling
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);
                self.maxPurchaseCount = maxPurchases;
                HarmonyLib.AccessTools.Field(HarmonyLib.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, CostMult);
            };

            // Remove timer on chance shrine
            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, interactor) =>
            {
                orig(self, interactor);
                self.SetFieldValue("refreshTimer", 0f);
            };
        }

        //Credit to https://thunderstore.io/package/der10pm/ChargeInHalf/
        public void IncreaseTeleporterChargeSpeed()
        {
            pluginLogger.LogInfo("Increasing Teleporter Charge Speed");
            
            On.RoR2.HoldoutZoneController.OnEnable += (orig, self) =>
            {
                self.baseChargeDuration /= 1.5f;
                orig(self);
            };
        }
    }

}