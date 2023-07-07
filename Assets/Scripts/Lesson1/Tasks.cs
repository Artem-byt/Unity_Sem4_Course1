using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;
using UniRx;
using UnityEngine.UI;

public class Tasks : MonoBehaviour
{
    [SerializeField] private int _maxFrames = 60;
    [SerializeField] private Button _btnTask1;
    [SerializeField] private Button _btnTask2;
    [SerializeField] private Button _btnTask3;
    [SerializeField] private Button _btnCancel;
    [SerializeField] private Unit _healingUnit;

    private CancellationTokenSource _cts;
    private bool _isTasksStart = false;
    private int _frames = 0;

    private void Start()
    {
        _btnTask1.onClick.AddListener(_healingUnit.ReceiveHealing);
        _btnTask2.onClick.AddListener(DoExercise2);
        _btnTask3.onClick.AddListener(DoExercise3);

        _btnCancel.onClick.AddListener(DoCancel);
    }

    private async void DoExercise2()
    {
        _cts = new CancellationTokenSource();
        Task1(_cts.Token);
        Task2(_cts.Token);
    }

    private async void DoExercise3()
    {
        _cts = new CancellationTokenSource();
        var result = await FastTask.WhatTaskFasterAsync(_cts.Token, Task1(_cts.Token), Task2(_cts.Token));
        Debug.Log(result);
    }

    private void DoCancel()
    {
        if (_cts != null) 
        {
            _cts.Cancel();
            _cts.Dispose();
            _cts= null;
        }
    }

    public async Task Task1(CancellationToken token)
    {
        Debug.Log("Task 1 is Start");
        await Task.Delay(1000, token);
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
            if(_cts.Token.IsCancellationRequested)
            {
                Debug.Log("Прервано токеном");
                break;
            }
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
