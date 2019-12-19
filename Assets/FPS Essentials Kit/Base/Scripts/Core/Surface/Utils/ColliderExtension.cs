/*
 * Copyright (c) 2017 The Asset Lab. All rights reserved.
 * https://www.theassetlab.com/
*/

using UnityEngine;
using System.Collections.Generic;

public static class ColliderExtension
{
    //Instead of trying to get the Surface Identifier using GetComponent() every time we need to check a object, 
    //a dictionary is much more performance efficient because we have to check a object only once.
    //However it's not possible to detect changes in the object (add a SurfaceIdentifier at runtime) once it has been registered.
    private static Dictionary<int, SurfaceIdentifier> surfs = new Dictionary<int, SurfaceIdentifier>();

    public static SurfaceIdentifier GetSurface (this Collider col)
	{
		if (surfs.ContainsKey(col.GetInstanceID()))
		{
			return surfs[col.GetInstanceID()];
		}

		surfs.Add(col.GetInstanceID(), col.GetComponent<SurfaceIdentifier>());
		return surfs[col.GetInstanceID()];
	}
}
