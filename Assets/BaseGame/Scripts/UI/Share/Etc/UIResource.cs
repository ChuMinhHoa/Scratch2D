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
    public GameResource.Type resourceType;
    [SerializeField] private GameResource resourceValue;
    
    [SerializeField] private Reactive<BigNumber> resourceAmount;
    public Button btnAdd;
    [SerializeReference] public ActionCallOnResource actionCallOnResource;

    public virtual void Start()
    {
        if (resourceType != GameResource.Type.None)
            SetResourceType();
        btnAdd.onClick.AddListener(actionCallOnResource.ActionCallOnUIResource);
    }

    public void SetResourceType()
    {
        switch (resourceType)
        {
            case GameResource.Type.Money:
            case GameResource.Type.Gem:
                resourceValue = PlayerResourceManager.Instance.GetGameResource(resourceType);
                break;
            case GameResource.Type.Energy:
                resourceValue = EnergyManager.Instance.energyResource;
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
        txtAmount.SetTextFormat(MyCache.strDefault, value.ToStringUIFloor());
    }
}
