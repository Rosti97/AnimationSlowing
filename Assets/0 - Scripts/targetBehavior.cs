using System.Collections;
using UnityEngine;

public class targetBehavior : MonoBehaviour
{
    public GameObject shadowObject;
    public GameObject explosionObject;
    public bool isLong = false;

    public float shadowDelay = 0.3f; // sphere = kurz, rec = lang
    public float multiplier = 1f;
    public float endScale = 3f;

    private DissolveEffect dissolve;
    private ExplosionAnim explAnim;

    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip bigHitSound;
    public AudioClip shadowSound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dissolve = GetComponent<DissolveEffect>();
        dissolve.OnDissolveEnd += HandleDissolveEnd;
        dissolve.OnDissolveStart += HandleDissolveStart;
    }

    private void HandleDissolveEnd()
    {
        StartCoroutine(ShadowThenExplosion());
    }

    private IEnumerator ShadowThenExplosion()
    {
        // if (shadowObject != null)
        // {
        //     shadowObject.SetActive(true);
        //     yield return StartCoroutine(GrowShadow(shadowObject, shadowDelay));
        // }


        yield return new WaitForSeconds(shadowDelay);

        if (explosionObject != null)
            explosionObject.SetActive(true);


        audioSource.Stop();
        if (isLong)
        {
            audioSource.PlayOneShot(bigHitSound);
            // Debug.Log("hit me baby one more time");
        }
        else
        {
            audioSource.PlayOneShot(hitSound);
        }


        // yield return new WaitForSeconds(0.5f);
        shadowObject.SetActive(false);
        // yield return new WaitForSeconds(1.5f);

        gameObject.SetActive(false);
        // explosionObject.SetActive(false);
        dissolve.ResetDissolve();
    }

    private void HandleDissolveStart()
    {
        shadowObject.SetActive(true);
        StartCoroutine(GrowShadow(shadowObject, shadowDelay));
        // Debug.Log("End: " + Time.time);
    }


    private IEnumerator GrowShadow(GameObject target, float duration)
    {
        if (isLong)
        {
            audioSource.PlayOneShot(shadowSound);

            float scaler = 0f;
            Vector3 startScaleVector = Vector3.one;
            Vector3 endScaleVector = new Vector3(endScale, endScale, 1);

            target.transform.localScale = startScaleVector;

            while (scaler < 1f)
            {
                scaler += Time.deltaTime / (duration * multiplier);
                target.transform.localScale = Vector3.Lerp(startScaleVector, endScaleVector, scaler);
                yield return null;
            }
            target.transform.localScale = endScaleVector;

        }
    }
}
