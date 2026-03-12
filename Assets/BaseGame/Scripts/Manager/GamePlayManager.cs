using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.Utility.DesignPattern;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public enum GameState
{
    None = 0,
    Normal = 1,
    Loading = 2,
    WaitingCheckLoseGame = 3,
    Playing = 10,
    OnBooster = 20,
}

public class GamePlayManager : Singleton<GamePlayManager>
{
    [SerializeField] private Camera cam;
    public Eraser eraser;

    public float radiusCheck = 1f;
    public LayerMask whatIsCardLayer;
    private Dictionary<Collider2D, Card> cardCollection = new();
    private Dictionary<Collider2D, ButtonGameObject> buttonGameObjects = new();
    public Reactive<bool> isFollowing;
    public Reactive<GameState> gameState = new (GameState.Normal);
    
    public Reactive<bool> onPlaying = new (false);

    protected override void Awake()
    {
        base.Awake();
        Input.multiTouchEnabled = false;
    }

    private void SetCurrentCard(Card card)
    {
        eraser.SetCurrentCard(card);
    }

    private bool IsCurrentCard(Card card)
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
        if (gameState.Value == GameState.OnBooster)
        {
            OnGameUsingBooster();
        }

        if (gameState.Value != GameState.Playing)
        {
            if (isFollowing.Value)
            {
                isFollowing.Value = false;
                eraser.SetActiveGraphic(false);
            }
            return;
        }
        
        OnGamePlaying();
        
    }

    private void OnGameUsingBooster()
    {
        if (Input.touchCount > 1)
            return;
        
        
        if (Input.GetMouseButtonUp(0) /*|| Input.GetTouch(0).phase == TouchPhase.Ended*/)
        {
            CheckOverSelectAbleOnBooster();
        }
    }

    private void OnGamePlaying()
    {
        if (Input.touchCount > 1)
            return;
        
        if (Input.GetMouseButtonDown(0) || (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            isFollowing.Value = true;
            eraser.SetActiveGraphic(true);
        }

        if (Input.GetMouseButtonUp(0) /*|| Input.GetTouch(0).phase == TouchPhase.Ended*/)
        {
            isFollowing.Value = false;
            eraser.SetActiveGraphic(false);
            CheckOverButtonGameObject();
            CheckOverSelectAbleOnBooster();
        }

        if (!isFollowing) return;

        if (Input.GetMouseButton(0) /*|| Input.GetTouch(0).phase == TouchPhase.Moved*/)
        {
            CheckOverLayerCard();
        }

        var mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(cam.transform.position.z - transform.position.z);
        var targetPos = cam.ScreenToWorldPoint(mouseScreenPos);
        targetPos.z = transform.position.z;

        eraser.Move(targetPos);
    }

    private void CheckOverSelectAbleOnBooster()
    {
        if (!BoosterManager.Instance.onUsingBooster)
            return;
        var mouseScreenPos = Input.mousePosition;
        var worldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        var r = Physics2D.OverlapCircle(worldPos, radiusCheck, 7);
        if (r)
        {
            BoosterManager.Instance.ChooseObjOnBooster(r);
        }
    }

    private void CheckOverButtonGameObject()
    {
        var mouseScreenPos = Input.mousePosition;

        var worldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        var hit = new Collider2D[5];
        if (Physics2D.OverlapCircleNonAlloc(worldPos, radiusCheck, hit) > 0)
        {
            var btn = GetButtonGameObjectFromDictionary(hit[0]);
            if (btn != null)
            {
                btn.OnClick();
            }
        }
        else
        {
            SetCurrentCard(null);
        }
    }

    private ButtonGameObject GetButtonGameObjectFromDictionary(Collider2D col)
    {
        if (buttonGameObjects.TryGetValue(col, out var dictionary))
            return dictionary;
        var btnGameObj = col.GetComponent<ButtonGameObject>();
        if (btnGameObj != null)
            buttonGameObjects.Add(col, btnGameObj);
        return btnGameObj;
    }

    private void CheckOverLayerCard()
    {
        var mouseScreenPos = Input.mousePosition;

        var worldPos = cam.ScreenToWorldPoint(mouseScreenPos);
        var hit = new Collider2D[5];
        if (Physics2D.OverlapCircleNonAlloc(worldPos, radiusCheck, hit, whatIsCardLayer) > 0)
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

    public void RemoveCurrentCard(Card card)
    {
        eraser.RemoveCurrentCard(card);
    }

    public void ChangeGameState(GameState state)
    {
        gameState.Value = state;
        onPlaying.Value = gameState > GameState.Loading;
    }
}