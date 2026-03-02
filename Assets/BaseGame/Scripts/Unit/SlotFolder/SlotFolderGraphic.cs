using UnityEngine;

public class SlotFolderGraphic : MonoBehaviour
{
   [SerializeField] private GameObject objAds;
   [SerializeField] private GameObject objPriceCoin;
   [SerializeField] private GameObject objPlus;
   
   public void ChangeType(SlotFolderType type)
   {
       objAds.SetActive(type == SlotFolderType.Ads);
       objPriceCoin.SetActive(type == SlotFolderType.Coin);
       objPlus.SetActive(objAds.activeSelf || objPriceCoin.activeSelf);
   }
}
