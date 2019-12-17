/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    public interface IExplosionDamageable
    {
        bool IsAlive { get; }
        void ExplosionDamage (float damage, Vector3 targetPosition);
    }
}
