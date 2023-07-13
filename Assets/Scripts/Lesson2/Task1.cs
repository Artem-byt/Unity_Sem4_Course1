using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public struct Task1 : IJob
{
    public NativeArray<int> Numbers;
    public void Execute()
    {
        for(int i = 0; i < Numbers.Length; i++)
        {
            if (Numbers[i] > 10)
            {
                Numbers[i] = 0;
            }
        }
    }
}
