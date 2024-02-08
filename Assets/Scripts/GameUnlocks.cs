using System.Collections.Generic;

[System.Serializable]
public class GameUnlocks
{
    [System.Serializable]
    public class UnlockData
    {
        public UnlockType unlockType;
        public int unlockLevel;
    }

    public List<UnlockData> unlocks = new List<UnlockData>();

}
