using System.Security.Cryptography;
using UnityEngine;

public class soundmanager : MonoBehaviour
{

    public AudioSource soundSource;
    public AudioClip hitSound;
    public AudioClip shootSound;
    public AudioClip passwordSound;
    public AudioClip middleSound;
    public AudioClip explosionSound;
    public AudioClip shadowSound;
    public AudioClip miniExplosionSound;


    void Start()
    {
        hitSound.LoadAudioData();
        shootSound.LoadAudioData();
        passwordSound.LoadAudioData();
        middleSound.LoadAudioData();
        explosionSound.LoadAudioData();
        shadowSound.LoadAudioData();
        miniExplosionSound.LoadAudioData();
    }

    void Update()
    {

    }

    // gets called with button click in the startUI
    public void PlayStartPassword()
    {
        soundSource.PlayOneShot(passwordSound, 1f);

    }

    public void PlayHitSound()
    {
        soundSource.PlayOneShot(hitSound, 1f);
    }

    public void PlayShootSound()
    {
        soundSource.PlayOneShot(shootSound, 1f);
    }

    public void PlayMiddleSound()
    {
        soundSource.PlayOneShot(middleSound, 0.7f);
    }

    public void PlayExplosionSound()
    {
        soundSource.PlayOneShot(explosionSound, 1f);
    }

    public void PlayShadowSound()
    {
        soundSource.PlayOneShot(shadowSound, 1f);
    }
    
    public void PlayMiniExplosionSound()
    {
        soundSource.PlayOneShot(miniExplosionSound, 1f);
    }
}
