using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Piston : MonoBehaviour
{
    public float InTime = 1f;
    public float OutTime = 1f;
    public bool ExpandOnStart = false;
    public float Delay = 0f;
    public AudioClip ExpandAC;
    public AudioClip ReverseAC;

    private Animator _anim;
    private AudioSource _audioSource;
    private bool _isExpanded = false;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (ExpandOnStart) Expand();

        StartCoroutine(WaitDelay());
    }

    private IEnumerator WaitDelay()
    {
        yield return new WaitForSeconds(Delay);
        StartCoroutine(MovePiston());
    }

    private IEnumerator MovePiston()
    {
        if (_isExpanded)
        {
            yield return new WaitForSeconds(OutTime);
            Reverse();
        }
        else
        {
            yield return new WaitForSeconds(InTime);
            Expand();
        }

        StartCoroutine(MovePiston());
    }

    private void Expand()
    {
        _anim.SetBool("Out", true);
        _audioSource.PlayOneShot(ExpandAC);
        _isExpanded = true;
    }

    private void Reverse()
    {
        _anim.SetBool("Out", false);
        _audioSource.PlayOneShot(ReverseAC);
        _isExpanded = false;
    }
}

