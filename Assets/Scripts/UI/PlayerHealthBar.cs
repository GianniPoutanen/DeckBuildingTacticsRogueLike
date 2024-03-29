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
        healthSlider.value = PlayerManager.Instance.Player.Health / PlayerManager.Instance.Player.MaxHealth;
    }

    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener(EventType.PlayerAttacked, UpdateHealthBar);
    }

    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener(EventType.PlayerAttacked, UpdateHealthBar);
    }
}
