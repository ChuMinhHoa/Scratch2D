using UnityEngine;

public class FHSGraphic : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprIcon;
    [SerializeField] private Animator anim;
    public void SetSprIcon()
    {
    }

    public void OnOpen()
    {
        anim.Play("Open");
    }

    public void OnClose()
    {
        anim.Play("Close");
    }

    public void OnIdle()
    {
        anim.Play("Idle");
    }
}
