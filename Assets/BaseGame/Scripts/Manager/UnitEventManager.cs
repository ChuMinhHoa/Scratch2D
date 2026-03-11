using System.Collections.Generic;
using TW.Utility.DesignPattern;
using UnityEngine;

public class UnitEventManager : Singleton<UnitEventManager>
{
    public List<int> eventId = new();

    public int RegisterEvent()
    {
        var id = -1;
        var indexEventCheck = 0;
        while (id == -1 && indexEventCheck < eventId.Count)
        {
            if (!eventId.Contains(indexEventCheck))
                id = indexEventCheck;
            indexEventCheck++;
        }

        if (id == -1)
            id = eventId.Count;
        eventId.Add(id);
        return id;
    }

    public void RemoveEventId(int id)
    {
        if (eventId.Contains(id))
            eventId.Remove(id);
        if (eventId.Count == 0)
            Debug.Log("all event is removed");
    }

    public bool IsHaveEvent()
    {
        return eventId.Count > 0;
    }
}
