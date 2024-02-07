using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public Slider healthSlider; // Reference to the UI Slider

    void Start()
    {
        UpdateHealthBar();
        SubscribeToEvents();
    }
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    void UpdateHealthBar()
    {
        // Update the UI Slider value based on the current health
        healthSlider.value = PlayerManager.Instance.playerHealth/ PlayerManager.Instance.maxPlayerHealth;
    }

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener(Enums.EventType.PlayerDamageTaken, UpdateHealthBar);
    }

    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener(Enums.EventType.PlayerDamageTaken, UpdateHealthBar);
    }
}
