using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveSphere : MonoBehaviour
{

    Material mat;
    float standardAmount = 0.4f;

    private void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        // mat.SetFloat("_DissolveAmount", Mathf.Sin(Time.time) / 2 + 0.5f);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartDissolving();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetDissolve();
        }
    }

    public void StartDissolving()
    {
        StartCoroutine(DissolveRoutine());
    }

    private IEnumerator DissolveRoutine(float duration = 0.1f)
    {
        float start = standardAmount; // e.g., 0.4
        float end = 0.63f;               // fully dissolved
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            float dissolveAmount = Mathf.Lerp(start, end, t);
            mat.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }
        mat.SetFloat("_DissolveAmount", end);
    }

    public void ResetDissolve()
    {
        mat.SetFloat("_DissolveAmount", standardAmount);
    }

}