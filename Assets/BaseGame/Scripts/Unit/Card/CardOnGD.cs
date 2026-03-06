using LitMotion;
using UnityEngine;

public class CardOnGD : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public partial class Card
{
    public void OnGDActive()
    {
        Debug.Log($"card active {layerIndex}");
        var trs = transform.position;
        var target = new Vector3(trs.x, trs.y, -5);
        LMotion.Create(trs, target, 0.25f).Bind(x=>transform.position = x).AddTo(this);
        cardGraphic.objLock.SetActive(false);
    }

    public void OnGDDeActive()
    {
        var trs = transform.position;
        var target = data.position;
        target.z = layerIndex;
        LMotion.Create(trs, target, 0.25f).Bind(x=>transform.position = x).AddTo(this);
        cardGraphic.objLock.SetActive(true);
    }
    
}
