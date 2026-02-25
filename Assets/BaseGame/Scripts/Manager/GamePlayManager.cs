using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TW.Utility.DesignPattern;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class GamePlayManager : Singleton<GamePlayManager>
{
    
    [SerializeField] private Camera cam;
    public Eraser eraser;

    public float radiusCheck = 1f;
    public LayerMask whatIsCardLayer;
    private Dictionary<Collider2D, Card> cardCollection = new();
    bool isFollowing;
    public Level level;
    
    public void SetCurrentCard(Card card)
    {
        eraser.SetCurrentCard(card);
    }

    public bool IsCurrentCard(Card card)
    {
        return eraser.currentCard == card;
    }

    private void Start()
    {
        if (cam == null)
            cam = Camera.main;
        this.UpdateAsObservable().Subscribe(_ => { UpdateFunction(); }).AddTo(this);
    }
    
    private void UpdateFunction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isFollowing = true;
            eraser.SetActiveGraphic(true);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isFollowing = false;
            eraser.SetActiveGraphic(false);
        }

        if (!isFollowing) return;
        
        if (Input.GetMouseButton(0))
        {
            CheckOverLayerCard();
        }

        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        var targetPos = cam.ScreenToWorldPoint(mouseScreenPos);
        targetPos.z = transform.position.z;
        
        eraser.Move(targetPos);
        
    }

    private void CheckOverLayerCard()
    {
        var mouseScreenPos = Input.mousePosition;
        
        var worldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        var hit = new Collider2D[5];
        if(Physics2D.OverlapCircleNonAlloc(worldPos, radiusCheck, hit, whatIsCardLayer)>0)
        {
            var card = GetCardFromDictionary(hit[0]);
            if (IsCurrentCard(card))
                return;
            SetCurrentCard(card);
        }
        else
        {
            SetCurrentCard(null);
        }
        
        
        // if (Physics.Raycast(ray, out var hit))
        // {
        //     var card = hit.collider.GetComponent<Card>();
        //     SetCurrentCard(card);
        // }
    }
    
    private Card GetCardFromDictionary(Collider2D col)
    {
        if (cardCollection.TryGetValue(col, out var dictionary))
            return dictionary;
        var card = col.GetComponent<Card>();
        if (card != null)
            cardCollection.Add(col, card);
        return card;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(eraser.transform.position, radiusCheck);
    }

    public void RegisterStickerDone(Sticker sticker)
    {
        level.RegisterStickerDone(sticker);
    }
}