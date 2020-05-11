using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Pressureplate : MonoBehaviour
{
    public UnityEvent OnPress;

    public AudioClip PressAC;

    private Animator _anim;
    private AudioSource _as;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _as = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "ARI")
        {
            if (GameState.Instance.IsPaused())
                return;

            _as.PlayOneShot(PressAC);
            _anim.SetBool("isPressed", true);

            OnPress?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ARI")
            _anim.SetBool("isPressed", false);
    }
}
