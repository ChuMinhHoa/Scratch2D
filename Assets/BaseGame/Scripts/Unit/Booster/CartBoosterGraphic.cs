using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class CartBoosterGraphic : MonoBehaviour
{
    public Animator anim;

    private void Start()
    {
        PlayAnimIdle();
    }
    
    [Button]
    public void PlayAnimIdle()
    {
        anim.Play("Idle");
    }
    
    [Button]
    public void PlayAnimOpen()
    {
        anim.Play("Open");
    }
    
    [Button]
    public void PlayAnimClose()
    {
        anim.Play("Close");
    }
    
    [Button]
    public void PlayAnimSpawn()
    {
        anim.Play("Spawn");
    }
    
    [Button]
    public void PlayAnimCollect()
    {
        anim.SetTrigger("Collect");
    }
}