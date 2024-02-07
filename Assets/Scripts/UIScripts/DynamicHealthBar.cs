using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DynamicHealthBar : MonoBehaviour
{
    public RectTransform bars;
    public Slider healthSlider; // Reference to the UI Slider
    public TextMeshProUGUI armourText; // Reference to the UI Slider
    public Transform entityTransform; // Reference to the entity's transform
    public Entity entity;

    void Start()
    {
        UpdateHealthBar();
        bars.gameObject.SetActive(false); // Initially hide the health bar
    }

    void Update()
    {
        // Check if the entity is under the mouse
        if (IsEntityUnderMouse())
        {
            UpdateHealthBar();
            // Activate and update the health bar position
            bars.gameObject.SetActive(true);
            UpdateHealthBarPosition();
        }
        else
        {
            // Deactivate the health bar if the entity is not under the mouse
            bars.gameObject.SetActive(false);
        }
    }

    void UpdateHealthBar()
    {
        // Update the UI Slider value based on the current health
        healthSlider.value = (((float)entity.Health) / ((float)entity.MaxHealth));
        armourText.text = entity.Armour.ToString();
    }
    void UpdateHealthBarPosition()
    {
        // Set the health bar position above the entity's head
        Vector3 entityHeadPosition = entity.healthBarLocation != null ? entity.healthBarLocation.position : entityTransform.position + Vector3.up * 2f;
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(entityHeadPosition);
        bars.transform.position = screenPosition;
    }

    bool IsEntityUnderMouse()
    {
        // Cast a ray from the mouse position to check if it hits the entity
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider != null)
        {
            return hit.collider.transform.root == entityTransform;
        }

        return false;
    }
}
