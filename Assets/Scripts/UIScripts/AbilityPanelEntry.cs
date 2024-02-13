using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityPanelEntry : MonoBehaviour
{
    [Header("Entry Content")]
    public TextMeshProUGUI TextMesh;
    public Image Image;
    public Image Positioning;

    [Header("Positioning Icons")]
    [SerializeField]
    public AbilityEntrySprites StraightAbility;
    public AbilityEntrySprites BasicAbility;
    public AbilityEntrySprites SuroundingAbility;
    public AbilityEntrySprites MovementAbility;

    public void BuildEntry(Ability ability)
    {
        switch (ability)
        {
            case SimpleAttackAbility:
                TextMesh.text = (ability as SimpleAttackAbility).damage.ToString();
                SetImages(BasicAbility);
                break;
            case StraightAttackAbility:
                TextMesh.text = (ability as StraightAttackAbility).damage.ToString();
                SetImages(StraightAbility);
                break;
            case MoveSelfAbility:
                TextMesh.text = (ability as MoveSelfAbility).range.ToString();
                SetImages(MovementAbility);
                break;
            case MoveTargetAbility:
                TextMesh.text = (ability as MoveTargetAbility).distance.ToString();
                SetImages(MovementAbility);
                break;
        }
    }

    public void SetImages(AbilityEntrySprites d)
    {
        Image.sprite = d.AbilityImage;
        Positioning.sprite = d.PositioningImage;
    }

    [Serializable]
    public class AbilityEntrySprites
    {
        public Sprite PositioningImage;
        public Sprite AbilityImage;
    }
}
