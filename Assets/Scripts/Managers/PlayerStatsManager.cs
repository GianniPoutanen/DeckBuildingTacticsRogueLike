using UnityEngine;

public class PlayerStatsManager : MonoBehaviour
{
    private PlayerStats playerStats;

    // Example: Call this function in the Start method of a GameManager or similar component
    void Start()
    {
        LoadPlayerStats();
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
    // Example: Load player stats at the start of the game
    private void LoadPlayerStats()
    {
        playerStats = SecureJsonManager.LoadEncryptedJsonData<PlayerStats>(JsonDataType.PlayerStats);

        if (playerStats == null)
        {
            // If player stats don't exist, create default stats and save them
            playerStats = new PlayerStats();
            SecureJsonManager.SaveEncryptedJsonData(JsonDataType.PlayerStats, playerStats);
        }

        // Optionally, apply loaded player stats to your game if needed
        ApplyPlayerStats(playerStats);
    }

    // Example: Apply loaded player stats to your game
    private void ApplyPlayerStats(PlayerStats stats)
    {
        // Implement logic to apply player stats to your game
        // For example, update UI elements to display player stats
    }

    private void SubscribeToEvents()
    {
    }

    private void UnsubscribeToEvents()
    {
    }

    // Example: Update player stats when the player takes damage
    private void UpdateDamageTaken(int damageAmount)
    {
        playerStats.damageTaken += damageAmount;
        SavePlayerStats();
    }

    // Example: Update player stats when a game is finished
    private void UpdateGamesPlayed()
    {
        playerStats.gamesPlayed++;
        SavePlayerStats();
    }

    // Example: Save player stats
    private void SavePlayerStats()
    {
        SecureJsonManager.SaveEncryptedJsonData(JsonDataType.PlayerStats, playerStats);
    }

}
