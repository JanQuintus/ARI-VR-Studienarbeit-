using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Trashbin : MonoBehaviour
{
    public ParticleSystem ThrowInParticle;

    private AudioSource _as;

    private void Awake()
    {
        _as = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ProgramBlock")
        {
            ThrowInParticle.Play();
            _as.Play();
            //other.gameObject.SetActive(false);
            //Destroy(other.gameObject, .2f);

            // Destroying causes errors, the following fix should be removed and replaced with a fix for the issue itself!
            other.gameObject.transform.position += new Vector3(0, -100, 0);
        }
    }
}
