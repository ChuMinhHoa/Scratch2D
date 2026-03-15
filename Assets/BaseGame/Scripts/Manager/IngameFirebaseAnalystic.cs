using Firebase.Analytics;
//using GlobalEnum;
//using Manager;
using SDK;
using TW.Utility.DesignPattern;
using UnityEngine;

public class IngameFirebaseAnalystic : Singleton<IngameFirebaseAnalystic>
{
    #region Gameplay

  

    #endregion

    #region ADS

    public void TrackAdsRewardShow(string placement)
    {
        throw new System.NotImplementedException();
    }

    public void TrackAdsInterShow()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Currentcy

    #endregion

    #region Purchase

    public void TrackEventPurchase(string packageName, string productID, string placement)
    {
        var parameters = new Parameter[]
        {
            new("product_id", productID),
            new("package_name", packageName),
            new("placement", placement),
        };
        FirebaseManager.Instance.LogFirebaseEvent("iapPurchased_confirmed", parameters);
    }


#endregion

}