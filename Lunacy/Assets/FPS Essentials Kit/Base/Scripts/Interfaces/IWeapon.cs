/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

namespace Essentials
{
    public interface IWeapon : IEquipable
    {
        int Identifier { get; }
        GameObject Viewmodel { get; }

        bool CanSwitch { get; }
        bool CanUseItems { get; }
        bool CanVault { get; }

        float HideAnimationLength { get; }
        float InteractAnimationLength { get; }
        float InteractDelay { get; }

        void Interact ();
    }
}