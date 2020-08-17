//using R2API.Utils;
using RoR2;
using UnityEngine;
//using MonoMod.Cil;

namespace FasterGames
{
    public class Hooks
    {
        public BepInEx.Logging.ManualLogSource pluginLogger;

        public void IncreaseSpawnRate(float spawnRate)
        {
            // Dunno if this is actually working...
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.minSeriesSpawnInterval = 1f * (spawnRate * 0.8f); // Default is 0.1
                self.maxSeriesSpawnInterval = 1f * spawnRate; // Default is 1
                orig(self, deltaTime);
            };
            pluginLogger.LogInfo($"Increased Spawn Rate: {spawnRate}x");
        }

        public void IncreaseExpCoefficient(float baseExpMultiplier, float expPerPlayerMultiplier)
        {
            
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.expRewardCoefficient = new float();
                self.expRewardCoefficient = baseExpMultiplier / 5 + Run.instance.participatingPlayerCount * expPerPlayerMultiplier/5; // Default is 0.2
                orig(self, deltaTime);
            };
            pluginLogger.LogInfo($"Increased Exp Rate: {baseExpMultiplier}x; Per Player: {expPerPlayerMultiplier}x");
        }

        public void IncreaseMoneyMultiplier(float baseMoney, float moneyPerPlayer)
        {
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.creditMultiplier = new float();
                self.creditMultiplier = baseMoney + Run.instance.participatingPlayerCount * moneyPerPlayer; // Default is 1
                orig(self, deltaTime);
            };
            pluginLogger.LogInfo($"Increased Money on Kill: {baseMoney}x; Per Player: {moneyPerPlayer}x");
        }

        public void IncreaseBaseStats(float moveSpeed)
        {
            foreach (string bodyName in new string[] { "CommandoBody", "ToolbotBody", "HuntressBody", "EngiBody", "MageBody", "MercBody", "TreebotBody", "LoaderBody", "CrocoBody", "CaptainBody", "SniperBody" })
            {
                GameObject obj = Resources.Load<GameObject>($"Prefabs/CharacterBodies/{bodyName}");
                if (obj)
                {
                    CharacterBody body = obj.GetComponent<CharacterBody>();
                    if (body)
                    {
                        body.baseMoveSpeed = moveSpeed; // Default is 7
                    }
                    else
                    {
                        pluginLogger.LogWarning($"The prefab {bodyName} loaded has no character body");
                    }
                }
                else
                {
                    pluginLogger.LogWarning($"That is not a valid prefab: {bodyName}");
                }
            }
            pluginLogger.LogInfo($"Increased Base Move Speed to {moveSpeed}");
        }


        public void OverrideDifficulties(float scalingMultiplier)
        {
            Color DifficultyColor = new Color(0.94f, 0.51f, 0.15f);
            DifficultyDef FasterDifficulty = new DifficultyDef(
                (scalingMultiplier-1)*5, //0 is Normal mode. 2.5f is 50% which is monsoon
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

            pluginLogger.LogInfo($"Overrided All Difficulties with Faster; Scaling Multiplier: {scalingMultiplier}x");
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
        }

        public void IncreaseChestSpawnRate(float baseInteractableMultiplier, float perPlayerInteractableMultiplier)
        {
            On.RoR2.ClassicStageInfo.Awake += (On.RoR2.ClassicStageInfo.orig_Awake orig, ClassicStageInfo self) =>
            {
                self.sceneDirectorInteractibleCredits = (int)(200 * baseInteractableMultiplier) + (int)((Run.instance.participatingPlayerCount - 1) * 200 * perPlayerInteractableMultiplier); // Default is 200
                orig(self);
            };
            pluginLogger.LogInfo($"Increased Chest Spawn Rate: {baseInteractableMultiplier}x; Per Player: {perPlayerInteractableMultiplier}x");
        }

        // Credit to https://github.com/HimeTwyla/TwylaRoR2Mods/blob/master/InfiniteChance/InfiniteChance.cs
        public void OverhaulChanceShrines(int maxPurchases, float costMult)
        {
            // Increase Max Purchases and update Cost Scaling
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);
                self.maxPurchaseCount = maxPurchases;

                // Default is 1.4
                HarmonyLib.AccessTools.Field(HarmonyLib.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, costMult);
            };

            // Remove timer on chance shrine
            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, interactor) =>
            {
                orig(self, interactor);
                HarmonyLib.AccessTools.Field(HarmonyLib.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "refreshTimer").SetValue(self, 0f);
                //self.SetFieldValue("refreshTimer", 0f);
            };

            pluginLogger.LogInfo($"Overhauled Chance Shrine - Max Items: {maxPurchases}; Cost Multiplier: {costMult}x");
        }

        //Credit to https://thunderstore.io/package/der10pm/ChargeInHalf/
        public void IncreaseTeleporterChargeSpeed(float teleporterChargeMultiplier)
        {
            On.RoR2.HoldoutZoneController.OnEnable += (orig, self) =>
            {
                self.baseChargeDuration /= teleporterChargeMultiplier;
                orig(self);
            };

            pluginLogger.LogInfo($"Increased Teleporter Charge Speed: {teleporterChargeMultiplier}x");
        }
    }

}