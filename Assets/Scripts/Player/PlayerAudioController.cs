using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    public AudioSource speaker;
    public AudioClip[] swordSounds;
    public AudioClip[] hitSounds;
    public AudioClip deathSound;
    

    public void PlaySwordNoise()
    {
        speaker.PlayOneShot(swordSounds[Random.Range(0, swordSounds.Length)]);
    }

    public void PlayHitNoise()
    {
        speaker.PlayOneShot(hitSounds[Random.Range(0, hitSounds.Length)]);
    }
    
    public void PlayDeathNoise()
    {
        speaker.PlayOneShot(deathSound);
    }
}
