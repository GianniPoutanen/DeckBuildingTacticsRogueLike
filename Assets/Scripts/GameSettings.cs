[System.Serializable]
public class GameSettings
{
    public float masterVolume = 1.0f;
    public float musicVolume = 0.8f;
    public float soundEffectsVolume = 0.5f;
    public int resolutionWidth = 1920;
    public int resolutionHeight = 1080;
    public bool fullscreen = true;
    public QualityLevel graphicsQuality = QualityLevel.High;
    public bool showTutorial = true;
    public float cameraShakeIntensity = 1;

    // You can add more settings as needed

    public enum QualityLevel
    {
        Low,
        Medium,
        High
    }
}
