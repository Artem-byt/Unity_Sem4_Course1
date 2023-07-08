using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

public struct Task3 : IJobParallelForTransform
{
    public void Execute(int index, TransformAccess transform)
    {
        transform.rotation *= Quaternion.Euler(30f, 30f, 30f);
    }
}
