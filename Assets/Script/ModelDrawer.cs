using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Klak.TestTools;

public class ModelDrawer
{
    public ModelDrawer(GameObject modelVisualize, Vector3 mPosition, Quaternion mRotation, float scaleFactor)
    {
        if (modelVisualize == null)
        {
            Debug.LogError("ModelVisualize is null!");
            return;
        }

        modelVisualize.transform.position = mPosition * scaleFactor;
        modelVisualize.transform.rotation = mRotation;
    }
}