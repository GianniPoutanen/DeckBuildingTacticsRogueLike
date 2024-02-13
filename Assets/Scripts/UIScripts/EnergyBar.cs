using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public GameObject bar;
    private RectTransform barRect;
    public GameObject barSegment;
    public List<GameObject> barSegments = new List<GameObject>();

    private void Start()
    {
        EventManager.Instance.AddListener(EventType.UpdateUI, UpdateEnergyBar);
        barRect = bar.GetComponent<RectTransform>();
        UpdateEnergyBar();
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventType.UpdateUI, UpdateEnergyBar);
    }

    public void UpdateEnergyBar()
    {
        int maxEnergy = PlayerManager.Instance.maxEnergy;
        int currentEnergy = PlayerManager.Instance.CurrentEnergy;
        barSegments.Clear();

        for (int i = bar.transform.childCount - 1; i >= 0; i--)
        {
            Destroy((bar.transform.GetChild(i)).gameObject);
        }

        for (int i = 0; i < currentEnergy; i++)
        {
            GameObject current = Instantiate(barSegment, bar.transform);
            RectTransform rectTransform = current.GetComponent<RectTransform>();
            //image.rectTransform.rect.Set((barRect.rect.width / maxEnergy) * i, 0, (barRect.rect.width / maxEnergy), barRect.rect.height);
            //rectTransform.rect.Set(100 * i, 0, 100, 50);
            rectTransform.localPosition = new Vector3((barRect.rect.width / maxEnergy) * i, 0, 0);
            rectTransform.sizeDelta = new Vector2((barRect.rect.width / maxEnergy), 0);
        }
    }



}
