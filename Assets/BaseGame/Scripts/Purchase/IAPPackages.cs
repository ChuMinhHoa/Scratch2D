using System;
using UnityEngine;
using UnityEngine.Purchasing;

[System.Serializable]
public class IAPPackage
{
    [field: SerializeField] public string ProductID { get; private set;}
    [field: SerializeField] private string Price { get; set;}
    private string LocalizedPriceString { get; set;}
    private decimal LocalizedPrice { get; set;}
    private string CurrencyCode { get; set;}
    
    public IAPPackage(string productID,string price) 
    {
        ProductID = productID;
        Price = $"${price}";
    }
    public string GetPrice() 
    {
#if UNITY_EDITOR
        return Price;
#endif
       
        try
        {
            var product = Purchaser.Instance.FindProduct(ProductID);
            Debug.Log("Find Product for ProductID: " + ProductID);
            Debug.Log("product title: " + product.metadata.localizedTitle);
            LocalizedPriceString = product.metadata.localizedPriceString;
            LocalizedPrice = product.metadata.localizedPrice;
            CurrencyCode = product.metadata.isoCurrencyCode;
            Debug.Log("Get Price for ProductID: " + ProductID+ " price: " + LocalizedPriceString);
            return LocalizedPriceString;
        }
        catch (Exception e) 
        {
            Debug.LogError($"GetPrice Error: {e.Message}");
            return Price;
        }
    }
}