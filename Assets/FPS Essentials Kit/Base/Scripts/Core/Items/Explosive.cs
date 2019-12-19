/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using Essentials;
using UnityEngine;

[AddComponentMenu("FPS Essentials/Items/Explosive"), DisallowMultipleComponent]
public sealed class Explosive : MonoBehaviour
{
    [SerializeField]
    [MinMax(0, Mathf.Infinity)]
    private float m_ExplosionRadius = 5;

    [SerializeField]
    [MinMax(0, Mathf.Infinity)]
    private float m_ExplosionForce = 10;

    [SerializeField]
    private bool m_IgnoreCover = false; // If false, the explosion effect will be only applied to objects in the sight

    [SerializeField]
    [MinMax(0, Mathf.Infinity)]
    private float m_Damage = 100;

    [SerializeField]
    private bool m_ExplodeWhenCollide = false; // If true, the explosive will be detonated when touch something

    [SerializeField]
    [MinMax(0, Mathf.Infinity)]
    private float m_TimeToExplode = 3;

    [SerializeField]
    [NotNull]
    private GameObject m_ExplosionParticle;

    [SerializeField]
    private AudioClip m_ExplosionSound;

    [SerializeField]
    [Range(0, 1)]
    private float m_ExplosionVolume = 0.5f;

    [SerializeField]
    [MinMax(0, Mathf.Infinity)]
    private float m_MinImpactForce = 0.5f; // Minimum force to detonate a grenade or to play collision sound

    [SerializeField]
    private AudioClip m_CollisionSound;

    [SerializeField]
    [Range(0, 1)]
    private float m_CollisionVolume = 0.3f;

    private void Start ()
    {
        if (!m_ExplodeWhenCollide)
            Invoke("Explosion", m_TimeToExplode);
    }

    public static void CalculateExplosionDamage (float radius, float force, float damage, Vector3 position, bool ignoreCover = false)
    {
        // List of colliders near of the player.
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);

        // For each collider near the player.
        foreach (Collider c in hitColliders)
        {
            if (c.gameObject.isStatic)
                continue;

            if (TargetInSight(position, c, ignoreCover))
            {
                Vector3 direction = (c.transform.position - position).normalized;

                Rigidbody rigidbody = c.GetComponent<Rigidbody>();
                if (rigidbody != null && c.tag != "Player")
                {
                    if (!rigidbody.isKinematic)
                    {
                        // Apply force to all rigidbody hit by explosion (except the player).
                        float intensity = (radius - Vector3.Distance(position, c.transform.position)) / radius;
                        rigidbody.AddForce(direction * force * intensity, ForceMode.Impulse);
                    }
                }

                IExplosionDamageable damageableTarget = c.GetComponent<IExplosionDamageable>();
                if (damageableTarget != null)
                {
                    float intensity = (radius - Vector3.Distance(position, c.transform.position)) / radius;
                    damageableTarget.ExplosionDamage(intensity * damage, position);
                }
            }
        }
    }

    private static bool TargetInSight (Vector3 position, Collider target, bool ignoreCover = false)
    {
        if (ignoreCover)
            return true;

        RaycastHit hitInfo;

        if (Physics.Linecast(position, target.bounds.center, out hitInfo) && hitInfo.collider == target)
            return true;

        if (Physics.Linecast(position, target.bounds.center + Vector3.up * target.bounds.extents.y, out hitInfo) && hitInfo.collider == target)
            return true;

        if (Physics.Linecast(position, target.bounds.center + Vector3.right * target.bounds.extents.x, out hitInfo) && hitInfo.collider == target)
            return true;

        if (Physics.Linecast(position, target.bounds.center + Vector3.left * target.bounds.extents.x, out hitInfo) && hitInfo.collider == target)
            return true;

        return false;
    }

    public void Explosion ()
    {
        if (m_ExplosionParticle != null)
            Instantiate(m_ExplosionParticle, transform.position, Quaternion.identity); //Instantiate the explosion particle

        // Calculate damage
        CalculateExplosionDamage(m_ExplosionRadius, m_ExplosionForce, m_Damage, new Vector3(transform.position.x, transform.position.y, transform.position.z), m_IgnoreCover);
        AudioManager.Instance.PlayClipAtPoint(m_ExplosionSound, transform.position, 10, 50, m_ExplosionVolume);
        Destroy(gameObject);
    }

    private void OnCollisionEnter (Collision col)
    {
        if (col.gameObject.tag == "Player")
            return;

        if (m_ExplodeWhenCollide)
            Explosion();
        else if (col.relativeVelocity.magnitude > m_MinImpactForce)
        {
            AudioSource source = GetComponent<AudioSource>();
            source.clip = m_CollisionSound; // Set AudioSource.clip as collision sound
            source.volume = m_CollisionVolume * AudioManager.Instance.SFxVolume; // Set AudioSource.volume
            source.Play();
        }
    }

    // Draw a sphere to show grenade explosion radius
    private void OnDrawGizmos ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_ExplosionRadius);
    }
}
