/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void Damage (float damage);
        void Damage (float damage, Vector3 targetPosition);
    }
}