using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class DissolveEffect : MonoBehaviour
{
    public Material mat;
    public float startDissolveAmount = 0f;
    public float dissolveEndAmount = 0.63f;
    public float duration = 0.1f;

    public event Action OnDissolveEnd;
    public event Action OnMiddleDissolveEnd;
    public event Action OnDissolveStart;

    private void Start()
    {
        if (mat == null)
            mat = GetComponent<Renderer>().material;
        ResetDissolve();
    }

    public void StartDissolving()
    {
        if (gameObject.name != "middleOrb")
        {
            OnDissolveStart?.Invoke();

        }
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine()
    {
        float t = 0f;
        float targetAmount = 0.68f; // complete dissolve
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float dissolveAmount = Mathf.Lerp(startDissolveAmount, targetAmount, t);
            mat.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }

        mat.SetFloat("_DissolveAmount", targetAmount);

        if (gameObject.name == "middleOrb")
        {
            // Sphere specific logic
            OnMiddleDissolveEnd?.Invoke();
        }
        else
        {
            OnDissolveEnd?.Invoke();
        }
        
    }

    public void ResetDissolve()
    {
        if (mat != null)
            mat.SetFloat("_DissolveAmount", startDissolveAmount);
    }
}
