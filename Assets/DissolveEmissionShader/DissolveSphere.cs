using System;
using System.Collections;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
    public Material mat;
    public float standardAmount = 0.4f;
    public float dissolveEndAmount = 0.63f;
    public float duration = 0.1f;

    public event Action OnDissolveEnd;
    public event Action OnDissolveStart;

    private void Start()
    {
        if (mat == null)
            mat = GetComponent<Renderer>().material;
        ResetDissolve();
    }

    public void StartDissolving()
    {
        OnDissolveStart?.Invoke();
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        float t = 0f;
        float targetAmount = 0.6f; // Shader cutoff erreicht
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float dissolveAmount = Mathf.Lerp(standardAmount, targetAmount, t);
            mat.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }

        mat.SetFloat("_DissolveAmount", targetAmount);

        OnDissolveEnd?.Invoke();
    }

    public void ResetDissolve()
    {
        if (mat != null)
            mat.SetFloat("_DissolveAmount", standardAmount);
    }
}
