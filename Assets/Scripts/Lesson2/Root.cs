using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
using UnityEngine.UI;

public class Root : MonoBehaviour
{
    private NativeArray<int> _numbers;

    private NativeArray<Vector3> _positions;
    private NativeArray<Vector3> _velocities;
    private NativeArray<Vector3> _finalPositions;

    [SerializeField] private Button _btnTask1;
    [SerializeField] private Button _btnTask2;
    [SerializeField] private Button _btnTask3;

    [SerializeField] private Button _btnReset;

    [SerializeField] private Transform[] _objectsForRotating;
    private TransformAccessArray _transformAccessArray;

    private bool _isTask3;

    private void Start()
    {
        _numbers = new NativeArray<int>(5, Allocator.Persistent);

        _positions = new NativeArray<Vector3>(5, Allocator.Persistent);
        _velocities = new NativeArray<Vector3>(5, Allocator.Persistent);
        _finalPositions = new NativeArray<Vector3>(5, Allocator.Persistent);

        _transformAccessArray = new TransformAccessArray(_objectsForRotating);
        ResetTasks();

        _btnTask1.onClick.AddListener(DoTask1);
        _btnTask2.onClick.AddListener(DoTask2);
        _btnTask3.onClick.AddListener(DoTask3);

        _btnReset.onClick.AddListener(ResetTasks);
    }

    private void FixedUpdate()
    {
        if (_isTask3)
        {
            Task3 task3 = new Task3();
            JobHandle task3Handle = task3.Schedule(_transformAccessArray);
            task3Handle.Complete();
        }

    }

    private void DoTask1()
    {
        ShowArray(_numbers, "BEFORE");
        Task1 task1 = new Task1()
        {
            Numbers = _numbers
        };
        JobHandle task1Handle = task1.Schedule();
        task1Handle.Complete();
        ShowArray(_numbers, "AFTER");
    }

    private void DoTask2()
    {
        ShowArray(_finalPositions, "BEFORE");
        Task2 task2 = new Task2()
        {
            Positions = _positions,
            Velocities = _velocities,
            FinalPositions = _finalPositions
        };
        JobHandle task2Handle = task2.Schedule(5, 0);
        task2Handle.Complete();
        ShowArray(_finalPositions, "AFTER");
    }

    private void DoTask3()
    {
        _isTask3 = true;
    }

    private void ShowArray<T>(NativeArray<T> values, string message) where T : struct
    {
        Debug.Log(message);
        for (int i = 0; i < values.Length; i++)
        {
            Debug.Log("i = " + i + " value = " + values[i]);
        }
    }

    private void ResetTasks()
    {
        for (int i = 0; i < _numbers.Length; i++)
        {
            _numbers[i] = Random.Range(0, 20);
          
        }

        for(int i = 0; i < _positions.Length; i++)
        {
            _positions[i] = Random.insideUnitCircle;
            _velocities[i] = Random.insideUnitCircle;
        }

        _isTask3 = false;
    }

    private void OnDestroy()
    {
        _numbers.Dispose();
        _positions.Dispose();
        _velocities.Dispose();
        _finalPositions.Dispose();
        _transformAccessArray.Dispose();

        _btnTask1.onClick.RemoveAllListeners();
        _btnTask2.onClick.RemoveAllListeners();
        _btnTask3.onClick.RemoveAllListeners();
        _btnReset.onClick.RemoveAllListeners();
    }
}
