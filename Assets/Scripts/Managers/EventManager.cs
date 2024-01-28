using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    #region Singleton Pattern

    private static EventManager instance;

    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<EventManager>();
                if (instance == null)
                {
                    GameObject obj = new GameObject();
                    obj.name = typeof(EventManager).Name;
                    instance = obj.AddComponent<EventManager>();
                }
            }
            return instance;
        }
    }

    #endregion
    // Define a dictionary to store event delegates based on the enum
    private readonly Dictionary<Enums.EventType, Delegate> eventVariabledDelegateDictionary = new Dictionary<Enums.EventType, Delegate>();
    private readonly Dictionary<Enums.EventType, Delegate> eventDictionary = new Dictionary<Enums.EventType, Delegate>();

    #region Event Handling With Parameters
    // Method to add a listener for a specific event type with parameters
    public void AddListener<T>(Enums.EventType eventType, Action<T> action)
    {
        if (!eventVariabledDelegateDictionary.ContainsKey(eventType))
        {
            eventVariabledDelegateDictionary[eventType] = null;
        }

        eventVariabledDelegateDictionary[eventType] = (Action<T>)eventVariabledDelegateDictionary[eventType] + action;
    }

    // Method to remove a listener for a specific event type with parameters
    public void RemoveListener<T>(Enums.EventType eventType, Action<T> action)
    {
        if (eventVariabledDelegateDictionary.ContainsKey(eventType))
        {
            eventVariabledDelegateDictionary[eventType] = (Action<T>)eventVariabledDelegateDictionary[eventType] - action;
        }
    }

    // Method to invoke an event for a specific event type with parameters
    public void InvokeEvent<T>(Enums.EventType eventType, T parameter)
    {
        Debug.Log(eventType.ToString());
        if (eventVariabledDelegateDictionary.ContainsKey(eventType))
        {
            (eventVariabledDelegateDictionary[eventType] as Action<T>)?.Invoke(parameter);
        }
    }
    #endregion Event Handling With Parameters

    #region Event Handling Without Parameters
    // Method to add a listener for a specific event type with parameters
    public void AddListener(Enums.EventType eventType, Action action)
    {
        if (!eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = null;
        }

        eventDictionary[eventType] = (Action)eventDictionary[eventType] + action; ;

    }

    // Method to remove a listener for a specific event type with parameters
    public void RemoveListener(Enums.EventType eventType, Action action)
    {
        if (eventDictionary.ContainsKey(eventType))
        {
            eventDictionary[eventType] = (Action)eventDictionary[eventType] - action;
        }
    }


    // Method to invoke an event for a specific event type with parameters
    public void InvokeEvent(Enums.EventType eventType)
    {
        Debug.Log(eventType.ToString());
        if (eventDictionary.ContainsKey(eventType))
        {
            (eventDictionary[eventType] as Action)?.Invoke();
        }
    }
    #endregion Event Handling Without Parameters

    // It's a good practice to clean up events when they're no longer needed
    private void OnDestroy()
    {
        instance = null;
    }
}
