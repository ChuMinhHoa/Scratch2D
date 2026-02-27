using LitMotion;
using UnityEngine;

public class Eraser : MonoBehaviour
{
    [SerializeField] private float easeSpeed = 20f;
    [SerializeField] private GameObject graphic;
    private bool isFollowing;
    public Card currentCard;
    private float lastProgress;
    private float coolDown;

    public float coolDownTimeSetting = 0.25f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetActiveGraphic(false);
    }
    
    public void SetActiveGraphic(bool active)
    {
        if (graphic != null)
            graphic.SetActive(active);
    }

    public void SetCurrentCard(Card card)
    {
        if (currentCard != null)
        {
            currentCard.RemoveActionCallbackChangeProgress(OnChangeProgress);
        }

        currentCard = card;
        if (currentCard != null)
            currentCard.SetActionCallbackChangeProgress(OnChangeProgress);
    }

    private void OnChangeProgress(float progress)
    {
        if (lastProgress != progress && coolDown <= 0f)
        {
            var e = PoolManager.Instance.SpawnEraserEffect(transform);
            LMotion.Create(0, 1, 0.5f).WithOnComplete(() =>
            {
                PoolManager.Instance.DespawnEraserEffect(e);
            }).RunWithoutBinding().AddTo(this);
            lastProgress = progress;
            LMotion.Create(1f, 0f, coolDownTimeSetting).Bind(x => coolDown = x).AddTo(this);
        }
    }

    public void Move(Vector3 targetPos)
    {
        transform.position = targetPos;
    }
    
    public void RemoveCurrentCard(Card card)
    {
        if (currentCard == card)
        {
            currentCard.RemoveActionCallbackChangeProgress(OnChangeProgress);
            currentCard = null;
        }
    }
}