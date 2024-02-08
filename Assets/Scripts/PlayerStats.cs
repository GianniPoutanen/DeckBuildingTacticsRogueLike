[System.Serializable]
public class PlayerStats
{
    public int damageDone;
    public int damageTaken;
    public int gamesPlayed;

    public PlayerStats()
    {
        damageDone = 0;
        damageTaken = 0;
        gamesPlayed = 0;
    }
}
