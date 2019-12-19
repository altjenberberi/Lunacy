/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

[AddComponentMenu("FPS Essentials/Managers/Audio Manager"), DisallowMultipleComponent]
public sealed class AudioManager : Singleton<AudioManager>
{
    [SerializeField]
    [Range(0, 1)]
    private float m_SfxVolume = 1.0f;

    [SerializeField]
    [Range(0, 1)]
    private float m_VoiceVolume = 1.0f;

    [SerializeField]
    [Range(0, 1)]
    private float m_MusicVolume = 1.0f;

    [SerializeField]
    [NotNull]
    private AudioMixerGroup m_SfxMixer;

    [SerializeField]
    [NotNull]
    private AudioMixerGroup m_MusicMixer;

    private Dictionary<string, PlayerAudioSource> m_Sources = new Dictionary<string, PlayerAudioSource>();

    #region PROPERTIES

    public float SFxVolume
    {
        get { return m_SfxVolume; }
        set { m_SfxVolume = Mathf.Clamp01(value); }
    }

    public float VoiceVolume
    {
        get { return m_VoiceVolume; }
        set { m_VoiceVolume = Mathf.Clamp01(value); }
    }

    public float MusicVolume
    {
        get { return m_MusicVolume; }
        set { m_MusicVolume = Mathf.Clamp01(value); }
    }

    public AudioMixerGroup SFxMixer { get { return m_SfxMixer; } }
    public AudioMixerGroup MusicMixer { get { return m_MusicMixer; } }

    #endregion

    public PlayerAudioSource RegisterSource (string name = "Generic Source", Transform parent = null, AudioCategory category = AudioCategory.SFx, float minDistance = 1, float maxDistance = 3, float spatialBlend = 0.3f)
    {
        if (ContainsSource(name))
            return m_Sources[name];

        PlayerAudioSource audioSource = new PlayerAudioSource(parent, name, category, minDistance, maxDistance, spatialBlend);
        m_Sources.Add(name, audioSource);

        return m_Sources[name];
    }

    public bool ContainsSource (string source)
    {
        return m_Sources.ContainsKey(source);
    }

    public PlayerAudioSource GetSource (string source)
    {
        return m_Sources.ContainsKey(source) ? m_Sources[source] : null;
    }

    public void Play (string source, AudioClip clip, float volume)
    {
        if (m_Sources.ContainsKey(source))
        {
            PlayerAudioSource audioSource = m_Sources[source];
            audioSource.Play(clip, volume);
        }
        else
        {
            Debug.LogError("AudioManager: AudioSource '" + source + "' was not found.");
        }
    }

    public void ForcePlay (string source, AudioClip clip, float volume)
    {
        if (m_Sources.ContainsKey(source))
        {
            PlayerAudioSource audioSource = m_Sources[source];
            audioSource.ForcePlay(clip, volume);
        }
        else
        {
            Debug.LogError("AudioManager: AudioSource '" + source + "' was not found.");
        }
    }

    public void CalculateVolumeByPercent (string source, float startValue, float value, float maxVolume)
    {
        if (m_Sources.ContainsKey(source))
        {
            PlayerAudioSource audioSource = m_Sources[source];
            audioSource.CalculateVolumeByPercent(startValue, value, maxVolume);
        }
        else
        {
            Debug.LogError("AudioManager: AudioSource '" + source + "' was not found.");
        }
    }

    public void Stop (string source)
    {
        if (m_Sources.ContainsKey(source))
        {
            PlayerAudioSource audioSource = m_Sources[source];
            audioSource.Stop();
        }
        else
        {
            Debug.LogError("AudioManager: AudioSource '" + source + "' was not found.");
        }
    }

    public void PlayClipAtPoint (AudioClip clip, Vector3 position, float minDistance, float maxDistance, float volume)
    {
        if (clip == null)
            return;

        GameObject go = new GameObject("Generic Source [Position " + position + "]");
        go.transform.position = position;

        AudioSource source = go.AddComponent<AudioSource>();
        source.playOnAwake = false;

        source.clip = clip;
        source.volume = volume * m_SfxVolume;

        source.rolloffMode = AudioRolloffMode.Linear;
        source.minDistance = minDistance;
        source.maxDistance = maxDistance;

        source.spatialBlend = 1;
        source.outputAudioMixerGroup = m_SfxMixer;

        source.Play();

        Destroy(go, clip.length);
    }
}
