using System;
using LitMotion;
using R3;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ButtonToggle : MonoBehaviour
{
    public Button btnToggle;
    public SettingKey settingKey;
    public Vector3 vectorOffset;
    public Transform trsToggle;
    public Sprite[] sprBg;
    public Image imgBg;

    public Reactive<bool> active = new(false);

    private void Start()
    {
        btnToggle.onClick.AddListener(ChangeSetting);
        var e = SoundManager.Instance.GetSettingData(settingKey);
        active = e.ableSetting;
        active.Subscribe(ChangeActive).AddTo(this);
    }

    private void ChangeSetting()
    {
        SoundManager.Instance.ChangeSettingData(settingKey);
    }

    private void ChangeActive(bool activeChange)
    {
        var targetPos = active.Value ? vectorOffset : -vectorOffset;
        var currentPos = trsToggle.localPosition;
        LMotion.Create(currentPos, targetPos, 0.15f).WithOnComplete(ChangeSpriteBg).Bind(x => trsToggle.localPosition = x).AddTo(trsToggle);
    }

    private void ChangeSpriteBg()
    {
        imgBg.sprite = active.Value ? sprBg[0] : sprBg[1];
    }
}