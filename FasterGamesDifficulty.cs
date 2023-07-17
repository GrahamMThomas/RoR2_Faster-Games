using System.Reflection;
using RoR2;
using UnityEngine;
using static FasterGames.FasterGames;

namespace FasterGames
{
    public static class FasterGamesDifficulty
    {
        public static DifficultyIndex AddDifficulty(float scalingPercentage)
        {
            DifficultyDef FasterDifficulty = CreateDifficultyDef(scalingPercentage);
            Sprite diffIcon = MainAssets.LoadAsset<Sprite>("Assets/icons/FasterDifficultyIcon_V2.png");
            return R2API.DifficultyAPI.AddDifficulty(FasterDifficulty, diffIcon);
        }

        private static DifficultyDef CreateDifficultyDef(float scalingPercentage)
        {
            return new DifficultyDef(
                (scalingPercentage - 1) * 5, //0 is Normal mode. 2.5f is 50% which is monsoon
                "Faster",
                "",
                GenerateDifficultyDescription(scalingPercentage),
                GetDifficultyColor(),
                "Faster",
                true
                );
        }

        private static Color GetDifficultyColor()
        {
            return new Color(0.875f, 0.875f, 0.14f);
        }

        private static string GenerateDifficultyDescription(float scalingPercentage)
        {
            string desc = "Games go 3x faster. For those who love the game, but not how long it takes.\n<style=cStack>";
            desc = string.Join("\n",
                desc,
                $"> Difficulty Scaling: <style=cDeath>+{(scalingPercentage - 1) * 100}%</style>\n",
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
