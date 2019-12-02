using R2API;
using RoR2;
using BepInEx;
using UnityEngine;
using MonoMod.Cil;
using System;

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
                self.minSeriesSpawnInterval = 2f; // Default is 0.1
                self.maxSeriesSpawnInterval = 3f; // Default is 1
                orig(self, deltaTime);
            };
        }

        public void IncreaseExpCoefficient()
        {
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.expRewardCoefficient = new float();
                self.expRewardCoefficient = 0.5f + Run.instance.participatingPlayerCount * 0.1f; // Default is 0.2
                orig(self, deltaTime);
            };
        }

        public void IncreaseMoneyMultiplier()
        {
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.creditMultiplier = new float();
                self.creditMultiplier = 1.5f + Run.instance.participatingPlayerCount * 0.5f; // Default is 1
                orig(self, deltaTime);
            };
        }

        public void IncreaseBaseStats()
        {
            foreach (string bodyName in new string[] { "CommandoBody", "ToolbotBody", "HuntressBody", "EngiBody", "MageBody", "MercBody", "TreebotBody", "LoaderBody" })
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
                        pluginLogger.LogInfo("The prefab loaded has no character body");
                    }
                }
                else
                {
                    pluginLogger.LogInfo("That is not a valid prefab");
                }
            }
        }

        public void IncreaseDifficultyScaling()
        {
            pluginLogger.LogInfo("Created Faster Difficulty");

            Color DifficultyColor = new Color(0.94f, 0.51f, 0.15f);

            DifficultyDef FasterDef = new DifficultyDef(
                            9f, // 0 is Normal mode. 2.5f is 50% which is monsoon
                            "Faster",
                            ":Assets/FasterGames/DifficultyIcon.png",
                            "Gotta go Faster!",
                            DifficultyColor
                            );
            DifficultyIndex DelugeIndex = R2API.DifficultyAPI.AddDifficulty(FasterDef);
        }

        public void IncreaseChestSpawnRate()
        {
            On.RoR2.ClassicStageInfo.Awake += (On.RoR2.ClassicStageInfo.orig_Awake orig, ClassicStageInfo self) =>
            {
                self.sceneDirectorInteractibleCredits = 400 + (Run.instance.participatingPlayerCount * 100); // Default is 200
                orig(self);
            };
        }
    }

}