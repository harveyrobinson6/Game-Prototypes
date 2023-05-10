using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip hitSound;
    [SerializeField] AudioClip tickSound;
    [SerializeField] AudioClip missSound;
    [SerializeField] AudioClip UISound;
    [SerializeField] AudioSource src;

    public void PlayHitSound()
    {
        src.clip = hitSound;
        src.Play();
    }
    public void PlayTickSound()
    {
        src.clip = tickSound;
        src.Play();
    }
    public void PlayMissSound()
    {
        src.clip = missSound;
        src.Play();
    }
    public void PlayUISound()
    {
        src.clip = UISound;
        src.Play();
    }
}
