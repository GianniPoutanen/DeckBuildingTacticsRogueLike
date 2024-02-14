using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AttacksPanelEntry : MonoBehaviour
{
    [Header("Content of Panel")]
    public Attack attack;
    public List<Ability> abilities = new List<Ability>();
    [Header("UI Eements")]
    public AbilityDisplayPanel abilityDisplayPanelGameObject;
    public List<AbilityDisplayPanel> entries = new List<AbilityDisplayPanel>();
    public TextMeshProUGUI attackNameLabel;
    public RectTransform content;
    [Header("Spacing")]
    public float spaceBetweenEntries;

    private void Start()
    {
        BuildAttackPanel();
    }

    public void BuildAttackPanel()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
        abilities.Clear();
        entries.Clear();

        abilities.Add(attack.triggerAbility);
        abilities.AddRange(attack.followUpAbilities);

        foreach (Ability ability in abilities)
        {
            AbilityDisplayPanel abilityDisplayPanel = NewEntry();
            abilityDisplayPanel.abilityOnCard = ability;
            abilityDisplayPanel.BuildAbilityCard();
        }

        content.sizeDelta = new Vector2(abilityDisplayPanelGameObject.GetComponent<RectTransform>().sizeDelta.x * abilities.Count + (spaceBetweenEntries * abilities.Count), 0);
        OrderEntries();
        attackNameLabel.text = attack.attackName;
    }

    public void OrderEntries()
    {
        float width = abilityDisplayPanelGameObject.GetComponent<RectTransform>().sizeDelta.x;
        for (int i = 0; i < entries.Count; i++)
        {
            RectTransform rect = entries[i].GetComponent<RectTransform>();

            entries[i].GetComponent<RectTransform>().anchoredPosition = new Vector2((width / 2) + (width * (float)i) - (width * ((float)entries.Count / 2f)) + (spaceBetweenEntries * i), 0);
        }
    }

    public AbilityDisplayPanel NewEntry()
    {
        AbilityDisplayPanel entry = GameObject.Instantiate(abilityDisplayPanelGameObject, content);
        entries.Add(entry);
        return entry;

    }
}
