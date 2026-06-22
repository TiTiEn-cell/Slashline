using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using System;
using MoreMountains.NiceVibrations;

public class AudioController : SingletonMono<AudioController>
{
    public AudioSource pSFXLoopAudio;
    public AudioSource pSFXAudio;
    public AudioSource pMusic;

    public List<AudioClip> ListAudio = new List<AudioClip>();
    private Dictionary<string, float> soundFXDelay = new Dictionary<string, float>();

    public bool EnableMusic
    {
        get { return PlayerPrefs.GetInt(PlayerPrefKeys.EnableMusic, 1) > 0; }
        set
        {
            PlayerPrefs.SetInt("EnableMusic", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public bool EnableSoundFx
    {
        get { return PlayerPrefs.GetInt(PlayerPrefKeys.EnableSoundFx, 1) > 0; }
        set
        {
            PlayerPrefs.SetInt("EnableSoundFx", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public bool EnableHaptic
    {
        get { return PlayerPrefs.GetInt(PlayerPrefKeys.EnableHaptic, 1) > 0; }
        set
        {
            PlayerPrefs.SetInt("EnableHaptic", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        if (soundFXDelay.Count > 0)
        {
            var keys = new List<string>();
            foreach (var item in soundFXDelay)
            {
                keys.Add(item.Key);
            }

            foreach (var key in keys)
            {
                if (soundFXDelay.ContainsKey(key))
                {
                    soundFXDelay[key] -= Time.deltaTime;
                    if (soundFXDelay[key] < 0)
                    {
                        soundFXDelay[key] = 0;
                    }
                }
            }
        }
    }

    public void PlayBgMusic(string soundName, float volume = 1)
    {
        if (!EnableMusic)
        {
            return;
        }

        if (pMusic.isPlaying)
        {
            if (pMusic.clip.name == soundName)
            {
                return;
            }

            StopBgMusic(() =>
            {
                pMusic.volume = 0f;
                pMusic.clip = GetBGMAudioClip(soundName);
                pMusic.Play();
                pMusic.DOFade(volume, 0.2f);
            });
        }
        else
        {
            pMusic.DOKill(true);
            pMusic.volume = 0f;
            pMusic.clip = GetBGMAudioClip(soundName);
            pMusic.Play();
            pMusic.DOFade(volume, 4f);
        }
    }

    public void StopBgMusic(Action onCompleteCallback = null)
    {
        pMusic.DOKill(true);

        if (pMusic.isPlaying)
        {
            pMusic.DOFade(0, 1f).OnComplete(() =>
            {
                pMusic.Stop();
                onCompleteCallback?.Invoke();
            });
        }
        else
        {
            onCompleteCallback?.Invoke();
        }
    }

    public void ForcePlayBgMusic(string soundName)
    {
        Debug.LogError("Play music BG");
        if (!EnableMusic)
        {
            return;
        }

        pMusic.DOKill(true);
        pMusic.volume = 1f;
        pMusic.clip = GetBGMAudioClip(soundName);
        pMusic.Play();
    }

    public void ForceStopBgMusic()
    {
        pMusic.DOKill(true);
        pMusic.Stop();
    }

    public void ForcePauseBgMusic()
    {
        pMusic.DOKill(true);
        pMusic.Pause();
    }

    public void ResumeBgMusic()
    {
        if (!EnableMusic)
        {
            return;
        }

        pMusic.DOKill(true);
        pMusic.Play();
    }

    public void PlaySFX(string soundName, float volume = 1, bool isLoop = false, float delay = 0f)
    {
        if (!EnableSoundFx)
        {
            return;
        }

        if (isLoop)
        {
            if (!pSFXLoopAudio.isPlaying || pSFXLoopAudio.clip.name != soundName)
            {
                pSFXLoopAudio.loop = isLoop;
                pSFXLoopAudio.volume = volume;
                pSFXLoopAudio.clip = GetSFXAudioClip(soundName);
                pSFXLoopAudio.Play();
            }
        }
        else
        {
            var canPlay = true;
            if (delay > 0)
            {
                if (soundFXDelay.ContainsKey(soundName))
                {
                    canPlay = soundFXDelay[soundName] <= 0;
                    soundFXDelay[soundName] = delay;
                }
                else
                {
                    soundFXDelay.Add(soundName, delay);
                }
            }

            if (canPlay)
            {

                pSFXAudio.volume = volume;
                pSFXAudio.PlayOneShot(GetSFXAudioClip(soundName));
            }
        }
    }

    public void StopSFX()
    {
        if (pSFXLoopAudio.isPlaying)
        {
            pSFXLoopAudio.Stop();
        }
    }

    public void InitHaptic()
    {
        if (!EnableHaptic)
        {
            return;
        }

#if UNITY_ANDROID
        MMVibrationManager.Haptic(HapticTypes.MediumImpact);
#endif
    }

    public void PlayHaptic(HapticTypes type)
    {
        if (!EnableHaptic)
        {
            return;
        }
        MMVibrationManager.Haptic(type);
    }

    public void MediumHaptic()
    {
        PlayHaptic(HapticTypes.MediumImpact);
    }

    public void HeavyHaptic()
    {
        PlayHaptic(HapticTypes.HeavyImpact);
    }

    private AudioClip GetSFXAudioClip(string clipName)
    {
        var clip = Resources.Load<AudioClip>("Sound/SFX/" + clipName);
        if (clip == null)
        {
            Debug.LogError("GetSFXAudioClip not found:: " + clipName);
        }
        return clip;
    }

    private AudioClip GetBGMAudioClip(string clipName)
    {

        //var clip = ListAudio.Find(x => x.name == clipName);
        var clip = Resources.Load<AudioClip>("Sound/BGM/" + clipName);
        if (clip == null)
        {
            Debug.LogError("GetBGMAudioClip not found:: " + clipName);
        }
        return clip;
    }
}


