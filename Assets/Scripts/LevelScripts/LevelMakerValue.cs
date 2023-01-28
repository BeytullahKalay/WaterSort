using UnityEngine;

namespace LevelScripts
{
    [System.Serializable]
    public class LevelMakerValue
    {
        public int LevelBeginningIndex;
        public int LevelFinishIndex;
        [Range(0, 12)] public int ColorAmount;
        public bool NoMatches;
        public bool RainbowBottle;
    }
}

