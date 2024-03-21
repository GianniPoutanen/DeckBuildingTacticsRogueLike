using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton Pattern

    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<UIManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(UIManager).Name;
                    instance = obj.AddComponent<UIManager>();
                    instance.canvas = obj.GetComponent<Canvas>();
                    instance.rectTransform = obj.GetComponent<RectTransform>();
                }
            }
            return instance;
        }
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            instance.canvas = this.GetComponent<Canvas>();
            instance.rectTransform = this.GetComponent<RectTransform>();
            instance.SubscribeToEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    public RectTransform rectTransform;
    public Canvas canvas;
    public Hand Hand;


    [Header("Variables")]
    public GraphicRaycaster raycaster;

    [Header("UI Object Under Mouse")]
    public bool IsUIUnderMouse;
    public GameObject UIUnderMouse;

    [Header("UI Sizing Settings")]
    public float CardBubblingFactor = 1.2f;
    public float CardSpawnSize = 0.2f;

    [Header("Player UI References")]
    public Slider playerHealthBar;

    [Header("Enemy UI References")]
    public GameObject smallDynamicHealthBarPrefab;

    [Header("Panels")]
    public AttacksPanelSingleton attackPanel;
    public UpgradeCardPanelSingleton upgradeCardPanel;

    public Stack<UIElement> panelStack = new Stack<UIElement>();

    private void Start()
    {
        raycaster = this.GetComponent<GraphicRaycaster>();
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }

    private void Update()
    {
        SetUIObjectUnderMouse();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseLastUI();
        }
    }

    public void OpenUI(UIElement element)
    {
        panelStack.Push(element);
        element.Open();
    }


    public void OpenUI(UIPanels panel)
    {
        switch (panel)
        {
            case UIPanels.EnemyAttacks:
                attackPanel.Open();
                panelStack.Push(attackPanel);
                break;
        }
    }

    public void CloseLastUI()
    {
        if (panelStack.Count > 0)
            panelStack.Pop().Close();
    }

    private void SetUIObjectUnderMouse()
    {
        // Check for the card under the mouse during drag
        if (EventSystem.current.IsPointerOverGameObject())
        {
            //Set up the new Pointer Event
            var pointerEventData = new PointerEventData(EventSystem.current);
            //Set the Pointer Event Position to that of the game object
            pointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            raycaster.Raycast(pointerEventData, results);
            bool cardAlreadyUnderMouse = results.Find(x => x.gameObject.GetComponent<CardUI>() != null && x.gameObject.GetComponent<CardUI>() == Hand.cardUnderMouse).isValid;
            if (results.Count > 0 && !cardAlreadyUnderMouse)
            {
                IsUIUnderMouse = true;

                foreach (RaycastResult result in results)
                {
                    if (result.gameObject.GetComponent<CardUI>() != null)
                    {
                        CardUI hitCard = result.gameObject.GetComponent<CardUI>();
                        if (hitCard != null && Hand.cardsInHand.Contains(hitCard))
                        {
                            Hand.cardUnderMouse = hitCard;
                            hitCard.targetSizeFactor = CardBubblingFactor;
                        }
                    }
                }
            }
            else
            {
                IsUIUnderMouse = false;
                UIUnderMouse = null;
            }
        }
    }

    public void CreateHealthBar(Entity entity)
    {
        if (entity.tag == "Enemy")
        {
            // Instantiate a new health bar from the prefab
            GameObject newHealthBar = Instantiate(smallDynamicHealthBarPrefab, canvas.transform);

            // Access the DynamicHealthBar script on the new health bar instance
            DynamicHealthBar dynamicHealthBar = newHealthBar.GetComponent<DynamicHealthBar>();

            // Set the entity for the health bar
            dynamicHealthBar.entityTransform = entity.transform;

            // Set other properties or customize the health bar as needed
            dynamicHealthBar.entity = entity;

            // Optionally: Subscribe to entity events or perform additional setup

            // Activate the health bar
            newHealthBar.gameObject.SetActive(true);
        }
    }

    public void UpdateUI()
    {
        playerHealthBar.value = (float)((float)PlayerManager.Instance.playerHealth / (float)PlayerManager.Instance.maxPlayerHealth);
    }

    #region Event Handlers
    public void SubscribeToEvents()
    {
        EventManager.Instance.AddListener(EventType.UpdateUI, UpdateUI);
        EventManager.Instance.AddListener<Entity>(EventType.EntitySpawned, CreateHealthBar);
    }

    public void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener(EventType.UpdateUI, UpdateUI);
        EventManager.Instance.RemoveListener<Entity>(EventType.EntitySpawned, CreateHealthBar);
    }
    #endregion Event Handlers
}
