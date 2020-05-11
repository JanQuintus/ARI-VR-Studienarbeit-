using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class ControlledPiston : MonoBehaviour
{
    public bool ExpandOnStart = false;
    public AudioClip ExpandAC;
    public AudioClip ReverseAC;
    public Renderer[] ColoredRenderers;

    private Animator _anim;
    private AudioSource _audioSource;
    private bool _isExpanded = false;


    protected int _colorIndex = -1;
    protected static Color[] _colors =
    {
        new Color(.341f, .396f, .454f),
        new Color(.113f, .819f, .631f),
        new Color(.372f, .152f, .803f),
        new Color(.996f, .792f, .341f),
        new Color(   1f, .419f, .419f),
        new Color(.329f, .627f,    1f),
        new Color(.000f, .823f, .827f),
        new Color(   1f, .623f, .952f),
        new Color(.784f, .839f, .898f),
        new Color(.282f, .858f, .984f),
    };
    

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();

        if (ExpandOnStart) Expand();
    }

    private void Start()
    {
        int highestIndex = -1;
        foreach (ControlledPiston cp in FindObjectsOfType<ControlledPiston>())
        {
            if (cp._colorIndex > highestIndex)
                highestIndex = cp._colorIndex;
        }

        _colorIndex = highestIndex + 1;
        if (_colorIndex > _colors.Length - 1)
        {
            _colorIndex = Random.Range(0, _colors.Length - 1);
            Debug.LogWarning("ControlledPiston Colors exceeded!");
        }

        foreach(Renderer r in ColoredRenderers)
        {
            r.material.SetColor("_BaseColor", _colors[_colorIndex]);
        }
    }

    public void TogglePiston()
    {
        if (_isExpanded)
            Reverse();
        else
            Expand();
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

