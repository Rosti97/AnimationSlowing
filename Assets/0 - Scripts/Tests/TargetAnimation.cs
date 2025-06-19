using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class TargetBehaviour : MonoBehaviour
{
    public GameObject shadowObject;
    public GameObject explosionObject;
    public bool isLong = false;

    public float shadowDelay = 0.3f; // sphere = kurz, rec = lang
    public float multiplier = 1f;

    private DissolveEffect dissolve;
    private ExplosionAnim explAnim;

    private float startTime = 0f;
    private float endTime = 0f;

    private void Awake()
    {
        dissolve = GetComponent<DissolveEffect>();
        dissolve.OnDissolveEnd += HandleDissolveEnd;
        dissolve.OnDissolveStart += HandleDissolveStart;
        //explAnim.OnExplosionEnd += HandleExplosionEnd;
    }

    private void OnMouseDown()
    {
        dissolve.StartDissolving();
        startTime = Time.time;
    }

    private void HandleDissolveEnd()
    {
        StartCoroutine(ShadowThenExplosion());
    }

    private void HandleDissolveStart()
    {
        shadowObject.SetActive(true);
        StartCoroutine(GrowShadow(shadowObject, shadowDelay));
    }
    private IEnumerator ShadowThenExplosion()
    {
        // if (shadowObject != null)
        // {
        //     shadowObject.SetActive(true);
        //     yield return StartCoroutine(GrowShadow(shadowObject, shadowDelay));
        // }


        yield return new WaitForSeconds(shadowDelay);

        
        float timestamp = Time.time;
        Debug.Log("Expl Start: " + (timestamp - startTime));

        if (explosionObject != null)
            explosionObject.SetActive(true);


        yield return new WaitForSeconds(0.5f);
        shadowObject.SetActive(false);
        yield return new WaitForSeconds(1.5f);

        // Deaktiviere das ganze Target-GameObject

        gameObject.SetActive(false);
        explosionObject.SetActive(false);
        dissolve.ResetDissolve();

    }

    private IEnumerator GrowShadow(GameObject target, float duration)
    {
        if (isLong)
        {

            float t = 0f;
            Vector3 startScale = Vector3.one;
            Vector3 endScale = new Vector3(3, 3, 1); // Z bleibt gleich


            target.transform.localScale = startScale;

            while (t < 5f)
            {
                t += Time.deltaTime / duration;
                target.transform.localScale = Vector3.Lerp(startScale, endScale, t);
                yield return null;
            }

            target.transform.localScale = endScale;
        }
    }
}
