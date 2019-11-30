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
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.minSeriesSpawnInterval = 2f;
                self.maxSeriesSpawnInterval = 3f;
                orig(self, deltaTime);
            };
        }

        public void IncreaseExpCoefficient()
        {
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.expRewardCoefficient = new float();
                self.expRewardCoefficient = 0.75f; // Default is 0.2
                orig(self, deltaTime);
            };
        }

        public void IncreaseMoneyMultiplier()
        {
            On.RoR2.CombatDirector.Simulate += (orig, self, deltaTime) =>
            {
                self.creditMultiplier = new float();
                self.creditMultiplier = 3f; // Default is 1
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
                        body.baseMoveSpeed = 12.5f; // Default is 7
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
                            7.5f,
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
                self.sceneDirectorInteractibleCredits = 800;
                orig(self);
            };
        }
    }

}