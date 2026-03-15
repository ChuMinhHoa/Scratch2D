using System;
using System.Collections.Generic;
using Lofelt.NiceVibrations;
using R3;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UniRx;
using UnityEngine;

[Serializable]
public class AudioConfig
{
    [field: SerializeField] public AudioKey AudioKey { get; private set; }
    [field: SerializeField] public AudioClip AudioClip { get; private set; }
}

public class SoundManager : Singleton<SoundManager>
{
    private bool isLoadDone;
    public List<SettingData> settingData = new();
    
    private void Start()
    {
        InitAudio();
        settingData = SettingDataSave.Instance.settingData;
        for (var i = 0; i < settingData.Count; i++)
        {
            settingData[i].ableSetting.Subscribe(ChangeSetting).AddTo(this);
        }
    }

    private void ChangeSetting(bool settingChange)
    {
        for (var i = 0; i < settingData.Count; i++)
        {
            switch (settingData[i].settingKey)
            {
                case SettingKey.MusicBg:
                    MuteBg(settingData[i].ableSetting.Value);
                    break;
                case SettingKey.Sound:
                    MuteSfx(settingData[i].ableSetting.Value);
                    break;
                case SettingKey.Vibrate:
                case SettingKey.None:
                default:
                    return;
            }
        }
    }

    [Button]
    private void InitAudio()
    {
        for (var i = 0; i < sfxConfig.Count; i++)
        {
            var e = new SoundData();
            var source = gameObject.AddComponent<AudioSource>();
            e.InitData(sfxConfig[i].AudioKey, sfxConfig[i].AudioClip, source, 1, false);
            sfxData.Add(e);
        }

        isLoadDone = true;
    }
    
    #region sfx Controller

    public List<AudioConfig> sfxConfig;
    public List<SoundData> sfxData;

    [Button]
    public void PlaySoundSfx(AudioKey audioKey)
    {
        for (var i = 0; i < sfxData.Count; i++)
        {
            if (sfxData[i].audioKey != audioKey) continue;
            sfxData[i].PlayOneShot();
            return;
        }
    }

    [Button]
    private void StopSoundSfx(AudioKey audioKey)
    {
        for (var i = 0; i < sfxData.Count; i++)
        {
            if (sfxData[i].audioKey != audioKey) continue;
            sfxData[i].Stop();
            return;
        }
    }

    [Button]
    private void PlayAtTimeSfx(AudioKey audioKey, float time)
    {
        for (var i = 0; i < sfxData.Count; i++)
        {
            if (sfxData[i].audioKey != audioKey) continue;
            sfxData[i].PlayAtTime(time);
        }
    }
    
    [Button]
    public void PlaySoundAtTimeSfx(AudioKey audioKey, float time)
    {
        for (var i = 0; i < sfxData.Count; i++)
        {
            if (sfxData[i].audioKey != audioKey) continue;
            sfxData[i].PlayAtTime(time);
        }
    }
    
    private void MuteSfx(bool mute)
    {
        for (var i = 0; i < sfxData.Count; i++)
        {
            sfxData[i].SetMute(mute);
        }
    }
    
    #endregion

    #region SoundBG

    public List<AudioConfig> bgConfig;
    public List<SoundData> bgData;

    public void InitBgSound()
    {
        for (var i = 0; i < bgConfig.Count; i++)
        {
            var e = new SoundData();
            var source = gameObject.AddComponent<AudioSource>();
            e.InitData(sfxConfig[i].AudioKey, sfxConfig[i].AudioClip, source, 1, true);
            bgData.Add(e);
        }
    }

    public void PlayBgSound(AudioKey audioKey)
    {
        for (var i = 0; i < bgData.Count; i++)
        {
            if (bgData[i].audioKey != audioKey) continue;
            bgData[i].Play();
        }
    }

    public void StopBgSound(AudioKey audioKey)
    {
        for (var i = 0; i < bgData.Count; i++)
        {
            if (bgData[i].audioKey != audioKey) continue;
            bgData[i].Stop();
        }
    }
    
    private void MuteBg(bool mute)
    {
        for (var i = 0; i < bgData.Count; i++)
        {
            bgData[i].SetMute(mute);
        }
    }

    #endregion

    #region Vibrate

    public void PlayVibrate(HapticPatterns.PresetType presetType)
    {   
        // find vibrate setting; if not found or disabled, do nothing
        var vibrateSetting = settingData.Find(s => s.settingKey == SettingKey.Vibrate);
        if (vibrateSetting != null && !vibrateSetting.ableSetting.Value) return;

        HapticPatterns.PlayPreset(presetType);
    }

    #endregion
}

[Serializable]
public class SoundData
{
    public AudioKey audioKey;
    public AudioSource source;
    private int frequence;
    private bool isOnLoop;

    public void Play()
    {
        source.Play();
    }

    public void PlayOneShot()
    {
        source.PlayOneShot(source.clip);
    }

    public void Stop()
    {
        source.Stop();
        isOnLoop = false;
    }

    public void InitData(AudioKey audioKey, AudioClip clip, AudioSource aSource, float volume, bool isLoop)
    {
        this.audioKey = audioKey;
        aSource.volume = volume;
        aSource.loop = isLoop;
        source = aSource;
        source.clip = clip;
        frequence = source.clip.frequency;
    }

    public void PlayAtTime(float timeStart)
    {
        var realTimeStart = 0f;
        if (source.isPlaying) return;
        if (isOnLoop)
        {
            realTimeStart = timeStart;
        }
        
        source.Stop();
        
        var startSample = Mathf.Clamp((int)(realTimeStart * frequence), 0, source.clip.samples - 1);
        
        source.timeSamples = startSample;

        source.Play();
        isOnLoop = true;
    }

    public void SetMute(bool mute)
    {
        if (mute)
        {
            source.Stop();
        }
        source.mute = mute;
    }
}

public enum SettingKey
{
    None,
    MusicBg,
    Sound,
    Vibrate
}

public enum AudioKey
{
    ButtonClick,
    Sfx_Scratch
}