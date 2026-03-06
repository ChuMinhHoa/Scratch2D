using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public interface ISpaceForSticker
{
    void ResetController();
}

[Serializable]
public class SpaceForSticker : ISpaceForSticker
{
    public virtual void ResetController()
    {
        
    }
}