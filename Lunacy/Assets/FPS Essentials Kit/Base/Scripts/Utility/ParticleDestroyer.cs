/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

public sealed class ParticleDestroyer : MonoBehaviour
{
    // Use this for initialization
    private void Start ()
    {
        InvokeRepeating("DestroyParticle", 0, 1);
    }

    private void DestroyParticle ()
    {
        ParticleSystem particle = GetComponentInChildren<ParticleSystem>();

        if (!particle.IsAlive(true))
        {
            Destroy(gameObject);
        }
    }
}
