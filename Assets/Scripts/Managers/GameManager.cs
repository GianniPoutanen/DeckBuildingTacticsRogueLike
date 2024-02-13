using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton Pattern

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(GameManager).Name;
                    instance = obj.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }

    #endregion

    private void Start()
    {
        EventManager.Instance.InvokeEvent(EventType.GameStart);
        LoadGameSettings();
    }

    public void SaveGameSettings()
    {
        GameSettings loadedSettings = new GameSettings();
        // Get game settings

        SecureJsonManager.SaveEncryptedJsonData(JsonDataType.GameSettings, loadedSettings);

        // Now loadedSettings will contain the default settings
    }

    public void LoadGameSettings()
    {
        GameSettings loadedSettings = SecureJsonManager.LoadEncryptedJsonData<GameSettings>(JsonDataType.GameSettings);

        if (loadedSettings == null)
        {
            // If settings don't exist, create default settings and save them
            GameSettings defaultSettings = new GameSettings();
            SecureJsonManager.SaveEncryptedJsonData(JsonDataType.GameSettings, defaultSettings);

            // Now loadedSettings will contain the default settings
            loadedSettings = defaultSettings;
        }

        // Apply loadedSettings to your game
        ApplyGameSettings(loadedSettings);
    }

    private void ApplyGameSettings(GameSettings settings)
    {
        // Implement logic to apply settings to your game
        // For example, set volumes, change screen resolution, etc.

        // For illustration purposes, let's print the loaded settings
        Debug.Log("Loaded Game Settings:");
        Debug.Log($"Master Volume: {settings.masterVolume}");
        Debug.Log($"Music Volume: {settings.musicVolume}");
        Debug.Log($"Sound Effects Volume: {settings.soundEffectsVolume}");
        Debug.Log($"Resolution: {settings.resolutionWidth}x{settings.resolutionHeight}");
        Debug.Log($"Fullscreen: {settings.fullscreen}");
        Debug.Log($"Graphics Quality: {settings.graphicsQuality}");
        Debug.Log($"Show Tutorial: {settings.showTutorial}");
    }
}
