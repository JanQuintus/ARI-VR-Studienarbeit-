using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ScanLight : MonoBehaviour
{
    private Animator _anim;

    public static ScanLight Instance;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _anim = GetComponent<Animator>();

        Instance = this;
    }

    public void Scan()
    {
        _anim.SetTrigger("Scan");
    }
}
