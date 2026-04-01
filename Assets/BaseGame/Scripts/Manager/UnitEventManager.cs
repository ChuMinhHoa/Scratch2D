using System;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;

public class UnitEventManager : Singleton<UnitEventManager>
{
    public List<GameObject> eventId = new();
    public List<Action> actions = new();

    public void RegisterEvent(GameObject gameObj)
    {
        eventId.Add(gameObj);
    }

    public void RemoveEventId(GameObject gameObj)
    {
        if (eventId.Contains(gameObj))
            eventId.Remove(gameObj);
        // if (eventId.Count == 0)
        //     Debug.Log("all event is removed");
    }

    public bool IsHaveEvent()
    {
        return eventId.Count > 0;
    }
}
