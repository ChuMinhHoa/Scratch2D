using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class FrontChecker2D : MonoBehaviour
{
    [SerializeField] private Collider currentCollider;
    [SerializeField] private Transform[] pointCheck;
    [SerializeField] private float distanceCheck = 50f;
    [SerializeField] private LayerMask layerCheck;

//     [Button]
//     public bool IsAnythingInFront()
//     {
//         for (var i = 0; i < pointCheck.Length; i++)
//         {
//             var vectorEnd = pointCheck[i].position;
//             vectorEnd.z -= distanceCheck;
//
//             if (Physics.Linecast(pointCheck[i].position, vectorEnd, layerCheck))
//             {
// #if UNITY_EDITOR
//                 Debug.DrawLine(pointCheck[i].position,vectorEnd, Color.green, 5f);
// #endif
//                 return true;
//             }
// #if UNITY_EDITOR
//             
//             else
//             {
//                 Debug.DrawLine(pointCheck[i].position,vectorEnd, Color.red, 5f);
//             }
// #endif
//         }
//
//
//         return false;
//     }

    public Collider2D collider2D;
    public Collider2D[] result = new Collider2D[5];

    public bool IsAnythingInFront()
    {
        var colCount = Physics2D.OverlapBoxNonAlloc(transform.position, collider2D.bounds.size, transform.eulerAngles.z, result, layerCheck);
        for (var i = 0; i < colCount; i++)
        {
            if (result[i] == collider2D) continue;
            if (result[i] == null) continue;
            if (result[i].transform.position.z > transform.position.z) continue;
            return true;
        }

        return false;
    }
}