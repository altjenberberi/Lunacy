/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;

public static class AnimatorExtension
{
    public static AnimationClip GetAnimationClip (this Animator animator, string name)
    {
        if (animator == null)
            return null;

        foreach (AnimationClip animClip in animator.runtimeAnimatorController.animationClips)
        {
            if (animClip.name == name)
                return animClip;
        }
        return null;
    }
}
