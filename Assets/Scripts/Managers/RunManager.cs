using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager instance; // Static reference to the RunManager instance

    [Header("Current Run Stats")]
    public int numberEncounters = 0; // Track the current floor/level

    [Header("Current Run Unlocks")]
    public List<Relics> UnlockedRelics = new List<Relics>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple instances of RunManager found!");
            Destroy(gameObject);
        }
    }


    public void StartEncounter()
    {

    }

    public void EndEncounter()
    {

    }
}