using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Artifacts;
using UnityEngine;
using static FasterGames.FasterGames;

namespace FasterGames
{
    public class NoChestArtifact : ArtifactBase
    {
        public static ConfigEntry<float> dropChancePercentage { get; set; }
        public static ConfigEntry<float> earlyRunDropChangePercentage { get; set; }
        public static ConfigEntry<int> killDibbsPercentage { get; set; }

        public override string ArtifactName => "Artifact of Transcendence";
        public override string ArtifactLangTokenName => "ARTIFACT_TRANSCENDENCE";
        public override string ArtifactDescription => "Items are rewarded straight to inventory in round robin style with the final hit getting a larger chance to receive it";
        public override Sprite ArtifactEnabledIcon => MainAssets.LoadAsset<Sprite>("Assets/icons/Enabled_Artifact_Icon.png");
        public override Sprite ArtifactDisabledIcon => MainAssets.LoadAsset<Sprite>("Assets/icons/Disabled_Artifact_Icon.png");

        private BepInEx.Logging.ManualLogSource _logger;
        public override void Init(ConfigFile config, BepInEx.Logging.ManualLogSource logger)
        {
            CreateConfig(config);
            CreateLang();
            CreateArtifact();
            Hooks();
            _logger = logger;
            _logger.LogInfo($"Total Network Users: {NetworkUser.instancesList.Count}");
        }

        public override void Hooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += OnServerCharacterDeath;
            SceneDirector.onPrePopulateSceneServer += SacrificeArtifactManager.OnPrePopulateSceneServer;
            SceneDirector.onGenerateInteractableCardSelection += SacrificeArtifactManager.OnGenerateInteractableCardSelection;
            DirectorCardCategorySelection.calcCardWeight += SacrificeArtifactManager.CalcCardWeight;
        }

        private void CreateConfig(ConfigFile config)
        {
            dropChancePercentage = config.Bind<float>(
                "NoChestArtifact",
                "dropChancePercentage",
                10f,
                "Increases Item rate on enemy death.  \nBase Game value: 5"
            );

            earlyRunDropChangePercentage = config.Bind<float>(
                "NoChestArtifact",
                "earlyRunDropChance",
                50f,
                "Increases Item rate on enemy death weighted towards the early game and slowly falls off."
            );

            killDibbsPercentage = config.Bind<int>(
                "NoChestArtifact",
                "killDibbsPercentage",
                25,
                "Player that gets the kill has x% chance to take the item before item is randomly distributed."
            );
        }

        private void OnServerCharacterDeath(DamageReport damageReport)
        {
            if (!damageReport.victimMaster)
            {
                return;
            }
            if (!damageReport.attackerMaster)
            {
                _logger.LogInfo("AttackerMaster is null. Skip.");
                return;
            }
            if (damageReport.attackerTeamIndex == damageReport.victimTeamIndex && damageReport.victimMaster.minionOwnership.ownerMaster)
            {
                return;
            }
            // Person who deals the final hit gives out the item so we don't have duplicates.
            if (!damageReport.attackerMaster.isClient)
            {
                _logger.LogInfo("Is not the person who killed it. Skipping.");
                return;
            }

            float baseDropChancePercent = dropChancePercentage.Value + (5f * (NetworkUser.instancesList.Count - 1));
            float expAdjustedDropChancePercent = Util.GetExpAdjustedDropChancePercent(baseDropChancePercent, damageReport.victim.gameObject);
            float difficultyAdjustedDropChancePercent = earlyRunDropChangePercentage.Value * (1 / Run.instance.difficultyCoefficient);
            float dropChance = expAdjustedDropChancePercent + difficultyAdjustedDropChancePercent;
            _logger.LogInfo($"[{Run.instance.difficultyCoefficient} diff.] Base Drop Chance: {baseDropChancePercent}; expAdjustedDropChancePercent: {expAdjustedDropChancePercent}; difficultyAdjustedDropChance: {difficultyAdjustedDropChancePercent}; dropChance: {dropChance}");


            if (Util.CheckRoll(dropChance, 0f, null))
            {
                PickupIndex pickupIndex = SacrificeArtifactManager.dropTable.GenerateDrop(SacrificeArtifactManager.treasureRng);
                if (pickupIndex != PickupIndex.none)
                {
                    PickupDef pickup = PickupCatalog.GetPickupDef(pickupIndex);
                    CharacterMaster targetForItem = ChooseDropTarget(damageReport.attackerMaster);
                    Chat.AddMessage($"{targetForItem.netIdentity} got {pickup.nameToken}");
                    targetForItem.inventory.GiveItem(pickup.itemIndex);
                }
            }
        }

        private CharacterMaster ChooseDropTarget(CharacterMaster finalBlowCharacter)
        {
            System.Random rng = new System.Random();
            int randomNum = rng.Next(100);
            if (randomNum < killDibbsPercentage.Value)
            {
                return finalBlowCharacter;
            }

            int roundRobinChoice = rng.Next(NetworkUser.instancesList.Count);
            return NetworkUser.instancesList[roundRobinChoice].master;
        }
    }
}