using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class FrontChecker2D : MonoBehaviour
{
    [SerializeField] private LayerMask layerCheck;
    public Collider2D collider2D;
    public List<Collider2D> results = new();

    public bool IsAnythingInFront()
    {
        var countCollider = Physics2D.OverlapCollider(collider2D, results);
        for (var i = 0; i < results.Count; i++)
        {
            if (results[i] == null) continue;
            if (results[i] == collider2D) continue;
            if (results[i].gameObject.layer == 6 && transform.position.z > results[i].transform.position.z) return true;
        }

        return false;
    }
}