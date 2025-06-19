using System.Collections;
using UnityEngine;

public class MiddleAnimation : MonoBehaviour
{
    public GameObject shadowObject;
    public GameObject explosionObject;

    public float shadowDelay = 0.3f; // sphere = kurz, rec = lang
    private DissolveEffect dissolve;

    void Awake()
    {
        dissolve = GetComponent<DissolveEffect>();
        dissolve.OnDissolveEnd += HandleDissolveEnd;
    }

    private void HandleDissolveEnd()
    {
        gameObject.SetActive(false);
        dissolve.ResetDissolve();
    }

    private void OnMouseDown()
    {
        dissolve.StartDissolving();
    }

}