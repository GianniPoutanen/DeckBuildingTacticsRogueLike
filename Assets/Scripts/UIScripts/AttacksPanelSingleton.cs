using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacksPanelSingleton : UIElement
{
    #region Singleton Pattern

    private static AttacksPanelSingleton instance;

    public static AttacksPanelSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AttacksPanelSingleton>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(AttacksPanelSingleton).Name;
                    instance = obj.AddComponent<AttacksPanelSingleton>();
                }
            }
            return instance;
        }
    }

    #endregion


    [SerializeField]
    public List<Attack> Attacks { get; set; } = new List<Attack>();
    public AttacksPanelEntry attackPanelEntryObject;
    public List<AttacksPanelEntry> entries = new List<AttacksPanelEntry>();
    public RectTransform content;
    public float spaceBetweenEntries = 4f;

    public override void Start()
    {
        base.Start();
        BuildAttackPanel();
    }
    public override void Open()
    {
        base.Open();
        BuildAttackPanel();
    }

    public void BuildAttackPanel(List<Attack> attacks)
    {
        Attacks = attacks;
        BuildAttackPanel();
    }

    public void BuildAttackPanel()
    {
        foreach (Transform child in content)
            Destroy(child.gameObject);
        entries.Clear();


        foreach (Attack attack in Attacks)
        {
            AttacksPanelEntry attackPanelEntry = NewEntry();
            attackPanelEntry.attack = attack;
            attackPanelEntry.BuildAttackPanel();
        }


        content.sizeDelta = new Vector2(content.sizeDelta.x, attackPanelEntryObject.GetComponent<RectTransform>().sizeDelta.y * Attacks.Count + (spaceBetweenEntries * Attacks.Count));
        OrderEntries();
    }

    public void OrderEntries()
    {
        float height = attackPanelEntryObject.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < entries.Count; i++)
        {
            RectTransform rect = entries[i].GetComponent<RectTransform>();

            entries[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (height / 2) - (height * (float)i) - (height * ((float)entries.Count / 2f)) + (spaceBetweenEntries * i));
        }
    }

    public AttacksPanelEntry NewEntry()
    {
        AttacksPanelEntry entry = GameObject.Instantiate(attackPanelEntryObject, content);
        entries.Add(entry);
        return entry;

    }
}
