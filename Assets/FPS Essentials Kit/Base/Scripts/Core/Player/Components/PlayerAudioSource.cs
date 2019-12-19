/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

public enum AudioCategory
{
    SFx,
    Voice,
    Music
}

public class PlayerAudioSource
{
    private AudioCategory m_Category;
    private AudioSource m_Source;

    public PlayerAudioSource (Transform parent, string name, AudioCategory category, float minDistance, float maxDistance, float spatialBlend)
    {
        m_Category = category;

        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        m_Source = go.AddComponent<AudioSource>();

        m_Source.rolloffMode = AudioRolloffMode.Linear;
        m_Source.minDistance = minDistance;
        m_Source.maxDistance = maxDistance;
        m_Source.spatialBlend = spatialBlend;

        if (category == AudioCategory.Music)
            m_Source.outputAudioMixerGroup = AudioManager.Instance.MusicMixer;
        else
            m_Source.outputAudioMixerGroup = AudioManager.Instance.SFxMixer;
    }

    public void Play (AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        if (!m_Source.isPlaying || m_Source.clip != clip)
        {
            m_Source.clip = clip;
            m_Source.volume = GetVolume(volume);
            m_Source.Play();
        }
    }

    public void ForcePlay (AudioClip clip, float volume)
    {
        if (clip == null)
            return;

        m_Source.clip = clip;
        m_Source.volume = GetVolume(volume);
        m_Source.Play();
    }

    public void Stop ()
    {
        m_Source.Stop();
    }

    private float GetVolume (float volume)
    {
        switch (m_Category)
        {
            case AudioCategory.SFx:
                return volume * AudioManager.Instance.SFxVolume;
            case AudioCategory.Voice:
                return volume * AudioManager.Instance.VoiceVolume;
            case AudioCategory.Music:
                return volume * AudioManager.Instance.MusicVolume;
            default:
                return 0;
        }
    }

    public void CalculateVolumeByPercent (float startValue, float value, float maxVolume)
    {
        float vol = 1 - value / startValue;
        m_Source.volume = GetVolume(Mathf.Clamp(vol, 0, maxVolume));
    }
}
