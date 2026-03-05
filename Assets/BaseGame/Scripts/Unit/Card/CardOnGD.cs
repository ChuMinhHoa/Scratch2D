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
    public void OnActive()
    {
        cardGraphic.objLock.SetActive(false);
    }

    public void OnDeActive()
    {
        cardGraphic.objLock.SetActive(true);
    }
    
}
