using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class FastTask
{
    public static async Task<bool> WhatTaskFasterAsync(CancellationToken ct, Task task1, Task task2)
    {
        var any = await Task.WhenAny(task1, task2);

        if (any == task1)
        {
            return true;
        }
        return false;
    }
}
