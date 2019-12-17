/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Essentials
{
    namespace Controllers
    {
        [AddComponentMenu("FPS Essentials/Controllers/Health Controller"), RequireComponent(typeof(FirstPersonController)), DisallowMultipleComponent]
        public class HealthController : MonoBehaviour, IDamageable, IBulletDamageable, IExplosionDamageable
        {
            [SerializeField]
            [MinMax(1, Mathf.Infinity)]
            protected float m_Life = 100; // Character hitpoints

            [SerializeField]
            protected bool m_Regenerate = true;

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_StartDelay = 5; // Delay in secs to start regenerating

            [SerializeField]
            [MinMax(0, Mathf.Infinity)]
            protected float m_RegenerateSpeed = 7.5f; // Hitpoints healed per sec

            [SerializeField]
            protected AudioClip m_BreakLegsSound;

            [SerializeField]
            [Range(0, 1)]
            protected float m_BreakLegsVolume = 0.3f;

            [SerializeField]
            protected List<AudioClip> m_HitSounds = new List<AudioClip>();

            [SerializeField]
            [Range(0, 1)]
            protected float m_HitVolume = 0.3f;

            [SerializeField]
            protected AudioClip m_ExplosionNoise;

            [SerializeField]
            [Range(0, 1)]
            protected float m_ExplosionNoiseVolume = 0.3f;

            [SerializeField]
            protected AudioClip m_HealSound;

            [SerializeField]
            [Range(0, 1)]
            protected float m_HealVolume = 0.3f;

            [SerializeField]
            protected AudioMixerSnapshot m_NormalSnapshot;

            [SerializeField]
            protected AudioMixerSnapshot m_StunnedSnapshot;

            [SerializeField]
            protected GameObject m_DeadPlayer; // GameObject instantiated when the character die

            protected float m_CurrentLife;
            protected float m_NextRegenTime;

            protected bool m_LowerBodyDamaged;
            protected bool m_Healing;

            protected BloodSplashEffect m_BloodSplashEffect;
            protected FirstPersonController m_FPController;
            protected PlayerAudioSource m_PlayerHealthSource;
            protected PlayerAudioSource m_PlayerBreathSource;

            public virtual bool IsAlive { get { return m_CurrentLife > 0; } }
            public float HealthPercent { get { return m_CurrentLife / m_Life; } }

            public event Action<Vector3> DamageEvent;
            public event Action ExplosionEvent;
            public event Action HitEvent;

            protected virtual void Start ()
            {
                m_FPController = GetComponent<FirstPersonController>();
                m_BloodSplashEffect = GetComponentInChildren<BloodSplashEffect>();
                m_FPController.LowerBodyDamaged = true;
                m_FPController.LandingEvent += FallDamage;

                m_CurrentLife = m_Life;

                m_PlayerHealthSource = AudioManager.Instance.RegisterSource("PlayerHealthSource", AudioManager.Instance.transform);
                m_PlayerBreathSource = AudioManager.Instance.RegisterSource("PlayerBreathSource", AudioManager.Instance.transform);
            }

            protected virtual void Update ()
            {
                // Hitpoints regeneration
                if (m_Regenerate && m_CurrentLife > 0 && m_CurrentLife < m_Life && m_NextRegenTime < Time.time)
                {
                    m_CurrentLife = Mathf.MoveTowards(m_CurrentLife, m_Life, Time.deltaTime * m_RegenerateSpeed);
                }

                // Sets if the character has broken their legs
                m_FPController.LowerBodyDamaged = m_LowerBodyDamaged;

                if (m_BloodSplashEffect != null)
                {
                    m_BloodSplashEffect.BloodAmount = 1 - m_CurrentLife / m_Life;
                }

                if (GameplayManager.Instance.IsDead && m_DeadPlayer != null)
                {
                    Die();
                }
            }

            public virtual void Heal (float healthAmount, bool bonus = false, float bonusDuration = 10)
            {
                if (healthAmount > 0)
                {
                    m_Healing = true;
                    StartCoroutine(HealProgressively(healthAmount, 2));
                    Invoke("SetNormalSnapshot", 0);
                    m_PlayerBreathSource.Stop();
                    m_PlayerHealthSource.Play(m_HealSound, m_HealVolume);

                    if (bonus)
                    {
                        m_LowerBodyDamaged = false;
                        StartCoroutine(m_FPController.AdrenalineShot(bonusDuration));
                    }
                }
            }

            protected virtual IEnumerator HealProgressively (float healthAmount, float duration = 1)
            {
                float targetLife = Mathf.Min(m_Life, m_CurrentLife + healthAmount);

                for (float t = 0f; t <= duration && m_Healing; t += Time.deltaTime)
                {
                    m_CurrentLife = Mathf.Lerp(m_CurrentLife, targetLife, t / duration);

                    yield return new WaitForFixedUpdate();
                }
                m_Healing = false;
            }

            protected virtual void ApplyDamage (float damage)
            {
                m_CurrentLife = Mathf.Max(m_CurrentLife - damage, 0);
                m_NextRegenTime = Time.time + m_StartDelay;
                m_Healing = false;
                GameplayManager.Instance.IsDead |= Mathf.Abs(m_CurrentLife) < Mathf.Epsilon;
            }

            protected virtual void FallDamage (float damage)
            {
                if (damage > 0)
                {
                    ApplyDamage(damage);
                }

                if (!m_Regenerate && damage > m_Life * 0.7f)
                {
                    m_LowerBodyDamaged = true;
                    m_PlayerHealthSource.ForcePlay(m_BreakLegsSound, m_BreakLegsVolume);
                }
            }

            public virtual void Damage (float damage)
            {
                if (damage > 0)
                {
                    ApplyDamage(damage);
                }
            }

            public virtual void Damage (float damage, Vector3 targetPosition)
            {
                // Apply damage and show the damage source direction
                if (damage > 0)
                {
                    ApplyDamage(damage);

                    if (DamageEvent != null)
                        DamageEvent.Invoke(targetPosition);
                }
            }

            public virtual void ExplosionDamage (float damage, Vector3 targetPosition)
            {
                // Apply explosion damage and show the explosion source direction
                if (damage > m_Life * 0.6f)
                {
                    m_PlayerHealthSource.ForcePlay(m_ExplosionNoise, m_ExplosionNoiseVolume);
                    m_StunnedSnapshot.TransitionTo(0.1f);
                    Invoke("SetNormalSnapshot", m_ExplosionNoise.length);
                }

                if (damage > 0)
                {
                    ApplyDamage(damage);
                    ExplosionEvent.Invoke();

                    if (DamageEvent != null)
                        DamageEvent.Invoke(targetPosition);
                }
            }

            protected virtual void SetNormalSnapshot ()
            {
                m_NormalSnapshot.TransitionTo(0.3f);
            }

            public virtual void BulletDamage (float damage, Vector3 targetPosition)
            {
                // Apply bullet damage and show the shooter direction
                if (damage > 0)
                {
                    ApplyDamage(damage);

                    if (DamageEvent != null)
                        DamageEvent.Invoke(targetPosition);

                    int i = UnityEngine.Random.Range(1, m_HitSounds.Count);
                    AudioClip a = m_HitSounds[i];

                    m_HitSounds[i] = m_HitSounds[0];
                    m_HitSounds[0] = a;

                    m_PlayerHealthSource.ForcePlay(a, m_HitVolume);
                    HitEvent.Invoke();
                }
            }

            protected virtual void Die ()
            {
                SetNormalSnapshot();
                gameObject.SetActive(false);
                Instantiate(m_DeadPlayer, transform.position, transform.rotation);
            }

            #region EDITOR

            public void AddHitSound ()
            {
                m_HitSounds.Add(null);
            }

            public void RemoveHitSound (int index)
            {
                m_HitSounds.RemoveAt(index);
            }

            #endregion
        }
    }
}