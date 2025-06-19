using System;
using System.Collections;
using UnityEngine;

public class ExplosionAnim : MonoBehaviour
{

    public event Action OnExplosionEnd;
    private ParticleSystem ps;


    private void Start()
    {
        ps = GetComponent<ParticleSystem>();

        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    public void StartExploding()
    {
        gameObject.SetActive(true);

        Debug.Log("START");

        // if (ps != null)
        // {
        //     ps.Play();
        //     StartCoroutine(WaitForExplosionEnd());
        //}
    }

    private IEnumerator WaitForExplosionEnd()
    {
        while (ps.IsAlive(true))
        {
            yield return null;
        }

        //OnExplosionEnd?.Invoke();
        Debug.Log("END INVOKE");
    }

    void OnParticleSystemStopped()
    {
        IDK();
        Debug.Log("System has stopped!");
    }

    private void IDK()
    {
        Debug.Log("Hey");
        OnExplosionEnd?.Invoke();
        Debug.Log("Ho");
    }

    // TODO Particlesystem on finish -> invoke on ExplosionEnd

}
