using System.Collections;
using UnityEngine;
using System;

public class MiddleAnimation : MonoBehaviour
{
    // public GameObject shadowObject;
    // public GameObject explosionObject;

    // public float shadowDelay = 0.3f; // sphere = kurz, rec = lang
    private DissolveEffect dissolve;
    // public event Action OnMiddleEnd;

    public void Start()
    {
        dissolve = GetComponent<DissolveEffect>();
        dissolve.OnMiddleDissolveEnd += HandleDissolveEnd;
    }

    private void HandleDissolveEnd()
    {
        mainmanager.Instance.OnMiddleEnd.Invoke();
        gameObject.SetActive(false);
        dissolve.ResetDissolve();
    }

    // private void OnMouseDown()
    // {
    //     dissolve.StartDissolving();
    // }

    public void StartDissolving()
    {
        dissolve.StartDissolving();
    }

}