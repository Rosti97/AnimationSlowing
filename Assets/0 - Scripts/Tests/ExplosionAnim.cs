using System;
using System.Collections;
using UnityEngine;

public class ExplosionAnim : MonoBehaviour
{

    private float delay = 0.4f;
    // public event Action OnExplosionEnd;
    private ParticleSystem ps;


    public void Start()
    {
        ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    void OnDisable()
    {
        mainmanager.Instance.OnExplosionEnd.Invoke();
    }

}
