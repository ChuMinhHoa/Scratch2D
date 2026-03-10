using Cysharp.Text;
using R3;
using TMPro;
using TW.Utility.CustomType;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class UIResource : MonoBehaviour
{
    [SerializeField] private Image imgIcon;
    [SerializeField] private TextMeshProUGUI txtAmount;
    [SerializeField] private GameResource.Type resourceType;
    [SerializeField] private GameResource resourceValue;
    
    [SerializeField] private Reactive<BigNumber> resourceAmount;

    private void Start()
    {
        if (resourceType != GameResource.Type.None)
            SetResourceType();
    }

    private void SetResourceType()
    {
        switch (resourceType)
        {
            case GameResource.Type.Money:
            case GameResource.Type.Gem:
                resourceValue = PlayerResourceManager.Instance.GetGameResource(resourceType);
                break;
            default:
                Debug.LogError($"UIResource: SetResourceType: {resourceType} not found icon sprite.");
                break;
        }

        imgIcon.sprite = SpriteGlobalConfig.Instance.GetResourceIcon(resourceType);

        resourceAmount = resourceValue.ReactiveAmount;
        resourceAmount.Subscribe(ChangeValue).AddTo(this);
    }

    private void ChangeValue(BigNumber value)
    {
        txtAmount.SetTextFormat(MyCache.strDefault, value);
    }
}
