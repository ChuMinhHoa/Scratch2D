using System;
using UnityEngine;

public class BtnBooster : MonoBehaviour
{
    [SerializeReference] public IBooster booster;

    private void Awake()
    {
        booster.InitData();
    }
}

public enum BoosterType
{
    Magnet
}
