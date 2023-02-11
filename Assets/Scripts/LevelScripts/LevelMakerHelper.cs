using DataRepo;

namespace LevelScripts
{
    public static class LevelMakerHelper
    {
        public static int FindParent(float numberOfBottleToCreate, int createdBottles)
        {
            return (createdBottles < (numberOfBottleToCreate / 2) ? 0 : 1);
        }

        public static void DecreaseTotalWaterCount(Bottle tempBottle, ref int totalWaterCount)
        {
            if (totalWaterCount >= 4)
            {
                tempBottle.NumberOfColorsInBottle = 4;
                totalWaterCount -= 4;
            }
            else
                tempBottle.NumberOfColorsInBottle = 0;
        }

        public static int RandomizeNumberOfBottle(Data data, LevelColorController levelColorController)
        {
            var hasString = "ExtraBottle " + data.GetAmountOfExtraBottleIndex().ToString();
            var rand = new Unity.Mathematics.Random((uint)hasString.GetHashCode());

            return rand.NextInt(levelColorController.NumberOfColorsToCreate + 1,
                levelColorController.NumberOfColorsToCreate + 3);
        }
    }
}