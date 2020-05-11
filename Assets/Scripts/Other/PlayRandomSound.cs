using System.Collections;
using UnityEngine;

public class PlayRandomSound : MonoBehaviour
{
    public AudioSource Source;
    public AudioClip[] Clips;
    public float MinBreak = 1f;
    public float MaxBreak = 6f;
    [Range(0, 1)]
    public float Volume = 1f;

    private void Awake()
    {
        StartCoroutine(PlaySound());
    }

    private IEnumerator PlaySound()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinBreak, MaxBreak));
            Source.PlayOneShot(Clips[Random.Range(0, Clips.Length - 1)], Random.Range(0, Volume));
        }
    }
}
