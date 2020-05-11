using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Dropper : MonoBehaviour
{
    public System.Action<AProgramBlock> OnDrop;

    public Transform SpawnPoint;
    public static Dropper Instance;
    public DropperSpace DS;
    public AudioClip DropAC;

    private AudioSource _audioSource;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        _audioSource = GetComponent<AudioSource>();
    }

    public void Drop(AProgramBlock pb)
    {
        if (DS.IsFree)
        {
            _audioSource.PlayOneShot(DropAC);
            AProgramBlock pbInstance = Instantiate(pb, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
            OnDrop?.Invoke(pbInstance);
        }
    }
}
