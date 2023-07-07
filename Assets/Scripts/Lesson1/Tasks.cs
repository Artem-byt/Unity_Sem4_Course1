using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UniRx;

public class Tasks : MonoBehaviour
{
    private int _maxFrames = 60;
    private CancellationTokenSource _cts;
    private bool _isTasksStart = false;
    private int _frames = 0;


    private void Start()
    {
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.S)).Subscribe(_ => 
        { 
            _cts = new CancellationTokenSource();
            Task1(_cts.Token);
            Task2(_cts.Token);
            
        });
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.D)).Subscribe(async _ => 
        {
            _cts = new CancellationTokenSource();           
            var result = await FastTask.WhatTaskFasterAsync(_cts.Token, Task1(_cts.Token), Task2(_cts.Token));
            Debug.Log(result);
        });
        Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.F)).Subscribe(_ => { if (_cts != null) { _cts.Cancel(); } });
    }
    public async Task Task1(CancellationToken token)
    {
        Debug.Log("Task 1 is Start");
        await Task.Delay(1000);
        Debug.Log("Task 1 is Done");
    }

    public async Task Task2(CancellationToken token)
    {
        Debug.Log("Task 2 is Start");
        await Task.Run(() => { CheckFrames(); });
        Debug.Log("Task 2 is Done");
    }

    private void CheckFrames()
    {
        _isTasksStart = true;
        while (_frames < _maxFrames) 
        {

        }
        _frames = 0;
        _isTasksStart = false;
    }

    private void Update()
    {
        if(!_isTasksStart)
        {
            return;
        }
        _frames++;
    }
}
