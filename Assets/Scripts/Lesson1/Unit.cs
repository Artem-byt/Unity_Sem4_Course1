using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class Unit : MonoBehaviour
{
    [SerializeField] int health = 50;

    private bool _isHealDone = true;

    private void Awake()
    {
       Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.A)).Subscribe(ReceiveHealing);
    }


    public void ReceiveHealing(long obj)
    {
        if(_isHealDone == true)
        {
            Debug.Log("Active");
            _isHealDone = false;
            StartCoroutine(Healing());
        }
        
    }

    private IEnumerator Healing()
    {
        var startTime = Time.time;
        var currentTime = Time.time;
        while (currentTime - startTime < 3f && health < 100)
        {
            health += 5;
            yield return new WaitForSeconds(0.5f);
            currentTime = Time.time;
            
        }
        _isHealDone = true;
    }
}
