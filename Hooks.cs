using RoR2;
using UnityEngine.Networking;
using R2API.Utils;
using UnityEngine;

namespace FasterGames
{
    public class Hooks
    {
        public BepInEx.Logging.ManualLogSource pluginLogger;

        public void IncreaseSpawnRate()
        {
            // Dunno if this is actually working...
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.minRerollSpawnInterval = 1f;
                self.maxRerollSpawnInterval = 4f;
                orig(self, deltaTime);
            };
            pluginLogger.LogInfo($"Increased Spawn Rate");
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
            foreach (SurvivorDef survivor in RoR2.SurvivorCatalog.allSurvivorDefs)
            {
                CharacterBody body = survivor.bodyPrefab.GetComponent<CharacterBody>();

                pluginLogger.LogInfo($"Updated speed for {body.name}");
                body.baseMoveSpeed = moveSpeed; // Default is 7
            }
            pluginLogger.LogInfo($"Increased Base Move Speed to {moveSpeed}");
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
                HarmonyLib.AccessTools.Field(HarmonyLib.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "refreshTimer").SetValue(self, 0.1f);
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

        //Credit to https://github.com/TheRealElysium/R2Mods/blob/master/Faster3DPrinters
        public void NoCoolDown3dPrinter()
        {
            On.RoR2.Stage.Start += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    typeof(EntityStates.Duplicator.Duplicating).SetFieldValue("initialDelayDuration", 0.1f);
                    typeof(EntityStates.Duplicator.Duplicating).SetFieldValue("timeBetweenStartAndDropDroplet", 0f);
                }
            };

            On.EntityStates.Duplicator.Duplicating.DropDroplet += (orig, self) =>
            {
                orig(self);
                if (NetworkServer.active)
                {
                    self.outer.GetComponent<PurchaseInteraction>().Networkavailable = true;
                }
            };

            On.EntityStates.Duplicator.Duplicating.BeginCooking += (orig, self) =>
            {
                if (!NetworkServer.active)
                {
                    orig(self);
                }
            };

            pluginLogger.LogInfo("Removed 3d printer cooldown");
        }
    }

}