using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Playables;
using UnityEngine;
using UnityEngine.EventSystems;

public class AbilityDisplayPanel : MonoBehaviour, IPointerDownHandler
{
    [Header("Default Values")]

    [Header("Ability Content")]
    public TextMeshProUGUI abilityNameText;
    public RectTransform content;
    public Ability abilityOnCard;

    [Header("Default Values")]
    public Sprite defaultIcon;
    public AbilityPanelEntry abilityEntryObject;
    private List<AbilityPanelEntry> entries = new List<AbilityPanelEntry>();

    // Start is called before the first frame update
    void Start()
    {
        BuildAbilityCard();
    }
    public void BuildAbilityCard()
    {

        foreach (Transform child in content)
            Destroy(child.gameObject);

        abilityNameText.text = abilityOnCard.name;

        AbilityPanelEntry newEntry = NewEntry();
        newEntry.BuildEntry(abilityOnCard);

        OrderEntries();
    }

    public void OrderEntries()
    {
        float height = abilityEntryObject.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < entries.Count; i++)
        {
            RectTransform rect = entries[i].GetComponent<RectTransform>();

            entries[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (height * (float)i) - (height * ((float)entries.Count / 2f)));
        }
    }

    public AbilityPanelEntry NewEntry()
    {
        AbilityPanelEntry entry = GameObject.Instantiate(abilityEntryObject, content);
        entries.Add(entry); 
        return entry;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        GridManager.Instance.UpdateSelectedEnemyAttackTiles(abilityOnCard.GetPossiblePositions(abilityOnCard.Performer.targetGridPosition));
    }
}
