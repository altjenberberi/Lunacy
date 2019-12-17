/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    public interface IBulletDamageable
    {
        bool IsAlive { get; }
        void BulletDamage (float damage, Vector3 targetPosition);
    }
}